using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Button SettingsBtn;
    [SerializeField] private Button MapBtn;

    [SerializeField] private GameObject Map;
    [SerializeField] private GameObject Settings;

    private void Start()
    {
        SettingsBtn.onClick.AddListener(OpenSettings);
        MapBtn.onClick.AddListener(OpenMap);
    }

    private void OpenSettings()
    {
        Settings.SetActive(true);
    }

    private void OpenMap()
    {
        Map.SetActive(true);
    }

}
