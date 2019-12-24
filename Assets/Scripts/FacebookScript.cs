using UnityEngine;
using Facebook.Unity;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class FacebookScript : MonoBehaviour
{
    private bool loggedIn = false;

    // Awake function from Unity's MonoBehavior
    void Awake()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        DontDestroyOnLoad(this.gameObject);

        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK Configuration
            FB.Init(null, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    private void Start()
    {
    }

    public void Login()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...

            var perms = new List<string>() { "public_profile", "email" };
            FB.LogInWithReadPermissions(perms, AuthCallback);
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void Update()
    {
        if (FB.IsLoggedIn && !loggedIn)
        {
            YGameLogin yGameLogin = new YGameLogin();
            StartCoroutine(yGameLogin.Login(LOGIN_TYPES.LOGIN_BY_FACEBOOK));

            if (yGameLogin.isLoggedIn())
            {
                Debug.Log("I was here");
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                SceneManager.LoadScene("Home");
                StartCoroutine(changeProfilePic());
            }
        }
    }

    IEnumerator changeProfilePic()
    {
        string url = "https" + "://graph.facebook.com/" + AccessToken.CurrentAccessToken.UserId + "/picture";
        url += "?access_token=" + AccessToken.CurrentAccessToken.TokenString;
        WWW www = new WWW(url);
        yield return www;

        Texture2D profilePic = www.texture;
        GameObject.Find("AccountAvatar").GetComponent<RawImage>().texture = profilePic;
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }
}