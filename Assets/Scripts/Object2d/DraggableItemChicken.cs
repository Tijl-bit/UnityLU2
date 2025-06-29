using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.EventSystems;

public class DraggableSpawnerChicken : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject prefabToSpawn; // Assign in Inspector

    private GameObject currentInstance;

    public void OnPointerDown(PointerEventData eventData)
    {
        // Spawn prefab at mouse world position
        Vector3 spawnPosition = GetMouseWorldPosition();
        spawnPosition.z = 0;
        currentInstance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (currentInstance == null) return;

        // Set final position
        Vector3 finalPosition = GetMouseWorldPosition();
        finalPosition.z = 0;
        currentInstance.transform.position = finalPosition;

        // Prepare object data
        Object2D obj = new Object2D
        {
            EnvironmentId = PlayerPrefs.GetString("authEnvironmentId"),
            PrefabId = "Prefab_01",
            PositionX = finalPosition.x,
            PositionY = finalPosition.y,
            ScaleX = currentInstance.transform.localScale.x,
            ScaleY = currentInstance.transform.localScale.y,
            RotationZ = currentInstance.transform.eulerAngles.z,
            SortingLayer = currentInstance.GetComponent<SpriteRenderer>()?.sortingOrder ?? 0
        };

        // Start POST coroutine
        StartCoroutine(PostObjectToAPI(obj));

        // Optional: Null out the instance
        currentInstance = null;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(Vector3.zero).z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private IEnumerator PostObjectToAPI(Object2D obj)
    {
        string json = JsonConvert.SerializeObject(obj);
        Debug.Log("📤 Sending Object: " + json);
        string apiUrl = "https://tijlswebsite-dygxf0d5dcehd9fg.northeurope-01.azurewebsites.net/api/objects";

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            string token = PlayerPrefs.GetString("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                request.SetRequestHeader("Authorization", "Bearer " + token);
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ Object successfully posted to API.");
            }
            else
            {
                Debug.LogError("❌ Failed to post object: " + request.responseCode + " | " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
        }
    }
}
