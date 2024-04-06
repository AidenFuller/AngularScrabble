namespace Scrabble.Server.Models.Extensions;

public static class CellExtensions
{
    public static bool IsEmpty(this Cell cell) => cell.Value is null;
    public static bool IsCommitted(this Cell cell) => cell.Value == cell.CommittedValue;
}
