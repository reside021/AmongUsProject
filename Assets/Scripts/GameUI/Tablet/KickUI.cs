using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class KickUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI IsImposterText;
    [SerializeField] private TextMeshProUGUI ImpostorRemainsText;
    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private AudioClip AudioText;

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

        AudioSource.clip = AudioText;
        AudioSource.loop = true;
        AudioSource.Play();

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

        AudioSource.loop = false;

        yield return new WaitForSeconds(3);

        this.gameObject.SetActive(false);
        OnKickEnds?.Invoke();
    }


    private void OnDisable()
    {
        LevelManager.OnVoteEnds -= OnVoteEnds;
    }
}
