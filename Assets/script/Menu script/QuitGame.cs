using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

public class QuitGame : MonoBehaviour
{
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in Editor
#else
        Application.Quit(); // Quit the built game
#endif
    }
}