using WorkflowEngine.Models; 

namespace WorkflowEngine.Validators
{
    public class WorkflowValidator
    {
        public static (bool IsValid, string? Error) ValidateDefinition(WorkflowDefinition definition)
        {
            
            var stateIds = definition.States.Select(s => s.Id).ToList();
            if (stateIds.Count != stateIds.Distinct().Count())
            {
                return (false, "Duplicate state IDs found");
            }

            
            var actionIds = definition.Actions.Select(a => a.Id).ToList();
            if (actionIds.Count != actionIds.Distinct().Count())
            {
                return (false, "Duplicate action Ids");
            }

            
            var initialStates = definition.States.Where(s => s.IsInitial).ToList();
            if (initialStates.Count == 0)
            {
                return (false, "No initial state found. Please check again");
            }
            if (initialStates.Count > 1)
            {
                return (false, "Multiple initial states found");
            }

           
            foreach (var action in definition.Actions)
            {
               
                foreach (var fromState in action.FromStates)
                {
                    if (!stateIds.Contains(fromState))
                    {
                        return (false, $"Action '{action.Id}' references unknown fromState '{fromState}'");
                    }
                }

                
                if (!stateIds.Contains(action.ToState))
                {
                    return (false, $"Action '{action.Id}' references unknown toState '{action.ToState}'");
                }
            }

            return (true, null);
        }
    }
}

