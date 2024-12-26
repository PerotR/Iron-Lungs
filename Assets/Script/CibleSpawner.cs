using UnityEngine;

public class CibleSpawner : MonoBehaviour
{
    public GameObject ciblePrefab; // Référence au prefab de la cible
    public float intervalle = 2f;  // Intervalle entre chaque apparition de cible (en secondes)
    public Vector3 positionMin = new Vector3(-10f, 1f, -10f); // Position minimale
    public Vector3 positionMax = new Vector3(10f, 5f, 10f);   // Position maximale

    private void Start()
    {
        // Démarrer l'apparition des cibles avec un délai régulier
        InvokeRepeating("GenererCible", 0f, intervalle);
    }

    void GenererCible()
    {
        // Générer une position aléatoire dans les limites spécifiées
        float x = Random.Range(positionMin.x, positionMax.x);
        float y = Random.Range(positionMin.y, positionMax.y); // Hauteur aléatoire
        float z = Random.Range(positionMin.z, positionMax.z);
        Vector3 positionAleatoire = new Vector3(x, y, z);

        // Créer la cible à la position aléatoire, orientée verticalement
        Instantiate(ciblePrefab, positionAleatoire, Quaternion.Euler(90,0,0));
    }
}
