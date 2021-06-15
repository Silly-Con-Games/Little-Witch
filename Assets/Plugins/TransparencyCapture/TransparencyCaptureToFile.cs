using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TransparencyCaptureToFile:MonoBehaviour
{
    public string fn = "capture";

    public IEnumerator capture()
    {

        yield return new WaitForEndOfFrame();
        //After Unity4,you have to do this function after WaitForEndOfFrame in Coroutine
        //Or you will get the error:"ReadPixels was called to read pixels from system frame buffer, while not inside drawing frame"
        zzTransparencyCapture.captureScreenshot("Assets/Plugins/TransparencyCapture/" + fn + ".png");
    }

    public void Update()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame)
            StartCoroutine(capture());
    }
}