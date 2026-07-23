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
    private CellColor currentAvatar;

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

        chooseButton.onClick.AddListener(ChooseAvatar);
    }

    // ==================================================
    // Base Class Methods
    // ==================================================

    // actually just remove this
    protected override void InitializeData(IUIData initData) { }

    protected override void SetInfo(IUIData data)
    {
        if (data is not ProfileData profile)
        {
            Debug.LogError($"Data type mismatch, wanted ProfileData, recieved {data?.GetType().Name}");
            return;
        }

        currentAvatar = profile.Avatar;
        SetAvatarSprite();
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private void ChooseAvatar()
    {
        Host.PatchUpdate(new AvatarPatch(currentAvatar));
    }

    private void PreviousAvatar()
    {
        currentAvatar--;
        if (currentAvatar == CellColor.Empty)
            currentAvatar = CellColor.WildCard;

        SetAvatarSprite();
    }

    private void NextAvatar()
    {
        if (currentAvatar == CellColor.WildCard)
            currentAvatar = CellColor.Empty;
        currentAvatar++;

        SetAvatarSprite();
    }

    private void SetAvatarSprite()
    {
        avatarImage.sprite = SpriteDatabase.Instance.GetSprite(currentAvatar);
    }
}
