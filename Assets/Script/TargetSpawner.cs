using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public Camera mainCamera;         // Référence à la caméra principale
    public GameObject targetPrefab;   // Le prefab de la cible (cylindre)
    public float minDistance = 10f;   // Distance minimale entre la caméra et la cible
    public float maxDistance = 50f;   // Distance maximale entre la caméra et la cible
    public float spawnHeight = 1.5f;  // Hauteur au-dessus du sol où les cibles apparaissent (environ hauteur humaine)
    public float spawnInterval = 2f;  // Intervalle de temps entre chaque apparition de cible

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

        // Créer la cible à la position générée
        Instantiate(targetPrefab, targetPosition, Quaternion.identity);
    }

    Vector3 GetRandomPositionInCameraView()
    {
        // Générer des valeurs aléatoires pour la position
        float randomX = Random.Range(-0.5f, 0.5f); // X aléatoire entre -0.5 et 0.5
        float randomY = Random.Range(-0.5f, 0.5f); // Y aléatoire entre -0.5 et 0.5
        float randomZ = Random.Range(minDistance, maxDistance); // Z aléatoire entre minDistance et maxDistance

        // Convertir ces valeurs en coordonnées mondiales (dans la vue de la caméra)
        Vector3 randomPosition = mainCamera.ViewportToWorldPoint(new Vector3(randomX + 0.5f, randomY + 0.5f, randomZ));
        return randomPosition;
    }
}
