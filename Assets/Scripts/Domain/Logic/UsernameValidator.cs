using UnityEngine;
using System.Text.RegularExpressions;

public enum InvalidInputType
{
    None,
    Short,
    SpecialChars,
    Profanity
}

public class UsernameValidator
{
    // ==================================================
    // Constants
    // ==================================================
    private const int LowerBound = 3;
    private const int UpperBound = 16;

    private const string Chars = @"^\w+$";

    // ==================================================
    // Private Fields
    // ==================================================
    private ProfanityService profanityDetector;

    // ==================================================
    // Public Methods
    // ==================================================

    public UsernameValidator()
    {
        profanityDetector = new ProfanityService();
    }

    public InvalidInputType ValidUsername(string name)
    {
        // correct length
        if (name.Length < LowerBound)
            return InvalidInputType.Short;

        // no special chars
        if (!Regex.IsMatch(name, Chars))
            return InvalidInputType.SpecialChars;

        // no profanity
        if (profanityDetector.ContainsProfanity(name))
            return InvalidInputType.Profanity;

        return InvalidInputType.None;
    }
}
