using UnityEngine;
using System.Collections.Generic;

public class TrackGenerator : MonoBehaviour
{
    [Header("Track Settings")]
    public int numberOfSegments = 10; // Number of segments to generate
    public int seed = 12345; // for random generation
    
    [Header("Segment Prefabs")]
    public List<TrackSegment> availableSegments = new List<TrackSegment>(); // List of segment prefabs
    
    [Header("Special Segments")]
    [Tooltip("Segment to use at the start")]
    public TrackSegment startSegment; // Start segment
    
    [Tooltip("Segment to use at the finish")]
    public TrackSegment finishSegment; // Finish segment
    
    [Header("Generated Track")]
    public Transform trackParent;
    
    private List<TrackSegment> generatedSegments = new List<TrackSegment>(); // List of spawned segments
    private Vector3 currentSpawnPosition; // Position for next segment
    private Quaternion currentSpawnRotation; // Orientation for next segment

    void Start()
    {
        GenerateTrack();
    }

    public void GenerateTrack()
    {
        // Remove previous track
        ClearTrack();
        
        // Initialize random with seed
        Random.InitState(seed);
        
        currentSpawnPosition = Vector3.zero;
        currentSpawnRotation = Quaternion.identity;
        
        // Generate segments
        for (int i = 0; i < numberOfSegments; i++)
        {
            TrackSegment segmentPrefab = GetSegmentForIndex(i);
            SpawnSegment(segmentPrefab, i);
        }
        
        Debug.Log($"Track generated with {generatedSegments.Count} segments using seed: {seed}");
    }

    private TrackSegment GetSegmentForIndex(int index)
    {
        // First segment: use startSegment if assigned
        if (index == 0 && startSegment != null)
        {
            return startSegment;
        }
        
        // Last segment: use finishSegment if assigned
        if (index == numberOfSegments - 1 && finishSegment != null)
        {
            return finishSegment;
        }
        
        // Otherwise, use random segment
        return GetRandomSegment();
    }

    private TrackSegment GetRandomSegment()
    {
        if (availableSegments.Count == 0)
        {
            Debug.LogError("No available segments! Add segment prefabs to the list.");
            return null;
        }
        
        int randomIndex = Random.Range(0, availableSegments.Count);
        return availableSegments[randomIndex];
    }

    private void SpawnSegment(TrackSegment segmentPrefab, int segmentIndex)
    {
        if (segmentPrefab == null) return;
        
        TrackSegment newSegment = Instantiate(segmentPrefab, trackParent);
        newSegment.name = $"Segment_{segmentIndex}_{segmentPrefab.type}";
        
        if (generatedSegments.Count == 0)
        {
            // first segment, place at origin
            newSegment.transform.position = currentSpawnPosition;
            newSegment.transform.rotation = currentSpawnRotation;
        }
        else
        {
            // Align the startPoint of the new segment with the endPoint of the last segment
            TrackSegment previousSegment = generatedSegments[generatedSegments.Count - 1];
            AlignSegment(newSegment, previousSegment);
        }
        
        generatedSegments.Add(newSegment);
        
        // Update spawn position and rotation for next segment
        UpdateSpawnTransform(newSegment);
    }

    private void AlignSegment(TrackSegment newSegment, TrackSegment previousSegment)
    {
        // Calculate offset between new segment's startPoint and its position
        Vector3 offset = newSegment.transform.position - newSegment.startPoint.position;
        
        // Align segment position
        newSegment.transform.position = previousSegment.endPoint.position + offset;
        
        // Align rotation
        Quaternion rotationDifference = Quaternion.Inverse(newSegment.startPoint.rotation) * newSegment.transform.rotation;
        newSegment.transform.rotation = previousSegment.endPoint.rotation * rotationDifference;
    }

    private void UpdateSpawnTransform(TrackSegment segment)
    {
        currentSpawnPosition = segment.endPoint.position;
        currentSpawnRotation = segment.endPoint.rotation;
    }

    public void ClearTrack()
    {
        foreach (TrackSegment segment in generatedSegments)
        {
            if (segment != null)
                Destroy(segment.gameObject);
        }
        generatedSegments.Clear();
    }

    // to regenerate track with a new seed
    public void RegenerateWithSeed(int newSeed)
    {
        seed = newSeed;
        GenerateTrack();
    }
    
    public List<TrackSegment> GetGeneratedSegments()
    {
        return generatedSegments;
    }

    private void OnValidate()
    {
        if (trackParent == null)
        {
            GameObject parent = new GameObject("GeneratedTrack");
            trackParent = parent.transform;
            trackParent.SetParent(transform);
        }
    }
}