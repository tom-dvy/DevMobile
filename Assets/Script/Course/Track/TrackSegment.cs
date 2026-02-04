using UnityEngine;

/// <summary>
/// Put this script on each TrackSegment GameObject
/// </summary>

public class TrackSegment : MonoBehaviour
{
    [Header("Segment Info")]
    public string segmentID; // ID of the segment
    public SegmentType type; // Type of segment
    
    [Header("Special Segment")]
    [Tooltip("Is this the start segment of the track?")]
    public bool isStartSegment = false;
    
    [Tooltip("Is this the finish segment of the track?")]
    public bool isFinishSegment = false;
    
    [Header("Connection Points")]
    public Transform startPoint; // Start point of the segment (where to connect the previous one)
    public Transform endPoint;   // End point of the segment (where to connect the next one)
    
    public enum SegmentType
    {
        Straight,
        CurveLeft,
        CurveRight
    }

    private void OnDrawGizmos()
    {
        // Visualisation in editor - For easy debugging
        if (startPoint != null && endPoint != null)
        {
            // Change color based on segment type
            if (isStartSegment)
            {
                Gizmos.color = Color.cyan;
            }
            else if (isFinishSegment)
            {
                Gizmos.color = Color.magenta;
            }
            else
            {
                Gizmos.color = Color.green;
            }
            
            Gizmos.DrawSphere(startPoint.position, 2f);
            
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(endPoint.position, 2f);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startPoint.position, endPoint.position);
        }
    }
}