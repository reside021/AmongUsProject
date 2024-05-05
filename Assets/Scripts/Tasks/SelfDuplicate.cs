using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDuplicate : MonoBehaviour
{
    private void OnEnable()
    {
        if (gameObject.activeSelf)
        {
            var newObj = Instantiate(gameObject, transform.parent);
            newObj.SetActive(false);
        }
    }
}
