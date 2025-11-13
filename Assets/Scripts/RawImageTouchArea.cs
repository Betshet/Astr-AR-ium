using UnityEngine;
using UnityEngine.EventSystems;

public class RawImageTouchArea : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    public GearRepairSystem gearSystem;
    public RectTransform gearCenterUI; // Centre de la roue (en coordonnées UI)

    [Header("Sound Settings")]
    public string ropeSound = "gear_reload";        // Son de la corde qui se remonte
    public string ropeStopSound = "gear_reload_end"; // "Clac" final quand c’est totalement remonté
    public float minRotationToTrigger = 0.2f;       // Évite les micro-déplacements

    private bool isDragging = false;
    private Vector2 lastPos;
    private bool ropeSoundPlaying = false;
    private bool repairCompleteTriggered = false;

    void Update()
    {
        // Si la corde est complètement remontée : joue le son final une seule fois
        if (!repairCompleteTriggered && gearSystem.IsFullyRepaired())
        {
            StopRopeSound(finalStop: true);
            repairCompleteTriggered = true;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (gearSystem.IsFullyRepaired()) return; // Empêche d'interagir après la fin
        isDragging = true;
        lastPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || gearSystem.IsFullyRepaired()) return;

        Vector2 currentPos = eventData.position;

        // Centre de la roue en coordonnées écran
        Vector2 center = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, gearCenterUI.position);
        Vector2 lastDir = lastPos - center;
        Vector2 currentDir = currentPos - center;

        // Calcul de l’angle et du sens de rotation
        float angle = Vector2.SignedAngle(lastDir, currentDir);

        // --- Rotation du gear ---
        gearSystem.RotateGear(-angle);

        // --- Gestion du son ---
        if (Mathf.Abs(angle) > minRotationToTrigger)
        {
            if (!ropeSoundPlaying)
            {
                SoundManager.Instance.Play(ropeSound, loop: true);
                ropeSoundPlaying = true;
            }
        }

        lastPos = currentPos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

        // Si la corde n’est pas encore terminée : arrête juste le son sans "clac"
        if (!gearSystem.IsFullyRepaired())
        {
            StopRopeSound(finalStop: false);
        }
    }

    private void StopRopeSound(bool finalStop)
    {
        if (ropeSoundPlaying)
        {
            SoundManager.Instance.Stop(ropeSound);
            ropeSoundPlaying = false;
        }

        if (finalStop)
        {
            SoundManager.Instance.Play(ropeStopSound);
        }
    }
}
