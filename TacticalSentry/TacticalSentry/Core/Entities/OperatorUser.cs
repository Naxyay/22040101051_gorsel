namespace TacticalSentry.Core.Entities
{
    public class OperatorUser : BaseEntity
    {
        public string Username { get; set; }
        public string Password { get; set; } 
        public string ServiceNumber { get; set; }
        public int ClearanceLevel { get; set; } 
    }
}