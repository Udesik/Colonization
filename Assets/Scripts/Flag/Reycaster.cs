using UnityEngine;
using System;

public class Reycaster : MonoBehaviour
{
    private InputSystem _inputSystem;
    private Camera _camera;
    public RaycastHit Hit { get; private set; }

    private void Awake()
    {
        _camera = Camera.main;
        _inputSystem = GetComponent<InputSystem>();
    }

    public bool TryGetHit(out RaycastHit hit)
    {
        int layerMask = ~LayerMask.GetMask("Ignore Raycast"); 
    
        return Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, layerMask);
    }
}
