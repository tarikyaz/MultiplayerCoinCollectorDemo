using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class AuthenticationManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text resultText;

    private string baseUrl = "http://localhost:3000";

    public void SignUp()
    {
        StartCoroutine(SendSignUpRequest());
    }

    public void Login()
    {
        StartCoroutine(SendLoginRequest());
    }

    IEnumerator SendSignUpRequest()
    {
        string url = baseUrl + "/signup";
        WWWForm form = new WWWForm();
        form.AddField("username", usernameInput.text);
        form.AddField("password", passwordInput.text);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                resultText.text = www.downloadHandler.text;
            }
            else
            {
                resultText.text = "Error: " + www.error;
            }
        }
    }

    IEnumerator SendLoginRequest()
    {
        string url = baseUrl + "/login";
        WWWForm form = new WWWForm();
        form.AddField("username", usernameInput.text);
        form.AddField("password", passwordInput.text);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                resultText.text = www.downloadHandler.text;
            }
            else
            {
                resultText.text = "Error: " + www.error;
            }
        }
    }
}
