using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Prologue : MonoBehaviour
{
    int pointer = -1;
    public string[] texts;
    string currentText = "";
    public TextMeshProUGUI text;


    public void NextText()
    {
        StopAllCoroutines();
        
        currentText = "";
        pointer++;

        StartCoroutine(Write(pointer));
    }

    WaitForSeconds wfs = new WaitForSeconds(0.05f);
    WaitForSeconds sw = new WaitForSeconds(0.2f);
    WaitForSeconds spaceWait = new WaitForSeconds(0.06f);

    IEnumerator Write(int num)
    {
        char[] chars = texts[num].ToCharArray();
        foreach (char c in chars)
        {
            if(c == ' ')
            {
                currentText += c;
                yield return spaceWait;
            }
            else if(c == '*')
            {
                yield return sw;
            }
            else
            {
                currentText += c;
                yield return wfs;
            }

            text.text = currentText;
        }
    }
}
