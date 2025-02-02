using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // Nécessaire pour manipuler l'UI Text

public class SceneResetTimer : MonoBehaviour
{
    public float timer = 10f;  // Temps initial avant le reset (en secondes)
    public bool isTimerActive = true;  // Si le timer est actif

    public Text timerText;  // Référence au composant Text pour afficher le timer
    public AudioSource timerAudioSource;  // AudioSource pour jouer le son
    public AudioClip timerSound;  // Le son à jouer dans les 5 dernières secondes
    private bool hasSoundPlayed = false;  // Pour s'assurer que le son ne soit joué qu'une fois

    void Start()
    {
        // Vérifie si le Text est bien assigné, sinon log une erreur
        if (timerText == null)
        {
            Debug.LogError("TimerText n'est pas assigné dans l'Inspector");
        }

        // Vérifie si l'AudioSource est bien assigné
        if (timerAudioSource == null || timerSound == null)
        {
            Debug.LogError("AudioSource ou AudioClip n'est pas assigné dans l'Inspector");
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

            // Vérifie si nous sommes dans les 5 dernières secondes du timer
            if (timer <= 6f && !hasSoundPlayed)
            {
                // Jouer le son
                timerAudioSource.PlayOneShot(timerSound);
                hasSoundPlayed = true;  // Empêche de rejouer le son plusieurs fois
            }

            // Si le timer atteint 0, reset la scène
            if (timer <= 0)
            {
                // Réinitialiser la scène
                SceneManager.LoadScene("Menu");
            }
        }
    }
}
