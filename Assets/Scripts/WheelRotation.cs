using UnityEngine;
using UnityEngine.EventSystems;

public class WheelRotation : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Transform rope; // ton rope parent
    public float ropeSpeed = 0.1f;
    public float rotationSpeed = 5f;

    private RectTransform wheelRect;
    private Vector2 wheelCenter;
    private float lastAngle;
    private bool isDragging = false;

    void Start()
    {
        wheelRect = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(wheelRect, eventData.position, eventData.pressEventCamera, out Vector2 localPos);
        wheelCenter = wheelRect.rect.center;
        lastAngle = Mathf.Atan2(localPos.y - wheelCenter.y, localPos.x - wheelCenter.x) * Mathf.Rad2Deg;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(wheelRect, eventData.position, eventData.pressEventCamera, out Vector2 localPos);
        float currentAngle = Mathf.Atan2(localPos.y - wheelCenter.y, localPos.x - wheelCenter.x) * Mathf.Rad2Deg;
        float angleDelta = Mathf.DeltaAngle(lastAngle, currentAngle);

        // Rotation de la roue
        transform.Rotate(Vector3.forward, -angleDelta * rotationSpeed);

        // Corde qui monte ou descend selon le sens de rotation
        Vector3 scale = rope.localScale;
        scale.z = Mathf.Clamp(scale.z + angleDelta * 0.001f * ropeSpeed, 0.1f, 1f);
        rope.localScale = scale;

        lastAngle = currentAngle;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }
}