using UnityEngine;

public static class PositionUtils
{
    public static Vector3 GetRandomCirclePosition(Vector3 center, float radius)
    {
        float theta = Random.Range(0f, Mathf.PI * 2f);
        float x = radius * Mathf.Cos(theta);
        float z = radius * Mathf.Sin(theta);

        return center + new Vector3(x, 0f, z);
    }
}
