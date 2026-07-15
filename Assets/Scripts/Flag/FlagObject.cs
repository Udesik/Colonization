using UnityEngine;
using System;

public class FlagObject : MonoBehaviour
{
    [SerializeField] private Flag _flag;
    [SerializeField] private Reycaster _raycaster;
    [SerializeField] private InputSystem _inputSystem;
    [SerializeField] private GameObject _flagPrefab;

    [SerializeField] private float _offsetY = 2.5f;
    [SerializeField] private float _borderX = 500f;
    [SerializeField] private float _borderZ = 500f;

    private bool _isCanSetFlag = false;
    private bool _isWaiting = false;

    public event Action<Vector3> FlagSetted;

    private void Awake()
    {
        _flagPrefab.SetActive(false);
        Material material = _flagPrefab.GetComponent<Renderer>().material;
        material.color = Color.red;
    }

    private void OnEnable()
    {
        _raycaster.ReycastConsidered += SetPosition;
        _flag.StartWorking += Enable;
        _flag.StopWorking += Disable;
        _inputSystem.LeftMouseClicked += SetFlag;
    }

    private void OnDisable()
    {
        _raycaster.ReycastConsidered -= SetPosition;
        _flag.StartWorking -= Enable;
        _flag.StopWorking -= Disable;
        _inputSystem.LeftMouseClicked -= SetFlag;
    }

    private void SetFlag()
    {
        if (_isCanSetFlag && _isWaiting == false)
        {
            if (_flagPrefab.transform.position[0] > _borderX || _flagPrefab.transform.position[2] > _borderZ || _flagPrefab.transform.position[0] < -_borderX || _flagPrefab.transform.position[2] < -_borderZ) 
            {
                return;
            }
            
            FlagSetted?.Invoke(_flagPrefab.transform.position);
            _isWaiting = true;

        }
        else
        {
            _isCanSetFlag = true;
        }
    }

    private void SetPosition(Vector3 position)
    {
        if (_isWaiting) return;

        _flagPrefab.transform.position = position + new Vector3(0f, _offsetY, 0f);
    }

    private void Enable()
    {
        _flagPrefab.SetActive(true);
    }

    private void Disable()
    {
        _flagPrefab.SetActive(false);
        _isWaiting = false;
        _isCanSetFlag = false;
    }
}
