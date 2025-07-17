using System.Collections.Concurrent;
using WorkflowEngine.DTOs;
using WorkflowEngine.Models; 
using WorkflowEngine.Validators;

namespace WorkflowEngine.Services
{
    public class WorkflowService
    {
        private readonly ConcurrentDictionary<string, WorkflowDefinition> _definitions = new();
        private readonly ConcurrentDictionary<string, WorkflowInstance> _instances = new();

        public async Task<(bool Success, string? Error, WorkflowDefinition? Definition)> CreateDefinitionAsync(CreateWorkflowDefinitionDto dto)
        {
            try
            {
                var definition = new WorkflowDefinition
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = dto.Name,
                    States = dto.States.Select(s => new State
                    {
                        Id = s.Id,
                        Name = s.Name,
                        IsInitial = s.IsInitial,
                        IsFinal = s.IsFinal,
                        Enabled = s.Enabled,
                        Description = s.Description
                    }).ToList(),
                    Actions = dto.Actions.Select(a => new WorkflowAction
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Enabled = a.Enabled,
                        FromStates = a.FromStates,
                        ToState = a.ToState,
                        Description = a.Description
                    }).ToList()
                };

                var validationResult = WorkflowValidator.ValidateDefinition(definition);
                if (!validationResult.IsValid)
                {
                    return (false, validationResult.Error, null);
                }

                _definitions[definition.Id] = definition;
                return (true, null, definition);
            }
            catch (Exception ex)
            {
                return (false, $"Error creating definition: {ex.Message}", null);
            }
        }

        public async Task<WorkflowDefinition?> GetDefinitionAsync(string definitionId)
        {
            _definitions.TryGetValue(definitionId, out var definition);
            return await Task.FromResult(definition);
        }

        public async Task<List<WorkflowDefinition>> GetAllDefinitionsAsync()
        {
            return await Task.FromResult(_definitions.Values.ToList());
        }

        public async Task<(bool Success, string? Error, WorkflowInstance? Instance)> StartInstanceAsync(string definitionId)
        {
            try
            {
                var definition = await GetDefinitionAsync(definitionId);
                if (definition == null)
                {
                    return (false, "Workflow definition not found", null);
                }

                var initialState = definition.GetInitialState();
                if (initialState == null)
                {
                    return (false, "No initial state found in definition", null);
                }

                var instance = new WorkflowInstance
                {
                    Id = Guid.NewGuid().ToString(),
                    DefinitionId = definitionId,
                    CurrentStateId = initialState.Id
                };

                _instances[instance.Id] = instance;
                return (true, null, instance);
            }
            catch (Exception ex)
            {
                return (false, $"Error starting instance: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string? Error, WorkflowInstance? Instance)> ExecuteActionAsync(string instanceId, string actionId)
        {
            try
            {
                var instance = await GetInstanceAsync(instanceId);
                if (instance == null)
                {
                    return (false, "Workflow instance not found", null);
                }

                var definition = await GetDefinitionAsync(instance.DefinitionId);
                if (definition == null)
                {
                    return (false, "Workflow definition not found", null);
                }

                var action = definition.GetAction(actionId);
                if (action == null)
                {
                    return (false, "Action not found in definition", null);
                }

                if (!action.Enabled)
                {
                    return (false, "Action is disabled", null);
                }

                var currentState = definition.GetState(instance.CurrentStateId);
                if (currentState == null)
                {
                    return (false, "Current state not found", null);
                }

                if (currentState.IsFinal)
                {
                    return (false, "Cannot execute actions on final states", null);
                }

                if (!action.FromStates.Contains(instance.CurrentStateId))
                {
                    return (false, $"Action '{actionId}' cannot be executed from current state '{instance.CurrentStateId}'", null);
                }

                var targetState = definition.GetState(action.ToState);
                if (targetState == null)
                {
                    return (false, "Target state not found", null);
                }

               
                var historyEntry = new HistoryEntry
                {
                    ActionId = actionId,
                    FromStateId = instance.CurrentStateId,
                    ToStateId = action.ToState
                };

                instance.History.Add(historyEntry);
                instance.CurrentStateId = action.ToState;
                instance.LastModified = DateTime.UtcNow;

                return (true, null, instance);
            }
            catch (Exception ex)
            {
                return (false, $"Error executing action: {ex.Message}", null);
            }
        }

        public async Task<WorkflowInstance?> GetInstanceAsync(string instanceId)
        {
            _instances.TryGetValue(instanceId, out var instance);
            return await Task.FromResult(instance);
        }

        public async Task<List<WorkflowInstance>> GetAllInstancesAsync()
        {
            return await Task.FromResult(_instances.Values.ToList());
        }
    }
}
