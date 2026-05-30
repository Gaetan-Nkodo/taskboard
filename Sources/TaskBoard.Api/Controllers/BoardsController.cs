using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TaskBoard.Application.Requests;
using TaskBoard.Application.UseCases.Boards;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Route("api/v1/boards")]
[Authorize]
public class BoardsController : ControllerBase
{
    private readonly CreateBoardHandler _createHandler;
    private readonly GetBoardHandler _getHandler;
    private readonly GetBoardsByUserHandler _getByUserHandler;
    private readonly UpdateBoardHandler _updateHandler;
    private readonly DeleteBoardHandler _deleteHandler;

    private readonly CreateTaskHandler _createTaskHandler;

    public BoardsController(
        CreateBoardHandler createHandler,
        GetBoardHandler getHandler,
        GetBoardsByUserHandler getByUserHandler,
        UpdateBoardHandler updateHandler,
        DeleteBoardHandler deleteHandler,
        CreateTaskHandler createTaskHandler)
    {
        _createHandler = createHandler;
        _getHandler = getHandler;
        _getByUserHandler = getByUserHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _createTaskHandler = createTaskHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetBoards()
    {
        var userId = GetUserId();
        var boards = await _getByUserHandler.Handle(userId);
        return Ok(boards);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBoard(Guid id)
    {
        var userId = GetUserId();
        var board = await _getHandler.Handle(id, userId);
        return Ok(board);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBoard([FromBody] CreateBoardRequest request)
    {
        var userId = GetUserId();
        var boardId = await _createHandler.Handle(userId, request);
        return CreatedAtAction(nameof(GetBoard), new { id = boardId }, new { id = boardId });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBoard(Guid id, [FromBody] UpdateBoardRequest request)
    {
        var userId = GetUserId();
        await _updateHandler.Handle(id, userId, request);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBoard(Guid id)
    {
        var userId = GetUserId();
        await _deleteHandler.Handle(id, userId);
        return NoContent();
    }

    // -----------------------------
    // NEW: Create Task (RESTful)
    // -----------------------------
    [HttpPost("{boardId:guid}/tasks")]
    public async Task<IActionResult> CreateTask(Guid boardId, [FromBody] CreateTaskRequest request)
    {
        var userId = GetUserId();
        var taskId = await _createTaskHandler.Handle(boardId, userId, request);
        return Created($"/api/v1/tasks/{taskId}", new { id = taskId });
    }

    private Guid GetUserId()
    {
        var id = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("nameid")?.Value ?? User.FindFirst("userid")?.Value;

        if (string.IsNullOrWhiteSpace(id))
            throw new UnauthorizedAccessException("Missing 'sub' claim in JWT.");

        return Guid.Parse(id);
    }
}
