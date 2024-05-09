using UnityEngine;
using UnityEngine.UI;

public class SelfClose : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => {
            gameObject.SetActive(false);
            Destroy(gameObject);
        });
    }
}
