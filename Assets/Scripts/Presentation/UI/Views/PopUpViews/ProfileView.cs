using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

// not sure how I want to implement the username input field yet:
    // edit only on button?
    // or edit both on button and tapping the field?

// write a new script for the input field
public class ProfileView : PopUpView<IAllData>
{
    // ==================================================
    // Inspector Fields
    // ==================================================
    [SerializeField] private Button editUsernameButton;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private Text invalidInput;

    [SerializeField] private Button editAvatarButton;
    [SerializeField] private Image avatarImage;

    [SerializeField] private ScoreHistoryScrollList listView;

    // ==================================================
    // Private Fields
    // ==================================================
    private string currentUsername;
    Dictionary<InvalidInputType, string> errorMessages = new Dictionary<InvalidInputType, string>
    {
        { InvalidInputType.Short, "Username must be at least 3 characters" },
        { InvalidInputType.SpecialChars, "Only letters, numbers, and underscores allowed" },
        { InvalidInputType.Profanity, "That username isn’t allowed" }
    };

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

        // update input text
        // can i have max number of characters here?
        
        usernameInput.onSubmit.AddListener(value =>
        {
            TryUpdateUsername(value);
        });
        usernameInput.onDeselect.AddListener(_ =>
        {
            usernameInput.text = currentUsername;
            invalidInput.gameObject.SetActive(false);
        });

        editAvatarButton.onClick.AddListener(() => Manager.PushOverlay(PopUpViewType.ChooseAvatar));
    }

    // ==================================================
    // Public Methods
    // ==================================================

    public override void Show(IAllData data)
    {
        SetUserProfile(data.UserSettings);
        SetScoreHistory(data.GameData);
        
        base.Show(data);
    }

    public override void Hide()
    {
        base.Hide();

        invalidInput.gameObject.SetActive(false);
    }

    public override void UpdateOverlay(IAllData data)
    {
        base.UpdateOverlay(data);

        SetUserProfile(data.UserSettings);
        SetScoreHistory(data.GameData);
    }

    

    // ==================================================
    // Private Methods
    // ==================================================

    private void TryUpdateUsername(string name)
    {
        if (Manager.TryUpdateUsername(name, out InvalidInputType error))
        {
            currentUsername = name;
            invalidInput.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log($"invalid input: {error}");

            usernameInput.text = currentUsername;

            // shake the text

            // display reason why invalid
            invalidInput.text = errorMessages[error];
            invalidInput.gameObject.SetActive(true);
        }
    }

    private void SetUserProfile(IUserSettings userSettings)
    {
        usernameInput.text = userSettings.Username;
        avatarImage.sprite = SpriteDatabase.Instance.GetSprite((CellColor)userSettings.Avatar);

        currentUsername = userSettings.Username;
    }

    private void SetScoreHistory(IGameData gameData)
    {
        if (listView == null) listView = GetComponent<ScoreHistoryScrollList>();

        listView.Populate(gameData.ScoreHistory);   // error - null reference
    }
}
