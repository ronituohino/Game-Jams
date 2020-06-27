using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTrigger : MonoBehaviour
{
    public bool activateDialogue = false;
    public int dialoguePos = 0;

    public bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            triggered = true;
            if (activateDialogue)
            {
                if(dialoguePos == 0)
                {
                    DialogueSystem.Instance.ActivateDialogue();
                } else
                {
                    DialogueSystem.Instance.ActivateDialogueAt(dialoguePos);
                }
                
            }
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            triggered = true;
            if (activateDialogue)
            {
                if(dialoguePos == 0)
                {
                    DialogueSystem.Instance.ActivateDialogue();
                }
                else
                {
                    DialogueSystem.Instance.ActivateDialogueAt(dialoguePos);
                }

            }
            gameObject.SetActive(false);
        }
    }
}
