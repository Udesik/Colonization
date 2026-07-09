using UnityEngine;

public class Vein : MonoBehaviour
{
    [SerializeField] private Ore _orePrefab;
    [SerializeField] private int _oreCount = 100;

    public Vector3 Position => transform.position;
    public ResourceType Type => _orePrefab.ResourceType;
    public bool HasOre => _oreCount > 0;

    public Ore GetOre(int count)
    {
        int amountToTake = Mathf.Min(count, _oreCount);
        _oreCount -= amountToTake;

        Ore ore = Instantiate(_orePrefab, transform.position, Quaternion.identity);
        ore.Init(amountToTake);

        return ore;
    }
}
