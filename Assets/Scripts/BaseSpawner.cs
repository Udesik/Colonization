using UnityEngine;
using System;
using System.Collections.Generic;

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
                Debug.Log("Try send builder");
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
        List<Robot> robots = _base.GetWaitingRobots();
        Debug.Log($"Count waiting robots: {robots.Count}");

        if (robots.Count == 0) return;

        Robot robot = robots[0];

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
