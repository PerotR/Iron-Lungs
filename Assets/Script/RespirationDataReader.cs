using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class RespirationDataReader : MonoBehaviour
{
    private string filePathTo = "Assets/RespirationData/run_simu.txt";
    private List<int> respirationData = new List<int>();

    // Fichiers de calibration
    private string calibrageVCFilePath = "Assets/RespirationData/calibration_aans_respiration_longue.txt";
    private string calibrageVRFilePath = "Assets/RespirationData/calibrage_vr.txt";

    // Variables de calibration
    public float VCI=-1; // Volume courant inspiré
    public float VCE=-1; // Volume courant expiré
    public float VRI=-1; // Volume de réserve inspiratoire
    public float VRE=-1; // Volume de réserve expiratoire

    public RawImage graphImage;
    private Texture2D texture;
    private Color graphColor = Color.green;

    public int textureWidth = 500;
    public int textureHeight = 300;

    void Start()
    {
        LoadRespirationData();

        // Charger et traiter les fichiers de calibrage
        LoadCalibrationData();

        // Ajuste la taille de la texture à celle du conteneur
        RectTransform containerRect = graphImage.GetComponent<RectTransform>();
        textureWidth = Mathf.RoundToInt(containerRect.rect.width);
        textureHeight = Mathf.RoundToInt(containerRect.rect.height);

        // Définir la couleur de la courbe en bleu
        graphColor = Color.blue;

        DrawGraph();
    }

    void LoadRespirationData()
    {
        if (File.Exists(filePathTo))
        {
            string[] lines = File.ReadAllLines(filePathTo);
            foreach (string line in lines)
            {
                // Ignorer les lignes de commentaire
                if (line.StartsWith("#")) continue;

                string[] values = line.Split(new char[] { '\t', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                // Vérification pour éviter les erreurs d'index
                if (values.Length >= 6 && int.TryParse(values[5], out int respirationValue))
                {
                    respirationData.Add(respirationValue);
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

    void LoadCalibrationData()
    {
        // Charger et traiter les fichiers de calibrage VC et VR
        LoadVCData(calibrageVCFilePath);
        LoadVRData(calibrageVRFilePath);
    }

    void LoadVCData(string filePath)
    {
        List<int> vcData = new List<int>();

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                if (!line.StartsWith("#"))
                {
                    string[] values = line.Split(new char[] { '\t', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                    // Vérification pour lire la 6e colonne (index 5)
                    if (values.Length >= 6 && int.TryParse(values[5], out int vcValue))
                    {
                        vcData.Add(vcValue);
                    }
                }
            }

            if (vcData.Count > 0)
            {
                VCI = Mathf.Max(vcData.ToArray()); // Volume courant inspiré
                VCE = Mathf.Min(vcData.ToArray()); // Volume courant expiré
                Debug.Log("Calibration VC - VCI: " + VCI + ", VCE: " + VCE);
            }
        }
        else
        {
            Debug.LogError("Fichier de calibrage VC introuvable : " + filePath);
        }
    }

    void LoadVRData(string filePath)
    {
        List<int> vrData = new List<int>();

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                if (!line.StartsWith("#"))
                {
                    string[] values = line.Split(new char[] { '\t', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                    // Lecture de la 6e colonne (index 5)
                    if (values.Length >= 6 && int.TryParse(values[5], out int vrValue))
                    {
                        vrData.Add(vrValue);
                    }
                }
            }

            if (vrData.Count > 0)
            {
                VRI = Mathf.Max(vrData.ToArray()); // Volume de réserve inspiratoire
                VRE = Mathf.Min(vrData.ToArray()); // Volume de réserve expiratoire
                Debug.Log("Calibration VR - VRI: " + VRI + ", VRE: " + VRE);
            }
        }
        else
        {
            Debug.LogError("Fichier de calibrage VR introuvable : " + filePath);
        }
    }


    void DrawGraph()
    {
        if (respirationData.Count == 0 || graphImage == null)
        {
            Debug.LogError("Données manquantes ou composant UI non assigné.");
            return;
        }

        // Initialiser la texture
        texture = new Texture2D(textureWidth, textureHeight);
        graphImage.texture = texture;

        // Effacer la texture (fond transparent)
        ClearTexture();

        float xStep = (float)textureWidth / (respirationData.Count - 1);
        float yMin = Mathf.Min(respirationData.ToArray());
        float yMax = Mathf.Max(respirationData.ToArray());

        Vector2 lastPoint = Vector2.zero;

        for (int i = 0; i < respirationData.Count; i++)
        {
            float xPos = i * xStep;
            float normalizedY = (respirationData[i] - yMin) / (yMax - yMin);
            float yPos = normalizedY * textureHeight;

            Vector2 newPoint = new Vector2(xPos, yPos);

            if (i > 0)
            {
                DrawLine(lastPoint, newPoint, graphColor); // Courbe bleue
            }

            lastPoint = newPoint;
        }

        // Appliquer la texture après modification
        texture.Apply();
    }

    void ClearTexture()
    {
        Color clearColor = Color.clear; // Fond transparent
        for (int x = 0; x < textureWidth; x++)
        {
            for (int y = 0; y < textureHeight; y++)
            {
                texture.SetPixel(x, y, clearColor);
            }
        }
    }

    void DrawLine(Vector2 pointA, Vector2 pointB, Color color)
    {
        int x0 = Mathf.RoundToInt(pointA.x);
        int y0 = Mathf.RoundToInt(pointA.y);
        int x1 = Mathf.RoundToInt(pointB.x);
        int y1 = Mathf.RoundToInt(pointB.y);

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = (x0 < x1) ? 1 : -1;
        int sy = (y0 < y1) ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            texture.SetPixel(x0, y0, color);

            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
}
