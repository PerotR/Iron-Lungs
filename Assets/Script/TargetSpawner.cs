using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public GameObject targetPrefab;   // Prefab pour les cibles (type "Target")
    public GameObject civilianPrefab; // Prefab pour les civils (type "Civilian")
    public Transform arenaTransform; // Transform de l'arène
    public float spawnHeight = 1.5f; // Hauteur où les cibles apparaissent
    public int numberOfEntities = 100; // Nombre total d'entités à générer

    private float radiusX;           // Rayon dynamique de l'arène sur l'axe X
    private float radiusZ;           // Rayon dynamique de l'arène sur l'axe Z
    private float entityRadius;      // Rayon des entités (taille pour éviter les débordements)

    private void Start()
    {
        // Calculer les dimensions dynamiques
        CalculateArenaDimensions(out radiusX, out radiusZ);

        // Calculer la taille des entités
        CalculateEntityRadius();

        // Réduire les rayons de l'arène pour prendre en compte la taille des entités
        radiusX -= entityRadius;
        radiusZ -= entityRadius;

        // Générer les entités
        SpawnEntities();
    }

    void SpawnEntities()
    {
        // Centre de l'arène
        Vector3 arenaCenter = arenaTransform.position;

        for (int i = 0; i < numberOfEntities; i++)
        {
            // Déterminer si on spawn une cible ou un civil (90% Civilian, 10% Target)
            GameObject prefabToSpawn = Random.value < 0.9f ? civilianPrefab : targetPrefab;

            // Obtenir une position aléatoire dans l'ellipse
            Vector3 entityPosition = GetRandomPositionInEllipse(arenaCenter, radiusX, radiusZ, spawnHeight);

            // Créer l'entité
            GameObject entity = Instantiate(prefabToSpawn, entityPosition, Quaternion.identity);

            // Assigner le tag en fonction du type d'entité
            entity.tag = prefabToSpawn == civilianPrefab ? "Civilian" : "Target";
        }
    }

    void CalculateArenaDimensions(out float radiusX, out float radiusZ)
    {
        // Obtenir le collider de l'arène
        Collider arenaCollider = arenaTransform.GetComponent<Collider>();
        if (arenaCollider == null)
        {
            Debug.LogError("Arena object is missing a Collider component!");
            radiusX = radiusZ = 0f;
            return;
        }

        // Obtenir les dimensions du collider (Bounds)
        Bounds bounds = arenaCollider.bounds;

        // Calculer les rayons sur X et Z
        radiusX = bounds.size.x / 2f;
        radiusZ = bounds.size.z / 2f;
    }

    void CalculateEntityRadius()
    {
        // Obtenir le collider de l'une des entités (targetPrefab ou civilianPrefab)
        Collider entityCollider = targetPrefab.GetComponent<Collider>();
        if (entityCollider == null)
        {
            Debug.LogError("Target prefab is missing a Collider component!");
            entityRadius = 0f;
            return;
        }

        // Calculer le rayon approximatif de l'entité (plus grande dimension)
        Bounds bounds = entityCollider.bounds;
        entityRadius = Mathf.Max(bounds.size.x, bounds.size.z) / 2f;
    }

    Vector3 GetRandomPositionInEllipse(Vector3 arenaCenter, float radiusX, float radiusZ, float height)
    {
        // Générer un angle aléatoire
        float angle = Random.Range(0f, 2f * Mathf.PI);

        // Générer une distance aléatoire (distribution uniforme)
        float distance = Mathf.Sqrt(Random.Range(0f, 1f));

        // Calculer les coordonnées dans l'ellipse
        float x = distance * radiusX * Mathf.Cos(angle);
        float z = distance * radiusZ * Mathf.Sin(angle);

        // Retourner la position finale
        return new Vector3(arenaCenter.x + x, height, arenaCenter.z + z);
    }
}
