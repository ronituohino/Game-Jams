using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Experimental.VFX;

public class Events : Singleton<Events>
{
    public Animator nothingAnimator;

    public Animation lever;
    public Animation door;

    bool triggeredDoorAndLever = false;

    public CollisionTrigger leverTrigger;
    bool nothingPulledLever = false;

    bool playerPulled = false;


    bool colorAsked = false;
    bool colorQuestionShown = false;

    public Light nothingLight;
    public ParticleSystem ps;

    public Gradient redGradient;
    public Gradient greenGradient;

    //Final
    public CollisionTrigger ct;

    bool finalTriggered = false;
    public Animation rocks;
    public Animation final;

    public bool grabNothign = false;
    public Transform finalPos;

    //Game start
    void Start()
    {
        StartCoroutine(StartWait());
    }

    void Update()
    {
        //First door and lever interaction
        if(DialogueSystem.Instance.dialoguePosition == 5 && !triggeredDoorAndLever)
        {
            StartCoroutine(WaitForDoorInteraction());
            triggeredDoorAndLever = true;
        }


        if(DialogueSystem.Instance.dialoguePosition == 7 && !nothingPulledLever)
        {
            StartCoroutine(NothingPull());

            nothingPulledLever = true;
        }
        if(DialogueSystem.Instance.dialoguePosition == 9 && !playerPulled && !nothingPulledLever)
        {
            leverTrigger.gameObject.SetActive(true);
            if (leverTrigger.triggered)
            {
                lever.Play();
                DialogueSystem.Instance.ActivateDialogue();
                door.Play();

                playerPulled = true;
                NothingMovement.Instance.following = true;
            }
        }
        
        if(DialogueSystem.Instance.dialoguePosition == 17 && !colorAsked)
        {
            colorAsked = true;
            StartCoroutine(ColorQuestion());
        }
        if(colorQuestionShown)
        {
            if (Input.GetKey(KeyCode.Alpha2))
            {
                colorQuestionShown = false;
                nothingLight.color = Color.red;

                ParticleSystem.ColorOverLifetimeModule cm = ps.colorOverLifetime;
                cm.color = new ParticleSystem.MinMaxGradient(redGradient);
            }
            if (Input.GetKey(KeyCode.Alpha3))
            {
                colorQuestionShown = false;
                nothingLight.color = Color.green;

                ParticleSystem.ColorOverLifetimeModule cm = ps.colorOverLifetime;
                cm.color = new ParticleSystem.MinMaxGradient(greenGradient);
            }
        }

        //Cave trigger
        if (ct.triggered && !finalTriggered)
        {
            DialogueSystem.Instance.silentTimeBeforeGeneral = 1000f;
            rocks.Play();
            StartCoroutine(DimLights());
            StartCoroutine(GrabNothing());
            StartCoroutine(Final());

            finalTriggered = true;
        }

        if (grabNothign)
        {
            NothingMovement.Instance.finalGrab = true;
            NothingMovement.Instance.lerp.transform.position = finalPos.position;
        } 
    }

    //Wait and start the game
    WaitForSeconds wfs = new WaitForSeconds(5f);
    IEnumerator StartWait()
    {
        yield return wfs;
        StartCoroutine(DialogueSystem.Instance.WriteDialogue(DialogueSystem.Instance.dialogue[0], false, ""));
    }

    IEnumerator WaitForDoorInteraction()
    {
        yield return new WaitForSeconds(1.5f);

        NothingMovement.Instance.following = false;
        nothingAnimator.SetTrigger("DoorAndLever");
        DialogueSystem.Instance.ActivateDialogue();
    }

    IEnumerator NothingPull()
    {
        yield return new WaitForSeconds(5f);

        door.Play();
        lever.Play();
        DialogueSystem.Instance.ActivateDialogue();
        NothingMovement.Instance.following = true;
    }

    IEnumerator ColorQuestion()
    {
        yield return new WaitForSeconds(3f);
        colorQuestionShown = true;
        DialogueSystem.Instance.ActivateDialogue();
    }

    IEnumerator DimLights()
    {
        DialogueSystem.Instance.ActivateDialogue();

        bool dim = false;
        while (!dim)
        {
            Color c = RenderSettings.ambientLight;
            RenderSettings.ambientLight = new Color(c.r - 0.01f, c.g - 0.01f, c.b - 0.01f);

            if(c.r - 0.01f <= 0)
            {
                dim = true;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator GrabNothing()
    {
        yield return new WaitForSeconds(1f);
        grabNothign = true;
    }

    IEnumerator Final()
    {
        yield return new WaitForSeconds(5f);
        final.Play();
    }
}
