using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] private RobotSpawner _robotSpawner;

    private void Start()
    {
        _robotSpawner.SpawnRobot();
        _robotSpawner.SpawnRobot();
    } 
}
