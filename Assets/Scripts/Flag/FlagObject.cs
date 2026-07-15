using UnityEngine;
using System;

public class FlagObject : MonoBehaviour
{
    [SerializeField] private Flag _flag;
    [SerializeField] private Reycaster _raycaster;
    [SerializeField] private InputSystem _inputSystem;
    [SerializeField] private GameObject _flagPrefab;

    [SerializeField] private float _offsetY = 3.5f;
    [SerializeField] private float _borderX = 500f;
    [SerializeField] private float _borderZ = 500f;

    private bool _isWaiting = false;

    private void Awake()
    {
        _flagPrefab.SetActive(false);
        Material material = _flagPrefab.GetComponent<Renderer>().material;
        material.color = Color.red;
    }

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

    private void Update()
    {
        if (_flagPrefab.activeSelf)
        {
            if (_raycaster.TryGetHit(out RaycastHit hit))
            {
                SetPosition(hit.point);
            }
        }
    }

    public void SetFlag()
    {
        if (_flagPrefab.transform.position[0] > _borderX || _flagPrefab.transform.position[2] > _borderZ || _flagPrefab.transform.position[0] < -_borderX || _flagPrefab.transform.position[2] < -_borderZ) 
        {
            return;
        }
            
        _isWaiting = true;
    }

    private void SetPosition(Vector3 position)
    {
        if (_isWaiting) return;

        _flagPrefab.transform.position = position + new Vector3(0f, _offsetY, 0f);
    }

    private void Enable()
    {
        _flagPrefab.SetActive(true);
        _isWaiting = false; 
    }

    private void Disable()
    {
        _flagPrefab.SetActive(false);
        _isWaiting = false;
    }
}
