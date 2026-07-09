public enum ResourceType
{
    Gold,
    Amethyst,
    Lazurit,
    Ruby,
    None = 0
}

public static class ResourceConfig
{
    private static readonly System.Collections.Generic.Dictionary<ResourceType, UnityEngine.Color> Colors = 
        new System.Collections.Generic.Dictionary<ResourceType, UnityEngine.Color>
        {
            { ResourceType.Gold, UnityEngine.Color.yellow },
            { ResourceType.Amethyst, new UnityEngine.Color(0.6f, 0f, 1f) },
            { ResourceType.Lazurit, UnityEngine.Color.blue },
            { ResourceType.Ruby, UnityEngine.Color.red }
        };

    public static UnityEngine.Color GetColor(ResourceType type)
    {
        return Colors.TryGetValue(type, out UnityEngine.Color color) ? color : UnityEngine.Color.white;
    }
}
