using UnityEngine;
using System.Text.RegularExpressions;

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

    public bool ValidUsername(string name)
    {
        // correct length
        if (name.Length < LowerBound || name.Length > UpperBound) return false;

        // no special chars
        if (!Regex.IsMatch(name, Chars)) return false;

        // no profanity
        if (profanityDetector.ContainsProfanity(name)) return false;

        return true;
    }

    // write a function to check for profanity
}
