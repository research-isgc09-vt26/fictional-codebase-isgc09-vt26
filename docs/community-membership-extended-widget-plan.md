# Extended widget for `CommunityMembershipSubscription` — implementation plan

## Goal

Add an "extended widget" surface for users whose `SubscriptionAccount.SubscriptionType ==
SubscriptionType.CommunityMembershipSubscription`. The extended widget must expose the same
content as the ordinary subscription overview (`SubscriptionOverviewDto`):

- Contact details (`Email`, `PhoneNumber`, `BillingAddress`, `FullName`, `PreferredDisplayName`)
- Other community preferences (`LocalCommunityRegion`, `AllowsEventCommunication`,
  `AccessibilityNotes`, `EmergencyContactPreference`, `HasAcceptedMembershipTerms`)
- The profile-review nudge (`RequiresMembershipProfileReview`) when applicable

The existing simple widget (`GET /api/subscription-widget/{userId}` →
`SimpleSubscriptionSummaryDto`) stays unchanged for non-community users.

## 1. Files inspected

### Source
- `src/SkillStarLearning.SubscriptionRules.Application/Models/SimpleSubscriptionSummaryDto.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Models/SubscriptionOverviewDto.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Models/SubscriptionProfileDto.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Models/MembershipSignupResultDto.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Models/SubscriptionAvailabilityDto.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Models/SubscriptionMessageDto.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Services/NewSubscriptionService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Services/OldSubscriptionService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Services/MembershipSignupService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Services/MarketSubscriptionPolicy.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Services/SubscriptionAvailabilityService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Services/SubscriptionMessageService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/INewSubscriptionService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/IOldSubscriptionService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/IMarketSubscriptionPolicy.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/IMembershipSignupService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/ISubscriptionMessageService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/ISubscriptionAvailabilityService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Persistence/ISubscriptionRepository.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Persistence/IUserProfileRepository.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Persistence/IMembershipSignupRepository.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Features/UpdateSubscriptionSettings/*.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Features/CreateMembershipSignup/CreateMembershipSignupCommand.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Infrastructure/IAuditLogWriter.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Exceptions/NotFoundException.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Exceptions/BusinessRuleException.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/ApplicationServiceRegistration.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Entities/SubscriptionAccount.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Entities/UserProfile.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Entities/MembershipSignup.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Entities/SubscriptionMessage.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Common/AuditableEntity.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/SubscriptionType.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/SubscriptionStatus.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/PaymentStatus.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/Segmentation.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/MarketSubscriptionModel.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/MembershipSignupType.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/SubscriptionMessageFlowType.cs`
- `src/SkillStarLearning.SubscriptionRules.Infrastructure/Repositories/InMemorySubscriptionRepository.cs`
- `src/SkillStarLearning.SubscriptionRules.Infrastructure/Repositories/InMemoryUserProfileRepository.cs`
- `src/SkillStarLearning.SubscriptionRules.Infrastructure/Repositories/InMemoryMembershipSignupRepository.cs`
- `src/SkillStarLearning.SubscriptionRules.Infrastructure/AuditLogWriter/AuditLogWriter.cs`
- `src/SkillStarLearning.SubscriptionRules.Infrastructure/InfrastructureServiceRegistration.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Program.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/SubscriptionWidgetController.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/SubscriptionSettingsController.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/SubscriptionAvailabilityController.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/MembershipSignupController.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/SubscriptionMessageController.cs`

### Tests
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/NewOldSubscriptionServiceTests.cs`
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/MarketSubscriptionPolicyTests.cs`
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/MembershipSignupTests.cs`
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/SubscriptionMessageTests.cs`
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/Util/TestFactory.cs`
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/Util/FixedTimeProvider.cs`

### Docs
- `docs/refactoring-notes.md` — explains why the `Old`/`New` split exists and asks us to keep using
  `OldSubscriptionService` until `TECHDEBT-123`.
- `docs/membership-signup.md`
- `docs/subscription-products.md`
- `docs/market-b-subscription-model.md`

### Project files
- All `.csproj` files (`Core`, `Application`, `Infrastructure`, `UI`, `UnitTests`, `Domain`). The
  `SkillStarLearning.SubscriptionRules.Domain` project still exists on disk but is **not**
  referenced by any other project — it is dead code and intentionally left untouched.

## 2. Recommended implementation path

### Design choices (confirmed with the user)
- **API shape**: a new dedicated route `GET /api/subscription-widget/{userId}/extended` that
  returns `SubscriptionOverviewDto` for community members and `404 Not Found` for any other
  subscription type or unknown user. The existing widget route stays untouched.
- **Service layer**: keep `NewSubscriptionService` as the entry point but compose
  `IOldSubscriptionService` to perform the existing fetch + projection (`ToOverview`). This
  honours the `docs/refactoring-notes.md` directive (no collapsing of `Old`/`New` before
  `TECHDEBT-123`) and reuses the existing nudge rule verbatim.

### Step-by-step

1. **Contract — `INewSubscriptionService`**
   Add a new method:
   ```csharp
   Task<SubscriptionOverviewDto?> GetExtendedWidgetAsync(
       string userId,
       CancellationToken cancellationToken = default);
   ```
   The return is `SubscriptionOverviewDto?` — `null` means "the user exists but is not eligible
   for the extended widget" (i.e., not a `CommunityMembershipSubscription`). Unknown users
   continue to surface via `NotFoundException`, mirroring `OldSubscriptionService`.

2. **Implementation — `NewSubscriptionService`**
   - Inject `IOldSubscriptionService` alongside the existing `ISubscriptionRepository`.
   - Implement `GetExtendedWidgetAsync`:
     ```csharp
     var account = await _subscriptionRepository.GetByUserIdAsync(userId, ct)
         ?? throw new NotFoundException(nameof(SubscriptionAccount), userId);

     if (account.SubscriptionType != SubscriptionType.CommunityMembershipSubscription)
     {
         return null;
     }

     return await _oldSubscriptionService.GetSubscriptionSettingsAsync(userId, ct);
     ```
   - The existing `GetSubscriptionWidgetSummaryAsync` is **not** changed. (Note: it has an
     unrelated latent NRE — when no account is found it dereferences `account.Status`. That bug
     is out of scope for this task and should be tracked separately.)

3. **Controller — `SubscriptionWidgetController`**
   - Add a new action:
     ```csharp
     [HttpGet("{userId}/extended", Name = "GetExtendedSubscriptionWidget")]
     [ProducesResponseType(typeof(SubscriptionOverviewDto), StatusCodes.Status200OK)]
     [ProducesResponseType(StatusCodes.Status404NotFound)]
     public async Task<ActionResult<SubscriptionOverviewDto>> GetExtended(
         string userId,
         CancellationToken cancellationToken)
     {
         var overview = await _newSubscriptionService.GetExtendedWidgetAsync(userId, cancellationToken);
         return overview is null ? NotFound() : Ok(overview);
     }
     ```
   - Inject `INewSubscriptionService` rather than the concrete `NewSubscriptionService` if we
     also want to align the existing constructor signature with the registered interface. To
     keep the diff small, we can stay on the concrete type for now; the registration in
     `ApplicationServiceRegistration` already binds both.
   - The `NotFoundException` thrown for unknown users will surface as a 500 today (no
     exception filter is configured). That is the same behaviour as
     `SubscriptionSettingsController`, so we will not introduce a new mapping here — it would
     be a cross-cutting change and belongs in its own ticket.

4. **DI registration — `ApplicationServiceRegistration.cs`**
   No new registrations needed: `INewSubscriptionService` and `IOldSubscriptionService` are
   already registered as scoped. The new constructor parameter on `NewSubscriptionService`
   resolves automatically.

5. **No new DTOs**
   `SubscriptionOverviewDto`, `SubscriptionProfileDto`, and `MembershipSignupResultDto` already
   carry everything the extended widget needs (contact details, preferences,
   `RequiresMembershipProfileReview`).

6. **Out of scope (intentionally not touched)**
   - `OldSubscriptionService.ToOverview` and its nudge rule — reused as-is.
   - The `SkillStarLearning.SubscriptionRules.Domain` project (dead code, unreferenced).
   - The duplicate-`UserId` seed bug in `InMemoryUserProfileRepository` for
     `community-user-02` (its `.UserId` is hard-coded to `"community-user-01"`). This will make
     the `Profile.UserId` field misleading when the extended widget is fetched for
     `community-user-02`. It is a pre-existing seed-data bug — flag in PR description, don't
     fix here.
   - The latent NRE in `NewSubscriptionService.GetSubscriptionWidgetSummaryAsync` when the
     account is missing.

## 3. Tests to add or update

All new tests live in `tests/SkillStarLearning.SubscriptionRules.UnitTests`. We follow the
existing MSTest style and reuse the in-memory repositories via `TestFactory`.

### Update `TestFactory.cs`
- `CreateSubscriptionService()` currently builds `NewSubscriptionService` with only the
  subscription repository. With the new dependency, change it to:
  ```csharp
  public static NewSubscriptionService CreateSubscriptionService()
  {
      return new NewSubscriptionService(
          CreateSubscriptionRepository(),
          CreateOldSubscriptionService());
  }
  ```
  Existing tests calling `CreateSubscriptionService()` continue to work.

### Add tests (suggested file: `NewOldSubscriptionServiceTests.cs` or a new
`ExtendedSubscriptionWidgetTests.cs`)

1. `ExtendedWidget_ReturnsOverview_ForCommunityMembershipSubscription`
   - Seed user: `community-user-01`.
   - Assert: result is non-null, `SubscriptionType == CommunityMembershipSubscription`,
     `Profile.PhoneNumber == "+45 55 66 77 88"`,
     `Profile.LocalCommunityRegion == "Copenhagen North"`,
     `Profile.AllowsEventCommunication == true`,
     `Profile.AccessibilityNotes == "Step-free access requested"`,
     `Profile.EmergencyContactPreference` is populated,
     `RequiresMembershipProfileReview == false`.

2. `ExtendedWidget_NudgesProfileReview_WhenCommunityMemberProfileIncomplete`
   - Seed user: `community-user-02` (no event-communication consent).
   - Assert: `RequiresMembershipProfileReview == true`,
     `Profile.AllowsEventCommunication == false`.

3. `ExtendedWidget_ReturnsNull_ForOnlineSubscription`
   - Seed user: `online-user-01`.
   - Assert: result is `null` (controller maps this to 404).

4. `ExtendedWidget_ReturnsNull_ForMembershipSignupSubscription`
   - Seed user: `signup-user-01`.
   - Assert: result is `null`.

5. `ExtendedWidget_ThrowsNotFound_ForUnknownUser`
   - Seed user: `"does-not-exist"`.
   - Assert: `NotFoundException` is thrown.

6. (Optional, but recommended) `ExtendedWidget_DelegatesToOldServiceForProjection`
   - Sanity test that the same `SubscriptionOverviewDto` fields are populated as
     `OldSubscriptionService.GetSubscriptionSettingsAsync` for the same user. This guards
     against the two paths drifting until `TECHDEBT-123` consolidates them.

No changes needed to `MarketSubscriptionPolicyTests`, `MembershipSignupTests`, or
`SubscriptionMessageTests` — they are orthogonal to this feature.

### Tests that need updating
- `SubscriptionService_ReturnsSimpleSubscriptionSummary_ForWidget` continues to work without
  changes once `CreateSubscriptionService()` is updated as described above.

### Controller tests
The project does not currently host controller-level / integration tests. We follow the
existing pattern and keep new tests at the service layer. If the team later adds a
`WebApplicationFactory`-based test project, a single happy-path/404 pair for
`GET /api/subscription-widget/{userId}/extended` would be the natural addition — out of scope
for this ticket.

## 4. Assumptions

1. **Eligibility is by subscription type only.** Account `Status` (Active, Trial, PastDue,
   Cancelled) does not gate the extended widget — only `SubscriptionType ==
   CommunityMembershipSubscription` does. The ordinary overview does not gate by status
   either, so we match its behaviour.
2. **Same content as the ordinary overview.** The phrase "in the same way as the ordinary
   subscription overview" is interpreted as "reuse `SubscriptionOverviewDto`", not "render the
   same UI block". No new DTO is added.
3. **Nudge rule is unchanged.** `OldSubscriptionService.ToOverview` already computes
   `RequiresMembershipProfileReview` correctly for community members (true when phone is
   empty or `AllowsEventCommunication == false`). We reuse it verbatim.
4. **`Old`/`New` services stay separate.** Per `docs/refactoring-notes.md`,
   `OldSubscriptionService` remains the source of truth for the rich projection until
   `TECHDEBT-123`. We compose, not collapse.
5. **Authorization / ownership is out of scope.** No existing controller validates that the
   caller owns `userId`. We do not add that check here.
6. **404 vs other status codes.** Returning `404 Not Found` for non-community users on the
   extended route treats the extended widget as a resource that does not exist for them. An
   alternative is `409 Conflict` or `400 Bad Request`; 404 is the most RESTfully neutral and
   matches the "resource not present" intuition for the front end.
7. **Pre-existing seed bug in `InMemoryUserProfileRepository`.** The entry keyed by
   `community-user-02` carries `UserId = "community-user-01"`. The extended widget will faithfully
   surface that wrong `UserId` until the seed is corrected. Out of scope.
8. **Latent NRE in `GetSubscriptionWidgetSummaryAsync` for unknown users.** Not introduced or
   touched by this work, but worth noting so the PR description can call it out.
9. **The `SkillStarLearning.SubscriptionRules.Domain` project is dead code** and is not part
   of this change set.
