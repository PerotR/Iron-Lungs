using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RespirationDataReader : MonoBehaviour
{
    //public string filePath = "../RespirationData/calibration_aans_respiration_longue.txt"; // Chemin vers le fichier de données
    public string filePathTo = "Assets/RespirationData/calibration_aans_respiration_longue.txt"; // Chemin vers le fichier de données
    //Debug.Log("filePath : "+filePathTo);
    private List<int> respirationData = new List<int>();

    void Start()
    {
        LoadRespirationData();
    }

    void LoadRespirationData()
    {
        if (File.Exists(filePathTo))
        {
            string[] lines = File.ReadAllLines(filePathTo);
            foreach (string line in lines)
            {
                if (!line.StartsWith("#")) // Ignorer les lignes de commentaires
                {
                    string[] values = line.Split(new char[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length >= 6) // La colonne A1 est la 6ème colonne (index 5)
                    {
                        if (int.TryParse(values[5], out int respirationValue))
                        {
                            respirationData.Add(respirationValue);
                        }
                    }
                }
            }
            Debug.Log("Données de respiration chargées : " + respirationData.Count + " points.");
        }
        else
        {
            Debug.LogError("Fichier de données introuvable : " + filePathTo);
        }
    }

    public List<int> GetRespirationData()
    {
        return respirationData;
    }
}