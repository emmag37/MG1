using UnityEngine;
using System.Text.RegularExpressions;

public enum InvalidInputType
{
    None,
    Short,
    SpecialChars,
    Profanity
}

[System.Serializable]
public class ValidatedUsername : ISerializationCallbackReceiver
{
    // ==================================================
    // Constants
    // ==================================================
    private const int LowerBound = 3;
    private const int UpperBound = 16;

    private const string Chars = @"^\w+$";

    // ==================================================
    // Serialized Fields - for JSON
    // ==================================================
    [SerializeField] private string username; // potentially make this read-only, but fine for now

    // ==================================================
    // Private Fields
    // ==================================================
    private ProfanityService profanityDetector;

    // ==================================================
    // Public Methods
    // ==================================================

    public ValidatedUsername()
    {
        profanityDetector = new ProfanityService();

        username = "default-username";
    }

    public void OnAfterDeserialize()
    {
        profanityDetector = new ProfanityService();

        // potentially re validate
    }

    public void OnBeforeSerialize() { } // for interface compile

    public InvalidInputType TrySetUsername(string newUsername)
    {
        InvalidInputType error = ValidUsername(newUsername);

        if (error == InvalidInputType.None)
            username = newUsername;

        return error;
    }

    public string GetUsername()
    {
        return username;
    }

    private InvalidInputType ValidUsername(string name)
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
