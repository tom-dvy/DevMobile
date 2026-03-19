using UnityEngine;

/// <summary>
/// Gère le respawn du joueur sans toucher au CarSpawner (qui gère uniquement le spawn initial).
/// Attachez ce script au GameObject de la voiture (ou au prefab du joueur).
/// </summary>
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

        // Position de respawn par défaut
        _lastSafePosition = transform.position;
    }

    /// <summary>
    /// Enregistre le segment actuel du joueur (à appeler quand on détecte que le joueur est sur la piste).
    /// </summary>
    public void SetCurrentSegment(TrackSegment segment)
    {
        _currentSegment = segment;
        UpdateSafePositionFromSegment();
    }

    /// <summary>
    /// Met à jour la position safe en calculant le milieu du segment courant.
    /// </summary>
    private void UpdateSafePositionFromSegment()
    {
        if (_currentSegment != null && _currentSegment.startPoint != null && _currentSegment.endPoint != null)
        {
            // Calcul du milieu du segment
            _lastSafePosition = (_currentSegment.startPoint.position + _currentSegment.endPoint.position) * 0.5f;
        }
    }

    /// <summary>
    /// Téléporte le joueur à la dernière position sûre et réinitialise la physique.
    /// </summary>
    public void Respawn()
    {
        // Utiliser le spawnPoint si défini, sinon utiliser la position du segment
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
