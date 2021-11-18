using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class SimpleHttpClient : MonoBehaviour
{
    // Start is called before the first frame update
    static SimpleHttpClient inst;
    static readonly string upload_uri = "http://127.0.0.1:5000/analytics";
    static readonly string upload_uri2 = "https://Rinter7.eu.pythonanywhere.com/analytics";
    private void Start()
    {
        inst = this;
    }

    private void OnDestroy()
    {
        inst = null;
    }

    public static void UploadFile(string fullPath, string fileName, string contentType)
    {
        inst?.SendFileInternal(upload_uri2, fullPath, fileName, contentType);
    }

    void SendFileInternal(string url, string fullPath, string fileName, string contentType)
    {
        StartCoroutine(PostRequestCor(url, fullPath, fileName, contentType));
    }

    static IEnumerator PostRequestCor(string url, string fullPath, string fileName, string contentType)
    {
        Debug.Log($"Sending {fileName} to {url}");

        var data = File.ReadAllBytes(fullPath);

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        //formData.Add(new MultipartFormDataSection("field1=foo&field2=bar"));
        formData.Add(new MultipartFormFileSection("file", data, fileName, contentType));

        UnityWebRequest www = UnityWebRequest.Post(url, formData);
        yield return www.SendWebRequest();

        Debug.Log($"Send finished");

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Upload complete!");
        }
    }
}
