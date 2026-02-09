using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Procedurally generates a racetrack by spawning and aligning track segments.
/// Supports seeded random generation, start and finish segments,
/// and automatic alignment using segment start/end points.
/// </summary>
public class TrackGenerator : MonoBehaviour
{
    [Header("Track Settings")]
    public int numberOfSegments = 10; // Total number of segments to generate
    public int seed = 12345;          // Seed for deterministic random generation

    [Header("Segment Prefabs")]
    public List<TrackSegment> availableSegments = new List<TrackSegment>(); // Pool of random segments

    [Header("Special Segments")]
    [Tooltip("Segment used at the beginning of the track")]
    public TrackSegment startSegment;

    [Tooltip("Segment used at the end of the track")]
    public TrackSegment finishSegment;

    [Header("Generated Track")]
    public Transform trackParent; // Parent transform for spawned segments

    private readonly List<TrackSegment> generatedSegments = new List<TrackSegment>();
    private Vector3 currentSpawnPosition;
    private Quaternion currentSpawnRotation;

    private void Start()
    {
        GenerateTrack();
    }

    public void GenerateTrack()
    {
        // Remove previously generated segments
        ClearTrack();

        // Initialize random generator
        Random.InitState(seed);

        currentSpawnPosition = Vector3.zero;
        currentSpawnRotation = Quaternion.identity;

        // Spawn all segments
        for (int i = 0; i < numberOfSegments; i++)
        {
            TrackSegment segmentPrefab = GetSegmentForIndex(i);
            SpawnSegment(segmentPrefab, i);
        }
    }

    private TrackSegment GetSegmentForIndex(int index)
    {
        // First segment
        if (index == 0 && startSegment != null)
            return startSegment;

        // Last segment
        if (index == numberOfSegments - 1 && finishSegment != null)
            return finishSegment;

        // Random segment
        return GetRandomSegment();
    }

    private TrackSegment GetRandomSegment()
    {
        if (availableSegments.Count == 0)
        {
            Debug.LogError("No available track segments assigned.");
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
            // First segment is placed at origin
            newSegment.transform.position = currentSpawnPosition;
            newSegment.transform.rotation = currentSpawnRotation;
        }
        else
        {
            // Align new segment with the previous one
            TrackSegment previousSegment = generatedSegments[^1];
            AlignSegment(newSegment, previousSegment);
        }

        generatedSegments.Add(newSegment);

        // Update transform for next spawn
        UpdateSpawnTransform(newSegment);
    }

    private void AlignSegment(TrackSegment newSegment, TrackSegment previousSegment)
    {
        // Calculate positional offset from the start point
        Vector3 offset = newSegment.transform.position - newSegment.startPoint.position;

        // Align position
        newSegment.transform.position = previousSegment.endPoint.position + offset;

        // Align rotation
        Quaternion rotationOffset =
            Quaternion.Inverse(newSegment.startPoint.rotation) * newSegment.transform.rotation;

        newSegment.transform.rotation =
            previousSegment.endPoint.rotation * rotationOffset;
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
            {
                Destroy(segment.gameObject);
            }
        }

        generatedSegments.Clear();
    }

    // Regenerates the track using a new seed value
    public void RegenerateWithSeed(int newSeed)
    {
        seed = newSeed;
        GenerateTrack();
    }

    // Returns the list of generated segments
    public List<TrackSegment> GetGeneratedSegments()
    {
        return generatedSegments;
    }

    private void OnValidate()
    {
        // Automatically create a parent object if none is assigned
        if (trackParent == null)
        {
            GameObject parent = new GameObject("GeneratedTrack");
            parent.transform.SetParent(transform);
            trackParent = parent.transform;
        }
    }
}
