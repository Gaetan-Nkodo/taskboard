using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Application.Requests;
using TaskBoard.Application.UseCases.Boards;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Route("boards")]
[Authorize]
public class BoardsController : ControllerBase
{
    private readonly CreateBoardHandler _createHandler;
    private readonly GetBoardHandler _getHandler;
    private readonly GetBoardsByUserHandler _getByUserHandler;
    private readonly UpdateBoardHandler _updateHandler;
    private readonly DeleteBoardHandler _deleteHandler;

    public BoardsController(CreateBoardHandler createHandler, GetBoardHandler getHandler, GetBoardsByUserHandler getByUserHandler, UpdateBoardHandler updateHandler, DeleteBoardHandler deleteHandler)
    {
        _createHandler = createHandler;
        _getHandler = getHandler;
        _getByUserHandler = getByUserHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    // GET /boards
    [HttpGet]
    public async Task<IActionResult> GetBoards()
    {
        var userId = GetUserId();
        var boards = await _getByUserHandler.Handle(userId);
        return Ok(boards);
    }

    // GET /boards/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBoard(Guid id)
    {
        var board = await _getHandler.Handle(id);
        return Ok(board);
    }

    // POST /boards
    [HttpPost]
    public async Task<IActionResult> CreateBoard([FromBody] CreateBoardRequest request)
    {
        var userId = GetUserId();
        var boardId = await _createHandler.Handle(userId, request);
        return CreatedAtAction(nameof(GetBoard), new { id = boardId }, new { id = boardId });
    }

    // PUT /boards/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBoard(Guid id, [FromBody] UpdateBoardRequest request)
    {
        await _updateHandler.Handle(id, request);
        return NoContent();
    }

    // DELETE /boards/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBoard(Guid id)
    {
        await _deleteHandler.Handle(id);
        return NoContent();
    }

    private Guid GetUserId()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "id");
        return claim is null ? Guid.Empty : Guid.Parse(claim.Value);
    }
}
