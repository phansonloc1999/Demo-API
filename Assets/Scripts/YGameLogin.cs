using System;
using System.Collections;
using System.Text;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.Networking;

public enum LOGIN_TYPES
{
    LOGIN_BY_ACCOUNT = 0, LOGIN_BY_QUICK = 3, LOGIN_BY_FACEBOOK = 4, LOGIN_BY_GOOGLE = 5
}

public class YGameJson
{
    public string accessToken;
}

public class YGameLogin
{
    private WWWForm form;

    private bool loggedIn = false;

    public IEnumerator Login(LOGIN_TYPES loginType)
    {
        int deviceOs = getDeviceOS();

        form = new WWWForm();

        form.AddField("appId", "cb5e0e1690d3c6c53916617bbea6dc19");

        if ((DEVICE_OS)deviceOs == DEVICE_OS.Windows)
            form.AddField("deviceOs", 0);
        else form.AddField("deviceOs", deviceOs);

        form.AddField("loginType", (int)loginType);

        if (loginType == LOGIN_TYPES.LOGIN_BY_ACCOUNT)
        {
            form.AddField("loginName", "minhdaica");
            string originalPassword = "12345";
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(originalPassword);
            string encodedPassword = Convert.ToBase64String(bytesToEncode);
            form.AddField("password", randomizedString() + encodedPassword);
        }

        if (loginType == LOGIN_TYPES.LOGIN_BY_GOOGLE)
        {
            throw new NotImplementedException();
        }

        if (loginType == LOGIN_TYPES.LOGIN_BY_FACEBOOK)
        {
            form.AddField("openId", AccessToken.CurrentAccessToken.TokenString);
        }

        DeviceInfo deviceInfo = new DeviceInfo((DEVICE_OS)deviceOs);
        string deviceInfoToJson = JsonUtility.ToJson(deviceInfo);
        form.AddField("deviceInfo", deviceInfoToJson);

        using (UnityWebRequest www = UnityWebRequest.Post("https://dev.ygame.vn/v1/user/login", form))
        {
            Debug.Log(System.Text.Encoding.UTF8.GetString(www.uploadHandler.data));

            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log("Response: \n" + www.downloadHandler.text);
                getLoginStatus(www.downloadHandler.text);

                if (loggedIn) saveAccessToken(www.downloadHandler.text);
            }
        }
    }

    private bool getSavedAccessToken(ref string accessToken)
    {
        string savedAccessToken = PlayerPrefs.GetString("accessToken", "");
        if (String.Compare(savedAccessToken, "") == 0) return false;

        accessToken = savedAccessToken;
        return true;
    }

    private void saveAccessToken(string response)
    {
        YGameJson deserializedResponse = JsonUtility.FromJson<YGameJson>(response);
        PlayerPrefs.SetString("accessToken", deserializedResponse.accessToken);
        PlayerPrefs.Save();
    }

    private string randomizedString()
    {
        string result = "";
        int resultLength = 5;
        string allowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        int randCharIndex = 0;

        System.Random rng = new System.Random();
        for (int i = 0; i < resultLength; i++)
        {
            randCharIndex = rng.Next(0, allowedCharacters.Length - 1);
            result += allowedCharacters[randCharIndex];
        }
        return result;
    }

    private int getDeviceOS()
    {
        if (Application.platform == RuntimePlatform.Android) return 0;
        if (Application.platform == RuntimePlatform.IPhonePlayer) return 1;
        if (Application.platform == RuntimePlatform.WindowsEditor) return 2;
        return -1;
    }

    public void getLoginStatus(string response)
    {
        if (response.Contains("\"status\":1"))
        {
            Debug.Log("Logged in successfully!");
            loggedIn = true;
        }
        else loggedIn = false;
    }

    public bool isLoggedIn()
    {
        return loggedIn;
    }
}
