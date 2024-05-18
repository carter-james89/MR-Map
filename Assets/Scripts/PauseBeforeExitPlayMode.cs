#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class PauseBeforeExitPlayMode
{
    static PauseBeforeExitPlayMode()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange stateChange)
    {
        if (stateChange == PlayModeStateChange.ExitingPlayMode)
        {
            EditorApplication.isPaused = true; // Pause the editor
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged; // Remove event to prevent recursion
            EditorApplication.playModeStateChanged += ResumeOnPlayModeChange; // Add event to resume if needed
        }
    }

    private static void ResumeOnPlayModeChange(PlayModeStateChange stateChange)
    {
        if (stateChange == PlayModeStateChange.EnteredEditMode)
        {
            EditorApplication.isPaused = false; // Resume the editor
            EditorApplication.playModeStateChanged -= ResumeOnPlayModeChange; // Clean up event handler
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged; // Re-add initial event handler
        }
    }
}
#endif