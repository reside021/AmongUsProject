using System;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private Vector3[] _points;


    private void Awake()
    {
        _lineRenderer = transform.GetComponent<LineRenderer>();
        _points = new Vector3[] { };
    }

    public void SetUpLine(Vector3[] points)
    {
        _lineRenderer.positionCount = points.Length;
        _points = points;
    }

    public void SetUpColorLine(Color color)
    {
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
    }

    private void Update()
    {
        for (var i = 0; i < _points.Length; i++)
        {
            _lineRenderer.SetPosition(i, _points[i]);
        }
    }
}
