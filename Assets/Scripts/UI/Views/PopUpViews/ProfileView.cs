using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;


public class ProfileView : PopUpView
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Button editUsernameButton;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private Text invalidInput;

    [SerializeField] private Button editAvatarButton;
    [SerializeField] private Image avatarImage;

    [SerializeField] private Transform scoreHistoryContent;
    [SerializeField] private GameObject scoreHistoryItem1;
    [SerializeField] private GameObject scoreHistoryItem2;

    // ==================================================
    // Private Fields
    // ==================================================
    private UIUsernameInputField usernameInputField;
    private UIAltVertScrollList<int> scoreHistoryScrollList;

    private ISpriteDatabase spriteDatabase;


    // ==================================================
    // Unity Lifecycle
    // ==================================================

    protected override void OnValidate()
    {
        base.OnValidate();

        Debug.Assert(usernameInput != null, "[ProfileView] Null username input");

        Debug.Assert(editAvatarButton != null, "[ProfileView] Null edit avatar button");
        Debug.Assert(avatarImage != null, "[ProfileView] Null avatar image");
    }


    // ==================================================
    // Inherited Methods
    // ==================================================

    public override void Initialize(IUIViewHost host)
    {
        base.Initialize(host);

        spriteDatabase = ServiceLocator.Get<ISpriteDatabase>();

        UIButtonFactory.EditInput(editUsernameButton, usernameInput);
        UIButtonFactory.Navigate<PopUpViewType>(editAvatarButton, Host, PopUpViewType.ChooseAvatar);

        scoreHistoryScrollList = new UIAltVertScrollList<int>(scoreHistoryContent, scoreHistoryItem1, scoreHistoryItem2, 10);
        scoreHistoryScrollList.Populate();

        usernameInputField = new UIUsernameInputField(usernameInput, invalidInput, (string value) => Host.PatchUpdate(new UsernamePatch(value)));
    }

    public override void Hide()
    {
        base.Hide();

        invalidInput.gameObject.SetActive(false);
    }

    protected override void SetInfo(IUIData data)
    {
        if (data is not ProfileData profile)
        {
            Debug.LogError($"[ProfileView] Data type mismatch, wanted ProfileData, recieved {data?.GetType().Name}");
            return;
        }

        usernameInputField.SetUsername(profile.Username);
        avatarImage.sprite = spriteDatabase.GetSprite((int)profile.Avatar);
        scoreHistoryScrollList.SetList(profile.ScoreList.ROList);
    }
}
