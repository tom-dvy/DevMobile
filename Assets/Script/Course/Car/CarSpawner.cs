using UnityEngine;
using System.Collections;

public class CarSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private TrackGenerator trackGenerator;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0, 1, 5);
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private float maxWaitTime = 2f;
    [SerializeField] private bool disableCarControllerOnSpawn = true; // NOUVEAU

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
                SpawnCar();
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
            waitedTime += 0.1f;
        }

        Debug.LogError($"Timeout waiting for track generation after {maxWaitTime}s");
    }

    private void SpawnCar()
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

        // NOUVEAU : Désactiver le CarController pour attendre le countdown
        if (disableCarControllerOnSpawn)
        {
            CarController carController = GetComponent<CarController>();
            if (carController != null)
            {
                carController.enabled = false;
            }
        }
    }

    public void RespawnCar()
    {
        SpawnCar();
    }
}