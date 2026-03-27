using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Application.Requests;
using TaskBoard.Application.UseCases.Tasks;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Route("tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly CreateTaskHandler _createHandler;
    private readonly UpdateTaskHandler _updateHandler;
    private readonly DeleteTaskHandler _deleteHandler;

    public TasksController(
        CreateTaskHandler createHandler,
        UpdateTaskHandler updateHandler,
        DeleteTaskHandler deleteHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    // POST /tasks/{boardId}
    [HttpPost("{boardId:guid}")]
    public async Task<IActionResult> CreateTask(Guid boardId, [FromBody] CreateTaskRequest request)
    {
        var taskId = await _createHandler.Handle(boardId, request);
        return CreatedAtAction(nameof(CreateTask), new { id = taskId }, new { id = taskId });
    }

    // PUT /tasks/{taskId}
    [HttpPut("{taskId:guid}")]
    public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] UpdateTaskRequest request)
    {
        await _updateHandler.Handle(taskId, request);
        return NoContent();
    }

    // DELETE /tasks/{taskId}
    [HttpDelete("{taskId:guid}")]
    public async Task<IActionResult> DeleteTask(Guid taskId)
    {
        await _deleteHandler.Handle(taskId);
        return NoContent();
    }
}
