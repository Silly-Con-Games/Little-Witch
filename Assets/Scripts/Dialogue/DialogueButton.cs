using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueButton : MonoBehaviour
{
    [SerializeField]
    GameObject dialogueWindowPrefab;
    [SerializeField]
    string dialoguePath;
    [SerializeField]
    Transform where = null;
    [SerializeField]
    Sprite leftChar, rightChar;



    public void initialize(string dialoguePath, GameObject dialogueWindowPrefab, Transform where, Sprite leftChar, Sprite rightChar)
    {
        this.dialoguePath = dialoguePath;
        this.where = where;
        this.leftChar = leftChar;
        this.rightChar = rightChar;
        this.dialogueWindowPrefab = dialogueWindowPrefab;

    }



    private void Update()
    {
        
        if (where != null)
            ((RectTransform)transform).position = Camera.main.WorldToScreenPoint(where.position );
    }

    public void onClick()
    {
        var dialogue = GameObject.Instantiate(dialogueWindowPrefab, transform.root);
        dialogue.GetComponent<DialogueController>().initialize(dialoguePath, leftChar, rightChar);
        GameObject.Destroy(gameObject);
    }

}
