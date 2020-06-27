using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueSystem : Singleton<DialogueSystem>
{
    [Header("References")]

    public RectTransform dialogueRect;
    public TextMeshProUGUI nothingText;

    public GameObject playerTexts;

    [Space]

    public GameObject player;
    public GameObject nothing;
    public Camera cam;

    [Space]

    public TextMeshProUGUI dec1;
    public TextMeshProUGUI dec2;
    public TextMeshProUGUI dec3;

    public CanvasGroup text;
    public CanvasGroup choices;
    public RectTransform seperator;

    [Space]

    public AudioSource speechAudio;

    [Space]

    //We use a simple integer to point to the string to display, 
    //decision change this to specific numbers
    public int dialoguePosition = 0;

    [Header("Dialogue show options")]

    public float dialogueMovementSpeed;

    [Space]

    public float dialogueSpeed; //The speed the characters are displayed at
    public float spaceWait; //Wait for this amount when a space is encountered
    public float characterWait; //Wait for this amount when we encounter *
    public float characterSpeedSway; //Add some randomness to the character speed
    public float waitBeforeNextDialogue; //Wait this amount before showing next dialogue

    [Space]

    public AnimationCurve fadeCurve;
    public float speed;
    public Animator anim;

    [Header("Actual dialogue data")]

    public Dialogue[] dialogue;
    Dialogue currentDialogue;

    [Space]

    public string[] general;

    bool showingDialogue = false;
    bool waitingForAnswer = false;

    [System.Serializable]
    public class Dialogue
    {
        public string text;

        [Space]

        public bool hasPlayerDecision;
        public Decision decision;

        [Space]

        public int nextDialogue; //If the dialogue doesn't have a decision, continue to this, if empty just add 1 to dialoguePosition
        public bool activateNextImmediately;
    }

    [System.Serializable]
    public class Decision
    {
        public string[] texts;
        public int[] dialoguePositions;
    }


    WaitForSeconds ds; //Dialogue speed
    WaitForSeconds sw; //Space wait
    WaitForSeconds cw;

    float silentTime = 0f;
    public float silentTimeBeforeGeneral;

    private void Start()
    {
        ds = new WaitForSeconds(dialogueSpeed);
        sw = new WaitForSeconds(spaceWait);
        cw = new WaitForSeconds(characterWait);

        waitNextText = new WaitForSeconds(waitBeforeNextDialogue);
    }

    // Update is called once per frame
    void Update()
    {
        //Some random dialogue if nothing is happening
        if (!showingDialogue)
        {
            silentTime += Time.deltaTime;
        }
        
        if(silentTime > silentTimeBeforeGeneral)
        {
            silentTime = 0f;
            StartCoroutine(WriteDialogue(null, true, general[Random.Range(0, general.Length-1)]));
        }

        //Timing thingies
        ds = new WaitForSeconds(dialogueSpeed + Random.Range(-characterSpeedSway, characterSpeedSway));
        sw = new WaitForSeconds(spaceWait + Random.Range(-characterSpeedSway, characterSpeedSway));
        cw = new WaitForSeconds(characterWait + Random.Range(-characterSpeedSway, characterSpeedSway));

        //Decisions
        if (waitingForAnswer)
        {
            if (Input.GetKey(KeyCode.Alpha1))
            {
                dialoguePosition = currentDialogue.decision.dialoguePositions[0];
                ChoseDecision();
            }
            if (Input.GetKey(KeyCode.Alpha2))
            {
                if(currentDialogue.decision.dialoguePositions.Length >= 2)
                {
                    dialoguePosition = currentDialogue.decision.dialoguePositions[1];
                    ChoseDecision();
                }
            }
            if (Input.GetKey(KeyCode.Alpha3))
            {
                if(currentDialogue.decision.dialoguePositions.Length >= 3)
                {
                    dialoguePosition = currentDialogue.decision.dialoguePositions[2];
                    ChoseDecision();
                }
            }
        }

        //Move dialogue box around toward Nothing
        Vector3 nothingPosition = cam.WorldToScreenPoint(nothing.transform.position);

        if(nothingPosition.z > 0)
        {
            float screenWidthFifth = Screen.width / 5f;
            float screenHeightFifth = Screen.height / 5f;

            if (nothingPosition.x < screenWidthFifth)
            {
                nothingPosition.x = screenWidthFifth;
            }
            if (nothingPosition.x > Screen.width - screenWidthFifth)
            {
                nothingPosition.x = Screen.width - screenWidthFifth;
            }

            if (nothingPosition.y < screenHeightFifth)
            {
                nothingPosition.y = screenHeightFifth;
            }
            if (nothingPosition.y > Screen.height - screenHeightFifth)
            {
                nothingPosition.y = Screen.height - screenHeightFifth;
            }

            //dialogueRect.position = nothingPosition;
            dialogueRect.position = Vector3.Lerp(dialogueRect.position, nothingPosition, Time.deltaTime * dialogueMovementSpeed);
        }
    }

    void ChoseDecision()
    {
        StopAllCoroutines();
        StartCoroutine(FadeChoicesOut());
        waitingForAnswer = false;

        if (currentDialogue.activateNextImmediately)
        {
            StartCoroutine(WaitNextText());
        }
        else
        {
            showingDialogue = false;
            StartCoroutine(WaitBeforeFade());
        }
    }

    //Function used to activate dialogue outside this script
    public void ActivateDialogue()
    {
        StartCoroutine(WaitNextText());
    }

    public void ActivateDialogueAt(int pos)
    {
        StopAllCoroutines();
        StartCoroutine(FadeChoicesOut());
        dialoguePosition = pos;
        StartCoroutine(WaitNextText());
    }

    public IEnumerator WriteDialogue(Dialogue d, bool general, string generalText)
    {
        //First write the text to display
        char[] characters = (general ? generalText.ToCharArray() : d.text.ToCharArray());
        string displayedText = "";

        if(!general)
            currentDialogue = d;

        showingDialogue = true;

        foreach(char c in characters)
        {
            if (c == ' ')
            {
                displayedText += c;
                yield return sw;
            }
            else if(c == '*')
            {
                yield return cw;
            }
            else
            {
                speechAudio.Stop();
                speechAudio.Play();
                
                displayedText += c;
                yield return ds;
            }

            nothingText.text = displayedText;
        }

        if (!general)
        {
            //If the dialogue has a choice, update texts and fade them in
            if (d.hasPlayerDecision)
            {
                int decisionAmount = d.decision.dialoguePositions.Length;
                switch (decisionAmount)
                {
                    case 1:
                        dec1.text = d.decision.texts[0];
                        dec2.text = "";
                        dec3.text = "";
                        break;
                    case 2:
                        dec1.text = d.decision.texts[0];
                        dec2.text = d.decision.texts[1];
                        dec3.text = "";
                        break;
                    case 3:
                        dec1.text = d.decision.texts[0];
                        dec2.text = d.decision.texts[1];
                        dec3.text = d.decision.texts[2];
                        break;
                }

                StartCoroutine(FadeChoicesIn());

                waitingForAnswer = true;
                while (waitingForAnswer)
                {
                    yield return null;
                }
            }
            //If not a choice, update dialoguePointer
            else
            {
                if (d.nextDialogue == 0)
                {
                    dialoguePosition++;
                }
                else
                {
                    dialoguePosition = d.nextDialogue;
                }
            }

            //If we show the next text immediately, start typing it, otherwise fade the text thingy out
            if (!d.activateNextImmediately)
            {
                showingDialogue = false;
                StartCoroutine(WaitBeforeFade());
            }
            else
            {
                StartCoroutine(WaitNextText());
            }
        } else
        {
            StartCoroutine(WaitBeforeFade());
        }
    }

    WaitForSeconds waitNextText;
    IEnumerator WaitNextText()
    {
        silentTime = 0f;
        yield return waitNextText;
        StartCoroutine(WriteDialogue(dialogue[dialoguePosition], false, ""));
    }

    //Wait the same time as a next dialogue
    IEnumerator WaitBeforeFade()
    {
        yield return waitNextText;
        StartCoroutine(FadeTextOut());
    }



    //Transitions
    IEnumerator FadeTextOut()
    {
        bool faded = false;
        float pos = 1f;
        while (!faded)
        {
            pos -= Time.deltaTime * speed;
            text.alpha = fadeCurve.Evaluate(pos);
            if (pos <= 0)
            {
                faded = true;

                //Reset the text thingy
                nothingText.text = "";
                text.alpha = 1f;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator FadeChoicesIn()
    {
        float w1 = dec1.preferredWidth;
        float w2 = dec2.preferredWidth;
        float w3 = dec3.preferredWidth;

        float goalWidth = 0;

        if (w1 > w2 && w1 > w3)
        {
            goalWidth = w1;
        }
        else if(w2 > w1 && w2 > w3)
        {
            goalWidth = w2;
        }
        else
        {
            goalWidth = w3;
        }
        goalWidth += 50f;

        float pos = 0f;

        bool faded = false;
        while (!faded)
        {
            pos += Time.deltaTime * speed;
            seperator.sizeDelta = new Vector2(seperator.sizeDelta.x, goalWidth * fadeCurve.Evaluate(pos));

            if(pos >= 1f)
            {
                seperator.sizeDelta = new Vector2(seperator.sizeDelta.x, goalWidth);
                faded = true;
            }

            yield return new WaitForEndOfFrame();
        }

        anim.SetBool("Drop", true);
    }

    IEnumerator FadeChoicesOut()
    {
        bool faded = false;
        float pos = 1f;
        while (!faded)
        {
            pos -= Time.deltaTime * speed * 1.5f;
            choices.alpha = fadeCurve.Evaluate(pos);
            if (pos <= 0)
            {
                faded = true;

                anim.SetBool("Drop", false);
                seperator.sizeDelta = new Vector2(seperator.sizeDelta.x, 0f);
                choices.alpha = 1f;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
