using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    // right character object has -1 on x scale in order to make it face the other one without changing sprites
    [SerializeField]
    GameObject leftCharacterImage, rightCharacterImage, textObject;
    [SerializeField]
    string dialoguePath;
    [SerializeField]
    Sprite leftCharacterPicture, rightCharacterPicture;
    [SerializeField]
    char dialogueLineSeparator = '@';


    Image left, right;
    TMPro.TextMeshPro text;

    int pageNumber = 0;

    // Start is called before the first frame update
    void Start()
    {
        left = leftCharacterImage.GetComponent<Image>();
        right = rightCharacterImage.GetComponent<Image>();
        text = textObject.GetComponent<TMPro.TextMeshPro>();
    }

    bool wasPressed = false;
    private void Update()
    {
        if (Input.GetAxis("ContinueDialogue") != 0)
        {
            if (!wasPressed)
            {
                wasPressed = true;
                nextPage();
            }
        }
        else
            wasPressed = false;
    }

    private void OnDestroy()
    {
        //resume the game from pause
        Time.timeScale = 1.0f;
    }

    public void initialize(string dialoguePath, Sprite leftCharacterPicture, Sprite rightCharacterPicture, int pageNumber = 0)
    {
        this.dialoguePath = dialoguePath;
        this.leftCharacterPicture = leftCharacterPicture;
        this.rightCharacterPicture = rightCharacterPicture;
        this.pageNumber = pageNumber;

        // pause the game
        Time.timeScale = 0;

        draw();
    }
    private bool draw()
    {
        left.sprite = leftCharacterPicture;
        right.sprite = rightCharacterPicture;

        text.text = getPage(pageNumber);
        // end of the dialogue
        if (text.text == "")
            return false;
        return true;
    }
    void nextPage()
    {
        ++pageNumber;
        if (!draw())
            GameObject.DestroyImmediate(gameObject, true);
    }

    string getPage(int pageNumber)
    {
        string outString = "";
        using (StreamReader sr = new StreamReader(dialoguePath))
        {
            int remainingPages = pageNumber;
            // scroll to the next
            while(remainingPages > 0)
            {
                if (nextChar(sr) == dialogueLineSeparator)
                    --remainingPages;
            }

            char character = nextChar(sr);
            StringBuilder sb = new StringBuilder();
            while(character != dialogueLineSeparator)
            {
                if (character != '\n')
                    sb.Append(character);
                else
                    sb.Append(System.Environment.NewLine);
            }
            outString = sb.ToString();
        }

        return outString;
    }
    string currentLine = null;
    int i=0;
    
    char nextChar(StreamReader sr)
    {
        
        if(currentLine == null || i>= currentLine.Length)
        {
            if (sr.EndOfStream)
                return (char)0;
            
            currentLine = sr.ReadLine();
            i = 0;

            if(i>= currentLine.Length)
                return '\n';
        }
        ++i;
        return currentLine[i - 1];

    }

}
