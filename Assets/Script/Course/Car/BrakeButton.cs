using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles a UI brake button
/// This script sends brake press and release events to a CarController,
/// allowing mobile or UI-based braking input.
/// </summary>
public class BrakeButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // Reference to the car controller that will receive brake inputs
    [SerializeField] private CarController carController;

    // Called when the UI button is pressed.
    public void OnPointerDown(PointerEventData eventData)
    {
        if (carController == null) return;

        carController.OnBrakePressed();
    }

    // Called when the UI button is released.
    // Stops the braking behavior on the car.
    public void OnPointerUp(PointerEventData eventData)
    {
        if (carController == null) return;

        carController.OnBrakeReleased();
    }
}
