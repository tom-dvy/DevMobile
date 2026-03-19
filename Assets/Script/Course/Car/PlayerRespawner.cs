using UnityEngine;


[DisallowMultipleComponent]
public class PlayerRespawner : MonoBehaviour
{
    [Tooltip("(Optionnel) Si vide, le respawn se fait au milieu du segment courant.")]
    [SerializeField] private Transform spawnPoint;

    private TrackSegment _currentSegment;
    private Vector3 _lastSafePosition;
    private Rigidbody _rigidbody;
    private CarController _carController;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _carController = GetComponent<CarController>();

        _lastSafePosition = transform.position;
    }

    public void SetCurrentSegment(TrackSegment segment)
    {
        _currentSegment = segment;
        UpdateSafePositionFromSegment();
    }

    private void UpdateSafePositionFromSegment()
    {
        if (_currentSegment != null && _currentSegment.startPoint != null && _currentSegment.endPoint != null)
        {
            _lastSafePosition = (_currentSegment.startPoint.position + _currentSegment.endPoint.position) * 0.5f;
        }
    }

    public void Respawn()
    {
        Vector3 respawnPos = _lastSafePosition;
        if (spawnPoint != null)
        {
            respawnPos = spawnPoint.position;
        }

        if (_carController != null)
            _carController.enabled = true;

        transform.position = respawnPos;

        if (_rigidbody != null)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }
}
