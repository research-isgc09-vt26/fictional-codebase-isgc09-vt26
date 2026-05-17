# S3-P-01 — Market B MembershipSignupSubscription (no trial, market-specific message)

## Goal

Market Segmentation B can now offer `MembershipSignupSubscription`. The Market B
flavour must:

1. **not** include the standard 30-day free trial that Market A receives, and
2. surface a **market-specific** customer-facing subscription message.

Today Market B is blocked from membership signup by both the policy layer
(`MarketSubscriptionPolicy`) and the service layer
(`MembershipSignupService` rejects the request via `BusinessRuleException`), and
its message branch in `SubscriptionMessageService` always returns the same
"community subscription is activated" string regardless of flow type.

---

## 1) Files inspected

### Domain / enums

- `src/SkillStarLearning.SubscriptionRules.Core/Enums/Segmentation.cs` — confirms
  the two markets (`SegmentationA = 1`, `SegmentationB = 2`).
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/MarketSubscriptionModel.cs`
  — `StandardMixedSubscriptionModel` vs. `CommunityOnlySubscriptionModel`.
  Market B is currently mapped to the latter.
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/SubscriptionType.cs` —
  `MembershipSignupSubscription = 3` already exists.
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/MembershipSignupType.cs` —
  `OnlineSignup` / `OfflineSignup` (default `OnlineSignup`).
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/SubscriptionMessageFlowType.cs`
  — `MembershipSignup = 3` flow already exists.

### Entities

- `src/SkillStarLearning.SubscriptionRules.Core/Entities/MembershipSignup.cs` —
  has `Segmentation`, `TrialStartsOn`/`TrialEndsOn`, `CreatesPaidSubscription`,
  `StaffMember` etc. No "trial enabled" flag today.
- `src/SkillStarLearning.SubscriptionRules.Core/Entities/SubscriptionAccount.cs`
  — `SubscriptionType` lives here; not directly changed by this story.
- `src/SkillStarLearning.SubscriptionRules.Core/Entities/SubscriptionMessage.cs`
  — placeholder entity, not used by `SubscriptionMessageService` today.

### Application / services & contracts

- `src/SkillStarLearning.SubscriptionRules.Application/Services/MarketSubscriptionPolicy.cs`
  — current gate. `IsSubscriptionAvailable` only allows
  `CommunityMembershipSubscription` for Market B;
  `IsMembershipSignupAvailable` returns `false` for Market B.
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/IMarketSubscriptionPolicy.cs`
  — interface (signatures stay the same).
- `src/SkillStarLearning.SubscriptionRules.Application/Services/MembershipSignupService.cs`
  — throws `BusinessRuleException` when `IsMembershipSignupAvailable` is `false`,
  and unconditionally sets `TrialEndsOn = now.AddDays(30)`.
- `src/SkillStarLearning.SubscriptionRules.Application/Services/SubscriptionMessageService.cs`
  — Market B branch is a single hard-coded string; `flowType` is currently
  ignored for Market B.
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Features/CreateMembershipSignup/CreateMembershipSignupCommand.cs`
  — `Segmentation` is `required`, `CreatesPaidSubscription` defaults to `true`.
- `src/SkillStarLearning.SubscriptionRules.Application/Models/MembershipSignupResultDto.cs`
  — output DTO. `TrialStatus`, `TrialStartsOn`, `TrialEndsOn`,
  `CreatesPaidSubscription`, `CustomerMessage`.
- `src/SkillStarLearning.SubscriptionRules.Application/Services/SubscriptionAvailabilityService.cs`
  and `Models/SubscriptionAvailabilityDto.cs` — read-side projection that the UI
  uses to discover what Market B can offer. Currently exposes
  `MembershipSignupAvailable`.
- `src/SkillStarLearning.SubscriptionRules.Application/Services/OldSubscriptionService.cs`
  — `ToOverview` already special-cases Market B for
  `RequiresMembershipProfileReview`; **no change required** but flagged because
  it touches `signupInfo.Segmentation`.
- `src/SkillStarLearning.SubscriptionRules.Application/ApplicationServiceRegistration.cs`
  — DI wiring confirms `IMarketSubscriptionPolicy`,
  `ISubscriptionMessageService`, `IMembershipSignupService` are all scoped; no
  registration change needed.

### UI / controllers (review-only)

- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/MembershipSignupController.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/SubscriptionAvailabilityController.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/SubscriptionMessageController.cs`

No route/signature changes are required — the new behaviour flows through the
existing service layer.

### Tests

- `tests/.../MarketSubscriptionPolicyTests.cs` —
  `SegmentationB_DoesNotAllowOnlineSubscriptionOrMembershipSignup` is
  contradicted by this story.
- `tests/.../MembershipSignupTests.cs` —
  `SubscribeOfflineMember_IsBlockedWhenMarketDoesNotOfferSignupTrial` will need
  re-targeting; the "happy path" test only covers Market A.
- `tests/.../SubscriptionMessageTests.cs` — has the Market B alignment test
  added in MKT-B-126 but no Market B + MembershipSignup coverage.
- `tests/.../Util/TestFactory.cs` — already builds `MembershipSignupService`
  with the policy + message service; no factory change required.
- `tests/.../Util/FixedTimeProvider.cs` — useful for asserting a deterministic
  zero-day trial.

### Documentation

- `docs/market-b-subscription-model.md` — currently states "Market B does not
  offer online-only subscription or offline event signup trial at launch!!!".
  This claim becomes stale and must be updated as part of the implementation
  PR.
- `docs/membership-signup.md`, `docs/subscription-products.md`,
  `docs/refactoring-notes.md` — read for context; no edits required.

### Git history (context only)

- `MKT-B-101` introduced `MarketSubscriptionPolicy` and the segmentation model.
- `MKT-B-118` fixed Market B to allow `CommunityMembershipSubscription` via
  `IsSubscriptionAvailable`.
- `MKT-B-126` extended `SubscriptionMessageService` with the Market B branch.

This story (`S3-P-01`) is the next iteration along that thread.

---

## 2) Recommended implementation path

Smallest-blast-radius change set. Each step is independently reviewable.

### Step 1 — Allow `MembershipSignupSubscription` in the Market B policy

File: `src/SkillStarLearning.SubscriptionRules.Application/Services/MarketSubscriptionPolicy.cs`

- In `IsSubscriptionAvailable`, broaden the `CommunityOnlySubscriptionModel`
  branch so it also returns `true` for `SubscriptionType.MembershipSignupSubscription`.
  Rather than continuing to layer booleans, switch on `subscriptionType`
  inside the `CommunityOnlySubscriptionModel` arm so the set of allowed
  products is explicit and discoverable. Example shape:

  ```csharp
  MarketSubscriptionModel.CommunityOnlySubscriptionModel => subscriptionType is
      SubscriptionType.CommunityMembershipSubscription
      or SubscriptionType.MembershipSignupSubscription,
  ```

- In `IsMembershipSignupAvailable`, return `true` for both
  `StandardMixedSubscriptionModel` and `CommunityOnlySubscriptionModel`.
  Equivalently: drop the model check and return `true` for any segmentation
  that maps to a known model (still guards against unknown markets via
  `GetSubscriptionModel`).

**Why this shape, not a rename / new model?** The enum value
`CommunityOnlySubscriptionModel` is now a slight misnomer. A rename
(`CommunityAndSignupSubscriptionModel`) or a brand-new model is in scope only
if the team wants to advertise the change in the public API. For this story I
recommend **keeping the enum name** and adding a follow-up tech-debt note;
otherwise the change ripples into every consumer of `MarketSubscriptionModel`
without functional benefit.

### Step 2 — Skip the standard trial in Market B's signup flow

File: `src/SkillStarLearning.SubscriptionRules.Application/Services/MembershipSignupService.cs`

- Branch on `command.Segmentation` when building the `MembershipSignup`
  entity. For Market B, set `TrialEndsOn = now` (zero-day trial, i.e. no trial
  period) and report `TrialStatus = SubscriptionStatus.Active` in the result
  DTO instead of `SubscriptionStatus.Trial`. Leave `CreatesPaidSubscription`
  at the command-provided value (default `true`); for Market B the absence of a
  trial means the subscription is paid from day one, which matches the default.
- Keep the policy check (`IsMembershipSignupAvailable`) at the top of the
  method; after Step 1 it now succeeds for Market B.
- Audit-log message stays as-is.

Recommended shape (illustrative — final wording matches house style):

```csharp
var trialEndsOn = command.Segmentation == Segmentation.SegmentationB
    ? now
    : now.AddDays(30);

var trialStatus = trialEndsOn == now
    ? SubscriptionStatus.Active
    : SubscriptionStatus.Trial;
```

A small inline comment justifying the Market B exception (e.g. "Market B
launches `MembershipSignupSubscription` without the 30-day online trial — see
`docs/market-b-subscription-model.md`") is warranted because the rule isn't
obvious from the call site.

### Step 3 — Provide a Market B + MembershipSignup-specific message

File: `src/SkillStarLearning.SubscriptionRules.Application/Services/SubscriptionMessageService.cs`

- Replace the single "all flow types collapse to one string" Market B branch
  with a `switch` on `flowType` (mirroring the Market A branch), so that:
  - `SubscriptionMessageFlowType.MembershipSignup` returns a market-specific
    message with `RefersToMembershipSignup = true`. **Placeholder copy** (see
    Assumptions): `"Your community membership signup is complete."`
  - `OnlineSubscription` and `CommunityMembershipSubscription` keep returning
    the existing `"Your community subscription is activated."` string with
    `RefersToMembershipSignup = false`, so MKT-B-126 behaviour is preserved.
- Throw `ArgumentOutOfRangeException` for unknown flow types, consistent with
  the existing Market A branch.

### Step 4 — Documentation refresh

File: `docs/market-b-subscription-model.md`

- Replace the "does not offer ... offline event signup trial at launch"
  sentence with a statement that Market B now offers
  `MembershipSignupSubscription` **without** the standard 30-day trial, and
  uses its own customer-facing message. Cross-link to this plan / the JIRA
  ticket (`S3-P-01`).
- Keep the stakeholder-language note ("they call community membership just
  'subscription'").

No changes needed to `subscription-products.md` or `membership-signup.md`.

### Step 5 — Read-side projection sanity check (no code change expected)

`SubscriptionAvailabilityService.GetAvailability` already derives
`MembershipSignupAvailable` from the policy. Once Step 1 lands, Market B's
availability response will flip to `MembershipSignupAvailable = true`
automatically. Add a test (see §3) but expect no code change here.

`OldSubscriptionService.ToOverview` already handles
`signupInfo.Segmentation == SegmentationB` for the "review profile" nudge.
**Verify** but don't change.

### Suggested commit shape

One commit, `S3-P-01 - Offer MembershipSignupSubscription in Market B without
trial`, touching the three service files, the docs file, and the new/updated
tests. Splitting policy and service into separate commits is acceptable but
adds noise; the change is small enough to land as one.

---

## 3) Tests to add / update

All MSTest, following the conventions in
`tests/SkillStarLearning.SubscriptionRules.UnitTests/`. New cases use
`FixedTimeProvider` for time-deterministic assertions and
`TestFactory.CreateMembershipSignupService(...)` for wiring.

### `MarketSubscriptionPolicyTests.cs`

| Change | Test |
| --- | --- |
| **Update / split** | `SegmentationB_DoesNotAllowOnlineSubscriptionOrMembershipSignup` — narrow to `SegmentationB_DoesNotAllowOnlineSubscription` (still asserting `IsSubscriptionAvailable(B, OnlineSubscription)` is `false`). Remove the `IsMembershipSignupAvailable` assertion. |
| **Add** | `SegmentationB_AllowsMembershipSignupSubscription` — `IsSubscriptionAvailable(SegmentationB, MembershipSignupSubscription)` is `true`. |
| **Add** | `SegmentationB_AllowsMembershipSignup` — `IsMembershipSignupAvailable(SegmentationB)` is `true`. |
| **Keep** | Existing Market A cases and `SegmentationB_CommunityMemebrshipSubscription` (note: keep the existing typo so unrelated history is untouched). |

### `MembershipSignupTests.cs`

| Change | Test |
| --- | --- |
| **Update / replace** | `SubscribeOfflineMember_IsBlockedWhenMarketDoesNotOfferSignupTrial` — Market B is no longer the negative case. Either delete it or repurpose to assert that an undefined/`(Segmentation)0` value still throws (via `BusinessRuleException` or `ArgumentOutOfRangeException`, depending on how the policy surfaces it). |
| **Add** | `SignupForMarketB_DoesNotIncludeStandardTrial` — call `StartOfflineEventSignupAsync` with `Segmentation.SegmentationB` using a `FixedTimeProvider`; assert `result.TrialEndsOn == result.TrialStartsOn` and `result.TrialStatus == SubscriptionStatus.Active`. |
| **Add** | `SignupForMarketB_UsesMarketSpecificCustomerMessage` — assert that `result.CustomerMessage` matches the Market B placeholder copy and differs from the Market A copy. Use `StringAssert.Contains` to stay resilient to minor wording changes. |
| **Add (optional)** | `SignupForMarketA_StillUsesThirtyDayTrial` — regression guard so a future change to the Market B branch cannot accidentally short-circuit Market A. Set `TrialEndsOn == now.AddDays(30)`. |

### `SubscriptionMessageTests.cs`

| Change | Test |
| --- | --- |
| **Add** | `MarketB_MembershipSignup_UsesMarketSpecificMessage` — assert that `GetMessage(MembershipSignup, SegmentationB)` returns the Market B placeholder copy **and** `RefersToMembershipSignup == true`. |
| **Keep** | `MarketB_SubscriptionMessageUsesLocalShorthandForCommunityMembership` — important regression guard for the existing `CommunityMembershipSubscription` flow. |

### Subscription-availability coverage (new or extend existing)

No `SubscriptionAvailabilityServiceTests.cs` exists today. **Optional but
recommended**: add one focused test asserting
`GetAvailability(SegmentationB).MembershipSignupAvailable == true` and
`OnlineSubscriptionAvailable == false`. Skip if the team prefers not to grow
test scope; the policy-layer tests already cover the underlying logic.

### Not changing

- `NewOldSubscriptionServiceTests.cs` — its
  `MembershipSignupSubscription_IncludesSignupInfo` test uses Market A
  fixtures (`signup-user-01` has `Segmentation = SegmentationA`) and is
  unaffected. Leave as-is rather than adding Market B fixtures to
  `InMemoryMembershipSignupRepository` / `InMemorySubscriptionRepository` for
  this story — that scope grows quickly and is not required to validate the
  rule.

---

## 4) Assumptions and open questions

These were called out during inspection. Confirm before/during code review.

1. **"No standard free trial" = zero-day trial, immediate `Active` status.**
   Primary interpretation: a `MembershipSignup` record is still created (so
   the offline-event audit trail, staff attribution, and the
   `OldSubscriptionService` overview projection all keep working), but
   `TrialEndsOn == TrialStartsOn` and the returned `TrialStatus` is
   `Active`, not `Trial`. **Alternative** (worth flagging to the team): skip
   the `MembershipSignup` record entirely and have the offline event create a
   paid `SubscriptionAccount` directly. That is a larger change — it would
   ripple into `OldSubscriptionService.ToOverview` (which surfaces
   `SignupInfo`) and the in-memory repository fixtures. Recommend confirming
   with the product owner before committing.

2. **`CreatesPaidSubscription` default stays `true`.** The command's default
   already matches the new Market B reality (paid from day one). No new
   server-side override is necessary; clients can still set it explicitly.

3. **Customer message wording is a placeholder.** Proposed:
   `"Your community membership signup is complete."`. Mirrors the
   "community" vocabulary used elsewhere for Market B (`docs/market-b-subscription-model.md`)
   while distinguishing the signup flow from the plain community
   subscription. The exact string needs sign-off from copy/legal — flagging
   as the same kind of decision that was made in `MKT-B-126`.

4. **`MarketSubscriptionModel.CommunityOnlySubscriptionModel` is not
   renamed.** Smallest blast radius. The enum value's literal meaning is now
   slightly misleading ("community + offline signup" rather than
   "community-only"). Suggest a follow-up tech-debt ticket if the
   accumulated drift starts to confuse new readers.

5. **`MembershipSignupType` (`OnlineSignup` / `OfflineSignup`) is unchanged.**
   The signup type on Market B is still `OfflineSignup` semantically — the
   trial is absent, not the offline-event origin.

6. **`SubscriptionMessageDto.Segmentation` is not populated by the service
   today.** Pre-existing gap; out of scope for this story but worth a
   follow-up if any consumer starts to need it.

7. **Backwards compatibility.** No public API (controllers, DTOs, command
   shape) changes. Behavioural changes are gated by `Segmentation`, so
   existing Market A clients are unaffected. Existing Market B clients that
   previously received a `BusinessRuleException` on the signup endpoint now
   succeed — this is an intentional, story-level behaviour change but worth
   calling out in the PR description so QA expects the new 2xx path.

8. **`SubscriptionMessage` entity in `Core/Entities/` is currently unused.**
   I did not consider it part of the message-resolution path because
   `SubscriptionMessageService` returns DTOs directly. If the team is mid-way
   through introducing it as a data-backed source, this plan may need to
   shift — please confirm.
