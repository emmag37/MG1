using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;


public class UIUsernameInputField
{
    // private fields

    private TMP_InputField inputField;
    private Text invalidInput;

    private Action<string> action;

    private string username;
    private Dictionary<InvalidUsernameType, string> errorMessages = new Dictionary<InvalidUsernameType, string>
    {
        { InvalidUsernameType.Short, "Username must be at least 3 characters" },
        { InvalidUsernameType.SpecialChars, "Only letters, numbers, and underscores allowed" },
        { InvalidUsernameType.Profanity, "That username isn’t allowed" }
    };


    // public methods

    public UIUsernameInputField(TMP_InputField inputField, Text invalidInput, Action<string> action)
    {
        this.inputField = inputField;
        this.invalidInput = invalidInput;

        this.action = action;

        inputField.onSubmit.AddListener(value =>
        {
            UpdateUsername(value);
        });
        inputField.onValueChanged.AddListener(_ =>
        {
            invalidInput.gameObject.SetActive(false);
        });
    }

    public void SetUsername(string name)
    {
        username = name;
        inputField.text = name;
    }

    // private methods

    // need to send a return value to profile view
    private void UpdateUsername(string name)
    {
        InvalidUsernameType error = UsernameValidator.IsUsernameValid(name);
        if (error == InvalidUsernameType.None)
        {
            Debug.Log("send username patch");

            username = name;
            invalidInput.gameObject.SetActive(false);

            action?.Invoke(name);
        }
        else
        {
            Debug.Log($"invalid input: {error}");

            invalidInput.text = errorMessages[error];
            invalidInput.gameObject.SetActive(true);

            CoroutineRunner.Instance.StartCoroutine(ShakeTextRoutine());
        }
    }

    // coroutines

    IEnumerator ShakeTextRoutine(float duration = 0.3f, float magnitude = 8f)
    {
        Vector3 originalPos = inputField.textComponent.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = originalPos.x + UnityEngine.Random.Range(-1f, 1f) * magnitude;
            inputField.textComponent.transform.localPosition = new Vector3(x, originalPos.y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        inputField.textComponent.transform.localPosition = originalPos;
    }

}
