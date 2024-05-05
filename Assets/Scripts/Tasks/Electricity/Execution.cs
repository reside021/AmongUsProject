using UnityEngine;

public class Execution : MonoBehaviour
{
    [SerializeField] private GameObject Task;

    private int _maxConnection = 4;

    private int _currenConnection = 0;

    private void OnEnable()
    {
        ElectricityButton.Progressed += Progressed;
    }

    private void Progressed()
    {
        _currenConnection++;

        if (_currenConnection == _maxConnection)
        {
            var notify = new TaskCompleted();
            notify.CompletedSuccessfully("Room", "Electricity");

            Task.SetActive(false);
        }
        
    }

    private void OnDisable()
    {
        ElectricityButton.Progressed -= Progressed;
    }

}
