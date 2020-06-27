using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarTrigger : MonoBehaviour
{
    PlayerControls player;
    CanvasGroup healtBar;


    bool showHealthBar = false;
    public AnimationCurve fadeCurve;
    float visibility;
    public float fadeSpeed;

    private void Start()
    {
        player = GameObject.Find("Player Controller").GetComponent<PlayerControls>();
        healtBar = GameObject.Find("HealthBar").GetComponent<CanvasGroup>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        showHealthBar = true;
    }

    bool startedTheShow = false;

    private void Update()
    {
        if (showHealthBar && !startedTheShow)
        {
            visibility += Time.deltaTime * fadeSpeed;
            healtBar.alpha = Mathf.Clamp01(visibility);

            if(visibility >= 1f)
            {
                StartCoroutine(StartTheShow());
                startedTheShow = true;
            }
        }
    }

    WaitForSeconds wait = new WaitForSeconds(5f);
    IEnumerator StartTheShow()
    {
        yield return wait;
        player.reduceHealth = true;
    }
}
