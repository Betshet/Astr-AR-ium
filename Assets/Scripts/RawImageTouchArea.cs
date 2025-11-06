using UnityEngine;
using UnityEngine.EventSystems;

public class RawImageTouchArea : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public GearRepairSystem gearSystem;
    private bool isDragging = false;
    private Vector2 lastPos;

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        lastPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector2 delta = eventData.position - lastPos;
        float rotation = delta.x * 0.5f;

        gearSystem.RotateGear(rotation);
        lastPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }
}