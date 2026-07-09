using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Flag), typeof(RobotSpawner), typeof(VeinSpawner))]
public class Base : MonoBehaviour
{
    [SerializeField] private GameObject _basePrefab;
    private RobotSpawner _robotSpawner;
    private VeinSpawner _veinSpawner;
    private Flag _flag;
    private List<Robot> _robots;
    private List<Vein> _veins;
    private int _radiuseReturn = 5;
    private int _countResurceToSpawn = 3;
    private int _countResurceToBuild = 5;

    private Dictionary<ResourceType, int> _oreCounts = new Dictionary<ResourceType, int>();
    private int _maxOreCount = 30;
    private HashSet<Vein> _targetedVeins = new HashSet<Vein>();
    private Vector3 _newBasePosition = Vector3.zero;

    public event Action<Dictionary<ResourceType, int>, int> ResourceReceived;

    private void Awake()
    {
        _robots = new List<Robot>();
        _veins = new List<Vein>();

        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            _oreCounts[type] = 0;
        }

        _flag = GetComponent<Flag>();
        _robotSpawner = GetComponent<RobotSpawner>();
        _veinSpawner = GetComponent<VeinSpawner>();
        _flag.NotPlan();
    }

    private void OnEnable()
    {
        _robotSpawner.Spawned += AddRobot;
        _veinSpawner.Spawned += AddVein;
        _flag.Request += AddBase;
    }

    private void OnDisable()
    {
        _robotSpawner.Spawned -= AddRobot;
        _veinSpawner.Spawned -= AddVein;
        _flag.Request -= AddBase;
    }

    private void Update()
    {
        if (_newBasePosition != Vector3.zero && _oreCounts[GetLeastNeededOre()] >= _countResurceToBuild)
        {
            SentToBuild();
        }
        else
        {
            ScanLocations();
        }
    }

    private void SentToBuild()
    {
        ResourceType resource = GetLeastNeededOre();

        _oreCounts[resource] -= _countResurceToBuild;
        ResourceReceived?.Invoke(_oreCounts, _maxOreCount);

        List<Robot> waitingRobots = GetWaitingRobots();

        if (waitingRobots.Count == 0) return;

        waitingRobots[0].SetBuilder(_newBasePosition, _basePrefab);
        _robots.Remove(waitingRobots[0]);
        _newBasePosition = Vector3.zero;
        _flag.NotPlan();
    }

    private void ScanLocations()
    {
        if (_veins.Count == 0 || _robots.Count == 0) return;

        List<Robot> waitingRobots = GetWaitingRobots();

        if (waitingRobots.Count == 0) return;

        foreach (Robot robot in waitingRobots)
        {
            if (!robot.IsWaiting) continue;

            ResourceType? mostNeededOre = GetMostNeededOre();

            if (mostNeededOre == null) break;

            Vein targetVein = FindFreeVeinByType(mostNeededOre.Value);

            if (targetVein == null)
            {
                targetVein = FindAnyAvailableVein();
            }

            if (targetVein == null) break;

            _targetedVeins.Add(targetVein);
            robot.TakeTarget(targetVein, GetRandomCirclePosition(_radiuseReturn), this);
        }
    }

    private List<Robot> GetWaitingRobots()
    {
        List<Robot> waiting = new List<Robot>();

        foreach (Robot robot in _robots)
        {
            if (robot != null && robot.IsWaiting)
            {
                waiting.Add(robot);
            }
        }

        return waiting;
    }

    private ResourceType? GetMostNeededOre()
    {
        ResourceType? bestType = null;
        int lowestCount = int.MaxValue;

        foreach (var pair in _oreCounts)
        {
            if (pair.Value < _maxOreCount && pair.Value < lowestCount)
            {
                lowestCount = pair.Value;
                bestType = pair.Key;
            }
        }

        return bestType;
    }

    private ResourceType GetLeastNeededOre()
    {
        ResourceType bestType = ResourceType.None;
        int lowestCount = int.MinValue;

        foreach (var pair in _oreCounts)
        {
            if (pair.Value > _maxOreCount && pair.Value > lowestCount)
            {
                lowestCount = pair.Value;
                bestType = pair.Key;
            }
        }

        return bestType;
    }

    private Vein FindFreeVeinByType(ResourceType type)
    {
        foreach (Vein vein in _veins)
        {
            if (vein != null && vein.Type == type && vein.HasOre && !_targetedVeins.Contains(vein))
            {
                return vein;
            }
        }

        return null;
    }

    private Vein FindAnyAvailableVein()
    {
        foreach (Vein vein in _veins)
        {
            if (vein == null || !vein.HasOre || _targetedVeins.Contains(vein)) continue;

            if (_oreCounts.TryGetValue(vein.Type, out int count) && count < _maxOreCount)
            {
                return vein;
            }
        }

        return null;
    }

    private void AddBase(Vector3 position)
    {
        if (_robots.Count > 1)
        {
            _newBasePosition = position;
        }
        else
        {
            _flag.NotPlan();
        }
    }

    public void AddRobot(Robot robot)
    {
        _robots.Add(robot);
    }

    private void AddVein(Vein vein)
    {
        _veins.Add(vein);
    }

    public void RemoveVein(Vein vein)
    {
        if (_veins.Contains(vein)) _veins.Remove(vein);
        if (_targetedVeins.Contains(vein)) _targetedVeins.Remove(vein);
    }

    public void ReleaseVein(Vein vein)
    {
        if (_targetedVeins.Contains(vein)) _targetedVeins.Remove(vein);
    }

    public void ReceiveOre(Ore ore)
    {
        if (ore == null) return;

        ResourceType type = ore.ResourceType;
        _oreCounts[type] = Mathf.Min(_oreCounts[type] + ore.GetOreCount(), _maxOreCount);

        if (_oreCounts[type] >= _countResurceToSpawn && _newBasePosition == Vector3.zero)
        {
            _oreCounts[type] -= _countResurceToSpawn;
            _robotSpawner.SpawnRobot();
        }
        
        ResourceReceived?.Invoke(_oreCounts, _maxOreCount);
        Destroy(ore.gameObject);
    }

    private Vector3 GetRandomCirclePosition(float radius)
    {
        float theta = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        float x = radius * Mathf.Cos(theta);
        float z = radius * Mathf.Sin(theta);

        return transform.position + new Vector3(x, 0f, z);
    }
}
