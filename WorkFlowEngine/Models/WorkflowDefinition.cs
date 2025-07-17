namespace WorkflowEngine.Models
{
    public class WorkflowDefinition
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required List<State> States { get; set; }
        public required List<WorkflowAction> Actions { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

       
        public State? GetInitialState() => States.FirstOrDefault(s => s.IsInitial);
        public State? GetState(string stateId) => States.FirstOrDefault(s => s.Id == stateId);
        public WorkflowAction? GetAction(string actionId) => Actions.FirstOrDefault(a => a.Id == actionId);
    }
}
