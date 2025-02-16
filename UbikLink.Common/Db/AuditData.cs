namespace UbikLink.Common.Db
{
    public  class AuditData(DateTime createdAt, Guid createdBy, DateTime modifiedAt, Guid modifiedBy)
    {
        public DateTime CreatedAt { get; private set; } = createdAt;
        public Guid CreatedBy { get; private set; } = createdBy;
        public DateTime ModifiedAt { get; private set; } = modifiedAt;
        public Guid ModifiedBy { get; private set; } = modifiedBy;
    }
}
