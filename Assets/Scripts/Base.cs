using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Flag), typeof(RobotSpawner), typeof(VeinSpawner))]
[RequireComponent(typeof(BaseSpawner))]
public class Base : MonoBehaviour
{
    [SerializeField] private GameObject _basePrefab;
    private RobotSpawner _robotSpawner;
    private VeinSpawner _veinSpawner;
    private BaseSpawner _baseSpawner;
    private List<Robot> _robots;
    private List<Vein> _veins;
    private int _radiuseReturn = 5;
    private int _countResurceToSpawn = 3;

    private Dictionary<ResourceType, int> _oreCounts = new Dictionary<ResourceType, int>();
    private int _maxOreCount = 30;
    private HashSet<Vein> _targetedVeins = new HashSet<Vein>();
    private Vector3 _newBasePosition = Vector3.zero;

    public event Action<Dictionary<ResourceType, int>, int> ResourceReceived;
    public int RobotCount => _robots.Count;

    private void Awake()
    {
        _robots = new List<Robot>();
        _veins = new List<Vein>();

        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            _oreCounts[type] = 0;
        }

        _robotSpawner = GetComponent<RobotSpawner>();
        _veinSpawner = GetComponent<VeinSpawner>();
        _baseSpawner = GetComponent<BaseSpawner>();
    }

    private void OnEnable()
    {
        _robotSpawner.Spawned += AddRobot;
        _veinSpawner.Spawned += AddVein;
    }

    private void OnDisable()
    {
        _robotSpawner.Spawned -= AddRobot;
        _veinSpawner.Spawned -= AddVein;
    }

    private void Update()
    {
        ScanLocations();
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
            robot.TakeTarget(targetVein, PositionUtils.GetRandomCirclePosition(transform.position ,_radiuseReturn), this);
        }
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

    private void AddVein(Vein vein)
    {
        _veins.Add(vein);
    }

    private void RemoveRobot(Robot robot)
    {
        if (_robots.Contains(robot)) 
        {
            robot.RemoveMe -= RemoveRobot;
            _robots.Remove(robot);
        }
    }

    public void RemoveVein(Vein vein)
    {
        if (_veins.Contains(vein)) _veins.Remove(vein);
        if (_targetedVeins.Contains(vein)) _targetedVeins.Remove(vein);
    }

    public void AddRobot(Robot robot)
    {
        _robots.Add(robot);
        robot.RemoveMe += RemoveRobot;
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

        if (_oreCounts[type] >= _countResurceToSpawn && _baseSpawner.GetNewBasePosition() == false)
        {
            _oreCounts[type] -= _countResurceToSpawn;
            _robotSpawner.SpawnRobot();
        }
        
        ResourceReceived?.Invoke(_oreCounts, _maxOreCount);
        Destroy(ore.gameObject);
    }

    public ResourceType? GetOreTypeAvailableForBuild()
    {
        foreach (var pair in _oreCounts)
        {
            if (pair.Value >= _baseSpawner.CountOreToBuild)
            {
                return pair.Key;
            }
        }
        
        return null;
    }

    public List<Robot> GetWaitingRobots()
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

    public void SpendResource(ResourceType type, int count)
    {
        _oreCounts[type] -= count;
        ResourceReceived?.Invoke(_oreCounts, _maxOreCount);
    }
}
