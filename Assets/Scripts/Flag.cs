using UnityEngine;
using System;
using System.Collections;

public class Flag : MonoBehaviour
{
    [SerializeField] private GameObject _uiCyrcle;
    [SerializeField] private GameObject _flag;
    [SerializeField] private float _offsetY = 2.5f;
    [SerializeField] private float _waitingTime = 5f;

    [SerializeField] private float _borderX = 500f;
    [SerializeField] private float _borderZ = 500f;

    private Camera _camera;

    private bool _plannedToCreate = false;
    private bool _canPlaceInThisFrame = false;

    public event Action<Vector3> Request;

    private void Awake()
    {
        _camera = Camera.main;
        _flag.GetComponent<Renderer>().material.color = Color.red;
    }

    private void Update()
    {
        if (_plannedToCreate)
        {
            RaycastHit hit;

            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Default"))
                {
                    Vector3 position = hit.point + new Vector3(0f, _offsetY, 0f);
                    _flag.transform.position = position;
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                NotPlan();
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (_canPlaceInThisFrame)
                {
                    SetFlag();
                }
                else
                {
                    _canPlaceInThisFrame = true; 
                }
            }
        }
    }

    private void OnMouseDown()
    {
        if (!_plannedToCreate) 
        {
            Plan();
        }
    }

    private void SetFlag()
    {
        Vector3 pos = _flag.transform.position;

        if (pos[0] > _borderX || pos[2] > _borderZ || pos[0] < -_borderX || pos[2] < -_borderZ) return;

        Color newColor = _flag.GetComponent<Renderer>().material.color;
        newColor.a = 1f;
        _flag.GetComponent<Renderer>().material.color = newColor;
        _plannedToCreate = false;
        _canPlaceInThisFrame = false;

        StartCoroutine(WaitForRequest());
    }

    public void Plan()
    {
        _plannedToCreate = true;
        _flag.SetActive(true);
        _uiCyrcle.SetActive(true);

        Color newColor = _flag.GetComponent<Renderer>().material.color;
        newColor.a = 0.2f;
        _flag.GetComponent<Renderer>().material.color = newColor;
    }

    public void NotPlan()
    {
        _plannedToCreate = false;
        _canPlaceInThisFrame = false;
        _flag.SetActive(false);
        _uiCyrcle.SetActive(false);
    }

    private IEnumerator WaitForRequest()
    {
        yield return new WaitForSeconds(_waitingTime);

        Request?.Invoke(_flag.transform.position);
    }
}
