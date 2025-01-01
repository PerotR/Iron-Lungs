using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sensibilitéSouris = 5f;   // Sensibilité du suivi avec la souris
    public float rotationLimiteVerticale = 60f; // Limitation de la rotation verticale

    private float rotationX = 0f; // Rotation horizontale (autour de l'axe Y)
    private float rotationY = 0f; // Rotation verticale (autour de l'axe X)

    void Start()
    {
        // Initialiser les rotations à la position actuelle de la caméra
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.y;
        rotationY = angles.x;

        // Cacher le curseur pendant le jeu (optionnel)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- Suivi de la souris ---
        float sourisDeltaX = Input.GetAxis("Mouse X") * sensibilitéSouris;
        float sourisDeltaY = Input.GetAxis("Mouse Y") * sensibilitéSouris;

        rotationX += sourisDeltaX;
        rotationY -= sourisDeltaY;

        // Limiter la rotation verticale pour éviter les inversions
        rotationY = Mathf.Clamp(rotationY, -rotationLimiteVerticale, rotationLimiteVerticale);

        // Appliquer les rotations calculées à la caméra
        transform.eulerAngles = new Vector3(rotationY, rotationX, 0);
    }
}
