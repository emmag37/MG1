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
    private ProfileData profile;

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

    protected override void InitializeData(IUIData initData)
    {
        if (initData is not ProfileData profile)
        {
            Debug.Log($"data passed to initialize choose avatar view is not profile, type: {initData?.GetType().Name}");
            return;
        }

        this.profile = profile;
    }

    protected override void SetInfo(IUIData data)
    {
        currentAvatar = profile.Avatar;
        SetAvatarSprite();
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private void ChooseAvatar()
    {
        profile.Avatar = currentAvatar;
        Host.UpdateData(profile);
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
