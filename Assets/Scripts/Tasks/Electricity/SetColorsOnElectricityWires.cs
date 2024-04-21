using UnityEngine;
using UnityEngine.UI;

public class SetColorsElectricityWires : MonoBehaviour
{
    [SerializeField] private GameObject[] LeftPoints;
    [SerializeField] private GameObject[] RightPoints;


    private Color[] _colors = new Color[] { Color.gray, Color.blue, Color.red, Color.yellow };

    void Start()
    {
        SetColorsPoint(LeftPoints);
        SetColorsPoint(RightPoints);
    }

    private void SetColorsPoint(GameObject[] array)
    {
        ShuffleColors();

        for(var i = 0; i < array.Length; i++)
        {
            array[i].GetComponent<Image>().color = _colors[i];
        }
    }

    public void ShuffleColors()
    {
        var rand = new System.Random();

        for (var i = _colors.Length - 1; i >= 1; i--)
        {
            var j = rand.Next(i + 1);

            (_colors[i], _colors[j]) = (_colors[j], _colors[i]);
        }
    }
}
