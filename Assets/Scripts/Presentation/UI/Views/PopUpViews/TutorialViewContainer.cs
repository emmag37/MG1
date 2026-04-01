using UnityEngine;

public class TutorialViewContainer : PopUpView
{
    protected override void Awake()
    {
        base.Awake();

        Manager.SwitchTutorial(TutorialViewType.Tutorial1);
    }
}
