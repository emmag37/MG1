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
    CellColor currentColor;


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
        //chooseButton.onClick.AddListener(() => Manager.UpdateAvatar(currentColor));
    }

    // ==================================================
    // Base Class Methods
    // ==================================================

    protected override void SetInfo(IUIData data)
    {
        if (data is not IUserSettings settings)
        {
            Debug.Log($"data passed to choose avatar view is not user settings, type: {data?.GetType().Name}");
            return;
        }

        currentColor = (CellColor)settings.Avatar;
        SetAvatarSprite();
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private void PreviousAvatar()
    {
        currentColor--;
        if (currentColor == CellColor.Empty)
            currentColor = CellColor.WildCard;

        SetAvatarSprite();
    }

    private void NextAvatar()
    {
        if (currentColor == CellColor.WildCard)
            currentColor = CellColor.Empty;
        currentColor++;

        SetAvatarSprite();
    }

    private void SetAvatarSprite()
    {
        avatarImage.sprite = SpriteDatabase.Instance.GetSprite(currentColor);
    }
}
