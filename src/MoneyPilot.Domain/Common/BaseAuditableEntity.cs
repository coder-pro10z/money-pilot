using System;

namespace MoneyPilot.Domain.Common
{
    public abstract class BaseAuditableEntity
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public string? LastModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}