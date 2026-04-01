using UnityEngine;
using UnityEngine.UI;
using System;

public class ChooseAvatarView : PopUpView<PlayerProfile>
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

        chooseButton.onClick.AddListener(() => Manager.UpdateAvatar(currentColor));
    }

    // ==================================================
    // Public Methods
    // ==================================================

    public override void Show(PlayerProfile data)
    {
        currentColor = (CellColor)data.Avatar;
        SetAvatarSprite();

        base.Show(data);
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
