using System;
using UnityEngine;

public class StartPlayerAnim : MonoBehaviour
{
    public static Action OnMoveInVent;
    public void MoveInVent()
    {
        OnMoveInVent?.Invoke();
    }
}
