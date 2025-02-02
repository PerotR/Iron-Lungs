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

    private float clockSound = 0.5f;  // Volume du son de l'horloge

    private float soundTimer = 1f;  // Temps entre chaque son (1 seconde)
    private float soundCooldown = 1f;  // Compteur pour contrôler le son


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
            timerText.text = "Temps restant : " + Mathf.Ceil(timer).ToString();

            // Jouer le son une fois par seconde
            soundCooldown -= Time.deltaTime;
            if (soundCooldown <= 0f)
            {
                timerAudioSource.PlayOneShot(timerSound, clockSound);
                clockSound+=0.5f;  // Augmente le volume du son à chaque itération
                soundCooldown = soundTimer;  // Réinitialise le compteur
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
