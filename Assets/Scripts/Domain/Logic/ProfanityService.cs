using UnityEngine;
using PF = ProfanityFilter.ProfanityFilter;

// potentially find a new service?
    // leaning towards using this one and doing some preprocessing myself

public class ProfanityService
{
    private PF filter;

    public ProfanityService()
    {
        filter = new PF();
    }

    public bool ContainsProfanity(string word)
    {
        // check for possible substrings? camelcase, _ separation

        return filter.ContainsProfanity(word);  // checks without scunthorpe

        // here you need to do your own logic checking
        // also maybe add ability to report if a username gets past my checking
    } 
}
