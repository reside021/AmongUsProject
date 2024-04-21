using UnityEngine.UI;
using UnityEngine;

public class CloseMap : MonoBehaviour
{
    [SerializeField] private Button CloseBtn;
    void Start()
    {
        CloseBtn.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
