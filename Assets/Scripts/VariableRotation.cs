using UnityEngine;

public class RotationGenerique : MonoBehaviour
{
    [Header("Parametres de rotation")]
    public float vitesse = 50f;

    // Equivalent de "up" mais modifiable dans l'inspecteur
    public Vector3 axe = Vector3.up;

    // Self = repere local, World = repere global
    public Space espace = Space.Self;

    // Pour pouvoir mettre en pause / play
    public bool rotationActive = true;

    void Update()
    {
        if (!rotationActive)
            return;

        transform.Rotate(axe, vitesse * Time.deltaTime, espace);
    }

    // Controle depuis un autre script ou un bouton UI
    public void ActiverRotation()
    {
        rotationActive = true;
    }

    public void MettreEnPauseRotation()
    {
        rotationActive = false;
    }

    public void InverserEtatRotation()
    {
        rotationActive = !rotationActive;
    }
}