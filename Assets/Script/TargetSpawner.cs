using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public Camera mainCamera;         // Référence à la caméra principale
    public GameObject targetPrefab;   // Le prefab de la cible (cylindre)
    public float minDistance = 10f;   // Distance minimale entre la caméra et la cible
    public float maxDistance = 50f;   // Distance maximale entre la caméra et la cible
    public float spawnHeight = 1.5f;  // Hauteur au-dessus du sol où les cibles apparaissent (environ hauteur humaine)
    public float spawnInterval = 0.5f;  // Réduit l'intervalle de temps entre chaque apparition de cible (0.5 seconde)

    // Limites de la zone où les cibles peuvent apparaître (hors du demi-carré)
    public float barrierXMin = -25f; // Limite de la barrière du côté gauche
    public float barrierXMax = 25f;  // Limite de la barrière du côté droit

    private void Start()
    {
        // Démarre la fonction pour générer les cibles de manière continue
        InvokeRepeating("SpawnTarget", 0f, spawnInterval);
    }

    void SpawnTarget()
    {
        // Générer une position aléatoire dans la vue de la caméra
        Vector3 targetPosition = GetRandomPositionInCameraView();

        // Ajuster la hauteur de la cible
        targetPosition.y = spawnHeight;

        // Vérifier si la cible est dans une position valide (Z positif et hors de la zone des barrières)
        if (IsValidTargetPosition(targetPosition))
        {
            // Créer la cible à la position générée
            Instantiate(targetPrefab, targetPosition, Quaternion.identity);
        }
    }

    Vector3 GetRandomPositionInCameraView()
    {
        // Générer des valeurs aléatoires pour la position
        float randomX = Random.Range(-2f, 2f); // X aléatoire entre -0.5 et 0.5
        float randomY = Random.Range(-0.5f, 0.5f); // Y aléatoire entre -0.5 et 0.5
        float randomZ = Random.Range(minDistance, maxDistance); // Z aléatoire entre minDistance et maxDistance

        // Convertir ces valeurs en coordonnées mondiales (dans la vue de la caméra)
        Vector3 randomPosition = mainCamera.ViewportToWorldPoint(new Vector3(randomX + 0.5f, randomY + 0.5f, randomZ));
        return randomPosition;
    }

    bool IsValidTargetPosition(Vector3 targetPosition)
    {
        // Vérifier si Z est positif
        if (targetPosition.z <= 0)
        {
            return false;
        }

        // Vérifier si la cible se trouve dans la zone des barrières
        if (targetPosition.x >= barrierXMin && targetPosition.x <= barrierXMax)
        {
            return false;
        }

        return true;
    }
}
