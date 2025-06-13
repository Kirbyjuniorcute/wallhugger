using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseVisibilityManager : MonoBehaviour
{
    public string[] cursorEnabledScenes; // List of scene names where cursor should be visible

    void Start()
    {
        UpdateCursorVisibility(SceneManager.GetActiveScene().name);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateCursorVisibility(scene.name);
    }

    void UpdateCursorVisibility(string sceneName)
    {
        if (System.Array.Exists(cursorEnabledScenes, s => s == sceneName))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}