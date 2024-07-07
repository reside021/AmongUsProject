using UnityEngine;

public class ShowTask : MonoBehaviour
{
    [SerializeField] private Transform TaskContainer;

    public void Display()
    {
        var taskCount = TaskContainer.childCount;

        int indexTask = Random.Range(0, taskCount);

        var task = TaskContainer.GetChild(indexTask);

        if (task != null)
        {
            Instantiate(task, TaskContainer).gameObject.SetActive(true);
        }

    }
}
