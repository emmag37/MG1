using UnityEngine;

public class TutorialViewContainer : PopUpView
{
    public override void Show()
    {
        base.Show();

        Manager.SwitchTutorial(TutorialViewType.Tutorial1);
    }
}
