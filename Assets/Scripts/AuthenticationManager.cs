using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using System.Diagnostics;
using System.Net.WebSockets;
using System;

public class AuthenticationManager : MonoBehaviour
{
    [SerializeField] Button login_Button, signup_Button;
    [SerializeField] TMP_InputField usernameInput,passwordInput;
    [SerializeField] TMP_Text resultText;

    private string baseUrl = "http://localhost:3000";
    Process server;
    private void Start()
    {
        OnInputChange();
        server = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "node"; // or provide the full path to node executable
        startInfo.Arguments = "Assets/Server~/App/app.js"; // specify your Node.js server file
        server.StartInfo = startInfo;
        server.Start();
        usernameInput.onValueChanged.AddListener((str) => {
            OnInputChange();
        });
        passwordInput.onValueChanged.AddListener((str) => {
            OnInputChange();
        });
        login_Button.onClick.AddListener(Login);
        signup_Button.onClick.AddListener(SignUp);
    }
    void OnInputChange()
    {
        login_Button.interactable = signup_Button.interactable = !string.IsNullOrWhiteSpace(usernameInput.text) && !string.IsNullOrWhiteSpace(passwordInput.text);
    }
    private void OnApplicationQuit()
    {
        server.Kill();
    }
    void SignUp()
    {
        usernameInput.text = "";
        passwordInput.text = "";
        StartCoroutine(SendSignUpRequest());
    }

    void Login()
    {
        usernameInput.text = "";
        passwordInput.text = "";
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
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);

                resultText.text = response.message;
                UnityEngine.Debug.Log($"AccessToken : {response.token}");
            }
            else
            {
                resultText.text = "Error: " + www.error;
            }
        }
    }
    [System.Serializable]
    public class LoginResponse
    {
        public string message;
        public string token;
    }
}
