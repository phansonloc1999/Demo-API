using System;
using System.Collections.Generic;
using Assets.SimpleAndroidNotifications;
using UnityEngine;

public class FirebaseCloudMessaging : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                //   app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.

                Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
                Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message from: " + e.Message.From);
        var firebaseMessage = e.Message;
        Debug.Log("Title:" + firebaseMessage.Notification.Title);
        Debug.Log("Body: " + firebaseMessage.Notification.Body);
        Debug.Log("Raw data: " + firebaseMessage.RawData);
        Debug.Log("Data count: " + firebaseMessage.Data.Count);
        Debug.Log("Error: " + firebaseMessage.ErrorDescription);
        Debug.Log("Boolean was:" + firebaseMessage.Data.ContainsKey("image"));
        NotificationManager.SendWithAppIcon(new TimeSpan(0, 0, 20), firebaseMessage.Notification.Title, firebaseMessage.Notification.Body, Color.white);
    }
}
