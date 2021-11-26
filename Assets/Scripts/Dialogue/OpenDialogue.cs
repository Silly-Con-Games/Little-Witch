using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDialogue : MonoBehaviour
{
    [SerializeField]
    GameObject dialogueWindowPrefab;
    [SerializeField]
    GameObject startConversationButtonPrefab;
    [SerializeField]
    string dialoguePath;
    [SerializeField]
    Sprite leftCharacterPicture, rightPicture;

    [SerializeField]
    bool openOnCollision;


    private void OnCollisionEnter(Collision collision)
    {
        
        if(collision.collider.transform.GetComponent<IObjectType>() is PlayerController)
        {
            

            if (openOnCollision)
            { 
                var dialogue = GameObject.Instantiate(dialogueWindowPrefab, transform.root);
                dialogue.GetComponent<DialogueController>().initialize(dialoguePath, leftCharacterPicture, rightPicture);
            }
            else
            {
                var button = GameObject.Instantiate(dialogueWindowPrefab);
                button.GetComponent<DialogueButton>().initialize(dialoguePath, dialogueWindowPrefab, transform , leftCharacterPicture, rightPicture);
            }
        }
    }
}
