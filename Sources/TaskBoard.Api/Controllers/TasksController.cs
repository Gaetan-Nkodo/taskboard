using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TaskBoard.Application.Requests;
using TaskBoard.Application.UseCases.Tasks;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Route("api/v1/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly UpdateTaskHandler _updateHandler;
    private readonly DeleteTaskHandler _deleteHandler;
    private readonly MoveTaskHandler _moveHandler;

    public TasksController(
        UpdateTaskHandler updateHandler,
        DeleteTaskHandler deleteHandler,
        MoveTaskHandler moveHandler)
    {
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _moveHandler = moveHandler;
    }

    [HttpPut("{taskId:guid}")]
    public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] UpdateTaskRequest request)
    {
        var userId = GetUserId();
        await _updateHandler.Handle(taskId, userId, request);
        return NoContent();
    }

    [HttpDelete("{taskId:guid}")]
    public async Task<IActionResult> DeleteTask(Guid taskId)
    {
        var userId = GetUserId();
        await _deleteHandler.Handle(taskId, userId);
        return NoContent();
    }

    [HttpPatch("{taskId:guid}/move")]
    public async Task<IActionResult> MoveTask(Guid taskId, [FromBody] MoveTaskRequest request)
    {
        var userId = GetUserId();
        try
        {
            await _moveHandler.Handle(taskId, userId, request);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    private Guid GetUserId()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "sub");
        return claim is null ? Guid.Empty : Guid.Parse(claim.Value);
    }
}
