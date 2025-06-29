using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ClearTokenOnEditorStop : MonoBehaviour
{
    static ClearTokenOnEditorStop()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            Debug.Log("🧹 Clearing token because Play Mode was stopped.");
            PlayerPrefs.DeleteKey("authToken");
            PlayerPrefs.DeleteKey("authUserId");
            PlayerPrefs.Save();
        }
    }
}
