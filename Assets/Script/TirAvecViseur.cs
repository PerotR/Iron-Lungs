using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TirAvecViseur : MonoBehaviour
{
    public float distanceRaycast = 100f;
    private Camera cameraPrincipale;
    private int score = 0;
    private int civilianHits = 0;
    public int maxCivilianHits = 3;

    public float size = 5f;

    private float fovNormal = 60f; // FOV normal de la caméra
    private float fovZoom = 30f;   // FOV réduit pour le zoom
    private bool isZoomed = false; // Indique si le zoom est activé

    public AudioSource audioSource;
    public AudioClip shootingSound;
    public AudioClip targetSound;
    public AudioClip civilianSound;

    public int totalTargets = 10; // Nombre total de cibles à toucher
    private int remainingTargets; // Nombre de cibles restantes

    private bool gameOver = false; // Indique si la partie est terminée

    private float endGameDelay = 2f; // Délai avant de retourner au menu
    private float endGameTimer = 0f; // Timer pour le délai

    public Font fontcustom; // Police pour le score

    public Texture2D customViseurTexture;
    public float sizeViseur = 50f;

    // Références aux objets TextMeshPro pour afficher les informations
    public Text scoreText;
    public Text civilianHitsText;
    public Text gameOverText;

    private void Start()
    {
        cameraPrincipale = Camera.main;
        if (cameraPrincipale == null)
        {
            Debug.LogError("Aucune caméra principale trouvée ! Assignez-en une dans votre scène.");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        remainingTargets = totalTargets; // Initialiser le nombre de cibles restantes
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

        // Mettre à jour le texte à chaque frame
        scoreText.text = "Bonnes cibles: " + score + "/" + totalTargets;
        civilianHitsText.text = "Mauvaises cibles: " + civilianHits + "/" + maxCivilianHits;

        // Si le jeu est terminé, afficher le message
        if (gameOver)
        {
            gameOverText.text = "Toutes les cibles sont eliminees";
        }
    }

    private void OnGUI()
    {
        // Calculer la position du centre de l'écran
        float xMin = (Screen.width / 2) - (sizeViseur / 2);
        float yMin = (Screen.height / 2) - (sizeViseur / 2);

        // Afficher la texture du viseur
        GUI.DrawTexture(new Rect(xMin, yMin, sizeViseur, sizeViseur), customViseurTexture);
    }

    private void Tirer()
    {
        if (cameraPrincipale == null || gameOver) return;

        Ray ray = cameraPrincipale.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        audioSource.PlayOneShot(shootingSound, 0.5f);

        if (Physics.Raycast(ray, out hit, distanceRaycast))
        {
            GameObject hitObject = hit.collider.transform.root.gameObject;

            if (hitObject.CompareTag("Target"))
            {
                if (targetSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(targetSound, 0.7f);
                }
                Destroy(hitObject);
                score++;
                remainingTargets--; // Réduire le nombre de cibles restantes

                // Si toutes les cibles ont été touchées
                if (remainingTargets <= 0)
                {
                    gameOver = true; // La partie est terminée
                }
            }
            else if (hitObject.CompareTag("Civilian"))
            {
                if (civilianSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(civilianSound, 2f);
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
