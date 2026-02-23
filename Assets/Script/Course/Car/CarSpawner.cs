using UnityEngine;
using System.Collections;

public class CarSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private TrackGenerator trackGenerator;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0, 0.1f, 5);
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private float maxWaitTime = 2f;
    [SerializeField] private bool disableCarControllerOnSpawn = true;

    [Header("Ghost Settings")]
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private Vector3 ghostOffset = new Vector3(-2, 0, 0);

    private void Start()
    {
        if (spawnOnStart)
        {
            StartCoroutine(WaitForTrackAndSpawn());
        }
    }

    private IEnumerator WaitForTrackAndSpawn()
    {
        if (trackGenerator == null)
        {
            trackGenerator = FindFirstObjectByType<TrackGenerator>();
        }

        if (trackGenerator == null)
        {
            Debug.LogError("TrackGenerator not found!");
            yield break;
        }

        float waitedTime = 0f;
        while (waitedTime < maxWaitTime)
        {
            var segments = trackGenerator.GetGeneratedSegments();

            if (segments != null && segments.Count > 0)
            {
                SpawnAll(segments[0]);
                //SpawnCar();
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
            waitedTime += 0.1f;
        }

        Debug.LogError($"Timeout waiting for track generation after {maxWaitTime}s");
    }

    /*private void SpawnCar()
    {
        var segments = trackGenerator.GetGeneratedSegments();
        
        if (segments == null || segments.Count == 0)
        {
            Debug.LogError("No segments available!");
            return;
        }

        TrackSegment firstSegment = segments[0];
        
        if (firstSegment == null || firstSegment.startPoint == null)
        {
            Debug.LogError("First segment or start point is null!");
            return;
        }

        Transform startPoint = firstSegment.startPoint;
        Vector3 spawnPosition = startPoint.position + startPoint.TransformDirection(spawnOffset);
        Quaternion spawnRotation = startPoint.rotation;

        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        SpawnGhost(spawnPosition, spawnRotation);
        if (disableCarControllerOnSpawn)
        {
            CarController carController = GetComponent<CarController>();
            if (carController != null)
            {
                carController.enabled = false;
            }
        }
    }

    private void SpawnGhost(Vector3 position, Quaternion rotation)
{
    if (ghostPrefab != null)
    {
        GameObject ghost = Instantiate(ghostPrefab, position, rotation);
        GhostRace ghostScript = ghost.GetComponent<GhostRace>();
        if (ghostScript != null)
        {
            // On s'assure que le ghost a bien accès au CloudSave de la scène
            ghostScript.cloud = FindFirstObjectByType<CloudSave>();
        }
    }
}
*/

    private void SpawnAll(TrackSegment firstSegment)
    {
        Transform startPoint = firstSegment.startPoint;
        Vector3 playerSpawnPos = startPoint.position + startPoint.TransformDirection(spawnOffset);
        Quaternion spawnRot = startPoint.rotation;

        transform.position = playerSpawnPos;
        transform.rotation = spawnRot;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }

        if (disableCarControllerOnSpawn)
        {
            CarController car = GetComponent<CarController>();
            if (car != null) car.enabled = false;
        }

        if (ghostPrefab != null)
        {
            Vector3 ghostSpawnPos = playerSpawnPos + startPoint.TransformDirection(ghostOffset);
            GameObject ghost = Instantiate(ghostPrefab, ghostSpawnPos, spawnRot);
            
            GhostRace ghostScript = ghost.GetComponent<GhostRace>();
            if (ghostScript != null)
            {
                ghostScript.cloud = FindFirstObjectByType<CloudSave>();

                if(ghostScript.cloud != null) {
            ghostScript.cloud.Timer = FindFirstObjectByType<RaceTimer>();
        }
            }
        }
    }
    public void RespawnCar()
    {
        SpawnAll(trackGenerator.GetGeneratedSegments()[0]);
        //SpawnCar();
    }
}