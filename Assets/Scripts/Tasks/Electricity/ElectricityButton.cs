using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ElectricityButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private LineController _controller;
    [SerializeField] private Camera Camera;
    private Vector3 _startPoint;
    private bool _onDown;
    private bool _connected;

    public static Action Progressed;


    private void OnEnable()
    {
        ElectricityWiresConnect.OnConnected += OnConnected;
    }

    private void OnConnected(Color color)
    {
        if (_onDown)
        {
            var colorLine = GetComponent<Image>().color;

            if (color == colorLine)
            {
                _onDown = false;
                _connected = true;
                Progressed?.Invoke();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _onDown = true;
        var color = GetComponent<Image>().color;
        _controller.SetUpColorLine(color);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_connected) return;

        _onDown = false;
        var points = new Vector3[] { _startPoint, _startPoint };
        _controller.SetUpLine(points);
    }

    void Start()
    {
        _startPoint = transform.position;
        var points = new Vector3[] { _startPoint, _startPoint };
        _controller.SetUpLine(points);
    }

    void Update()
    {
        if (_onDown)
        {
            var currentPos = Camera.ScreenToWorldPoint(Input.mousePosition);

            var points = new Vector3[] { _startPoint, currentPos };

            _controller.SetUpLine(points);

        }
    }

    private void OnDisable()
    {
        ElectricityWiresConnect.OnConnected -= OnConnected;
    }

}
