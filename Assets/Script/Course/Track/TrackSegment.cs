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
    public string segmentID;
    public SegmentType type;

    [Header("Special Segment")]
    public bool isStartSegment = false;
    public bool isFinishSegment = false;

    [Header("Connection Points")]
    public Transform startPoint;
    public Transform endPoint;

    [Header("Animation Settings")]
    [Tooltip("Duration of the descent animation")]
    public float descentDuration = 1.5f;

    [Tooltip("Height offset applied during the descent animation")]
    public float heightOffset = 10f;

    [Tooltip("Delay between each segment number (ex: 0.1 means Segment_2 starts 0.1s after Segment_1)")]
    public float delayPerIndex = 0.15f;

    public enum SegmentType
    {
        Straight,
        CurveLeft,
        CurveRight
    }

    public void Start()
    {
        if (!isStartSegment)
        {
            int index = GetIndexFromName();

            Vector3 basePosition = transform.position;
            Vector3 startPosition = basePosition + (Vector3.up * heightOffset);

            transform.position = startPosition;

            float finalDelay = index * delayPerIndex;

            LMotion.Create(startPosition, basePosition, descentDuration)
                .WithDelay(finalDelay)
                .WithEase(Ease.OutBack)
                .BindToPosition(transform);
        }
    }

    /// <summary>
    /// Read the segment index from the GameObject's name
    /// attended format : "Text_Digit_Text" (ex: "Segment_1_Straight")
    /// </summary>
    private int GetIndexFromName()
    {
        string[] parts = gameObject.name.Split('_');
        
        if (parts.Length > 1 && int.TryParse(parts[1], out int result))
        {
            return result;
        }

        return 0;
    }

    private void OnDrawGizmos()
    {
        if (startPoint == null || endPoint == null) return;

        if (isStartSegment) Gizmos.color = Color.cyan;
        else if (isFinishSegment) Gizmos.color = Color.magenta;
        else Gizmos.color = Color.green;

        Gizmos.DrawSphere(startPoint.position, 1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(endPoint.position, 1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPoint.position, endPoint.position);
    }
}