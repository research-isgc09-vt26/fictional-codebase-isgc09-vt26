# Plan: Extended widget for `CommunityMembershipSubscription` users

## Goal

Add an "extended" subscription widget for users whose `SubscriptionAccount.SubscriptionType == CommunityMembershipSubscription`. The extended widget must show the user's **contact details**, **other preferences**, and the **profile-review nudge** (when applicable), using the same rules and shape as the ordinary subscription overview (`SubscriptionOverviewDto`).

Out of scope: changing the existing simple widget contract, refactoring `OldSubscriptionService` (tracked under `TECHDEBT-123`), or touching the duplicate `Domain` namespace.

---

## 1. Files inspected

### Core / domain
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/SubscriptionType.cs` — confirms `CommunityMembershipSubscription = 2` is the discriminator.
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/SubscriptionStatus.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/PaymentStatus.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/Segmentation.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/MarketSubscriptionModel.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/MembershipSignupType.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/SubscriptionMessageFlowType.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Entities/SubscriptionAccount.cs` — fields the widget needs: `SubscriptionType`, `Status`, `RenewalDate`, `PaymentStatus`, `CanManageSubscription`.
- `src/SkillStarLearning.SubscriptionRules.Core/Entities/UserProfile.cs` — source of contact details (`Email`, `PhoneNumber`, `BillingAddress`) and preferences (`LocalCommunityRegion`, `AllowsEventCommunication`, `AccessibilityNotes`, `EmergencyContactPreference`, `HasAcceptedMembershipTerms`).
- `src/SkillStarLearning.SubscriptionRules.Core/Entities/MembershipSignup.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Entities/SubscriptionMessage.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Common/AuditableEntity.cs`

### Application
- `src/SkillStarLearning.SubscriptionRules.Application/Models/SubscriptionOverviewDto.cs` — the "ordinary subscription overview" shape we will mirror.
- `src/SkillStarLearning.SubscriptionRules.Application/Models/SubscriptionProfileDto.cs` — already covers all contact + preference fields.
- `src/SkillStarLearning.SubscriptionRules.Application/Models/SimpleSubscriptionSummaryDto.cs` — current widget response.
- `src/SkillStarLearning.SubscriptionRules.Application/Models/MembershipSignupResultDto.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Models/SubscriptionMessageDto.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Models/SubscriptionAvailabilityDto.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Services/OldSubscriptionService.cs` — **source of truth for the nudge rule** (`ToOverview`, lines 44–82). Nudge fires when either (a) a `MembershipSignup` exists and its `Segmentation != SegmentationB`, or (b) subscription type is `CommunityMembershipSubscription` and either `!AllowsEventCommunication` or `string.IsNullOrEmpty(PhoneNumber)`.
- `src/SkillStarLearning.SubscriptionRules.Application/Services/NewSubscriptionService.cs` — where the widget logic lives today; will be extended.
- `src/SkillStarLearning.SubscriptionRules.Application/Services/SubscriptionMessageService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Services/SubscriptionAvailabilityService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Services/MarketSubscriptionPolicy.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Services/MembershipSignupService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/INewSubscriptionService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/IOldSubscriptionService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/ISubscriptionMessageService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/IMarketSubscriptionPolicy.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/ISubscriptionAvailabilityService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/IMembershipSignupService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Persistence/ISubscriptionRepository.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Persistence/IUserProfileRepository.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Persistence/IMembershipSignupRepository.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Features/UpdateSubscriptionSettings/UpdateSubscriptionSettingsCommand.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Features/UpdateSubscriptionSettings/UpdateSubscriptionSettingsHandler.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Features/UpdateSubscriptionSettings/IUpdateSubscriptionSettingsHandler.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Features/CreateMembershipSignup/CreateMembershipSignupCommand.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Exceptions/NotFoundException.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Exceptions/BusinessRuleException.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/ApplicationServiceRegistration.cs`

### Infrastructure / UI
- `src/SkillStarLearning.SubscriptionRules.Infrastructure/Repositories/InMemorySubscriptionRepository.cs` — seeded accounts: `online-user-01`, `community-user-01`, `community-user-02`, `signup-user-01`.
- `src/SkillStarLearning.SubscriptionRules.Infrastructure/Repositories/InMemoryUserProfileRepository.cs` — `community-user-02` has `AllowsEventCommunication = false` (drives the nudge in existing tests).
- `src/SkillStarLearning.SubscriptionRules.Infrastructure/Repositories/InMemoryMembershipSignupRepository.cs`
- `src/SkillStarLearning.SubscriptionRules.Infrastructure/AuditLogWriter/AuditLogWriter.cs`
- `src/SkillStarLearning.SubscriptionRules.Infrastructure/InfrastructureServiceRegistration.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/SubscriptionWidgetController.cs` — `GET /api/subscription-widget/{userId}`; **the file the new endpoint will live in**.
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/SubscriptionSettingsController.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/MembershipSignupController.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/SubscriptionMessageController.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/SubscriptionAvailabilityController.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Program.cs`

### Tests
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/NewOldSubscriptionServiceTests.cs` — sets the pattern; `community-user-02` is the canonical "incomplete profile" case.
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/SubscriptionMessageTests.cs`
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/MembershipSignupTests.cs`
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/MarketSubscriptionPolicyTests.cs`
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/Util/TestFactory.cs`
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/Util/FixedTimeProvider.cs`

### Docs
- `docs/subscription-products.md` — confirms what `CommunityMembershipSubscription` is and what additional profile fields it implies.
- `docs/refactoring-notes.md` — explains the `Old`/`New` subscription-service split; `OldSubscriptionService` still owns the overview (and the nudge), pending `TECHDEBT-123`.
- `docs/membership-signup.md`
- `docs/market-b-subscription-model.md` — relevant for the signup-segmentation half of the nudge rule.

---

## 2. Recommended implementation path

### High-level design
- A **new dedicated endpoint** `GET /api/subscription-widget/{userId}/extended` is added. The simple widget at `/api/subscription-widget/{userId}` is left untouched, so existing callers and their tests are unaffected.
- The extended endpoint returns a new DTO (**`ExtendedSubscriptionWidgetDto`**) shaped after `SubscriptionOverviewDto`, populated for `CommunityMembershipSubscription` users only.
- For users without a CMS account (or no account at all), the extended endpoint returns **404 Not Found**. This keeps the response shape mono-typed and forces clients to fall back to the simple widget explicitly — no silent shape switching.
- The **profile-review nudge rule is extracted** into a shared helper (`MembershipProfileReviewPolicy`) so the same rule serves both `OldSubscriptionService.ToOverview` and the new code path. This is the only refactor allowed into pre-existing code; it is purely a move of one expression. The `OldSubscriptionService` is updated to call the helper; behavior is unchanged.

### Step-by-step

1. **Add DTO** `src/SkillStarLearning.SubscriptionRules.Application/Models/ExtendedSubscriptionWidgetDto.cs`.
   - Fields (mirrors `SubscriptionOverviewDto` so clients can render it "in the same way as the ordinary subscription overview"):
     - `string UserId` (required)
     - `SubscriptionType SubscriptionType`
     - `SubscriptionStatus Status`
     - `DateTime? RenewalDate`
     - `PaymentStatus PaymentStatus`
     - `bool CanManageSubscription`
     - `bool RequiresMembershipProfileReview`
     - `SubscriptionProfileDto Profile` (required — populated from `UserProfile`)
     - `MembershipSignupResultDto? SignupInfo` (always `null` here since CMS users are not from the offline signup flow, but the field is present for shape parity with the overview)
   - Sealed class, same style as `SubscriptionOverviewDto`.

2. **Extract the nudge rule** to `src/SkillStarLearning.SubscriptionRules.Application/Services/MembershipProfileReviewPolicy.cs`.
   - Static method `public static bool RequiresReview(SubscriptionAccount account, UserProfile profile, MembershipSignup? signup)`.
   - Body is the existing expression copied verbatim from `OldSubscriptionService.ToOverview` (lines 46–48):
     ```csharp
     (signup is not null && signup.Segmentation != Segmentation.SegmentationB)
     || (account.SubscriptionType == SubscriptionType.CommunityMembershipSubscription
         && (!profile.AllowsEventCommunication || string.IsNullOrEmpty(profile.PhoneNumber)));
     ```
   - **Modify `OldSubscriptionService.ToOverview`** to delegate to `MembershipProfileReviewPolicy.RequiresReview(...)`. Behavior is unchanged; existing tests must still pass.

3. **Extend `INewSubscriptionService`** at `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/INewSubscriptionService.cs`:
   ```csharp
   Task<ExtendedSubscriptionWidgetDto?> GetExtendedSubscriptionWidgetAsync(
       string userId,
       CancellationToken cancellationToken = default);
   ```
   - Returns `null` when there is no account, or when the account is not `CommunityMembershipSubscription`. The controller translates `null` to `404`.

4. **Implement in `NewSubscriptionService`** at `src/SkillStarLearning.SubscriptionRules.Application/Services/NewSubscriptionService.cs`:
   - Inject `IUserProfileRepository` and `IMembershipSignupRepository` alongside the existing `ISubscriptionRepository`.
   - Logic:
     1. Load `SubscriptionAccount` by `userId`. If `null` or `SubscriptionType != CommunityMembershipSubscription`, return `null`.
     2. Load `UserProfile`; if `null`, throw `NotFoundException(nameof(UserProfile), userId)` (matches `OldSubscriptionService` behavior — a CMS user without a profile is a data error, not a "no extended widget" condition).
     3. Load `MembershipSignup` (optional; always `null` in practice for a CMS account, but the call is symmetric with the overview and harmless).
     4. Compute `RequiresMembershipProfileReview` via `MembershipProfileReviewPolicy.RequiresReview(...)`.
     5. Map account + profile (+ signup if non-null) into `ExtendedSubscriptionWidgetDto`. Profile mapping is the same field-by-field copy used in `OldSubscriptionService.ToOverview` — consider extracting that into `SubscriptionProfileDto.FromEntity(...)` if duplication becomes a concern; not required for this story.
   - The existing `GetSubscriptionWidgetSummaryAsync` is **not** changed.

5. **Update DI** in `ApplicationServiceRegistration.cs`:
   - No new registration needed for the policy (it is a static helper).
   - `NewSubscriptionService` registration stays the same; the new constructor dependencies (`IUserProfileRepository`, `IMembershipSignupRepository`) are already registered in `InfrastructureServiceRegistration`.

6. **Wire the new endpoint** in `src/SkillStarLearning.SubscriptionRules.UI/Controllers/SubscriptionWidgetController.cs`:
   ```csharp
   [HttpGet("{userId}/extended", Name = "GetExtendedSubscriptionWidget")]
   [ProducesResponseType(typeof(ExtendedSubscriptionWidgetDto), StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   public async Task<ActionResult<ExtendedSubscriptionWidgetDto>> GetExtended(
       string userId,
       CancellationToken cancellationToken)
   {
       var dto = await _newSubscriptionService.GetExtendedSubscriptionWidgetAsync(userId, cancellationToken);
       return dto is null ? NotFound() : Ok(dto);
   }
   ```
   - Same controller, same dependency on `NewSubscriptionService` (consistent with how the existing widget is wired). The existing `Get(userId)` action is untouched.

### Why this path
- **No breaking change** to the existing `/api/subscription-widget/{userId}` contract: the simple-summary callers are unaffected.
- **Single source of truth** for the nudge rule via the policy helper — eliminates the drift risk that would otherwise appear the moment Old and New services compute the same thing in two places.
- **Matches the overview shape** the prompt asks for ("in the same way as the ordinary subscription overview"), without forcing `SubscriptionOverviewDto` itself to be reused (its `Profile` is `required`, which suits the overview-only use and avoids tempting future callers to misuse the same DTO for the simple summary).
- **Respects the `TECHDEBT-123` direction** in `docs/refactoring-notes.md`: the rich logic continues to migrate into `NewSubscriptionService`; `OldSubscriptionService` is only minimally edited to swap one inline expression for a call to the shared policy.

### Trade-offs considered
- **Polymorphic response on the existing endpoint** was rejected: it would force every existing client to deserialize an evolving DTO and would couple two different read shapes to one URL.
- **Reusing `SubscriptionOverviewDto` directly** from the widget service was rejected because the overview is owned by `OldSubscriptionService` and is on the deprecation path. A dedicated DTO keeps the widget contract movable.
- **Inlining the nudge rule** in two places was rejected because the rule is non-trivial (it combines an account-type branch and a signup-segmentation branch) and is exactly the kind of logic that drifts silently.

---

## 3. Tests to add or update

All tests use the existing `TestFactory` style and the seeded in-memory repositories.

### `tests/SkillStarLearning.SubscriptionRules.UnitTests/Util/TestFactory.cs` (update)
- Update `CreateSubscriptionService()` (the factory for `NewSubscriptionService`) to also pass `CreateUserProfileRepository()` and `CreateMembershipSignupRepository()` — needed by the new constructor.

### New file: `tests/SkillStarLearning.SubscriptionRules.UnitTests/ExtendedSubscriptionWidgetTests.cs`
Add the following test cases — each one is one focused assertion or a small cluster:

1. `ExtendedWidget_ReturnsContactDetailsAndPreferences_ForCommunityMembershipUser`
   - Arrange: `community-user-01` (complete profile).
   - Act: `GetExtendedSubscriptionWidgetAsync("community-user-01")`.
   - Assert: `SubscriptionType == CommunityMembershipSubscription`; `Profile.PhoneNumber == "+45 55 66 77 88"`; `Profile.Email == "john.doe@example.test"`; `Profile.LocalCommunityRegion == "Copenhagen North"`; `Profile.AllowsEventCommunication == true`; `Profile.AccessibilityNotes == "Step-free access requested"`; `Profile.EmergencyContactPreference` non-empty.

2. `ExtendedWidget_RaisesProfileReviewNudge_WhenAllowsEventCommunicationIsFalse`
   - Arrange: `community-user-02` (`AllowsEventCommunication = false`).
   - Assert: `RequiresMembershipProfileReview == true`.

3. `ExtendedWidget_RaisesProfileReviewNudge_WhenPhoneNumberIsEmpty`
   - Arrange: build a CMS account with the existing repos but mutate a profile to set `PhoneNumber = ""` (via repo `SaveAsync`), or — preferred — add a fresh seeded profile in a test-local repository setup.
   - Assert: nudge true.

4. `ExtendedWidget_DoesNotRaiseNudge_WhenProfileIsComplete`
   - Arrange: `community-user-01`.
   - Assert: `RequiresMembershipProfileReview == false`.

5. `ExtendedWidget_ReturnsNull_ForOnlineSubscriptionUser`
   - Arrange: `online-user-01`.
   - Assert: result is `null` (controller will translate to 404).

6. `ExtendedWidget_ReturnsNull_ForMembershipSignupSubscriptionUser`
   - Arrange: `signup-user-01`.
   - Assert: result is `null`.

7. `ExtendedWidget_ReturnsNull_WhenAccountIsMissing`
   - Arrange: `unknown-user`.
   - Assert: result is `null`.

8. `ExtendedWidget_ThrowsNotFound_WhenProfileMissingForCommunityUser`
   - Arrange: a CMS account exists but the matching profile is removed (or use a fresh test repo seeded with only the account).
   - Assert: `NotFoundException` is thrown — parity with `OldSubscriptionService.GetSubscriptionSettingsAsync`.

### `tests/SkillStarLearning.SubscriptionRules.UnitTests/NewOldSubscriptionServiceTests.cs` (touch lightly)
- No tests need to be removed. Re-running them must still pass after the policy extraction; treat that as a smoke check that `OldSubscriptionService.ToOverview` was not behavior-changed by the refactor.
- Specifically: `OldSubscriptionService_ReturnsMembershipProfileFields_WhenMembershipCommunitySubscription`, `OldSubscriptionService_CommunityMembershipSubscription_GetNudge_WhenIncompleteProfile`, and `SubscriptionService_MembershipSignupSubscription_IncludesSignupInfo` are the ones to watch.

### Optional: `MembershipProfileReviewPolicyTests.cs` (new, recommended)
- One direct unit test per branch of the nudge rule, against the policy helper:
  - CMS account + `AllowsEventCommunication=false` → true.
  - CMS account + empty `PhoneNumber` → true.
  - CMS account + complete profile → false.
  - Non-CMS account + no signup → false.
  - Non-CMS account + signup with `SegmentationA` → true.
  - Non-CMS account + signup with `SegmentationB` → false.
- This locks the rule down independent of either subscription service and is the test I'd lean on hardest going forward.

---

## 4. Assumptions

1. **"Ordinary subscription overview" = `SubscriptionOverviewDto`** returned by `OldSubscriptionService.GetSubscriptionSettingsAsync`. The extended widget mirrors its shape and reuses its nudge rule. If the team meant something else by "ordinary subscription overview", the DTO shape will need to be revisited.
2. **The nudge rule is the existing rule, unchanged.** Specifically the two-clause expression at `OldSubscriptionService.cs:46-48`. We do **not** introduce new triggers (e.g. missing `EmergencyContactPreference` or `AccessibilityNotes`) without a product decision.
3. **Contact details** for the widget are: `Email`, `PhoneNumber`, `BillingAddress`. **Other preferences** are: `LocalCommunityRegion`, `AllowsEventCommunication`, `AccessibilityNotes`, `EmergencyContactPreference`, `HasAcceptedMembershipTerms`. Both buckets fit in the existing `SubscriptionProfileDto`, so no new profile shape is required.
4. **Non-CMS users get 404** from `/api/subscription-widget/{userId}/extended`. The alternative — returning a 200 with the simple shape — was rejected to keep the response mono-typed. If product wants a graceful fallback, the controller can switch to returning the simple summary instead; the service signature already supports detecting the case via the `null` return.
5. **A CMS account without a matching `UserProfile` is a data integrity error**, not a "no extended widget" condition; it surfaces as `NotFoundException`, matching `OldSubscriptionService`. If product instead wants a 404 here, the service should return `null` for the missing-profile case too.
6. **CMS users are not expected to have a `MembershipSignup`.** The service still reads the signup repository for shape parity with `ToOverview`; the resulting `SignupInfo` is expected to be `null` in practice. No new business rule is asserted on this combination.
7. **Backward compatibility:** the existing `GET /api/subscription-widget/{userId}` and `SimpleSubscriptionSummaryDto` contract are untouched. Existing widget callers continue to work without changes.
8. **Authorization / market checks are unchanged.** The widget endpoints don't currently consult `IMarketSubscriptionPolicy`, and this task does not introduce a market gate on the extended endpoint. If a Market B (`CommunityOnlySubscriptionModel`) user can have a CMS account, they will see the extended widget — consistent with the policy at `MarketSubscriptionPolicy.cs:30` which allows CMS in that market.
9. **The latent NRE in the existing widget** (`NewSubscriptionService.cs:35`: dereferencing `account.Status` when `account` is `null`) is pre-existing and **out of scope** for this story. Flagging here only because the new endpoint passes through similar null-account territory and the new code path must be explicit about it (the recommendation handles this by returning `null` before any account dereference).
10. **The duplicate `Domain` namespace** (`src/SkillStarLearning.SubscriptionRules.Domain/...`) appears to be leftover from a prior rename and is not used by the wiring (`Program.cs` registers `Application` + `Infrastructure` only). It is intentionally left alone.
11. **Several controllers depend on concrete service classes** (e.g. `NewSubscriptionService`, `OldSubscriptionService`) while DI only registers their interfaces — this is a pre-existing wiring concern visible in `ApplicationServiceRegistration.cs` and the controller constructors. The plan does not introduce or fix this issue; the new endpoint follows the same convention as the existing one in the same controller for consistency.

---

## 5. Files the implementation will touch (summary)

**Add**
- `src/SkillStarLearning.SubscriptionRules.Application/Models/ExtendedSubscriptionWidgetDto.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Services/MembershipProfileReviewPolicy.cs`
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/ExtendedSubscriptionWidgetTests.cs`
- (optional) `tests/SkillStarLearning.SubscriptionRules.UnitTests/MembershipProfileReviewPolicyTests.cs`

**Modify**
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/INewSubscriptionService.cs` — add new method.
- `src/SkillStarLearning.SubscriptionRules.Application/Services/NewSubscriptionService.cs` — extra repo dependencies + new method.
- `src/SkillStarLearning.SubscriptionRules.Application/Services/OldSubscriptionService.cs` — delegate the nudge expression to `MembershipProfileReviewPolicy`. No behavior change.
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/SubscriptionWidgetController.cs` — add `GetExtended` action.
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/Util/TestFactory.cs` — update `CreateSubscriptionService` to satisfy the expanded constructor.

**Do not touch**
- The existing simple-widget endpoint and DTO.
- `Domain` namespace.
- DI registrations beyond what is already in place (no new interface registrations required).
