using UnityEngine;
using UnityEngine.UI;
using System;

public class ProfileView : PopUpView<PlayerProfile>
{
    
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private InputField usernameInput;

    [SerializeField] private Button editAvatarButton;
    [SerializeField] private Image avatarImage;

    // ==================================================
    // Unity Lifecycle
    // ==================================================

    protected override void OnValidate()
    {
        base.OnValidate();

        Debug.Assert(usernameInput != null, "Username input not set in profile view");

        Debug.Assert(editAvatarButton != null, "Edit avatar button not set in profile view");
        Debug.Assert(avatarImage != null, "Avatar image not set in profile view");
    }

    protected override void Awake()
    {
        base.Awake();

        //usernameInput.onEndEdit.AddListener(UI.UpdateUsername);  fix data entry

        editAvatarButton.onClick.AddListener(() => Controller.PushOverlay(PopUpViewType.ChooseAvatar, new NoData())); 
    }

    // ==================================================
    // Public Methods
    // ==================================================

    public override void Show(PlayerProfile data)
    {
        usernameInput.text = data.Username;
        avatarImage.sprite = SpriteDatabase.Instance.GetSprite((CellColor)data.Avatar);

        base.Show(data);
    }

    public override void UpdateOverlay(PlayerProfile data)
    {
        base.UpdateOverlay(data);

        usernameInput.text = data.Username;
        avatarImage.sprite = SpriteDatabase.Instance.GetSprite((CellColor)data.Avatar);
    }

}
