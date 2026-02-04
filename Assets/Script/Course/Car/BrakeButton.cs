using UnityEngine;
using UnityEngine.EventSystems;

public class BrakeButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private CarController carController;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (carController != null)
        {
            carController.OnBrakePressed();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (carController != null)
        {
            carController.OnBrakeReleased();
        }
    }
}