using System.Collections;
using UnityEngine;

public class GearRepairSystem : MonoBehaviour
{
    [Header("References")]
    public GameObject repairCanvas; // Canvas contenant la RawImage
    public GameObject mechanismCamera; // Caméra qui rend la RenderTexture

    [Header("Mechanism Parts")]
    public Transform gear;
    public Transform rope;
    public Transform weight;
    public Transform ropeTopPoint;


    [Header("Rope Settings")]
    public float baseRopeLength = 240f;
    public float maxRopeScale = 1f;
    public float minRopeScale = 0.1f;
    public float ropeRetractionPerRotation = 0.05f;
    private float currentRopeScale;

    [Header("Rotation Settings")]
    public float rotationSpeed = 1f;
    private bool isTouching = false;
    private Vector2 lastTouchPosition;
    private bool isActive = false;

    private bool waitingForMechanism = false;

    void Start()
    {
        currentRopeScale = maxRopeScale;
        //repairCanvas.SetActive(false);
        //mechanismCamera.SetActive(false);
    }

    public void WaitActivateMechanism()
    {
        waitingForMechanism = true;
    }

    public void ActivateMechanism()
    {
        GameManager.Instance.DateCanvas.SetActive(false);
        GameManager.Instance.MiniGearIcon.SetActive(false);
        GameManager.Instance.ResetPosition();

        StartCoroutine(ActivateMechanism_Delay());
    }

    IEnumerator ActivateMechanism_Delay()
    {
        yield return new WaitForSeconds(2);

        Debug.Log("Mécanisme activé !");
        isActive = true;
        currentRopeScale = maxRopeScale;

        repairCanvas.SetActive(true);
        mechanismCamera.SetActive(true);

        UpdateRope();
    }

    void Update()
    {
        //if (isActive)
          //  HandleTouchInput();
        if(waitingForMechanism)
        {
            if(!GameManager.Instance.PlanetsMoving)
            {
                waitingForMechanism = false;
                ActivateMechanism();
            }
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                isTouching = true;
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved && isTouching)
            {
                Vector2 delta = touch.position - lastTouchPosition;
                float rotation = delta.x * rotationSpeed * Time.deltaTime;

                gear.Rotate(Vector3.right, rotation, Space.Self);

                float scaleReduction = (Mathf.Abs(rotation) / 360f) * ropeRetractionPerRotation;
                currentRopeScale = Mathf.Max(minRopeScale, currentRopeScale - scaleReduction);

                UpdateRope();

                if (currentRopeScale <= minRopeScale)
                    CompleteRepair();

                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
                isTouching = false;
        }
    }

    void UpdateRope()
    {
        // Met à jour la scale (corde qui se rétracte)
        Vector3 ropeScale = rope.localScale;
        ropeScale.z = currentRopeScale;
        rope.localScale = ropeScale;

        // Calcule la vraie longueur visible de la corde
        float ropeLength = baseRopeLength * currentRopeScale;

        // Calcule la position du bas de la corde (axe Z local descendant)
        Vector3 bottomOfRope = ropeTopPoint.position - ropeTopPoint.forward * ropeLength;

        // Position du poids au bas de la corde
        weight.position = bottomOfRope;
        weight.rotation = Quaternion.identity;

        Debug.Log($"Rope scale: {currentRopeScale}, Rope length: {ropeLength}, Weight pos: {weight.position}");
    }

    void CompleteRepair()
    {
        SoundManager.Instance.Stop("gear_reload");
        SoundManager.Instance.Play("gear_reload_end");
        Debug.Log("Réparation terminée !");
        isActive = false;
        repairCanvas.SetActive(false);
        mechanismCamera.SetActive(false);
        
        
        GameManager gm = GameManager.Instance;
        gm.MiniGearIcon.SetActive(true);
        gm.GetComponent<MiniGearCountdown>().ResetFicelle();
        gm.DateCanvas.SetActive(true);
        gm.PlanetsDeployed = false;
        gm.MoveAllPlanetsToDate(gm.currentDate.ToString());
    }

    public void RotateGear(float rotation)
    {
        // Tourne la roue sur son axe
        gear.Rotate(Vector3.right, rotation, Space.Self);

        // Détecte le sens de rotation (horaire ou anti-horaire)
        float ropeChange = (rotation / 360f) * ropeRetractionPerRotation;

        // Si rotation > 0 : remonte (réduction de la longueur)
        // Si rotation < 0 : descend (augmentation de la longueur)
        currentRopeScale = Mathf.Clamp(currentRopeScale - ropeChange, minRopeScale, maxRopeScale);

        UpdateRope();

        if (currentRopeScale <= minRopeScale)
            CompleteRepair();
    }

    public bool IsFullyRepaired()
    {
        return currentRopeScale <= minRopeScale;
    }
}
