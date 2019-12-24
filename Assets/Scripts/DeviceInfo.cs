using UnityEngine;

public enum DEVICE_OS
{
    Android, iOS, Windows
}

public class DeviceInfo
{
    public string deviceId;

    public DeviceInfo(DEVICE_OS deviceOs)
    {
        if (deviceOs == DEVICE_OS.Android)
        {
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
            deviceId = secure.CallStatic<string>("getString", contentResolver, "android_id");
        }
        if (deviceOs == DEVICE_OS.Windows)
        {
            deviceId = SystemInfo.deviceUniqueIdentifier;
        }
    }
}