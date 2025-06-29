using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClearTokenData : MonoBehaviour
{
    public Button clearButton;  // Button to trigger clearing of data
    public TextMeshProUGUI feedbackText;  // Optional: For showing feedback to the user

    void Start()
    {
        // Add listener to the button to clear data when clicked
        clearButton.onClick.AddListener(ClearTokenAndData);
    }

    // This method will clear the stored token and other associated data
    void ClearTokenAndData()
    {
        // Clear all relevant PlayerPrefs keys (Bearer token, authUserId, etc.)
        PlayerPrefs.DeleteKey("authToken");
        PlayerPrefs.DeleteKey("authUserId");

        // Optionally, you can also clear any other stored user data here
        // PlayerPrefs.DeleteKey("otherUserDataKey");

        // Make sure to save PlayerPrefs after deleting keys
        PlayerPrefs.Save();

        // Provide feedback to the user (optional)
        if (feedbackText != null)
        {
            feedbackText.text = "Session data cleared. You are now logged out.";
        }

        // Debug log to confirm it's been cleared
        Debug.Log("Bearer token and user data have been cleared.");
    }
}
