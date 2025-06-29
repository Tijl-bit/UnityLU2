using UnityEngine;

public class PanelToggler : MonoBehaviour
{
    public GameObject panelToShow; // Assign this in the Inspector

    // Call this from your button
    public void ShowPanel()
    {
        if (panelToShow != null)
        {
            panelToShow.SetActive(true);
        }
    }

    // Optional: Add a hide method too
    public void HidePanel()
    {
        if (panelToShow != null)
        {
            panelToShow.SetActive(false);
        }
    }
}
