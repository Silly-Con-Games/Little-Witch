using UnityEngine;
using System.IO;
using System.Net;

public class SimpleHttpClient
{
    static readonly string upload_uri = "http://127.0.0.1:5000/analytics";
    static readonly string upload_uri2 = "https://Rinter7.eu.pythonanywhere.com/analytics";

    public static void UploadFileBlocking(string fullPath, bool deleteAfterSend = false)
    {
        var url = upload_uri2;
        Debug.Log($"Sending {fullPath} to {url}");
       
        try
        {
            WebClient webClient = new WebClient();
            byte[] response = webClient.UploadFile(url, "POST", fullPath);
            Debug.Log("Upload completed!");
            if (deleteAfterSend)
            {
                File.Delete(fullPath);
                Debug.Log("deleting file");
            }            
        }
        catch (WebException e)
        {
            Debug.Log("error uploading file");
        }        
    }
}
