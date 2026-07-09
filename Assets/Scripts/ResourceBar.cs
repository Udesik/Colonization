using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ResourceBar : MonoBehaviour
{
    [SerializeField] private Base _base;
    [SerializeField] private List<Slider> _sliders;
    [SerializeField] private float _speed = 2f;
    
    private Coroutine[] _updateCoroutines;

    private void Awake()
    {
        _updateCoroutines = new Coroutine[_sliders.Count];
    }

    private void OnEnable()
    {
        _base.ResourceReceived += OnResourceReceived;
    }

    private void OnDisable()
    {
        _base.ResourceReceived -= OnResourceReceived;
    }

    private void OnResourceReceived(Dictionary<ResourceType, int> resources, int max)
    {
        int index = 0;
        foreach (var pair in resources)
        {
            if (index >= _sliders.Count) break;
            if (_sliders[index] == null) 
            {
                index++;
                continue;
            }

            float target = (float)pair.Value / max;
        
            if (_updateCoroutines[index] != null) 
                StopCoroutine(_updateCoroutines[index]);
        
            _updateCoroutines[index] = StartCoroutine(SmoothUpdate(target, index));
            index++;
        }
    }

    private IEnumerator SmoothUpdate(float targetValue, int index)
    {
        while (!Mathf.Approximately(_sliders[index].value, targetValue))
        {
            _sliders[index].value = Mathf.MoveTowards(_sliders[index].value, targetValue, _speed * Time.deltaTime);
            yield return null;
        }
    }
}
