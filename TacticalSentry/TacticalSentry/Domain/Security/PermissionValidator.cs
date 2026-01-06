using TacticalSentry.Core.Entities;

namespace TacticalSentry.Domain.Security
{
    public class PermissionValidator
    {
        public bool CanViewEvidence(OperatorUser user) => user.ClearanceLevel >= 3;
        public bool CanDeleteLogs(OperatorUser user) => user.ClearanceLevel == 5;
    }
}