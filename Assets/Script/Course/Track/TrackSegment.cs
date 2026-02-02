using UnityEngine;

/// <summary>
/// Put this script on each TrackSegment GameObject
/// </summary>

public class TrackSegment : MonoBehaviour
{
    [Header("Segment Info")]
    public string segmentID; // ID of the segment
    public SegmentType type; // Type of segment
    
    [Header("Connection Points")]
    public Transform startPoint; // Start point of the segment (where to connect the previous one)
    public Transform endPoint;   // End point of the segment (where to connect the next one)
    
    public enum SegmentType
    {
        Straight,
        CurveLeft,
        CurveRight,
        SharpTurnLeft,
        SharpTurnRight,
        Chicane
    }

    private void OnDrawGizmos()
    {
        // Visualisation dans l'éditeur
        if (startPoint != null && endPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(startPoint.position, 2f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(endPoint.position, 2f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startPoint.position, endPoint.position);
        }
    }
}