# Subscription service refactoring notes

IMPORTANT!!!
`NewSubscriptionService` was a temporary solution, because we thought that generation of subscription details summary (aka overview) should be moved there.
Old `SubscriptionService` was optimistically renamed to `OldSubscriptionService`, because we thought that next time we make changes after launch offline membership, we deprecate it completely.
That's because billing address and consent info were moved to Privacy module so we didn't need them in dtos - and at that time there was no point in whole profile (we didn't really use other properties).
Still waiting from legal regarding the widget. Continue using OldSubscriptionService so far, refactoring task TECHDEBT-123 has been registered. Still unclear what would happen in connection with "Membership Signup"-campaign.

