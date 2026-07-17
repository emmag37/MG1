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

        // re-implement
        chooseButton.onClick.AddListener(() => Host.UpdateData(profile));
    }

    // ==================================================
    // Base Class Methods
    // ==================================================

    protected override void SetInfo(IUIData data)
    {
        if (data is not ProfileData profile)
        {
            Debug.Log($"data passed to choose avatar view is not profile, type: {data?.GetType().Name}");
            return;
        }

        this.profile = profile;

        SetAvatarSprite();
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private void PreviousAvatar()
    {
        profile.Avatar--;
        if (profile.Avatar == CellColor.Empty)
            profile.Avatar = CellColor.WildCard;

        SetAvatarSprite();
    }

    private void NextAvatar()
    {
        if (profile.Avatar == CellColor.WildCard)
            profile.Avatar = CellColor.Empty;
        profile.Avatar++;

        SetAvatarSprite();
    }

    private void SetAvatarSprite()
    {
        avatarImage.sprite = SpriteDatabase.Instance.GetSprite(profile.Avatar);
    }
}
