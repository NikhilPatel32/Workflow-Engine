﻿namespace WorkflowEngine.Models
{
    public class State
    {
        public required string Id { get; set; }
        public required string Name { get; set; } //for human friendly name
        public bool IsInitial { get; set; }
        public bool IsFinal { get; set; }
        public bool Enabled { get; set; } = true;
        public string? Description { get; set; }
    }
}
