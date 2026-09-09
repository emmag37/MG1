using UnityEngine;
using System.Text.RegularExpressions;


public enum InvalidInputType
{
    None,
    Short,
    SpecialChars,
    Profanity
}

public static class UsernameValidator
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
    private static ProfanityService profanityDetector = new ProfanityService();


    // ==================================================
    // Public Methods
    // ==================================================

    public static InvalidInputType IsUsernameValid(string name)
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
