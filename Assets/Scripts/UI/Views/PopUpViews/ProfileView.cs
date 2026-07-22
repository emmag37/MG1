using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;

// todo: reimplement try update username

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

    [SerializeField] private ScoreHistoryScrollList listView;

    // ==================================================
    // Private Fields
    // ==================================================
    private ProfileData profile;

    private ValidatedUsername username;

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

        editAvatarButton.onClick.AddListener(() => Host.PushView<PopUpViewType>(PopUpViewType.ChooseAvatar));
        editUsernameButton.onClick.AddListener(usernameInput.ActivateInputField);

        usernameInput.onSubmit.AddListener(value =>
        {
            UpdateUsername(value);
        });
        usernameInput.onDeselect.AddListener(_ =>
        {
            if (!gameObject.activeInHierarchy) return;  // keeps incorrect text on screen on exit
            usernameInput.SetTextWithoutNotify(username.GetUsername());
        });
        usernameInput.onValueChanged.AddListener(_ =>
        {
            invalidInput.gameObject.SetActive(false);
        });
    }

    // ==================================================
    // Base Class Methods
    // ==================================================

    public override void Hide()
    {
        base.Hide();

        invalidInput.gameObject.SetActive(false);
    }

    protected override void InitializeData(IUIData initData)
    {
        if (initData is not ProfileData profile)
        {
            Debug.Log($"data passed to initialize profile view is not profile, type: {initData?.GetType().Name}");
            return;
        }

        this.profile = profile;
    }

    protected override void SetInfo(IUIData data)
    {
        Debug.Log("set profile view");

        usernameInput.text = profile.Username.GetUsername();
        avatarImage.sprite = SpriteDatabase.Instance.GetSprite((CellColor)profile.Avatar);

        username = profile.Username;

        // set the score list
        if (listView == null)
            listView = GetComponent<ScoreHistoryScrollList>();

        listView.Populate(profile.ScoreList.ROList);
    }


    // ==================================================
    // Private Methods
    // ==================================================

    private void UpdateUsername(string name)
    {
        InvalidInputType error = profile.Username.TrySetUsername(name);
        if (error == InvalidInputType.None)
        {
            invalidInput.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log($"invalid input: {error}");

            invalidInput.text = errorMessages[error];
            invalidInput.gameObject.SetActive(true);

            StartCoroutine(ShakeTextRoutine());
        }
    }

    // ==================================================
    // Coroutines
    // ==================================================

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

}
