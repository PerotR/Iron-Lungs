using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    public float speed = 2f;               // Vitesse de déplacement
    public Vector3 movementDirection;     // Direction de déplacement
    public float movementRange = 5f;      // Distance maximale de déplacement depuis le point d'apparition

    private Vector3 startingPosition;     // Position de départ pour limiter le mouvement

    private void Start()
    {
        // Enregistrer la position de départ
        startingPosition = transform.position;

        // Générer une direction aléatoire pour le mouvement, uniquement dans le plan XZ
        movementDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
    }

    private void Update()
    {
        // Déplacer la cible
        transform.position += movementDirection * speed * Time.deltaTime;

        // Vérifier si la cible dépasse les limites de déplacement
        if (Vector3.Distance(startingPosition, transform.position) > movementRange)
        {
            // Inverser la direction pour rester dans les limites
            movementDirection = -movementDirection;
        }
    }
}
