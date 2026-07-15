using UnityEngine;
using System;
using System.Collections;

public class Flag : MonoBehaviour
{
    [SerializeField] private float _waitingTime = 5f;
    [SerializeField] private InputSystem _inputSystem;
    [SerializeField] private FlagObject _flagObject;

    public event Action<Vector3> Request;
    public event Action StartWorking;
    public event Action StopWorking;

    private void OnEnable()
    {
        _inputSystem.RightMouseClicked += Stop;
        _flagObject.FlagSetted += SetRequest;
    }

    private void OnDisable()
    {
        _inputSystem.RightMouseClicked -= Stop;
        _flagObject.FlagSetted -= SetRequest;
    }

    private void OnMouseDown()
    {
        StartWorking?.Invoke();
    }

    private void SetRequest(Vector3 position)
    {
        StartCoroutine(WaitForRequest(position));
    }

    private IEnumerator WaitForRequest(Vector3 position)
    {
        yield return new WaitForSeconds(_waitingTime);

        Request?.Invoke(position);
    }

    public void Stop()
    {
        Request?.Invoke(Vector3.zero);
        StopWorking?.Invoke();
    }
}
