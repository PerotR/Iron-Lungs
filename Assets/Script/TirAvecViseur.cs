using UnityEngine;
using UnityEngine.SceneManagement;

public class TirAvecViseur : MonoBehaviour
{
    public float distanceRaycast = 100f; 
    private Camera cameraPrincipale;     
    private int score = 0;               
    private int civilianHits = 0;        
    public int maxCivilianHits = 3;      

    public float size = 5f; 

    private float fovNormal = 60f;  // FOV normal de la caméra
    private float fovZoom = 30f;    // FOV réduit pour le zoom
    private bool isZoomed = false;  // Indique si le zoom est activé

    public AudioSource audioSource;
    public AudioClip targetSound;
    public AudioClip civilianSound;

    public int totalTargets = 10;  // Nombre total de cibles à toucher
    private int remainingTargets;  // Nombre de cibles restantes

    private bool gameOver = false; // Indique si la partie est terminée

    private float endGameDelay = 2f;  // Délai avant de retourner au menu
    private float endGameTimer = 0f; // Timer pour le délai

    private void Start()
    {
        cameraPrincipale = Camera.main;
        if (cameraPrincipale == null)
        {
            Debug.LogError("Aucune caméra principale trouvée ! Assignez-en une dans votre scène.");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        remainingTargets = totalTargets;  // Initialiser le nombre de cibles restantes
    }

    private void Update()
    {
        // Gestion du zoom avec le clic droit
        if (Input.GetMouseButtonDown(1) && !gameOver)
        {
            isZoomed = !isZoomed; // Alterne entre zoomé et non zoomé
            cameraPrincipale.fieldOfView = isZoomed ? fovZoom : fovNormal;
        }

        // Tir avec le clic gauche
        if (Input.GetMouseButtonDown(0) && !gameOver)
        {
            Tirer();
        }

        // Si la partie est terminée, attendre avant de charger le menu
        if (gameOver)
        {
            endGameTimer += Time.deltaTime;
            if (endGameTimer >= endGameDelay)
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }

    private void OnGUI()
    {
        // Afficher le viseur (un point rouge au centre)
        float xMin = (Screen.width / 2) - (size / 2);
        float yMin = (Screen.height / 2) - (size / 2);
        GUI.color = Color.red;
        GUI.DrawTexture(new Rect(xMin, yMin, size, size), Texture2D.whiteTexture);

        // Afficher le score en haut de l'écran
        GUI.color = Color.white;
        GUI.Label(new Rect(10, 10, 200, 50), "Bonnes cibles: " + score, new GUIStyle()
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.green }
        });

        // Afficher le nombre de mauvaises cibles touchées
        GUI.color = Color.red;
        GUI.Label(new Rect(10, 50, 300, 50), "Mauvaises cibles " + civilianHits + "/" + maxCivilianHits, new GUIStyle()
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.red }
        });

        // Si le jeu est terminé, afficher le message
        if (gameOver)
        {
            GUI.color = Color.white;
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 50), "Toutes les cibles sont éliminées !", new GUIStyle()
            {
                fontSize = 30,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.green }
            });
        }
    }

    private void Tirer()
    {
        if (cameraPrincipale == null || gameOver) return;

        Ray ray = cameraPrincipale.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanceRaycast))
        {
            GameObject hitObject = hit.collider.transform.root.gameObject;

            if (hitObject.CompareTag("Target"))
            {
                if (targetSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(targetSound);
                }
                Destroy(hitObject);
                score++;
                remainingTargets--;  // Réduire le nombre de cibles restantes

                // Si toutes les cibles ont été touchées
                if (remainingTargets <= 0)
                {
                    gameOver = true;  // La partie est terminée
                }
            }
            else if (hitObject.CompareTag("Civilian"))
            {
                if (civilianSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(civilianSound);
                }
                Destroy(hitObject);
                civilianHits++;

                if (civilianHits >= maxCivilianHits)
                {
                    Debug.Log("Trop de civils touchés ! Retour au menu...");
                    SceneManager.LoadScene("Menu");
                }
            }
        }
    }
}
