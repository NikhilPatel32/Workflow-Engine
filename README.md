#  Workflow Engine

This is a simple and flexible Workflow Engine that I built using C#. It lets you define workflows by setting up states and actions, and then manage instances that move between these states based on the defined actions. The idea was to simulate real-life processes (like approval systems) in a way that's easy to extend and understand

---

## Folder Structure

```
WorkflowEngine/
│
├── Models/ # Core domain models (State, Action, Workflow, Instance)
│ └── State.cs
│ └── WorkflowAction.cs
│ └── WorkflowDefinition.cs
│ └── WorkflowInstance.cs
│
├── DTOs/ # Data Transfer Objects used for input/output
│ └── RequestDTOs.cs
│
├── Services/ # Contains the main workflow engine logic
│ └── WorkflowService.cs
│
├── Validators/ # Validation logic for definitions and transitions
│ └── WorkflowValidator.cs
│
├── Program.cs # Minimal hosting setup and route configuration
```

---

##  Features

-  Create workflow definitions using states and transitions
-  Mark states as Initial / Final and handle disabled states
-  Execute transitions (actions) between valid states
-  Track full history of each workflow instance

---

##  API Endpoints

| Method | Endpoint                          | Description                        |
|--------|-----------------------------------|------------------------------------|
| POST   | `/definitions`                    | Create a new workflow definition   |
| GET    | `/definitions`                    | Get all workflow definitions       |
| GET    | `/definitions/{definitionId}`     | Get a specific definition by ID    |
| POST   | `/instances?definitionId=...`     | Start a new instance               |
| GET    | `/instances`                      | Get all workflow instances         |
| GET    | `/instances/{instanceId}`         | Get a specific instance by ID      |
| POST   | `/instances/{instanceId}/actions` | Execute an action on an instance   |

---

##  Example JSON for Creating a Definition

```json
{
  "name": "LeaveApproval",
  "states": [
    { "id": "applied", "name": "Applied", "isInitial": true, "isFinal": false },
    { "id": "approved", "name": "Approved", "isFinal": true },
    { "id": "rejected", "name": "Rejected", "isFinal": true }
  ],
  "actions": [
    {
      "id": "approve",
      "name": "Approve Leave",
      "fromStates": ["applied"],
      "toState": "approved"
    },
    {
      "id": "reject",
      "name": "Reject Leave",
      "fromStates": ["applied"],
      "toState": "rejected"
    }
  ]
}

```
## How It Works (Brief)
- You define a workflow (states + transitions).

- You then start instances of that workflow (like live forms).

- As actions are performed, the instance moves from one state to another.

- Every move is stored in HistoryEntry.


## Technology Stack
C#

---

## Thoughtful TODOs
-  Add tagging support for states (critical, waiting, error, etc.)
-  Add notification system on state change 
