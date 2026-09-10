using UnityEngine;
using System.Text.RegularExpressions;
using PF = ProfanityFilter.ProfanityFilter;

/// <summary>
/// Static class to validate a username against length, special characters,
/// and profanity.
/// </summary>
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
    private static PF filter = new PF();


    // ==================================================
    // Public Methods
    // ==================================================

    /// <summary>
	/// Validates a username against length, special characters, and profanity.
	/// </summary>
	/// <param name="name">The username to validate.</param>
	/// <returns>
	/// <see cref="InvalidUsernameType.None"/> if the username passes all checks, otherwise the
	/// specific <see cref="InvalidUsernameType"/> indicating why it failed. Checks are evaluated
	/// in order (length, then characters, then profanity), so only the first failure is returned.
	/// </returns>
    public static InvalidUsernameType IsUsernameValid(string name)
    {
        // correct length
        if (name.Length < LowerBound)
            return InvalidUsernameType.Short;

        // no special chars
        if (!Regex.IsMatch(name, Chars))
            return InvalidUsernameType.SpecialChars;

        // no profanity
        if (filter.ContainsProfanity(name))
            return InvalidUsernameType.Profanity;

        return InvalidUsernameType.None;
    }
}
