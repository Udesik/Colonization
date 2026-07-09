using UnityEngine;
using System;

public abstract class BaseSpawner<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private System.Collections.Generic.List<T> _prefabs;
    [SerializeField] private float _minRadius;
    [SerializeField] private float _maxRadius;

    public event Action<T> Spawned;

    protected void SpawnAll()
    {
        for (int i = 0; i < _prefabs.Count; i++)
        {
            if (_prefabs[i] == null) continue;

            Vector3 position = GetRandomRingPosition();
            T obj = Instantiate(_prefabs[i], position, Quaternion.identity);

            Spawned?.Invoke(obj);
        }
    }

    protected void SpawnOne()
    {
        if (_prefabs[0] == null) return;

        Vector3 position = GetRandomRingPosition();
        T obj = Instantiate(_prefabs[0], position, Quaternion.identity);

        Spawned?.Invoke(obj);
    }

    private Vector3 GetRandomRingPosition()
    {
        float theta = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        float u = UnityEngine.Random.value;
        float r = Mathf.Sqrt(u * (_maxRadius * _maxRadius - _minRadius * _minRadius) + _minRadius * _minRadius);

        float x = r * Mathf.Cos(theta);
        float z = r * Mathf.Sin(theta);

        return transform.position + new Vector3(x, 0f, z);
    }
}
