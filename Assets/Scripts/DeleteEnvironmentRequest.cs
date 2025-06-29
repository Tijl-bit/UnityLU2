using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Newtonsoft.Json;

public class DeleteEnvironmentRequest : MonoBehaviour
{
    public TMP_InputField worldNameInput;
    public TextMeshProUGUI errorText;
    public Button deleteButton;

    private string apiBaseUrl = "https://tijlswebsite-dygxf0d5dcehd9fg.northeurope-01.azurewebsites.net/api/environments";

    void Start()
    {
        deleteButton.onClick.AddListener(OnDeleteClicked);
    }

    void OnDeleteClicked()
    {
        string worldName = worldNameInput.text.Trim();

        if (string.IsNullOrEmpty(worldName))
        {
            errorText.text = "World name is required!";
            return;
        }

        StartCoroutine(FetchAndDeleteEnvironment(worldName));
    }

    IEnumerator FetchAndDeleteEnvironment(string worldName)
    {
        string token = PlayerPrefs.GetString("authToken");

        UnityWebRequest getRequest = UnityWebRequest.Get(apiBaseUrl);
        getRequest.SetRequestHeader("Authorization", "Bearer " + token);

        yield return getRequest.SendWebRequest();

        if (getRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Failed to fetch environments: " + getRequest.error);
            errorText.text = "Failed to fetch environments.";
            yield break;
        }

        // Parse environments
        List<Environment2D> environments = JsonConvert.DeserializeObject<List<Environment2D>>(getRequest.downloadHandler.text);
        Environment2D envToDelete = environments.Find(e => e.name.ToLower() == worldName.ToLower());

        if (envToDelete == null)
        {
            errorText.text = "No environment found with that name.";
            yield break;
        }

        // Delete the found environment
        string deleteUrl = apiBaseUrl + "/" + envToDelete.id;
        UnityWebRequest deleteRequest = UnityWebRequest.Delete(deleteUrl);
        deleteRequest.SetRequestHeader("Authorization", "Bearer " + token);

        yield return deleteRequest.SendWebRequest();

        if (deleteRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Environment deleted: " + envToDelete.name);
            errorText.text = "Environment deleted successfully!";
        }
        else
        {
            Debug.LogError("❌ Delete failed: " + deleteRequest.error);
            errorText.text = "Delete failed: " + deleteRequest.downloadHandler.text;
        }
    }

  
    public class Environment2D
    {
        public string id; // Add this line
        public string name;
        public int maxLength;
        public int maxHeight;
    }
}
