using UnityEngine;
using UnityEngine.EventSystems;

public class RawImageTouchArea : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public GearRepairSystem gearSystem;
    public RectTransform gearCenterUI; // Centre de la roue (en coordonnées UI)

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

        Vector2 currentPos = eventData.position;

        // On calcule l'angle entre la position précédente et la nouvelle par rapport au centre de la roue
        Vector2 center = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, gearCenterUI.position);
        Vector2 lastDir = lastPos - center;
        Vector2 currentDir = currentPos - center;

        // On mesure l'angle et le signe via le produit vectoriel (détermine sens horaire/anti-horaire)
        float angle = Vector2.SignedAngle(lastDir, currentDir);

        // On transmet cet angle au système
        gearSystem.RotateGear(-angle);

        lastPos = currentPos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }
}
