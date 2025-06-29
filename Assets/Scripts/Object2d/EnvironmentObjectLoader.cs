using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class EnvironmentObjectLoader : MonoBehaviour
{
    private string apiBaserequestUrl = "https://tijlswebsite-dygxf0d5dcehd9fg.northeurope-01.azurewebsites.net/api/objects/by-environment"; // Base URL for environment filtered API
    public List<PrefabMapping> prefabMappings; // Assign mappings between PrefabId and Prefab in inspector

    [System.Serializable]
    public class PrefabMapping
    {
        public string PrefabId;
        public GameObject Prefab;
    }

    public class Object2D
    {
        public string EnvironmentId;
        public string PrefabId;
        public float PositionX;
        public float PositionY;
        public float ScaleX;
        public float ScaleY;
        public float RotationZ;
        public int SortingLayer;
    }

    private void Start()
    {
        StartCoroutine(LoadEnvironmentObjects());
    }

    private IEnumerator LoadEnvironmentObjects()
    {
        string environmentId = PlayerPrefs.GetString("authEnvironmentId");
        string token = PlayerPrefs.GetString("authToken");

        // Debugging logs
        Debug.Log("🔍 Environment ID from PlayerPrefs: " + environmentId);
        Debug.Log("🔐 Auth token from PlayerPrefs: " + token);

        if (string.IsNullOrEmpty(environmentId))
        {
            Debug.LogError("❌ Environment ID not found in PlayerPrefs.");
            yield break;
        }

        string url = $"{apiBaserequestUrl}?environmentId={UnityWebRequest.EscapeURL(environmentId)}";
        Debug.Log("🌐 Final URL being requested: " + url);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(token))
            {
                request.SetRequestHeader("Authorization", "Bearer " + token);
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                List<Object2D> objects = JsonConvert.DeserializeObject<List<Object2D>>(json);

                Debug.Log($"📦 Received {objects.Count} objects from API");

                foreach (var obj in objects)
                {
                    Debug.Log($"🔧 Spawning object with PrefabId: {obj.PrefabId}, EnvId: {obj.EnvironmentId}, Pos: ({obj.PositionX}, {obj.PositionY})");
                    SpawnObject(obj);
                }

                Debug.Log("✅ All objects loaded into the environment.");
            }
            else
            {
                Debug.LogError("❌ Failed to load objects: " + request.responseCode + " | " + request.error);
                Debug.LogError("❗ Response text: " + request.downloadHandler.text);
            }
        }
    }

    private void SpawnObject(Object2D obj)
    {
        GameObject prefab = prefabMappings.Find(p => p.PrefabId == obj.PrefabId)?.Prefab;

        if (prefab == null)
        {
            Debug.LogWarning("⚠️ No prefab found for ID: " + obj.PrefabId);
            return;
        }

        Vector3 position = new Vector3(obj.PositionX, obj.PositionY, 0);
        Quaternion rotation = Quaternion.Euler(0, 0, obj.RotationZ);
        GameObject newObj = Instantiate(prefab, position, rotation);
        newObj.transform.localScale = new Vector3(obj.ScaleX, obj.ScaleY, 1);

        SpriteRenderer renderer = newObj.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = obj.SortingLayer;
        }
    }
}
