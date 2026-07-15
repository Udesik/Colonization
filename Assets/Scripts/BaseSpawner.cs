using UnityEngine;
using System;

public class BaseSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _countOreToBuild;

    private Flag _flag;
    private Base _base;
    private Vector3 _newBasePosition = Vector3.zero;

    public int CountOreToBuild => _countOreToBuild;

    private void Awake()
    {
        _flag = GetComponent<Flag>();
        _base = GetComponent<Base>();
    }

    private void OnEnable()
    {
        _flag.Request += OnBaseBuildRequest;
    }

    private void OnDisable()
    {
        _flag.Request -= OnBaseBuildRequest;
    }

    private void Update()
    {
        if (_newBasePosition != Vector3.zero)
        {
            ResourceType? availableOre = _base.GetOreTypeAvailableForBuild();

            if (availableOre != null)
            {
                TrySendBuilder(availableOre.Value);
            }
        }
    }

    private void OnBaseBuildRequest(Vector3 position)
    {
        if (_base.RobotCount > 1)
        {
            _newBasePosition = position;
        }
        else
        {
            _flag.Stop();
        }
    }

    private void TrySendBuilder(ResourceType resource)
    {
        Robot robot = _base.GetWaitingRobots()[0];

        if (robot != null)
        {
            _base.SpendResource(resource, _countOreToBuild);
            robot.SetBuilder(_newBasePosition, _prefab);
            _newBasePosition = Vector3.zero;
            _flag.Stop();
        }
    }

    public bool GetNewBasePosition()
    {
        if (_newBasePosition != Vector3.zero)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
