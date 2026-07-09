using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Ore : MonoBehaviour
{
    [SerializeField] private ResourceType _resourceType;
    private int _oreCount;

    public ResourceType ResourceType => _resourceType;

    public void Init(int oreCount)
    {
        _oreCount = oreCount;
        
        if (TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
        {
            meshRenderer.material.color = ResourceConfig.GetColor(_resourceType);
        }
    }

    public int GetOreCount()
    {
        return _oreCount;
    }

    public void SetParentToOre(Transform parent, Vector3 position)
    {
        transform.SetParent(parent, false);
        transform.position = position;
    }
}
