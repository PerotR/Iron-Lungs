using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // Nécessaire pour manipuler l'UI Text

public class SceneResetTimer : MonoBehaviour
{
    public float timer = 10f;  // Temps initial avant le reset (en secondes)
    public bool isTimerActive = true;  // Si le timer est actif

    public Text timerText;  // Référence au composant Text pour afficher le timer

    void Start()
    {
        // Vérifie si le Text est bien assigné, sinon log une erreur
        if (timerText == null)
        {
            Debug.LogError("TimerText n'est pas assigné dans l'Inspector");
        }
    }

    void Update()
    {
        if (isTimerActive)
        {
            // Décrémenter le timer
            timer -= Time.deltaTime;

            // Afficher le timer dans le UI Text
            timerText.text = "Temps restant : " + Mathf.Ceil(timer).ToString();  // Affiche le timer avec un arrondi

            // Si le timer atteint 0, reset la scène
            if (timer <= 0)
            {
                // Réinitialiser la scène
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
