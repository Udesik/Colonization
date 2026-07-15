using UnityEngine;
using System;

public class InputSystem : MonoBehaviour
{
    [SerializeField] private Flag _flag;

    private bool _startingCheck = false;

    public event Action LeftMouseClicked;
    public event Action RightMouseClicked;

    private void OnEnable()
    {
        _flag.StartWorking += Enable;
        _flag.StopWorking += Disable;
    }

    private void OnDisable()
    {
        _flag.StartWorking -= Enable;
        _flag.StopWorking -= Disable;
    }

    public void Update()
    {
        if (_startingCheck == false) return;

        if (Input.GetMouseButtonDown(0))
        {
            LeftMouseClicked?.Invoke();
        }

        if (Input.GetMouseButtonDown(1))
        {
            RightMouseClicked?.Invoke();
        }
    }

    private void Enable()
    {
        _startingCheck = true;
    }

    private void Disable()
    {
        _startingCheck = false;
    }
}
