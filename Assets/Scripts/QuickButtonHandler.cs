using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickButtonHandler : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Login()
    {
        YGameLogin yGameLogin = new YGameLogin();
        yGameLogin.Login(LOGIN_TYPES.LOGIN_BY_QUICK);
    }
}
