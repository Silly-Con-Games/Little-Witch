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
    GameObject player;

    private void OnCollisionEnter(Collision collision)
    {
        throw  new NotImplementedException("need to convey that the collision is with the player");
        //if(collision.collider == player)
        {
            // this.player = player  

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
