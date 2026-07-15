using UnityEngine;
using System;
using System.Collections;

public class Flag : MonoBehaviour
{
    [SerializeField] private float _waitingTime = 5f;
    [SerializeField] private InputSystem _inputSystem;
    [SerializeField] private FlagObject _flagObject;
    [SerializeField] private Reycaster _reycaster;

    private Base _myBase;
    private bool _isPlacingMode = false;

    public event Action<Vector3> Request;
    public event Action StartWorking;
    public event Action StopWorking;

    private void OnEnable()
    {
        _inputSystem.LeftMouseClicked += CheckMousePosition;
        _inputSystem.RightMouseClicked += Stop;
    }

    private void OnDisable()
    {
        _inputSystem.LeftMouseClicked -= CheckMousePosition;
        _inputSystem.RightMouseClicked -= Stop;
    }

    private void Awake()
    {
        _myBase = GetComponent<Base>();
    }

    private void CheckMousePosition()
    {
        if (_reycaster.TryGetHit(out RaycastHit hit))
        {
            // 1. Клик по зданиям
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Buildings"))
            {
                Base clickedBase = hit.collider.gameObject.GetComponent<Base>();

                if (clickedBase == _myBase)
                {
                    _isPlacingMode = true;
                    StartWorking?.Invoke(); 
                }
            }
            else if (_isPlacingMode)
            {
                _flagObject.SetFlag();
                SetRequest(hit.point);
            }
        }
    }

    private void SetRequest(Vector3 position)
    {
        Debug.Log("Set request");
        StartCoroutine(WaitForRequest(position));
    }

    private IEnumerator WaitForRequest(Vector3 position)
    {
        yield return new WaitForSeconds(_waitingTime);

        Request?.Invoke(position);
    }

    public void Stop()
    {
        _isPlacingMode = false;
        StopWorking?.Invoke();
    }
}
