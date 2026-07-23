using UnityEngine;
using UnityEngine.UI;
using System;

// todo: reimplement update avatar
public class ChooseAvatarView : PopUpView
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Image avatarImage;

    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button chooseButton;


    // ==================================================
    // Private Fields
    // ==================================================
    private CellColor avatar;

    private UIButton leftUIButton;
    private UIButton rightUIButton;
    private UIButton chooseUIButton;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    protected override void OnValidate()
    {
        base.OnValidate();

        Debug.Assert(avatarImage != null, "Avatar image not set in choose avatar view");

        Debug.Assert(leftButton != null, "Left button not set in choose avatar view");
        Debug.Assert(rightButton != null, "Right button not set in choose avatar view");
        Debug.Assert(chooseButton != null, "Choose button not set in choose avatar view");
    }

    protected override void Awake()
    {
        base.Awake();

        leftButton.onClick.AddListener(PreviousAvatar);
        rightButton.onClick.AddListener(NextAvatar);

        //chooseButton.onClick.AddListener(ChooseAvatar);

        chooseUIButton = UIButtonFactory.SendPatch(chooseButton, Host, () => new AvatarPatch(avatar));  // patch always needs to send the current value
    }

    // ==================================================
    // Base Class Methods
    // ==================================================

    protected override void SetInfo(IUIData data)
    {
        if (data is not ProfileData profile)
        {
            Debug.LogError($"Data type mismatch, wanted ProfileData, recieved {data?.GetType().Name}");
            return;
        }

        avatar = profile.Avatar;
        SetAvatarSprite();
    }


    // ==================================================
    // Private Methods
    // ==================================================

    /*
    private void ChooseAvatar()
    {
        Host.PatchUpdate(new AvatarPatch(avatar));
    }
    */

    private void PreviousAvatar()
    {
        avatar--;
        if (avatar == CellColor.Empty)
            avatar = CellColor.WildCard;

        SetAvatarSprite();
    }

    private void NextAvatar()
    {
        if (avatar == CellColor.WildCard)
            avatar = CellColor.Empty;
        avatar++;

        SetAvatarSprite();
    }

    private void SetAvatarSprite()
    {
        avatarImage.sprite = SpriteDatabase.Instance.GetSprite(avatar);
    }
}
