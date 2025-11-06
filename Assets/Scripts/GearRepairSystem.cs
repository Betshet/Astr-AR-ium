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
    public float maxRopeScale = 1f;
    public float minRopeScale = 0.1f;
    public float ropeRetractionPerRotation = 0.5f;
    private float currentRopeScale;

    [Header("Rotation Settings")]
    public float rotationSpeed = 1f;
    private bool isTouching = false;
    private Vector2 lastTouchPosition;
    private bool isActive = false;

    void Start()
    {
        currentRopeScale = maxRopeScale;
        //repairCanvas.SetActive(false);
        //mechanismCamera.SetActive(false);
    }

    public void ActivateMechanism()
    {
        Debug.Log("Mécanisme activé !");
        isActive = true;
        currentRopeScale = maxRopeScale;

        repairCanvas.SetActive(true);
        mechanismCamera.SetActive(true);

        UpdateRope();
    }

    void Update()
    {
        if (isActive)
            HandleTouchInput();
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

    /*void UpdateRope()
    {
        Vector3 ropeScale = rope.localScale;
        ropeScale.z = currentRopeScale;
        rope.localScale = ropeScale;

        float actualRopeLength = currentRopeScale;

        Vector3 weightPosition = ropeTopPoint.position - ropeTopPoint.up * actualRopeLength;

        weight.position = weightPosition;

        rope.position = ropeTopPoint.position - ropeTopPoint.up * (actualRopeLength / 2f);
        Debug.Log($"Rope Position: {rope.position}");
    }*/

    void UpdateRope()
    {
        Vector3 ropeScale = rope.localScale;
        ropeScale.z = currentRopeScale;
        rope.localScale = ropeScale;

        float actualRopeLength = currentRopeScale;
        Vector3 weightPosition = weight.position + ropeTopPoint.forward * ropeRetractionPerRotation;

        weight.position = weightPosition;
        //float weightZ = weight.position.z;
        //weightZ -= 0.1f;
        //weight.position = new Vector3(94, 94, weightZ);
        //float actualRopeLength = currentRopeScale;
        //Vector3 weightPosition = weight.position - ropeTopPoint.up * actualRopeLength;

        //weight.position = weightPosition;
    }

    void CompleteRepair()
    {
        Debug.Log("Réparation terminée !");
        isActive = false;
        repairCanvas.SetActive(false);
        mechanismCamera.SetActive(false);
    }

    public void RotateGear(float rotation)
    {
        gear.Rotate(Vector3.right, rotation, Space.Self);

        float scaleReduction = (Mathf.Abs(rotation) / 360f) * ropeRetractionPerRotation;
        currentRopeScale = Mathf.Max(minRopeScale, currentRopeScale - scaleReduction);
        UpdateRope();
        Debug.Log("currentRopeScale: " + currentRopeScale);

        if (currentRopeScale <= minRopeScale)
            CompleteRepair();
    }
}
