using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoManager : MonoBehaviour
{
    public float tipSpeed;
    public float waitUntilRemoving;
    public float textPauseTime;
    public float tipBackingSpeed;
    public TextMeshProUGUI text;

    public string[] tips;
    
    float shownText;
    bool showingTip = false;
    bool continueWriting = true;
    string tipToShow;
    int len = 0;
    bool backingUp = false;
    bool startedWait = false;

    private void Start()
    {
        waitTime = new WaitForSeconds(waitUntilRemoving);
        pauseTime = new WaitForSeconds(textPauseTime);
    }

    public void ShowTip(int tipId)
    {
        tipToShow = tips[tipId];
        len = tipToShow.Length;
        showingTip = true;
        shownText = 0f;
        continueWriting = true;
        backingUp = false;
        startedWait = false;

        StopAllCoroutines();
    }

    private void Update()
    {
        if (showingTip)
        {
            if (!backingUp)
            {
                if (shownText >= len)
                {
                    if (!startedWait)
                    {
                        StartCoroutine(Wait());
                        startedWait = true;
                    }
                } else
                {
                    if (continueWriting)
                    {
                        shownText += tipSpeed * Time.deltaTime;
                        int amountOfCharsToShow = Mathf.FloorToInt(shownText);
                        text.text = tipToShow.Substring(0, amountOfCharsToShow).Replace('*', ' ');
                        try
                        {
                            if (tipToShow[amountOfCharsToShow] == '*')
                            {
                                continueWriting = false;
                                StartCoroutine(Pause());
                            }
                        }
                        catch { }
                    }
                    
                }
            }
            else
            {
                shownText -= tipBackingSpeed * Time.deltaTime * len;
                int amountOfCharsToShow = Mathf.FloorToInt(Mathf.Clamp(shownText, 0f, 10000f));
                text.text = tipToShow.Substring(0, amountOfCharsToShow).Replace('*', ' ');

                if (shownText <= 0)
                {
                    showingTip = false;
                    backingUp = false;
                    startedWait = false;
                }
            }
        }
    }

    WaitForSeconds waitTime;
    IEnumerator Wait()
    {
        yield return waitTime;
        backingUp = true;
    }

    WaitForSeconds pauseTime;
    IEnumerator Pause()
    {
        yield return pauseTime;
        continueWriting = true;
    }
}
