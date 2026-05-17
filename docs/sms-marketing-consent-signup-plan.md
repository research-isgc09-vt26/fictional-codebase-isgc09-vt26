# SMS marketing consent for ordinary subscription sign-up — implementation plan

## Issue
Add optional SMS marketing consent to the ordinary subscription sign-up flow for
`OnlineSubscription` and `CommunityMembershipSubscription`. If SMS consent is
selected, a phone number must be provided.

## TL;DR
- **There is no existing "ordinary subscription sign-up flow" in this repository.**
  The only sign-up handler today is the offline-event flow
  (`MembershipSignupService.StartOfflineEventSignupAsync`) for the third
  product, `MembershipSignupSubscription`. So this ticket effectively introduces
  the ordinary sign-up flow as well as the consent field. This is the single
  largest assumption — flag with the team before merging code (see
  [Assumptions](#assumptions)).
- The model layer already has `UserProfile.PhoneNumber`. We add one new
  property, `UserProfile.SmsMarketingConsent`, and a new feature folder
  `Contracts/Features/CreateOrdinarySubscription/`.
- The conditional rule "phone required if consent" is enforced as a
  `BusinessRuleException` in the new handler, mirroring how
  `UpdateSubscriptionSettingsHandler.ValidateMembershipFields` enforces
  community-only rules today.

## 1. Files inspected

### Documentation
- `docs/subscription-products.md` — describes the three products.
- `docs/membership-signup.md` — clarifies that `MembershipSignup` is the
  offline-event product, not "ordinary sign-up".
- `docs/market-b-subscription-model.md` — Market B (`Segmentation.SegmentationB`)
  currently only supports `CommunityMembershipSubscription`; no online or
  offline-event signup.
- `docs/refactoring-notes.md` — explains the `OldSubscriptionService` /
  `NewSubscriptionService` split and notes that "billing address and consent
  info were moved to Privacy module" historically. Worth re-checking with team
  whether SMS consent should live here or in Privacy.

### Solution / project layout
- `SkillStarLearning.SubscriptionRules.slnx` — confirms the solution only
  references `Core`, `Application`, `Infrastructure`, `UI`, and `UnitTests`.
- `src/SkillStarLearning.SubscriptionRules.Domain/**` — **orphaned project**,
  not in the solution, not referenced by anything. Ignored for this work.
- `src/SkillStarLearning.SubscriptionRules.Core/SkillStarLearning.SubscriptionRules.Core.csproj`
- `src/SkillStarLearning.SubscriptionRules.Application/SkillStarLearning.SubscriptionRules.Application.csproj`
- `src/SkillStarLearning.SubscriptionRules.Infrastructure/SkillStarLearning.SubscriptionRules.Infrastructure.csproj`
- `src/SkillStarLearning.SubscriptionRules.UI/SkillStarLearning.SubscriptionRules.UI.csproj`
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/SkillStarLearning.SubscriptionRules.UnitTests.csproj`

### Domain / Core
- `src/SkillStarLearning.SubscriptionRules.Core/Entities/UserProfile.cs` —
  already has `PhoneNumber`, `AllowsEventCommunication`,
  `HasAcceptedMembershipTerms`. **No** marketing-consent field today.
- `src/SkillStarLearning.SubscriptionRules.Core/Entities/SubscriptionAccount.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Entities/MembershipSignup.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Entities/SubscriptionMessage.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Common/AuditableEntity.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/SubscriptionType.cs` —
  has `OnlineSubscription`, `CommunityMembershipSubscription`,
  `MembershipSignupSubscription`.
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/Segmentation.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/MarketSubscriptionModel.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/MembershipSignupType.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/SubscriptionMessageFlowType.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/PaymentStatus.cs`
- `src/SkillStarLearning.SubscriptionRules.Core/Enums/SubscriptionStatus.cs`

### Application
- `src/SkillStarLearning.SubscriptionRules.Application/ApplicationServiceRegistration.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Services/OldSubscriptionService.cs`
  — read-only; produces `SubscriptionOverviewDto`. The "nudge" rule today
  checks `AllowsEventCommunication` & `PhoneNumber` for community members.
- `src/SkillStarLearning.SubscriptionRules.Application/Services/NewSubscriptionService.cs`
  — read-only widget summary.
- `src/SkillStarLearning.SubscriptionRules.Application/Services/MembershipSignupService.cs`
  — offline-event sign-up. **Closest existing analogue** to the new ordinary
  flow; mirror its shape (command → policy check → entity creation → audit →
  result DTO).
- `src/SkillStarLearning.SubscriptionRules.Application/Services/MarketSubscriptionPolicy.cs`
  — already has `IsSubscriptionAvailable(Segmentation, SubscriptionType)`.
  Reuse to gate the new flow per market.
- `src/SkillStarLearning.SubscriptionRules.Application/Services/SubscriptionAvailabilityService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Services/SubscriptionMessageService.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Features/CreateMembershipSignup/CreateMembershipSignupCommand.cs`
  — template for shape of the new `CreateOrdinarySubscriptionCommand`.
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Features/UpdateSubscriptionSettings/UpdateSubscriptionSettingsCommand.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Features/UpdateSubscriptionSettings/UpdateSubscriptionSettingsHandler.cs`
  — template for handler shape (validate → mutate → persist → return overview).
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Features/UpdateSubscriptionSettings/IUpdateSubscriptionSettingsHandler.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Services/*.cs`
  — all six interfaces.
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Persistence/ISubscriptionRepository.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Persistence/IUserProfileRepository.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Persistence/IMembershipSignupRepository.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Infrastructure/IAuditLogWriter.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Models/SubscriptionProfileDto.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Models/SubscriptionOverviewDto.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Models/MembershipSignupResultDto.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Models/SimpleSubscriptionSummaryDto.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Models/SubscriptionAvailabilityDto.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Models/SubscriptionMessageDto.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Exceptions/BusinessRuleException.cs`
- `src/SkillStarLearning.SubscriptionRules.Application/Exceptions/NotFoundException.cs`

### Infrastructure / UI
- `src/SkillStarLearning.SubscriptionRules.Infrastructure/InfrastructureServiceRegistration.cs`
- `src/SkillStarLearning.SubscriptionRules.Infrastructure/AuditLogWriter/AuditLogWriter.cs`
- `src/SkillStarLearning.SubscriptionRules.Infrastructure/Repositories/InMemorySubscriptionRepository.cs`
- `src/SkillStarLearning.SubscriptionRules.Infrastructure/Repositories/InMemoryUserProfileRepository.cs`
- `src/SkillStarLearning.SubscriptionRules.Infrastructure/Repositories/InMemoryMembershipSignupRepository.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Program.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/MembershipSignupController.cs`
  — template for a `POST`-based "create" endpoint.
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/SubscriptionSettingsController.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/SubscriptionAvailabilityController.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/SubscriptionMessageController.cs`
- `src/SkillStarLearning.SubscriptionRules.UI/Controllers/SubscriptionWidgetController.cs`

### Tests
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/MarketSubscriptionPolicyTests.cs`
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/MembershipSignupTests.cs`
  — template for write-path tests (uses `FixedTimeProvider`).
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/NewOldSubscriptionServiceTests.cs`
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/SubscriptionMessageTests.cs`
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/Util/TestFactory.cs`
  — extend to wire up the new handler.
- `tests/SkillStarLearning.SubscriptionRules.UnitTests/Util/FixedTimeProvider.cs`

## 2. Recommended implementation path

The task is best modelled as a new "feature" command + handler, mirroring the
existing `CreateMembershipSignup` and `UpdateSubscriptionSettings` patterns.
All changes happen in the `Core` model, the `Application` layer (new feature,
DI registration, audit), the `UI` controller, and the test project. No
schema/EF work needed (the repositories are in-memory).

### Step 1 — Core: persist consent

In `src/SkillStarLearning.SubscriptionRules.Core/Entities/UserProfile.cs`,
add:

```csharp
public bool SmsMarketingConsent { get; set; }
```

(`PhoneNumber` already exists.) No new entity, no new enum — SMS consent is a
profile-level marketing preference, parallel to `AllowsEventCommunication`
(which is *event coordination*, not marketing — keep them distinct).

### Step 2 — Application: new feature folder

Create `src/SkillStarLearning.SubscriptionRules.Application/Contracts/Features/CreateOrdinarySubscription/`
with:

- `CreateOrdinarySubscriptionCommand.cs`
  - `string UserId`, `string Email`, `string BillingAddress`
  - `SubscriptionType SubscriptionType` — handler rejects
    `MembershipSignupSubscription`
  - `Segmentation Segmentation`
  - `bool SmsMarketingConsent`
  - `string? PhoneNumber` (nullable — required only when consent is true)
  - Community-only fields (kept optional at command level, enforced by handler
    when `SubscriptionType == CommunityMembershipSubscription`):
    `FullName`, `PreferredDisplayName`, `LocalCommunityRegion`,
    `AllowsEventCommunication`, `HasAcceptedMembershipTerms`,
    `AccessibilityNotes`, `EmergencyContactPreference`.
- `ICreateOrdinarySubscriptionHandler.cs`
- `CreateOrdinarySubscriptionHandler.cs` (sealed, follows
  `UpdateSubscriptionSettingsHandler` style):
  1. Validate `SubscriptionType` ∈ `{OnlineSubscription,
     CommunityMembershipSubscription}` → `BusinessRuleException` otherwise.
  2. `IMarketSubscriptionPolicy.IsSubscriptionAvailable(seg, type)` →
     `BusinessRuleException` if false. (This covers Market B blocking
     `OnlineSubscription`.)
  3. **SMS consent rule**: if `command.SmsMarketingConsent && string.IsNullOrWhiteSpace(command.PhoneNumber)`
     → `throw new BusinessRuleException("SMS marketing consent requires a phone number.")`.
  4. If `SubscriptionType == CommunityMembershipSubscription`, run the
     existing community-membership checks (region present, terms accepted).
     Consider extracting the validator that `UpdateSubscriptionSettingsHandler`
     already has into a small private helper or shared static — only if it
     stays simple; otherwise duplicate to avoid premature abstraction.
  5. Build a new `SubscriptionAccount` (defaults flagged in
     [Assumptions](#assumptions) #7) and a new `UserProfile` that includes
     `PhoneNumber` and `SmsMarketingConsent`. Persist via
     `ISubscriptionRepository.SaveAsync` and
     `IUserProfileRepository.SaveAsync`.
  6. Write an `IAuditLogWriter` entry — keep PII out (no phone number, just
     consent flag + type).
  7. Return a `SubscriptionOverviewDto` produced by
     `IOldSubscriptionService.ToOverview(...)` so the response shape stays
     consistent with `GET /api/subscription-settings/{userId}` and
     `PUT /api/subscription-settings/{userId}`.

### Step 3 — Application: surface consent in DTOs

- `Application/Models/SubscriptionProfileDto.cs` — add
  `bool SmsMarketingConsent { get; set; }`.
- `Application/Services/OldSubscriptionService.cs` — set
  `SmsMarketingConsent = profile.SmsMarketingConsent` in the projection inside
  `ToOverview`. No nudge logic change — see [Assumptions](#assumptions) #5.

### Step 4 — Application: align `UpdateSubscriptionSettings` (recommended, scoped)

To keep settings updates internally consistent with sign-up:

- `Application/Contracts/Features/UpdateSubscriptionSettings/UpdateSubscriptionSettingsCommand.cs`
  — add `bool SmsMarketingConsent`.
- `Application/Contracts/Features/UpdateSubscriptionSettings/UpdateSubscriptionSettingsHandler.cs`
  — apply the same "consent ⇒ phone required" rule **before** persisting and
  copy `command.SmsMarketingConsent` to `profile.SmsMarketingConsent`. This
  prevents a user from enabling consent later without a phone number.

If the team wants to keep the ticket strictly to sign-up, defer step 4 to a
follow-up.

### Step 5 — Application: DI

- `Application/ApplicationServiceRegistration.cs` — register
  `ICreateOrdinarySubscriptionHandler` → `CreateOrdinarySubscriptionHandler`
  (scoped, like the others).

### Step 6 — UI: expose the endpoint

Add a new controller `src/SkillStarLearning.SubscriptionRules.UI/Controllers/OrdinarySubscriptionController.cs`
modelled on `MembershipSignupController`:

- Route: `api/subscriptions`
- `HttpPost("ordinary", Name = "CreateOrdinarySubscription")`
- Body: `CreateOrdinarySubscriptionCommand`
- Returns `SubscriptionOverviewDto` (200 OK).

(Choosing a dedicated controller over extending
`SubscriptionSettingsController` keeps the existing GET/PUT controller focused
on the post-sign-up profile management it already does.)

### Step 7 — No persistence/schema changes needed

`InMemoryUserProfileRepository` round-trips the whole `UserProfile` instance,
so adding `SmsMarketingConsent` is automatically persisted. Real EF migration
work would be a follow-up if/when this project switches to a database.

## 3. Tests to add / update

All tests follow the existing MSTest style and use `Util/TestFactory.cs` for
wiring (extend it with a `CreateOrdinarySubscriptionHandler()` helper plus
shared in-memory repositories so tests can assert persisted state).

### New: `tests/.../CreateOrdinarySubscriptionTests.cs`
- `OnlineSubscription_CreatesAccount_WhenSmsConsentFalse_AndPhoneOmitted`
  — happy path, no consent, phone not required.
- `OnlineSubscription_CreatesAccount_WhenSmsConsentTrue_AndPhoneProvided`
  — happy path with consent; assert `profile.SmsMarketingConsent == true`,
  `profile.PhoneNumber` persisted.
- `OnlineSubscription_Throws_WhenSmsConsentTrue_AndPhoneMissing`
  — `BusinessRuleException`, no account/profile written.
- `OnlineSubscription_Throws_WhenSmsConsentTrue_AndPhoneWhitespace`
  — `BusinessRuleException` (covers the `IsNullOrWhiteSpace` branch).
- `CommunityMembershipSubscription_CreatesAccount_WithSmsConsent_SegmentationA`
- `CommunityMembershipSubscription_CreatesAccount_WithSmsConsent_SegmentationB`
  — verifies Market B path works for the only product available there.
- `CommunityMembershipSubscription_Throws_WhenSmsConsentTrue_AndPhoneMissing`
- `OnlineSubscription_Throws_InMarketB`
  — exercises `MarketSubscriptionPolicy.IsSubscriptionAvailable`.
- `MembershipSignupSubscription_Throws_FromOrdinaryFlow`
  — guardrail against routing offline signup through this handler.
- `CommunityMembershipSubscription_Throws_WhenRegionMissing`
- `CommunityMembershipSubscription_Throws_WhenTermsNotAccepted`
- `AuditLog_RecordsCreation_OnSuccess`
  — assert an entry was written (we can expose `AuditLogWriter.Entries` like
  `MembershipSignupTests` does via the in-memory writer).

### Update: `tests/.../NewOldSubscriptionServiceTests.cs`
- Add `OldSubscriptionService_IncludesSmsMarketingConsent_InOverview`:
  seed a user with `SmsMarketingConsent = true` and verify
  `overview.Profile.SmsMarketingConsent`.

### Update: `tests/.../Util/TestFactory.cs`
- Add `CreateOrdinarySubscriptionHandler` helper.
- Consider exposing a shared `InMemory*` repository instance so a test can
  both run the handler and read back the saved entity. Today
  `CreateOldSubscriptionService` and `CreateMembershipSignupService` each
  create *new* in-memory instances, which won't work for write+read assertions
  in the new tests. Adjust with a single shared instance per test (a
  `TestFactory.CreateScenario()` returning a small bundle, or accept
  repository instances as parameters).

### Update: `tests/.../MarketSubscriptionPolicyTests.cs`
- No behavior change needed. Optional: add an explicit assertion for
  `IsSubscriptionAvailable(SegmentationB, OnlineSubscription) == false` if not
  already covered — already covered by
  `SegmentationB_DoesNotAllowOnlineSubscriptionOrMembershipSignup`.

### Update (only if Step 4 included): `UpdateSubscriptionSettings` tests
- `UpdateSubscriptionSettings_Throws_WhenSmsConsentTrue_AndPhoneMissing`
- `UpdateSubscriptionSettings_PersistsSmsMarketingConsent`
  (No existing test file for the handler — add
  `UpdateSubscriptionSettingsTests.cs` if Step 4 is included.)

## 4. Assumptions

1. **The "ordinary subscription sign-up flow" does not exist yet in this
   repository.** The grep across `src/` shows no `CreateSubscription`,
   `StartSubscription`, `SubscribeOnline`, or similar handler; the only
   `HttpPost` is `MembershipSignupController` (offline event). I'm assuming
   the intent of the ticket is to *introduce* the sign-up flow here, with SMS
   consent built in. If the real sign-up lives in another service (e.g. an
   identity/billing system) and this repo just sees an existing
   `SubscriptionAccount`, the plan changes substantially — we'd add the
   `SmsMarketingConsent` field on `UserProfile` and wire it through an
   integration event/contract instead of a controller. **Please confirm with
   the team before implementing.**
2. **`AllowsEventCommunication` is *not* SMS marketing consent.** Today it
   gates community-event coordination (and contributes to the
   "incomplete profile" nudge in `OldSubscriptionService.ToOverview`).
   `SmsMarketingConsent` is a separate, optional marketing preference and
   needs its own boolean. We do not repurpose `AllowsEventCommunication`.
3. **SMS consent lives on `UserProfile`.** `docs/refactoring-notes.md` notes
   consent info historically moved to a Privacy module, but the current code
   still keeps consent-like flags on `UserProfile` (e.g.
   `HasAcceptedMembershipTerms`). Adding the new field here matches the
   established pattern. If the Privacy module is real and live, the field
   belongs there instead and this plan should be revisited.
4. **Phone number validation is non-empty/non-whitespace only.** Format
   validation (E.164, libphonenumber, country prefix) is out of scope and
   should be done by the UI or a dedicated module — consistent with how
   `BillingAddress` is treated today (free-text string).
5. **Existing nudge logic is left alone.** `OldSubscriptionService.ToOverview`
   currently nudges community members with no phone number or no
   `AllowsEventCommunication`. We do **not** retrofit it to also nudge for
   missing SMS consent — that's a product question. The new field is purely
   additive.
6. **`MembershipSignupSubscription` is excluded.** The task says "ordinary"
   and explicitly lists only the two non-offline products. If SMS consent
   should also be added to the offline-event sign-up
   (`MembershipSignupService.StartOfflineEventSignupAsync`), that's a separate
   ticket.
7. **Defaults for newly created `SubscriptionAccount`.** The ticket does not
   specify billing or status; I'd default to
   `Status = SubscriptionStatus.Trial`,
   `PaymentStatus = PaymentStatus.Pending`,
   `CanManageSubscription = true`,
   `RenewalDate = null` until billing rules clarify. These are not
   consent-related — flagging only because the new handler must produce a
   valid account.
8. **The orphaned `SkillStarLearning.SubscriptionRules.Domain` project is
   ignored.** It is not in `SkillStarLearning.SubscriptionRules.slnx`, no
   other project references it, and its `UserProfile`/`SubscriptionAccount`
   diverge from the live `Core` versions. All changes target `Core`.
9. **Market B remains restricted.** Market B (`Segmentation.SegmentationB`)
   only permits `CommunityMembershipSubscription`. The new handler relies on
   `IMarketSubscriptionPolicy.IsSubscriptionAvailable` for this; no policy
   change.
10. **Audit log records the consent decision but never the phone number.** To
    avoid PII in the audit trail, the entry should include the subscription
    type and consent flag only.
