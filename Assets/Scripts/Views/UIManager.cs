using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Controls the screen that is displayed.
///
/// Also contains all button functions.
/// </summary>
public class UIManager : MonoBehaviour
{
    // ==================================================
    // Inspector Fields
    // ==================================================

    // Canvases
    [SerializeField] private GameObject homeCanvas;
    [SerializeField] private GameObject gamePlayCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject settingsCanvas;

    // text
    [SerializeField] private Text scoreText;
    [SerializeField] private Text highScoreText;
    [SerializeField] private Text gameOverScoreText;

    // ==================================================
    // Events
    // ==================================================
    public event Action StartGame;
    public event Action EndGame;
    public event Action PauseGame;
    public event Action ResumeGame;

    // ==================================================
    // Private Fields
    // ==================================================
    private Stack<GameObject> canvasStack = new Stack<GameObject>;


    // ================================
    // Unity Lifecycle Methods
    // ================================

    void Start()
    {
        PushCanvas(homeCanvas);
    }


    // ==================================================
    // Public Methods
    // ==================================================
    public void GameOver(int score)
    {
        gameOverScoreText.text = $"{score}";    // update the score text

        PushCanvas(gameOverCanvas);
    }

    public void UpdateScoreText(int score, int highScore)
    {
        scoreText.text = $"{score}";
        highScoreText.text = $"{highScore}";
    }


    // ================================
    // Button Methods
    // ================================

    public void OnPlayClicked()
    {
        ClearStack();
        PushCanvas(gamePlayCanvas);

        StartGame?.Invoke();
    }

    public void OnHomeClicked()
    {
        ClearStack();
        PushCanvas(homeCanvas);
    }

    public void OnReplayClicked()
    {
        EndGame?.Invoke();

        PopCanvas();

        StartGame?.Invoke();
    }

    public void OnExitGameClicked()
    {
        EndGame?.Invoke();

        ClearStack();
        PushCanvas(homeCanvas);
    }

    public void OnCloseSettingsClicked()
    {
        PopCanvas();

        ResumeGame?.Invoke();
    }

    public void OnSettingsClicked()
    {
        PauseGame?.Invoke();

        PushCanvas(settingsCanvas);
    }


    // ==================================================
    // Private Methods
    // ==================================================

    // Stack Navigation
    private void PushCanvas(GameObject canvas)
    {
        if (canvasStack.Count > 0)
        {
            canvasStack.Peek().SetActive(false);
        }

        canvas.SetActive(true);
        canvasStack.Push(canvas);
    }
    private void PopCanvas()
    {
        if (canvasStack.Count == 0) return;

        GameObject top = canvasStack.Pop();
        top.SetActive(false);

        if (canvasStack.Count > 0)
        {
            canvasStack.Peek().SetActive(true);
        }
    }
    private void ClearStack()
    {
        while (canvasStack.Count > 0)
        {
            GameObject canvas = canvasStack.Pop();
            canvas.SetActive(false);
        }
    }

}
