using UnityEngine;
using UnityEngine.UI;

public class HowToView2 : PopUpView
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button backButton;

    public void OnNextClicked()
    {
        // add next how to view to the stack
    }

    public void OnBackClicked()
    {
        // pop this view from the stack
    }
}
