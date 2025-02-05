using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class OpenSignalsReceiver : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private byte[] data = new byte[1024]; // Buffer pour les données reçues

    public string serverIP = "127.0.0.1"; // Adresse IP du serveur (OpenSignals)
    public int port = 5555; // Port utilisé par OpenSignals

    void Start()
    {
        ConnectToServer();
        SendCommand("devices"); // Récupérer la liste des appareils
    }

    void ConnectToServer()
    {
        try
        {
            client = new TcpClient(serverIP, port);
            stream = client.GetStream();
            Debug.Log("Connected to OpenSignals server");
        }
        catch (Exception e)
        {
            Debug.LogError("Error connecting to server: " + e.Message);
        }
    }

    void Update()
    {
        if (stream != null && stream.DataAvailable)
        {
            // Lire les données reçues
            int bytesRead = stream.Read(data, 0, data.Length);
            string receivedData = Encoding.UTF8.GetString(data, 0, bytesRead);
            Debug.Log("Received: " + receivedData);

            // Traiter les données
            ProcessServerResponse(receivedData);
        }
    }

    void SendCommand(string command)
    {
        if (stream != null)
        {
            byte[] commandBytes = Encoding.UTF8.GetBytes(command);
            stream.Write(commandBytes, 0, commandBytes.Length);
            Debug.Log("Sent command: " + command);
        }
        else
        {
            Debug.LogError("Stream is not available.");
        }
    }

    void ProcessServerResponse(string response)
    {
        try
        {
            Debug.Log("Processing response: " + response);

            // Vérifiez si la réponse contient des données
            if (response.Contains("returnCode"))
            {
                if (response.Contains("\"returnCode\":0"))
                {
                    Debug.Log("Command successful: " + response);

                    // Si la commande "devices" réussit, activez l'appareil
                    if (response.Contains("00:21:06:BE:15:4F"))
                    {
                        SendCommand("enable, 00:21:06:BE:15:4F"); // Activer l'appareil
                        SendCommand("start"); // Démarrer l'acquisition
                    }
                }
                else if (response.Contains("\"returnCode\":2"))
                {
                    Debug.LogError("Unknown device. Check the MAC address.");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error processing server response: " + e.Message);
        }
    }

    void OnDestroy()
    {
        if (stream != null)
            stream.Close();
        if (client != null)
            client.Close();
    }
}