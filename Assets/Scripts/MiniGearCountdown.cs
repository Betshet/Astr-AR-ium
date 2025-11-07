using UnityEngine;

public class MiniGearCountdown : MonoBehaviour
{
    public Transform poids;         // Le poids
    public Transform corde;         // La corde
    public Transform roue;
    public float duration = 10f;   // Le temps de déroulement
    public Transform startPoint;    // Position haute de la ficelle
    public Transform endPoint;      // Position basse de la ficelle
    public GameObject systemToActivate; // Système d'engrenages à remonter
    public GearRepairSystem repairSystem; // référence au script

    private float timer = 0f;
    private bool active = true;

    void Start()
    {
        ResetFicelle();
    }

    void Update()
    {
        if (!active) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);

        // Descente uniquement sur Y
        Vector3 newPos = poids.position;
        newPos.y = Mathf.Lerp(startPoint.position.y, endPoint.position.y, t);
        // X et Z restent fixes
        newPos.x = startPoint.position.x;
        newPos.z = startPoint.position.z;
        poids.position = newPos;

        // Ajustement de la corde
        if (corde != null)
        {
            float ropeLength = Vector3.Distance(startPoint.position, poids.position);
            Vector3 scale = corde.localScale;
            scale.z = ropeLength / 2;
            corde.localScale = scale;
        }

        // Rotation de la roue selon le temps
        if (roue != null)
        {
            float rotationSpeed = 50f; // degrés par seconde, ajuste à ton goût
            roue.Rotate(Vector3.left, rotationSpeed * Time.deltaTime, Space.Self);
        }

        // Timer terminé
        if (timer >= duration)
        {
            active = false;
            if (repairSystem != null)
                repairSystem.ActivateMechanism();
        }
    }

    public void ResetFicelle()
    {
        timer = 0f;
        active = true;
        if (poids != null)
            poids.position = startPoint.position;
        if (corde != null)
            corde.position = startPoint.position;
        if (systemToActivate != null)
            systemToActivate.SetActive(false);
    }
}
