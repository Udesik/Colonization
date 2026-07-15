using UnityEngine;
using System;

public class InputSystem : MonoBehaviour
{
    public event Action LeftMouseClicked;
    public event Action RightMouseClicked;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LeftMouseClicked?.Invoke();
        }

        if (Input.GetMouseButtonDown(1))
        {
            RightMouseClicked?.Invoke();
        }
    }
}
