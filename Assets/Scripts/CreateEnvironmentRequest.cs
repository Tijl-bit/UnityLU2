using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;
using Newtonsoft.Json;
using TMPro;
using System;

public class CreateEnvironmentRequest : MonoBehaviour
{
    public TMP_InputField worldNameInput;
    public Button submitButton;
    public TextMeshProUGUI errorText;

    private string apiUrl = "https://tijlswebsite-dygxf0d5dcehd9fg.northeurope-01.azurewebsites.net/api/environments";

    void Start()
    {
        submitButton.onClick.AddListener(OnClicked);
    }

    void OnClicked()
    {
        string worldName = worldNameInput.text.Trim();

        if (string.IsNullOrEmpty(worldName))
        {
            errorText.text = "World name is required!";
            return;
        }

        StartCoroutine(PostEnvironmentRequest(worldName));
    }

    IEnumerator PostEnvironmentRequest(string worldName)
    {
        Environment2D requestData = new Environment2D
        {
            
            name = worldName,
            maxLength = 200,         // ✅ As int
            maxHeight = 200
        };


        string jsonData = JsonConvert.SerializeObject(requestData);

        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl, jsonData, "application/json"))
        {
            string token = PlayerPrefs.GetString("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                request.SetRequestHeader("Authorization", "Bearer " + token);
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Environment created successfully!");
                errorText.text = "Environment created successfully!";
                // Optional: load a new scene or refresh environments

            }
            else
            {
                errorText.text = "Error: " + request.downloadHandler.text;
                Debug.LogError("Failed: " + request.error);
            }
        }
    }
}

