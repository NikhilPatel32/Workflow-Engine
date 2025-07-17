using WorkflowEngine.Services;
using WorkflowEngine.DTOs;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<WorkflowService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var workflowService = app.Services.GetRequiredService<WorkflowService>();


app.MapPost("/definitions", async ([FromBody] CreateWorkflowDefinitionDto dto) =>
{
    var result = await workflowService.CreateDefinitionAsync(dto);
    return result.Success
        ? Results.Ok(result.Definition)
        : Results.BadRequest(result.Error);
});

app.MapGet("/definitions", async () =>
{
    var definitions = await workflowService.GetAllDefinitionsAsync();
    return Results.Ok(definitions);
});

app.MapGet("/definitions/{definitionId}", async (string definitionId) =>
{
    var definition = await workflowService.GetDefinitionAsync(definitionId);
    return definition != null
        ? Results.Ok(definition)
        : Results.NotFound("Definition not found");
});


app.MapPost("/instances", async ([FromQuery] string definitionId) =>
{
    var result = await workflowService.StartInstanceAsync(definitionId);
    return result.Success
        ? Results.Ok(result.Instance)
        : Results.BadRequest(result.Error);
});

app.MapGet("/instances", async () =>
{
    var instances = await workflowService.GetAllInstancesAsync();
    return Results.Ok(instances);
});

app.MapGet("/instances/{instanceId}", async (string instanceId) =>
{
    var instance = await workflowService.GetInstanceAsync(instanceId);
    return instance != null
        ? Results.Ok(instance)
        : Results.NotFound("Instance not found");
});

app.MapPost("/instances/{instanceId}/actions", async (string instanceId, [FromBody] ExecuteActionDto dto) =>
{
    var result = await workflowService.ExecuteActionAsync(instanceId, dto.ActionId);
    return result.Success
        ? Results.Ok(result.Instance)
        : Results.BadRequest(result.Error);
});

app.Run();
