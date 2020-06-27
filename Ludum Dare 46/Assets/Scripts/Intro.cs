using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Intro : MonoBehaviour
{
    [Header("Texts")]
    public List<TextMeshProUGUI> textElements = new List<TextMeshProUGUI>();
    List<string> texts = new List<string>();
    public List<float> multipliers = new List<float>();

    public float characterTime;

    bool[] written;

    int currentText = 0;
    bool done = false;

    bool waitParagraph = false;

    public Animation exitAnimation;
    public AudioSource audioSource;

    private void Start()
    {
        written = new bool[textElements.Count]; 

        foreach(TextMeshProUGUI text in textElements)
        {
            texts.Add(text.text);
            text.text = "";
        }

        StartCoroutine(WriteText(currentText, multipliers[currentText]));
    }

    private void Update()
    {
        if (!done && written[currentText] && !waitParagraph)
        {
            currentText++;
            if(currentText < textElements.Count)
            {
                StartCoroutine(WriteText(currentText, multipliers[currentText]));
            } else
            {
                done = true;
            }
        }

        if(done)
        {
            exitAnimation.Play();
        }
    }

    public void StartGameFromLevelManager()
    {
        LevelManager.Instance.StartGame();
    }

    IEnumerator WriteText(int textNum, float speedMultiplier)
    {
        char[] textToWrite = texts[textNum].ToCharArray();
        int length = textToWrite.Length;

        string textWritten = "";

        foreach(char c in textToWrite)
        {
            textWritten += c;
            textElements[textNum].text = textWritten;

            audioSource.Play();

            if (c == ' ')
            {
                yield return new WaitForSeconds(characterTime * 1.5f * speedMultiplier);
            }
            else if(c == '.' || c== ',')
            {
                yield return new WaitForSeconds(characterTime * 20 * speedMultiplier);
            }
            else
            {
                yield return new WaitForSeconds(characterTime * speedMultiplier);
            }
        }

        written[textNum] = true;

        waitParagraph = true;
        StartCoroutine(ParagraphWait());
    }

    IEnumerator ParagraphWait()
    {
        yield return new WaitForSeconds(1.5f);
        waitParagraph = false;
    }
}
