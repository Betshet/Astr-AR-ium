using UnityEngine;

public class RotationGenerique : MonoBehaviour
{
    [Header("Paramètres de rotation")]
    public float vitesse = 50f;

    [Tooltip("Choisis l'axe de rotation dans l'inspecteur")]
    public Vector3 axe = Vector3.up;

    [Tooltip("Rotation dans l'espace local ou global")]
    public Space espace = Space.Self;

    void Update()
    {
        transform.Rotate(axe, vitesse * Time.deltaTime, espace);
    }
}