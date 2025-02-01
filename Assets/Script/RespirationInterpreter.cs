using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespirationInterpreter : MonoBehaviour
{
    public RespirationDataReader dataReader;
    public int batchSize = 2000; 

    private List<int> respirationData;
    private int currentIndex = 0;
    private float timer = 0f;
    private float interval = 2f;

    private float currentOscillation = 0f;

    public RectTransform tracker; // Flèche ou indicateur visuel
    public RectTransform graphContainer; // Conteneur du graphique
    public RawImage graphImage;

    void Start()
    {
        respirationData = dataReader.GetRespirationData();

        if (tracker != null && graphImage != null)
        {
            // Calculer la position Y pour le bas du graphique
            float yPos = -graphImage.rectTransform.rect.height / 2f; // Placer la flèche au bas de l'image

            // Positionner la flèche à l'extrémité gauche (0) et en bas de l'image
            tracker.anchoredPosition = new Vector2(0, yPos);

            // Optionnel : Vérification dans la console pour s'assurer que la position est correcte
            Debug.Log("Position initiale du tracker : " + tracker.anchoredPosition);
        }

        if (respirationData.Count == 0)
        {
            Debug.LogError("Pas de données à interpréter !");
        }
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
                Debug.Log("État de respiration : " + respirationState);

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
        float moyenne = CalculateMean(batch);
        float ecartType = CalculateStandardDeviation(batch, moyenne);

        if (ecartType < 1)
        {
            return "apnee";
        }
        else if (ecartType > 5)
        {
            return "essoufflement";
        }
        else
        {
            return "normal";
        }
    }

    float CalculateMean(List<int> data)
    {
        float sum = 0;
        foreach (int value in data)
        {
            sum += value;
        }
        return sum / data.Count;
    }

    float CalculateStandardDeviation(List<int> data, float mean)
    {
        float sum = 0;
        foreach (int value in data)
        {
            sum += Mathf.Pow(value - mean, 2);
        }
        return Mathf.Sqrt(sum / data.Count);
    }

    float GetCameraOscillation(string respirationState)
    {
        switch (respirationState)
        {
            case "apnee": return 0.1f;
            case "essoufflement": return 2.0f;
            case "normal": return 1.0f;
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
}
