using System.Security.Cryptography;

namespace Scrabble.Server.Services;

public class DictionaryLookupService
{
    private readonly HashSet<string> _dictionary;

    public DictionaryLookupService()
    {
        _dictionary = new();

        // _dictionary = File.ReadAllLines("dictionary.txt")
        //     .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public bool IsValidWord(string word)
    {
        return _dictionary.Contains(word);
    }
}
