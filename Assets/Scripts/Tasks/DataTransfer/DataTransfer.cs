using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataTransfer : MonoBehaviour
{
    [SerializeField] private GameObject ProgressBar;
    [SerializeField] private GameObject Progress;
    [SerializeField] private GameObject Procent;
    [SerializeField] private GameObject EstimatedTime;
    [SerializeField] private GameObject Download;
    [SerializeField] private Button DownloadButton;
    [SerializeField] private GameObject Task;

    void Start()
    {
        DownloadButton.onClick.AddListener(StartTransferData);
    }

    private void StartTransferData()
    {
        Download.SetActive(false);
        ProgressBar.SetActive(true);
        Procent.SetActive(true);
        EstimatedTime.SetActive(true);  

        StartCoroutine(Transfering());
    }

    IEnumerator Transfering()
    {
        GetComponent<Animator>().SetTrigger("StartAnimation");

        UpdateProgressBar(0f);
        UpdateProcentText("0%");
        UpdateEstimatedText("Estimated Time: 3d 10hr 42m 28s");

        yield return new WaitForSeconds(1.5f);

        UpdateProgressBar(0.14f);
        UpdateProcentText("14%");
        UpdateEstimatedText("Estimated Time: 15hr 41m 56s");

        yield return new WaitForSeconds(1.5f);

        UpdateProgressBar(0.28f);
        UpdateProcentText("28%");
        UpdateEstimatedText("Estimated Time: 6hr 5m 20s");

        yield return new WaitForSeconds(1.5f);

        UpdateProgressBar(0.42f);
        UpdateProcentText("42%");
        UpdateEstimatedText("Estimated Time: 2hr 16m 32s");

        yield return new WaitForSeconds(1.5f);

        UpdateProgressBar(0.56f);
        UpdateProcentText("56%");
        UpdateEstimatedText("Estimated Time: 12m 59s");

        yield return new WaitForSeconds(1.5f);

        var estTime = 4;


        for(var i = 70; i <= 100; i++)
        {
            var progress = (float)i / 100;
            UpdateProgressBar(progress);
            UpdateProcentText($"{i}%");

            if (i % 10 == 0)
            {
                estTime--;
            }

            UpdateEstimatedText($"Estimated Time: {estTime}s");

            yield return new WaitForSeconds(0.1f);
        }

        UpdateEstimatedText("Complete");
        GetComponent<Animator>().SetTrigger("StopAnimation");

        yield return new WaitForSeconds(1);

        ExitTask();
    }

    private void ExitTask()
    {
        var notify = new TaskCompleted();
        notify.CompletedSuccessfully("Room", "Download");

        Destroy(Task);
    }

    private void UpdateProgressBar(float value)
    {
        Progress.GetComponent<Image>().fillAmount = value;
    }

    private void UpdateProcentText(string value)
    {
        Procent.GetComponent<TextMeshProUGUI>().text = value;
    }

    private void UpdateEstimatedText(string value)
    {
        EstimatedTime.GetComponent<TextMeshProUGUI>().text = value;
    }
}
