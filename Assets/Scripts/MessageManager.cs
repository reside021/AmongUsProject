using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    public TextMeshProUGUI UserName;
    public TextMeshProUGUI Message;

    public void SetUserName(string userName)
    {
        UserName.text = userName;
    }

    public void SetMessage(string msg)
    {
        Message.text = msg;
    }

}
