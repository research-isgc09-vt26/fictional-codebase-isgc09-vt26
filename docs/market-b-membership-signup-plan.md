# Plan: Enable MembershipSignupSubscription for Market Segmentation B

## Issue
Market Segmentation B can now offer `MembershipSignupSubscription`. The market-B
variant **must not** include the standard 30-day free trial and **must** use a
market-specific subscription message.

---

## 1. Files inspected

### Domain / Core
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/SubscriptionType.cs` — defines `MembershipSignupSubscription = 3`.
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/Segmentation.cs` — `SegmentationA`, `SegmentationB`.
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/MarketSubscriptionModel.cs` — `StandardMixedSubscriptionModel`, `CommunityOnlySubscriptionModel`.
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/MembershipSignupType.cs` — `OnlineSignup`, `OfflineSignup`.
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/SubscriptionMessageFlowType.cs` — `OnlineSubscription`, `CommunityMembershipSubscription`, `MembershipSignup`.
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/SubscriptionStatus.cs` — `None`, `Trial`, `Active`, `PastDue`, `Cancelled`.
- `src/SkillStarLearning.SubscriptionRules.Core/Entities/MembershipSignup.cs` — currently has non-nullable `TrialStartsOn` / `TrialEndsOn`.
- `src/SkillStarLearning.SubscriptionRules.Core/Entities/SubscriptionMessage.cs`, `SubscriptionAccount.cs`, `UserProfile.cs` — unchanged but reviewed for shape.

### Application
- `src/.../Application/Services/MarketSubscriptionPolicy.cs` — the central market rule (`IsMembershipSignupAvailable`, `IsSubscriptionAvailable`).
- `src/.../Application/Contracts/Services/IMarketSubscriptionPolicy.cs`.
- `src/.../Application/Services/SubscriptionMessageService.cs` — has an early `if (segmentation == SegmentationB)` branch that returns a single text for every flow type, ignoring `flowType`. This is the root of the "market-specific message" gap.
- `src/.../Application/Services/SubscriptionAvailabilityService.cs` & `Models/SubscriptionAvailabilityDto.cs` — read the policy and report availability to UI.
- `src/.../Application/Services/MembershipSignupService.cs` — creates the signup; today always sets `now.AddDays(30)` and `TrialStatus = Trial`.
- `src/.../Application/Contracts/Features/CreateMembershipSignup/CreateMembershipSignupCommand.cs` — has `Segmentation` already.
- `src/.../Application/Models/MembershipSignupResultDto.cs` — exposes `TrialStartsOn`, `TrialEndsOn`, `TrialStatus`, `CreatesPaidSubscription`, `CustomerMessage`.
- `src/.../Application/Models/SubscriptionMessageDto.cs`.
- `src/.../Application/Services/OldSubscriptionService.cs` — reads signups for overview; the `nudgeToReviewProfile` rule already treats `Segmentation.SegmentationB` differently for signups.

### Infrastructure / UI (read for impact only)
- `src/.../Infrastructure/Repositories/InMemoryMembershipSignupRepository.cs` — seed data uses Segmentation A.
- `src/.../Infrastructure/Repositories/InMemorySubscriptionRepository.cs` — `signup-user-01` is `MembershipSignupSubscription`.
- `src/.../UI/Controllers/MembershipSignupController.cs`, `SubscriptionMessageController.cs`, `SubscriptionAvailabilityController.cs` — pass-through; no changes required.

### Tests
- `tests/.../MarketSubscriptionPolicyTests.cs` — currently asserts SegmentationB rejects MembershipSignup (will break).
- `tests/.../MembershipSignupTests.cs` — currently asserts SegmentationB throws `BusinessRuleException` (will break).
- `tests/.../SubscriptionMessageTests.cs` — covers existing Market B community-membership shorthand.
- `tests/.../Util/TestFactory.cs`, `Util/FixedTimeProvider.cs` — test plumbing.

### Documentation
- `docs/market-b-subscription-model.md` — "Market B does not offer ... offline event signup trial at launch" — this statement will become outdated.
- `docs/membership-signup.md`, `docs/subscription-products.md`.

---

## 2. Recommended implementation path

The change touches three concerns: **availability**, **trial semantics**, and
**customer-facing message**. Treat them as separate small changes that compose.

### Step 1 — Availability (MarketSubscriptionPolicy)
1. Update `MarketSubscriptionPolicy.IsMembershipSignupAvailable(Segmentation segmentation)`
   to also return `true` for `Segmentation.SegmentationB`. Easiest form: enumerate
   the markets explicitly rather than gating on `GetSubscriptionModel`, since
   "membership signup available" is now orthogonal to the broad `MarketSubscriptionModel`.
2. Update `MarketSubscriptionPolicy.IsSubscriptionAvailable` so that for the
   `CommunityOnlySubscriptionModel` branch, `MembershipSignupSubscription` is
   also reported as available (currently only `CommunityMembershipSubscription`
   is allowed).
3. Leave `MarketSubscriptionModel` enum **unchanged** for now (see Assumption A).
   Document that Market B's model remains `CommunityOnlySubscriptionModel`
   with `MembershipSignup` as an additional offering — and rename in a follow-up
   ticket if stakeholders agree.

### Step 2 — Trial semantics (MembershipSignupService)
1. In `MembershipSignupService.StartOfflineEventSignupAsync`, branch on
   `command.Segmentation`:
   - `SegmentationA`: keep the existing 30-day trial (`TrialEndsOn = now.AddDays(30)`,
     `TrialStatus = SubscriptionStatus.Trial`).
   - `SegmentationB`: skip the trial. Two reasonable representations — pick
     one based on Assumption B:
     - **Preferred:** make `MembershipSignup.TrialStartsOn` and `TrialEndsOn`
       nullable, leave them `null`, and set
       `MembershipSignupResultDto.TrialStatus = SubscriptionStatus.Active`
       (or `None`).
     - **Fallback (no entity change):** set `TrialStartsOn = TrialEndsOn = now`
       (zero-length trial) and set `TrialStatus = SubscriptionStatus.Active`.
2. `CreatesPaidSubscription` already defaults to `true` on the command — Market B
   signups will simply create a paid subscription directly.

### Step 3 — Market-specific customer message (SubscriptionMessageService)
The current `if (segmentation == SegmentationB)` early-return returns a single
generic text for **all** flow types and forces `RefersToMembershipSignup = false`.
That is incorrect once Market B supports the membership-signup flow.

Refactor so that the lookup is `(flowType, segmentation)` based:
1. Add a Market B branch inside the `flowType` switch:
   - `OnlineSubscription` for SegmentationB → keep returning a sensible default
     (this flow is not actually offered in Market B, but the service should
     stay total).
   - `CommunityMembershipSubscription` for SegmentationB → keep the existing
     "Your community subscription is activated." text.
   - `MembershipSignup` for SegmentationB → **new** text supplied by
     marketing/product, with `RefersToMembershipSignup = true`. Placeholder
     until copy is confirmed: e.g., `"Welcome — your community membership is active."`
     (no trial language, since there is no trial).
2. Drop the early-return so all paths flow through one switch; this also makes
   the `OnlineSubscription` + SegmentationB case explicit rather than
   accidentally generic.
3. Populate `SubscriptionMessageDto.Segmentation` (currently unset by the
   service) so consumers can tell which market the message is for.

### Step 4 — Documentation
Update `docs/market-b-subscription-model.md`:
- Remove the "does not offer ... offline event signup trial at launch" sentence.
- Add: "Market B offers `MembershipSignupSubscription` without the standard
  30-day free trial. Customers are signed up directly to a paid community
  subscription. The customer-facing message is market-specific."

### Step 5 (optional, defer) — Model naming
If product confirms that Market B's offering will keep growing, add a new
`MarketSubscriptionModel` value (e.g., `CommunityWithMembershipSignupModel`)
and switch Market B over. Not required for this ticket and listed as
Assumption A.

---

## 3. Tests to add / update

All in `tests/SkillStarLearning.SubscriptionRules.UnitTests/`.

### `MarketSubscriptionPolicyTests.cs`
- **Update** `SegmentationB_DoesNotAllowOnlineSubscriptionOrMembershipSignup`
  → split into two tests:
  - `SegmentationB_DoesNotAllowOnlineSubscription` (still asserts `false`).
  - `SegmentationB_AllowsMembershipSignup` (now asserts `true`).
- **Add** `SegmentationB_AllowsMembershipSignupSubscriptionType` — asserts
  `policy.IsSubscriptionAvailable(SegmentationB, MembershipSignupSubscription)` is `true`.
- **Add** `SegmentationB_ModelIsCommunityOnly` (regression) — confirms
  `GetSubscriptionModel(SegmentationB) == CommunityOnlySubscriptionModel`
  to lock in Assumption A.

### `MembershipSignupTests.cs`
- **Remove or rewrite** `SubscribeOfflineMember_IsBlockedWhenMarketDoesNotOfferSignupTrial`
  — Market B is no longer blocked. Replace with:
  - `SegmentationB_Signup_DoesNotCreateTrial` — asserts the resulting DTO has
    no trial window (`TrialEndsOn == TrialStartsOn`, or nulls if Assumption B
    chooses nullable fields) and `TrialStatus != SubscriptionStatus.Trial`.
- **Add** `SegmentationB_Signup_UsesMarketSpecificCustomerMessage` — asserts
  `result.CustomerMessage` matches the Market B membership-signup copy and
  does **not** match the SegmentationA copy.
- **Add** `SegmentationB_Signup_CreatesPaidSubscription` — asserts
  `CreatesPaidSubscription` is `true` for Market B signups.
- **Keep** `SignupCreatesTrialSubscription_ButDoesNotCreatePaidSubscription_WhenNoConsent`
  (Segmentation A trial path unchanged).

### `SubscriptionMessageTests.cs`
- **Add** `MarketB_MembershipSignup_UsesMarketSpecificMessage` — asserts the
  text differs from the Market A signup text and `RefersToMembershipSignup` is
  `true`.
- **Keep** `MarketB_SubscriptionMessageUsesLocalShorthandForCommunityMembership`
  (regression for the existing community shorthand).
- **Add** `MarketB_OnlineSubscription_StillReturnsADefaultMessage` — guards
  the refactor that drops the segmentation-first early return.

### (Optional) Availability-level test
A `SubscriptionAvailabilityServiceTests` file does not exist today. If we want
end-to-end coverage of the DTO surface, add one with:
- `SegmentationB_Availability_IncludesMembershipSignup` — asserts
  `MembershipSignupAvailable == true` and `OnlineSubscriptionAvailable == false`.

---

## 4. Assumptions

**A. Market subscription model name stays put.**
`MarketSubscriptionModel.CommunityOnlySubscriptionModel` will continue to
describe Market B even though it now offers two subscription types. The
existing test `SegmentationB_CommunityMemebrshipSubscription` and the
seeded `signup-user-01` (already `MembershipSignupSubscription`) make the
narrow availability change low-risk; the enum rename is a larger refactor that
should be its own ticket. **Confirm with product before locking in.**

**B. Representing "no trial" on the entity.**
`MembershipSignup.TrialStartsOn` / `TrialEndsOn` are non-nullable today.
Preferred approach is to make them nullable `DateTime?` and use `null` for
the Market B case. If avoiding a Core entity change is required for this
ticket, fall back to setting `TrialStartsOn == TrialEndsOn == now` and rely on
`TrialStatus != Trial` to express "no trial window." Either way,
`MembershipSignupResultDto.TrialStatus` for Market B will be
`SubscriptionStatus.Active` (the customer is paid immediately) — confirm with
billing that this is correct vs. `None`.

**C. Customer message copy is provided by product/marketing.**
The implementation will use a placeholder string for the Market B membership
signup message and the test will assert it differs from the Market A message
and references membership (rather than asserting exact text), so copy can be
swapped without breaking tests once provided.

**D. `CreatesPaidSubscription` semantics in Market B.**
The Market B flow always creates a paid subscription on the spot (no trial,
no cancel-to-avoid-charge window). `CreateMembershipSignupCommand.CreatesPaidSubscription`
already defaults to `true`; we will ignore an explicit `false` value for
Market B or throw a `BusinessRuleException` — leaning toward **throw**, to
surface integration bugs early. **Decision needed.**

**E. UI / availability endpoint is the consumer of the new availability flag.**
`SubscriptionAvailabilityDto.MembershipSignupAvailable` already exists; the
front-end is expected to use it to show the signup form for Market B. No
controller changes required.

**F. The "documentation folder" requested in the task is `docs/`.**
This file lives alongside `market-b-subscription-model.md`,
`membership-signup.md`, and `subscription-products.md`. There is no separate
top-level `documentation/` directory.

---

## Out of scope (explicitly not changing)
- The `NewSubscriptionService` / `OldSubscriptionService` split — refactoring
  is tracked under TECHDEBT-123 per `docs/refactoring-notes.md`.
- The `SubscriptionAccount` model and renewal/payment flow.
- `UpdateSubscriptionSettingsHandler` membership-field validation, which is
  scoped to `CommunityMembershipSubscription` only.
