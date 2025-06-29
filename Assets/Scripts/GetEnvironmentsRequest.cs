using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using System;

public class GetUserEnvironmentsRequest : MonoBehaviour
{
    [Header("Environment UI")]
    public GameObject environmentSlotPrefab; // Prefab for each environment slot
    public Transform slotContainer; // Parent where slots will be instantiated
    public TextMeshProUGUI errorText;
    public Button fetchButton; // Button to trigger the fetch request

    private string apiUrl = "https://tijlswebsite-dygxf0d5dcehd9fg.northeurope-01.azurewebsites.net/api/environments";
    private List<Environment2DModel> environments = new List<Environment2DModel>();
    private List<GameObject> spawnedSlots = new List<GameObject>();

    // Static variable to store the selected environment ID
    public static string SelectedEnvironmentId;

    void Start()
    {
        fetchButton.onClick.AddListener(OnFetchClick);
    }

    void OnFetchClick()
    {
        StartCoroutine(FetchEnvironments());
    }

    IEnumerator FetchEnvironments()
    {
        Debug.Log("Fetching environments...");
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            string token = PlayerPrefs.GetString("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                request.SetRequestHeader("Authorization", "Bearer " + token);
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Environments fetched successfully");

                string json = request.downloadHandler.text;
                environments = JsonConvert.DeserializeObject<List<Environment2DModel>>(json);

                Debug.Log($"Total environments fetched: {environments.Count}");

                PopulateEnvironmentSlots();
            }
            else
            {
                Debug.LogError("❌ Failed to fetch environments: " + request.downloadHandler.text);
                errorText.text = "Error fetching environments!";
            }
        }
    }

    void PopulateEnvironmentSlots()
    {
        // Clear existing slots
        foreach (var slot in spawnedSlots)
        {
            Destroy(slot);
        }
        spawnedSlots.Clear();

        int maxSlots = Mathf.Min(environments.Count, 5);

        for (int i = 0; i < maxSlots; i++)
        {
            var env = environments[i];
            GameObject slot = Instantiate(environmentSlotPrefab, slotContainer);
            spawnedSlots.Add(slot);

            TextMeshProUGUI label = slot.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = env.name;
            }

            Button button = slot.GetComponentInChildren<Button>();
            if (button != null)
            {
                Environment2DModel capturedEnv = env;
                button.onClick.AddListener(() => OnEnvironmentSelected(capturedEnv));
            }
        }
    }

    public void OnEnvironmentSelected(Environment2DModel selectedEnvironment)
    {
        Debug.Log($"Environment selected: {selectedEnvironment.name}, ID: {selectedEnvironment.id}");
        PlayerPrefs.SetString("authEnvironmentId", selectedEnvironment.id);

        // Store the environment ID in a static variable
        SelectedEnvironmentId = selectedEnvironment.id;

        // Load the "WereldScene" and pass the environment ID
        LoadEnvironmentScene();
    }

    // This method loads the "WereldScene" and passes the environment ID to it
    void LoadEnvironmentScene()
    {
        // Store the environment ID using PlayerPrefs (persistent storage)
        PlayerPrefs.SetString("SelectedEnvironmentId", SelectedEnvironmentId);

        // Load the "WereldScene" scene
        SceneManager.LoadScene("WereldScene");
    }
}

[System.Serializable]
public class Environment2DModel
{
    public string id;
    public string name;
    public string ownerUserId;
    public int maxLength;
    public int maxHeight;
}
