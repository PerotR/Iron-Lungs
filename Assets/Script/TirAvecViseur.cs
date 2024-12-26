using UnityEngine;

public class TirAvecViseur : MonoBehaviour
{
    public float distanceRaycast = 100f; // Distance maximale du raycast
    private Camera cameraPrincipale;    // Caméra utilisée pour viser
    private int score = 0;              // Score du joueur

    private void Start()
    {
        // Trouver automatiquement la caméra principale
        cameraPrincipale = Camera.main;
        if (cameraPrincipale == null)
        {
            Debug.LogError("Aucune caméra principale trouvée ! Assignez-en une dans votre scène.");
        }

        // Cacher le curseur système et centrer le viseur
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnGUI()
    {
        // Afficher le viseur (un point rouge au centre)
        float size = 10f; // Taille du point
        float xMin = (Screen.width / 2) - (size / 2);
        float yMin = (Screen.height / 2) - (size / 2);
        GUI.color = Color.red;
        GUI.DrawTexture(new Rect(xMin, yMin, size, size), Texture2D.whiteTexture);

        // Afficher le score en haut de l'écran
        GUI.color = Color.white;
        GUI.Label(new Rect(10, 10, 200, 50), "Score : " + score, new GUIStyle()
        {
            fontSize = 30,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.yellow }
        });
    }

    private void Update()
    {
        // Vérifier si le joueur clique avec le bouton gauche de la souris
        if (Input.GetMouseButtonDown(0))
        {
            Tirer();
        }
    }

    private void Tirer()
    {
        if (cameraPrincipale == null) return;

        // Lancer un raycast depuis le centre de la caméra
        Ray ray = cameraPrincipale.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanceRaycast))
        {
            Debug.Log("Objet touché : " + hit.collider.name);

            // Vérifier si l'objet touché est une cible
            if (hit.collider.CompareTag("Cible"))
            {
                Destroy(hit.collider.gameObject); // Détruire la cible
                score++; // Augmenter le score de 1
                Debug.Log("Cible détruite ! Score : " + score);
            }
            else
            {
                Debug.Log("L'objet touché n'est pas une cible.");
            }
        }
        else
        {
            Debug.Log("Aucune cible touchée.");
        }
    }
}
