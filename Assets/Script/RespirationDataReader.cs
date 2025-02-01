using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class RespirationDataReader : MonoBehaviour
{
    public string filePathTo = "Assets/RespirationData/calibration_aans_respiration_longue.txt";
    private List<int> respirationData = new List<int>();

    public RawImage graphImage;
    private Texture2D texture;
    private Color graphColor = Color.green;

    public int textureWidth = 500;
    public int textureHeight = 300;

    void Start()
    {
        LoadRespirationData();

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
                if (!line.StartsWith("#"))
                {
                    string[] values = line.Split(new char[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length >= 6)
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
