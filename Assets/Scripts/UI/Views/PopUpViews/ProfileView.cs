using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;

// need to decide what to do with the profanity filter
// unity has some built in content checkers for alphanum, etc
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

    /*
    private string username;
    Dictionary<InvalidInputType, string> errorMessages = new Dictionary<InvalidInputType, string>
    {
        { InvalidInputType.Short, "Username must be at least 3 characters" },
        { InvalidInputType.SpecialChars, "Only letters, numbers, and underscores allowed" },
        { InvalidInputType.Profanity, "That username isn’t allowed" }
    };
    */

    private UIAltVertScrollList<int> scoreHistoryScrollList;

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

    public override void Initialize(IUIViewHost host)
    {
        base.Initialize(host);

        UIButtonFactory.EditInput(editUsernameButton, usernameInput);       // could potentially move this to my input field script
        UIButtonFactory.Navigate<PopUpViewType>(editAvatarButton, Host, PopUpViewType.ChooseAvatar);

        scoreHistoryScrollList = new UIAltVertScrollList<int>(scoreHistoryContent, scoreHistoryItem1, scoreHistoryItem2);
        usernameInputField = new UIUsernameInputField(usernameInput, invalidInput, (string value) => Host.PatchUpdate(new UsernamePatch(value)));

        // remove
        /*
        usernameInput.onSubmit.AddListener(value =>
        {
            UpdateUsername(value);
        });
        usernameInput.onDeselect.AddListener(_ =>
        {
            if (!gameObject.activeInHierarchy) return;  // keeps incorrect text on screen on exit
            usernameInput.SetTextWithoutNotify(username);
        });
        usernameInput.onValueChanged.AddListener(_ =>
        {
            invalidInput.gameObject.SetActive(false);
        });
        */
    }

    // ==================================================
    // Base Class Methods
    // ==================================================

    public override void Hide()
    {
        base.Hide();

        invalidInput.gameObject.SetActive(false);
    }

    protected override void SetInfo(IUIData data)
    {
        if (data is not ProfileData profile)
        {
            Debug.LogError($"Data type mismatch, wanted ProfileData, recieved {data?.GetType().Name}");
            return;
        }

        //username = profile.Username;    // remove
        //usernameInput.text = username;      // remove

        usernameInputField.SetUsername(name);
        avatarImage.sprite = SpriteDatabase.Instance.GetSprite((CellColor)profile.Avatar);
        scoreHistoryScrollList.Populate(profile.ScoreList.ROList);
    }


    // ==================================================
    // Private Methods
    // ==================================================
    /*
    private void UpdateUsername(string name)
    {
        InvalidInputType error = UsernameValidator.IsUsernameValid(name);
        if (error == InvalidInputType.None)
        {
            Debug.Log("send username patch");

            invalidInput.gameObject.SetActive(false);
            Host.PatchUpdate(new UsernamePatch(name));
        }
        else
        {
            Debug.Log($"invalid input: {error}");

            invalidInput.text = errorMessages[error];
            invalidInput.gameObject.SetActive(true);

            StartCoroutine(ShakeTextRoutine());
        }
    }
    */
    // ==================================================
    // Coroutines
    // ==================================================

    /*
    IEnumerator ShakeTextRoutine(float duration = 0.3f, float magnitude = 8f)
    {
        Vector3 originalPos = usernameInput.textComponent.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = originalPos.x + UnityEngine.Random.Range(-1f, 1f) * magnitude;
            usernameInput.textComponent.transform.localPosition = new Vector3(x, originalPos.y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        usernameInput.textComponent.transform.localPosition = originalPos;
    }
    */
}
