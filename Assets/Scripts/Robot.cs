using UnityEngine;
using UnityEngine.AI;

public class Robot : MonoBehaviour
{
    [SerializeField] private Transform _positionOre;
    [SerializeField] private int _countGetOre = 4;
    private Vein _target;
    private Vector3 _basePosition;
    private Base _base;
    private NavMeshAgent _agent;
    private Ore _ore;
    private GameObject _basePrefab = null;

    private bool _isWaiting = true;
    private bool _hasOre = false;

    public bool IsWaiting => _isWaiting;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void TakeTarget(Vein target, Vector3 basePosition, Base baseObject)
    {
        _isWaiting = false;
        _hasOre = false;

        _target = target;
        _basePosition = basePosition;
        _base = baseObject;

        _agent.SetDestination(target.Position);
    }

    public void SetBuilder(Vector3 position, GameObject basePrefab)
    {
        _basePrefab = basePrefab;
        _agent.SetDestination(position);

        _isWaiting = false;
        _hasOre = true;
    }

    private void Update()
    {
        if (_isWaiting) return;

        if (!_agent.pathPending && _agent.hasPath && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            if (_basePrefab == null)
            {
                if (!_hasOre)
                {
                    TryCollectOre();
                }
                else
                {
                    DeliverOreToBase();
                }
            }
            else
            {
                BuildBase();
            }
        }
    }

    private void BuildBase()
    {
        GameObject newBase = Instantiate(_basePrefab, transform.position, transform.rotation);
        Base baseObject = newBase.GetComponent<Base>();
        baseObject.AddRobot(this); 
        _basePrefab = null;
    }

    private void TryCollectOre()
    {
        if (_target != null && _target.HasOre)
        {
            _ore = _target.GetOre(_countGetOre);
            
            if (_ore != null)
            {
                _ore.SetParentToOre(transform, _positionOre.position);
            }

            _hasOre = true;
            _agent.avoidancePriority = 10;
            _agent.SetDestination(_basePosition);
            _base.ReleaseVein(_target);
        }
        else
        {
            if (_target != null) _base.RemoveVein(_target);
            
            ResetToWaiting();
        }
    }

    private void DeliverOreToBase()
    {
        if (_ore != null)
        {
            _base.ReceiveOre(_ore);
            _ore = null;
        }

        ResetToWaiting();
    }

    private void ResetToWaiting()
    {
        _isWaiting = true;
        _agent.avoidancePriority = 50;
        _hasOre = false;
        _agent.ResetPath();
    }
}
