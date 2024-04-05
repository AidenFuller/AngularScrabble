using Microsoft.AspNetCore.Mvc;
using Scrabble.Server.Models;
using Scrabble.Server.Services;

namespace Scrabble.Server.Controllers;

[Route("[controller]")]
public class BoardController : Controller
{
    private readonly BoardService _boardService;

    public BoardController(BoardService boardService)
    {
        _boardService = boardService ?? throw new ArgumentNullException(nameof(boardService));
    }

    [HttpPost]
    [Route("new")]
    public async Task<Guid> NewSession()
    {
        return _boardService.CreateSession();
    }

    [HttpGet]
    [Route("{sessionId:guid}")]
    public async Task<Board> GetBoard(Guid sessionId)
    {
        return _boardService.GetBoardForSession(sessionId);
    }



    [HttpPost]
    public async Task<bool> PlayWord(Guid boardId, string word, int x, int y, bool isHorizontal)
    {
        return true;
    }
}
