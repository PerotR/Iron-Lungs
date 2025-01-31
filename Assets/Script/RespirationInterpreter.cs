using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespirationInterpreter : MonoBehaviour
{
    public RespirationDataReader dataReader;
    public int batchSize = 2000; // Taille du batch pour 2 secondes (1000 Hz * 2 secondes)

    private List<int> respirationData;
    private int currentIndex = 0;
    private float timer = 0f;
    private float interval = 2f; // Intervalle de 2 secondes

    private float currentOscillation = 0f; // Oscillation actuelle

    void Start()
    {
        respirationData = dataReader.GetRespirationData();
    }

    void Update()
    {
        // Incrémenter le timer
        timer += Time.deltaTime;

        // Toutes les 2 secondes, traiter un nouveau batch de données
        if (timer >= interval)
        {
            if (currentIndex + batchSize <= respirationData.Count)
            {
                List<int> batch = respirationData.GetRange(currentIndex, batchSize);
                string respirationState = InterpretRespirationBatch(batch);
                Debug.Log("État de respiration : " + respirationState);

                // Définir l'oscillation en fonction de l'état de respiration
                currentOscillation = GetCameraOscillation(respirationState);

                currentIndex += batchSize;
            }

            // Réinitialiser le timer
            timer = 0f;
        }

        // Appliquer l'oscillation à la rotation de la caméra
        ApplyCameraOscillation(currentOscillation);
    }

    string InterpretRespirationBatch(List<int> batch)
    {
        float moyenne = CalculateMean(batch);
        float ecartType = CalculateStandardDeviation(batch, moyenne);

        if (ecartType < 1) // Apnée
        {
            return "apnee";
        }
        else if (ecartType > 5) // Essoufflement
        {
            return "essoufflement";
        }
        else // Respiration normale
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
            case "apnee":
                return 0.1f; // Très faible oscillation
            case "essoufflement":
                return 2.0f; // Forte oscillation
            case "normal":
                return 1.0f; // Oscillation modérée
            default:
                return 0.0f; // Pas d'oscillation
        }
    }

    void ApplyCameraOscillation(float oscillation)
    {
        // Récupérer la rotation actuelle de la caméra
        Vector3 currentRotation = Camera.main.transform.localEulerAngles;

        // Calculer le décalage oscillant pour la rotation
        float offsetX = Mathf.Sin(Time.time * 2f) * oscillation; // Oscillation horizontale (rotation Y)
        float offsetY = Mathf.Cos(Time.time * 2f) * oscillation; // Oscillation verticale (rotation X)

        // Ajouter le décalage à la rotation actuelle de la caméra
        Camera.main.transform.localEulerAngles = currentRotation + new Vector3(offsetY, offsetX, 0);
    }
}