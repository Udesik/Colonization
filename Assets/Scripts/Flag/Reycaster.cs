using UnityEngine;
using System;

public class Reycaster : MonoBehaviour
{
    [SerializeField] private Flag _flag;

    private bool _isWorking = false;
    private Camera _camera;

    public event Action<Vector3> ReycastConsidered; 

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        _flag.StartWorking += SetStart;
        _flag.StopWorking += SetStop;
    }

    private void OnDisable()
    {
        _flag.StartWorking -= SetStart;
        _flag.StopWorking -= SetStop;
    }

    private void Update()
    {
        if (_isWorking)
        {
            RaycastHit hit;

            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Default"))
                {
                    Vector3 position = hit.point;
                    ReycastConsidered?.Invoke(position);
                }
            }
        }
    }

    private void SetStart()
    {
        _isWorking = true;
    }

    private void SetStop()
    {
        _isWorking = false;
    }
}
