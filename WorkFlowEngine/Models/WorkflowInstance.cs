namespace WorkflowEngine.Models
{
    public class WorkflowInstance
    {
        public required string Id { get; set; }
        public required string DefinitionId { get; set; }
        public required string CurrentStateId { get; set; }
        public List<HistoryEntry> History { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastModified { get; set; } = DateTime.UtcNow;
    }

    public class HistoryEntry
    {
        public required string ActionId { get; set; }
        public required string FromStateId { get; set; }
        public required string ToStateId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
