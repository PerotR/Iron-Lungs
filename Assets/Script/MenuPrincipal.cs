using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    void Start()
    {
        // Cacher le curseur système
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    // Fonction pour charger la scène du jeu
    public void Jouer()
    {
        SceneManager.LoadScene("SceneVille"); // Remplace par le vrai nom de ta scène
    }

    // Fonction pour quitter le jeu
    public void Quitter()
    {
    #if UNITY_EDITOR
        // Code spécifique à l'éditeur
        UnityEditor.EditorApplication.isPlaying = false;
    #else
            // Code pour quitter le jeu en runtime
            Application.Quit();
    #endif
    }
}
