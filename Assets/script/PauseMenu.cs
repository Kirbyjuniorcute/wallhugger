using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseCanvas;
    public MonoBehaviour[] componentsToDisable;

    private bool isPaused = false;
    private float previousTimeScale;

    // Static flag to trigger double-toggle
    public static bool autoFixPauseBug = false;

    void Start()
    {
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);

        LockCursor();

        // Trigger the quick double toggle to simulate your manual fix
        if (autoFixPauseBug)
        {
            autoFixPauseBug = false;
            StartCoroutine(QuickToggleFix());
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        if (pauseCanvas != null)
            pauseCanvas.SetActive(true);

        foreach (var component in componentsToDisable)
        {
            if (component != null)
                component.enabled = false;
        }

        UnlockCursor();
    }

    void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = previousTimeScale > 0 ? previousTimeScale : 1f;

        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);

        foreach (var component in componentsToDisable)
        {
            if (component != null)
                component.enabled = true;
        }

        LockCursor();
        Input.ResetInputAxes();
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    IEnumerator QuickToggleFix()
    {
        yield return null; // wait 1 frame after scene load
        PauseGame();
        yield return new WaitForSecondsRealtime(0.1f);
        ResumeGame();
    }
}
