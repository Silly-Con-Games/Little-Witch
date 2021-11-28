using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Threading;
using System.Net;

public class SimpleHttpClient : MonoBehaviour
{
    // Start is called before the first frame update
    static SimpleHttpClient inst;
    static readonly string upload_uri = "http://127.0.0.1:5000/analytics";
    static readonly string upload_uri2 = "https://Rinter7.eu.pythonanywhere.com/analytics";
    private void Awake()
    {
        if (inst != null)
            return;
        DontDestroyOnLoad(transform.gameObject);
        inst = this;
    }

    private void OnDestroy()
    {
        inst = null;
    }

    public static void UploadFileBlocking(string fullPath, string fileName, string contentType, bool deleteAfterSend = false)
    {
        var url = upload_uri2;
        Debug.Log($"Sending {fileName} to {url}");
       
        try
        {
            WebClient webClient = new WebClient();
            //webClient.Headers.Add("Content-Type", contentType);
            //webClient.Headers.Add(" filename", fileName);
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

    public static void UploadFileCor(string fullPath, string fileName, string contentType, bool deleteAfterSend = false)
    {
        inst.SendFileInternal(upload_uri2, fullPath, fileName, contentType, deleteAfterSend);
    }

    void SendFileInternal(string url, string fullPath, string fileName, string contentType, bool deleteAfterSend)
    {
        StartCoroutine( PostRequestCor(url, fullPath, fileName, contentType, deleteAfterSend));
    }

    static IEnumerator PostRequestCor(string url, string fullPath, string fileName, string contentType, bool deleteAfterSend)
    {
        Debug.Log($"Sending {fileName} to {url}");

        var data = File.ReadAllBytes(fullPath);

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        //formData.Add(new MultipartFormDataSection("field1=foo&field2=bar"));
        formData.Add(new MultipartFormFileSection("file", data, fileName, contentType));

        UnityWebRequest www = UnityWebRequest.Post(url, formData);
        yield return www.SendWebRequest();


        //yield return op;

        Debug.Log($"Send finished");

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (deleteAfterSend)
                File.Delete(fullPath);
            Debug.Log("Upload completed!, deleting file");
        }
    }
}
