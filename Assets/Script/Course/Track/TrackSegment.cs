using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

/// <summary>
/// Represents a single track segment used by the procedural track generator.
/// Defines connection points, segment type, and editor visualization.
/// </summary>
public class TrackSegment : MonoBehaviour
{
    [Header("Segment Info")]
    public string segmentID;        // Optional identifier for the segment
    public SegmentType type;        // Gameplay type of the segment

    [Header("Special Segment")]
    [Tooltip("True if this segment is used as the track start")]
    public bool isStartSegment = false;

    [Tooltip("True if this segment is used as the track finish")]
    public bool isFinishSegment = false;

    [Header("Connection Points")]
    public Transform startPoint;    // Where the previous segment connects
    public Transform endPoint;      // Where the next segment connects

    [Header("Animation Settings")]
    [Tooltip("Duration of the descent animation when the segment is spawned")]
    public float descentDuration = 1.5f;

    [Tooltip("Height offset applied during the descent animation to create a falling effect")]
    public float heightOffset = 10f;

    public enum SegmentType
    {
        Straight,
        CurveLeft,
        CurveRight
    }

    public void Start()
    {
        if (!isStartSegment && !isFinishSegment)
        {
            Vector3 BasePosition = transform.position;

            Vector3 StartPosition = BasePosition + (Vector3.up * heightOffset);

            transform.position = StartPosition;

            LMotion.Create(StartPosition, BasePosition, descentDuration)
                .WithEase(Ease.OutQuad)
                .BindToPosition(transform);
        }
    }

    private void OnDrawGizmos()
    {
        // Editor-only visualization to help with segment alignment
        if (startPoint == null || endPoint == null) return;

        // Color coding for easier identification
        if (isStartSegment)
            Gizmos.color = Color.cyan;
        else if (isFinishSegment)
            Gizmos.color = Color.magenta;
        else
            Gizmos.color = Color.green;

        // Draw start point
        Gizmos.DrawSphere(startPoint.position, 2f);

        // Draw end point
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(endPoint.position, 2f);

        // Draw direction line
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPoint.position, endPoint.position);
    }
}
