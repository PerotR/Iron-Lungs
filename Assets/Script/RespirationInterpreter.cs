using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespirationInterpreter : MonoBehaviour
{
    public RespirationDataReader dataReader;
    public int batchSize = 2000; // Nombre de points par batch (3 secondes à 1000 Hz)

    private List<int> respirationData;
    private int currentIndex = 0;
    private float timer = 0f;
    private float interval = 3f;

    private float currentOscillation = 0f;

    private float vci;
    private float vce;
    private float vri;
    private float vre;

    public RectTransform tracker; // Flèche ou indicateur visuel
    public RectTransform graphContainer; // Conteneur du graphique
    public RawImage graphImage;
    public Text feedbackText;

    void Start()
    {
        if (dataReader == null)
        {
            Debug.LogError("Veuillez assigner un RespirationDataReader dans l'inspecteur !");
            return;
        }

        StartCoroutine(WaitOneSecond());

        respirationData = dataReader.GetRespirationData();

        // Récupération des données de calibration depuis RespirationDataReader
        vci = dataReader.VCI;
        vce = dataReader.VCE;
        vri = dataReader.VRI;
        vre = dataReader.VRE;

        Debug.Log($"Données de calibration récupérées : VCI={vci}, VCE={vce}, VRI={vri}, VRE={vre}");

        if (tracker != null && graphImage != null)
        {
            // Calculer la position Y pour le bas du graphique
            float yPos = -graphImage.rectTransform.rect.height / 2f; // Placer la flèche au bas de l'image
            tracker.anchoredPosition = new Vector2(0, yPos);
        }

        if (respirationData.Count == 0)
        {
            Debug.LogError("Pas de données à interpréter !");
        }
    }

    IEnumerator WaitOneSecond()
    {
        yield return new WaitForSeconds(1f);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            if (currentIndex + batchSize <= respirationData.Count)
            {
                List<int> batch = respirationData.GetRange(currentIndex, batchSize);
                string respirationState = InterpretRespirationBatch(batch);
                //Debug.Log("État de respiration : " + respirationState);

                currentOscillation = GetCameraOscillation(respirationState);

                currentIndex += batchSize;
            }

            timer = 0f;
        }

        ApplyCameraOscillation(currentOscillation);
        UpdateTrackerPosition();
    }

    void UpdateTrackerPosition()
    {
        if (respirationData.Count == 0) return;

        // Calculer la position en pourcentage
        float progress = (float)currentIndex / respirationData.Count;

        // Obtenir les dimensions de l'image
        float graphWidth = graphImage.rectTransform.rect.width;

        // Calculer la position X du tracker
        float trackerX = progress * graphWidth;

        // Mettre à jour la position du tracker en bas
        tracker.anchoredPosition = new Vector2(trackerX, -graphImage.rectTransform.rect.height / 2f);
    }

    string InterpretRespirationBatch(List<int> batch)
    {
        // Calcul des métriques
        float batchAmplitude = CalculateAmplitude(batch);
        float referenceAmplitude = vci - vce; // Amplitude de référence normale

        float frequency = CalculateFrequency(batch);
        Debug.Log($"Amplitude du batch : {batchAmplitude}, Amplitude de référence : {referenceAmplitude}, Fréquence : {frequency}");

        // Règles de décision
        if (batchAmplitude < 100f && frequency < 0.2f)
        {
            Debug.Log("État de respiration : " + "apnee");
            TriggerApneaEffect();
            feedbackText.text = "Entree en apnee";
            return "apnee";
        }
        else if (frequency > 0.1f)
        {
            Debug.Log("État de respiration : " + "essoufflement");
            feedbackText.text = "Vous etes essouffle, ralentissez votre respiration";
            return "essoufflement";
        }
        else if (batchAmplitude > referenceAmplitude * 1.2f && frequency <= 1.2f)
        {
            Debug.Log("État de respiration : " + "effort maîtrisé");
            feedbackText.text = "Bon rythme, continuez comme ca";
            return "effort maîtrisé";
        }
        else
        {
            Debug.Log("État de respiration : " + "normal");
            feedbackText.text = "Respiration normale";
            return "normal";
        }
    }


    // Calcul de l'amplitude (différence entre pics et creux)
    float CalculateAmplitude(List<int> batch)
    {
        int min = int.MaxValue;
        int max = int.MinValue;

        foreach (int value in batch)
        {
            if (value < min) min = value;
            if (value > max) max = value;
        }

        return max - min;
    }

    // Calcul de la fréquence respiratoire (nombre de cycles par seconde)
    float CalculateFrequency(List<int> batch)
    {
        int cycles = 0;
        bool inCycle = false;
        float threshold = (vci + vri) / 2; // Seuil moyen de la plage respiratoire

        for (int i = 1; i < batch.Count; i++)
        {
            if (!inCycle && batch[i-1] < threshold && batch[i] >= threshold)
            {
                inCycle = true;
            }
            else if (inCycle && batch[i-1] >= threshold && batch[i] < threshold)
            {
                inCycle = false;
                cycles++;
            }
        }

        // Fréquence : nombre de cycles détectés par seconde
        return cycles / 3.0f;
    }

    // Détection d'apnée (plateau prolongé, sans variation notable)
    bool IsApnea(List<int> batch)
    {
        float threshold = 0.1f; // Seuil de variation
        for (int i = 1; i < batch.Count; i++)
        {
            if (Mathf.Abs(batch[i] - batch[i-1]) > threshold)
                return false;
        }
        return true;
    }

    float GetCameraOscillation(string respirationState)
    {
        switch (respirationState)
        {
            case "apnee": return 0.1f;
            case "essoufflement": return 4f;
            case "effort maîtrisé": return 1f;
            case "normal": return 2f;
            default: return 0.0f;
        }
    }

    void ApplyCameraOscillation(float oscillation)
    {
        Vector3 currentRotation = Camera.main.transform.localEulerAngles;
        float offsetX = Mathf.Sin(Time.time * 2f) * oscillation;
        float offsetY = Mathf.Cos(Time.time * 2f) * oscillation;
        Camera.main.transform.localEulerAngles = currentRotation + new Vector3(offsetY, offsetX, 0);
    }

    void TriggerApneaEffect()
    {
        var targets = FindObjectsOfType<TargetMovement>();
        foreach (var target in targets)
        {
            target.ActivateSlowMotion();
        }
    }

}
