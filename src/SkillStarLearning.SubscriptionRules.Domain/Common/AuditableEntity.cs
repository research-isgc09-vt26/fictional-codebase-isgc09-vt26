namespace SkillStarLearning.SubscriptionRules.Domain.Common
{
    public abstract class AuditableEntity
    {
        public required DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
