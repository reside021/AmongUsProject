using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Button backBtn;
    void Start()
    {
        backBtn.onClick.AddListener(BackToScene);
    }

    private void BackToScene()
    {
        SceneManager.LoadScene(1);
    }
}
