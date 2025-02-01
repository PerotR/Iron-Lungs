using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    public float speed = 5f;               // Vitesse de déplacement
    public Vector3 movementDirection;     // Direction de déplacement
    private Rigidbody rb;
    public float raycastDistance = 1f;   // Distance du raycast pour détecter les obstacles

    private void Start()
    {
        // Obtenez ou ajoutez un Rigidbody et configurez-le comme kinematic
        rb = gameObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true; // Pour éviter la gravité et contrôler le mouvement manuellement
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Pour éviter le tunneling

        // Générer une direction aléatoire pour le mouvement, uniquement dans le plan XZ
        movementDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
    }

    private void FixedUpdate()
    {
        // Vérifier s'il y a un obstacle devant avec un Raycast
        if (!Physics.Raycast(transform.position, movementDirection, raycastDistance))
        {
            // Déplacer la cible avec le Rigidbody (en utilisant la physique)
            rb.MovePosition(transform.position + movementDirection * speed * Time.deltaTime);
        }
        else
        {
            // Changer la direction si un obstacle est détecté
            movementDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        }
    }

    // Détecte les collisions avec des objets ayant un collider
    private void OnCollisionEnter(Collision collision)
    {
        // Quand la cible entre en collision avec un objet
        // Changer la direction de manière aléatoire après la collision
        movementDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;

        // Optionnel : Afficher la collision dans la console pour débogage
        Debug.Log("Collision détectée! Nouvelle direction : " + movementDirection);
    }
}