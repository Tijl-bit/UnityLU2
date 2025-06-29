using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;  // Zorg ervoor dat je deze gebruikt voor TMP componenten
using System.Collections;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class ApiDataSenderLogin : MonoBehaviour
{
    // Referenties naar TMP UI elementen
    public TMP_InputField inputField1;  // Email InputField
    public TMP_InputField inputField2;  // Password InputField
    public Button sendButton;
    public TextMeshProUGUI StatusText;

    // De URL van je Web API
    private string apiUrl = "https://tijlswebsite-dygxf0d5dcehd9fg.northeurope-01.azurewebsites.net/accounts/login"; // Change this to your login endpoint

    void Start()
    {
        // Stel de knop in om de gebeurtenis aan te roepen bij klikken
        sendButton.onClick.AddListener(OnSendButtonClick);
    }

    void OnSendButtonClick()
    {
        // Verkrijg de tekst die in de invoervelden is ingevoerd
        string email = inputField1.text;
        string password = inputField2.text;

        // Zorg ervoor dat beide velden tekst bevatten voordat je verzendt
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            StatusText.text = "Vul beide velden in.";
            return;
        }

        // Roep de functie aan om de gegevens naar de Web API te sturen
        StartCoroutine(SendDataToApi(email, password));
    }

    IEnumerator SendDataToApi(string email, string password)
    {

        var loginData = new { email = email, password = password };
        string jsonData = JsonConvert.SerializeObject(loginData);


        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl, jsonData, "application/json"))
        {

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {

                LoginResponse response = JsonConvert.DeserializeObject<LoginResponse>(request.downloadHandler.text);

                if (!string.IsNullOrEmpty(response.accessToken))
                {

                    PlayerPrefs.SetString("authToken", response.accessToken);
                    PlayerPrefs.SetString("refreshToken", response.refreshToken);
                    PlayerPrefs.SetString("tokenType", response.tokenType);
                    PlayerPrefs.SetInt("expiresIn", response.expiresIn);
                    PlayerPrefs.SetString("userEmail", email);
                    PlayerPrefs.Save();

                    StatusText.text = "Login Successful!";
                    SceneManager.LoadScene("MijnWereldenScene");

                }
                else
                {
                    StatusText.text = "Invalid login";
                }
            }
            else
            {
                StatusText.text = "Your email or password is not valid!";
            }
        }
    }
}



[System.Serializable]
public class LoginResponse
{
    public string tokenType;
    public string accessToken;
    public int expiresIn;
    public string refreshToken;
}