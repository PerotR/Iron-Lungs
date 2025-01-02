using UnityEngine;

public class GenerateBarriers : MonoBehaviour
{
    public GameObject barrierPrefab;  // Le prefab de la barrière
    public float lineLength = 225f;   // La longueur de la ligne de barrières (250 -> 25 ou -25 -> -250)
    public float squareSide = 25f;    // Le côté du carré
    public int numBarriers = 20;     // Le nombre de barrières pour créer l'arc
    public int height = 5;           // Hauteur des barrières
    public float barrierWidth = 1f;  // Largeur de chaque barrière

    void Start()
    {
        // Vérifiez que le prefab est assigné
        if (barrierPrefab == null)
        {
            Debug.LogError("Le prefab de barrière n'est pas assigné !");
            return;
        }

        // Créer les barrières de la première ligne de barrières
        CreateLineOfBarriers(250f, 0.5f);  // Modification ici : de (250, 0, 0) à (0.5, 0, 0)

        // Créer les côtés du carré (demi-carré autour de la tour)
        CreateSquareBarriers();

        // Créer les barrières de la deuxième ligne de barrières
        CreateLineOfBarriers(-0.5f, -250f);  // Modification ici : de (-0.5, 0, 0) à (-250, 0, 0)
    }

    void CreateLineOfBarriers(float startX, float endX)
    {
        // Créer des barrières le long de l'axe X
        for (float x = startX; x >= endX; x -= barrierWidth) // Espacement égal à la largeur des barrières
        {
            CreateBarrier(new Vector3(x, height / 2, 0), Quaternion.identity);
        }
    }

    void CreateSquareBarriers()
    {
        // Côté supérieur du carré (aligné avec l'axe X)
        for (float x = -squareSide; x <= squareSide; x += barrierWidth)
        {
            // Les barrières doivent être placées sur l'axe Z
            CreateBarrier(new Vector3(x, height / 2, squareSide), Quaternion.Euler(0, 0f, 0)); // Rotation de 0 degré pour aligner la barrière horizontalement
        }

        // Côté droit du carré (aligné avec l'axe Z), il sera deux fois plus court
        for (float z = 0f; z <= squareSide; z += barrierWidth) // Restriction à la zone positive sur l'axe Z
        {
            // Les barrières doivent être placées sur l'axe X
            CreateBarrier(new Vector3(squareSide, height / 2, z), Quaternion.Euler(0, 90f, 0)); // Rotation de 90 degrés pour aligner la barrière verticalement
        }

        // Côté gauche du carré (aligné avec l'axe Z), il sera deux fois plus court
        for (float z = 0f; z <= squareSide; z += barrierWidth) // Restriction à la zone positive sur l'axe Z
        {
            // Les barrières doivent être placées sur l'axe X
            CreateBarrier(new Vector3(-squareSide, height / 2, z), Quaternion.Euler(0, 90f, 0)); // Rotation de 90 degrés pour aligner la barrière verticalement
        }
    }

    void CreateBarrier(Vector3 position, Quaternion rotation)
    {
        // Créer une nouvelle barrière à la position spécifiée avec la rotation demandée
        Instantiate(barrierPrefab, position, rotation);
    }
}
