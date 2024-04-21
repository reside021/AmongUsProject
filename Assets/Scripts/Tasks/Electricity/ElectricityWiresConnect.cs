using System;
using UnityEngine;
using UnityEngine.UI;

public class ElectricityWiresConnect : MonoBehaviour
{
    public static Action<Color> OnConnected;

    private void OnMouseEnter()
    {
        var color = GetComponent<Image>().color;

        OnConnected?.Invoke(color);
    }
}
