using UnityEngine;

public class FlagUI : MonoBehaviour
{
    [SerializeField] private GameObject _uiCyrcle;
    [SerializeField] private Flag _flag;

    private void Awake()
    {
        _uiCyrcle.SetActive(false);
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

    private void Enable()
    {
        _uiCyrcle.SetActive(true);
    }

    private void Disable()
    {
        _uiCyrcle.SetActive(false);
    }
}
