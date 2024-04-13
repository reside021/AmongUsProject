using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private Button ReturnBtn;
    [SerializeField] private Button LeaveBtn;
    void Start()
    {
        ReturnBtn.onClick.AddListener(Deactivate);
        LeaveBtn.onClick.AddListener(Leave);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
    private void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }
}
