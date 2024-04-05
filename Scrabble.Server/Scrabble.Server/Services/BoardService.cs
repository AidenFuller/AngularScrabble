using Scrabble.Server.Models;

namespace Scrabble.Server.Services;

public class BoardService
{
    private readonly BoardServiceConfiguration _configuration;
    private readonly IBoardValidator _boardValidator;

    public BoardService(BoardServiceConfiguration configuration, IBoardValidator boardValidator)
    {
        _configuration = configuration;
        _boardValidator = boardValidator;
    }

    public Board GetNewBoard()
    {
        return new Board(15);
    }
}
