using UnityEngine;

public class FixLighting : MonoBehaviour
{
    void Start()
    {
        DynamicGI.UpdateEnvironment(); // Met à jour l'éclairage global
        RenderSettings.ambientLight = Color.white; // Ajoute de la lumière ambiante si besoin
    }
}
