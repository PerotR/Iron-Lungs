using UnityEngine;
using System.Collections.Generic;

public class TargetSpawner : MonoBehaviour
{
    public GameObject targetPrefab;   // Prefab pour les cibles (type "Target")
    public GameObject civilianPrefab; // Prefab pour les civils (type "Civilian")
    public float spawnHeight = 1.5f;  // Hauteur où les cibles apparaissent
    public int numberOfEntities = 150; // Nombre total d'entités à générer
    public int numberOfTargets = 5;  // Nombre de cibles à générer

    private List<Transform> arenaTransforms = new List<Transform>();
    private float entityRadius;       // Rayon des entités (taille pour éviter les débordements)

    private void Start()
    {
        // Trouver toutes les arènes avec le tag "Arene"
        FindArenas();
        
        // Calculer la taille des entités
        CalculateEntityRadius();

        // Générer les entités
        SpawnEntities();
    }

    void FindArenas()
    {
        GameObject[] arenaObjects = GameObject.FindGameObjectsWithTag("Arena");
        foreach (GameObject arena in arenaObjects)
        {
            arenaTransforms.Add(arena.transform);
        }
    }

    void SpawnEntities()
    {
        if (arenaTransforms.Count == 0)
        {
            Debug.LogError("Aucune arène trouvée avec le tag 'Arene'!");
            return;
        }

        // Calculer le nombre de civils à spawn
        int numberOfCivilians = numberOfEntities - numberOfTargets;

        for (int i = 0; i < numberOfEntities; i++)
        {
            // Sélectionner une arène au hasard
            Transform selectedArena = arenaTransforms[Random.Range(0, arenaTransforms.Count)];

            // Calculer les dimensions de l'arène
            float radiusX, radiusZ;
            CalculateArenaDimensions(selectedArena, out radiusX, out radiusZ);

            // Déterminer si on spawn une cible ou un civil
            GameObject prefabToSpawn = (i < numberOfTargets) ? targetPrefab : civilianPrefab;

            // Obtenir une position aléatoire dans l'ellipse de l'arène sélectionnée
            Vector3 entityPosition = GetRandomPositionInEllipse(selectedArena.position, radiusX, radiusZ, spawnHeight);

            // Utiliser un Raycast pour s'assurer que l'entité spawn sur la surface de l'arène
            RaycastHit hit;
            if (Physics.Raycast(entityPosition + Vector3.up * 10f, Vector3.down, out hit, Mathf.Infinity))
            {
                // Assurez-vous que le Raycast touche bien une surface de l'arène
                if (hit.collider.CompareTag("Arena"))
                {
                    // Créer l'entité sur la surface de l'arène
                    GameObject entity = Instantiate(prefabToSpawn, hit.point, Quaternion.identity);

                    // Assigner le tag en fonction du type d'entité
                    entity.tag = prefabToSpawn == civilianPrefab ? "Civilian" : "Target";
                }
                else
                {
                    // Si le raycast n'a pas touché une arène, recommencer avec ce spawn
                    i--;
                }
            }
            else
            {
                // Si le raycast échoue, recommencer avec ce spawn
                i--;
            }
        }
    }

    void CalculateArenaDimensions(Transform arenaTransform, out float radiusX, out float radiusZ)
    {
        Collider arenaCollider = arenaTransform.GetComponent<Collider>();
        if (arenaCollider == null)
        {
            Debug.LogError($"L'arène {arenaTransform.name} est manquante d'un composant Collider!");
            radiusX = radiusZ = 0f;
            return;
        }

        Bounds bounds = arenaCollider.bounds;
        radiusX = bounds.size.x / 2f - entityRadius;
        radiusZ = bounds.size.z / 2f - entityRadius;
    }

    void CalculateEntityRadius()
    {
        Collider entityCollider = targetPrefab.GetComponent<Collider>();
        if (entityCollider == null)
        {
            Debug.LogError("Le prefab Target est manquant d'un composant Collider!");
            entityRadius = 0f;
            return;
        }

        Bounds bounds = entityCollider.bounds;
        entityRadius = Mathf.Max(bounds.size.x, bounds.size.z) / 2f;
    }

    Vector3 GetRandomPositionInEllipse(Vector3 arenaCenter, float radiusX, float radiusZ, float height)
    {
        float angle = Random.Range(0f, 2f * Mathf.PI);
        float distance = Mathf.Sqrt(Random.Range(0f, 1f));
        float x = distance * radiusX * Mathf.Cos(angle);
        float z = distance * radiusZ * Mathf.Sin(angle);
        return new Vector3(arenaCenter.x + x, height, arenaCenter.z + z);
    }
}
