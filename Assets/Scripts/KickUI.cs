using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KickUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI IsImposterText;
    [SerializeField] private TextMeshProUGUI ImpostorRemainsText;

    public static Action OnKickEnds;

    private void OnEnable()
    {
        LevelManager.OnVoteEnds += OnVoteEnds;
    }

    private void OnVoteEnds(string resultText, string remainsText)
    {
        IsImposterText.text = string.Empty;
        ImpostorRemainsText.text = string.Empty;

        StartCoroutine(WriteTextSlowly(resultText, remainsText));

    }

    private IEnumerator WriteTextSlowly(string resultText, string remainsText)
    {
        yield return new WaitForSeconds(1);

        foreach (var symbol in resultText)
        {
            IsImposterText.text += symbol;
            yield return new WaitForSeconds(0.2f);
        }
        foreach (var symbol in remainsText)
        {
            ImpostorRemainsText.text += symbol;
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(3);

        this.gameObject.SetActive(false);
        OnKickEnds?.Invoke();
    }


    private void OnDisable()
    {
        LevelManager.OnVoteEnds -= OnVoteEnds;
    }
}
