using PF = ProfanityFilter.ProfanityFilter;


public class ProfFilter : IProfanityFilter
{
    // ==================================================
    // Private Fields
    // ==================================================
    private PF profanityFilter = new PF();
    

    // ==================================================
    // Interface Methods
    // ==================================================

    public bool ContainsProfanity(string input) => profanityFilter.ContainsProfanity(input);
}
