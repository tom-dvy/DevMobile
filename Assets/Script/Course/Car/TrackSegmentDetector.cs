using UnityEngine;


public class TrackSegmentDetector : MonoBehaviour
{
    [SerializeField] private LayerMask trackLayer = LayerMask.GetMask("Default");
    [SerializeField] private float raycastDistance = 20f;
    [SerializeField] private float updateInterval = 0.1f;
    [SerializeField] private bool debugMode = true;

    private PlayerRespawner _playerRespawner;
    private float _nextUpdateTime;

    private void Start()
    {
        _playerRespawner = GetComponent<PlayerRespawner>();
        if (_playerRespawner == null)
        {
            Debug.LogError("TrackSegmentDetector : PlayerRespawner non trouvé sur ce GameObject !");
        }
    }

    private void Update()
    {
        if (Time.time >= _nextUpdateTime)
        {
            DetectCurrentSegment();
            _nextUpdateTime = Time.time + updateInterval;
        }
    }

    private void DetectCurrentSegment()
    {
        if (_playerRespawner == null)
            return;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, raycastDistance, trackLayer))
        {
            if (debugMode)
                Debug.Log($"Raycast hit: {hit.collider.name}");

            TrackSegment segment = hit.collider.GetComponent<TrackSegment>();
            
            if (segment == null)
            {
                segment = hit.collider.GetComponentInParent<TrackSegment>();
            }

            if (segment != null)
            {
                if (debugMode)
                    Debug.Log($"Segment trouvé: {segment.gameObject.name}");
                
                _playerRespawner.SetCurrentSegment(segment);
            }
            else if (debugMode)
            {
                Debug.LogWarning($"Collider touché ({hit.collider.name}) n'a pas de TrackSegment!");
            }
        }
        else if (debugMode)
        {
            Debug.LogWarning("Raycast n'a touché aucun segment - vérifier le Layer et la distance!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * raycastDistance);
    }
}
