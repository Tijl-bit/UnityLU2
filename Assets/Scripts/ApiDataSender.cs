using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;  // Zorg ervoor dat je deze gebruikt voor TMP componenten
using System.Collections;

public class ApiDataSender : MonoBehaviour
{
    // Referenties naar TMP UI elementen
    public TMP_InputField inputField1;  // Email InputField
    public TMP_InputField inputField2;  // Password InputField
    public Button sendButton;
    public TMP_Text statusText;  // Voor het weergeven van status aan de gebruiker

    // De URL van je Web API
    private string apiUrl = "https://tijlswebsite-dygxf0d5dcehd9fg.northeurope-01.azurewebsites.net/accounts/register";

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
            statusText.text = "Vul beide velden in.";
            return;
        }

        // Roep de functie aan om de gegevens naar de Web API te sturen
        StartCoroutine(SendDataToApi(email, password));
    }

    IEnumerator SendDataToApi(string email, string password)
    {
        // Maak een nieuw object van de UserData klasse
        UserDataLogin userData = new UserDataLogin
        {
            email = email,
            password = password
        };

        // Converteer de gegevens naar JSON-formaat
        string jsonData = JsonUtility.ToJson(userData);

        // Maak de UnityWebRequest met de JSON-gegevens
        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);  // Zet JSON om naar een byte array
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();

            // Stel de verzoekkop in voor JSON-content
            www.SetRequestHeader("Content-Type", "application/json");

            // Geef een laadbericht weer of geef aan dat het verzoek in uitvoering is
            statusText.text = "Gegevens verzenden...";

            // Stuur het verzoek en wacht op een antwoord
            yield return www.SendWebRequest();

            // Verwerk het antwoord
            if (www.result == UnityWebRequest.Result.Success)
            {
                // Geef een succesbericht weer
                statusText.text = "Gegevens succesvol verzonden!";
            }
            else
            {
                // Geef een foutmelding weer
                statusText.text = "Fout: " + www.error;
            }
        }
    }
}

// Klasse voor de JSON structuur
[System.Serializable]
public class UserDataLogin
{
    public string email;  // Veld voor email
    public string password;  // Veld voor wachtwoord
}
