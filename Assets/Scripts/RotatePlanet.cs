using UnityEngine;

public class RotatePlanet : MonoBehaviour
{
    public float rSpeed = 100f;

    void Update()
    {
        transform.Rotate(Vector3.up * rSpeed * Time.deltaTime);

        transform.LookAt(new Vector3(0, 0, 0));
    }
}
