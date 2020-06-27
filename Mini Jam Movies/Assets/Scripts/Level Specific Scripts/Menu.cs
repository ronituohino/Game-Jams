using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : Singleton<Menu>
{
    public GameObject logo;
    public GameObject pressToStart;

    public AnimationCurve smoothing;

    [Space]

    public float fadeInSpeed;
    float fade = 1f;
    public Image fadeIn;
    bool fadedIn = false;

    bool shownTip = false;

    [Space]

    float movieFade = 1f;
    bool startMovieFade = false;
    public float movieFadeSpeed;
    public CanvasGroup movieScreenFade;

    [Space]

    public GameObject rightThingy;
    public GameObject leftThingy;
    public float thingySpeed;

    [Space]

    public InfoManager info;


    public ParticleSystem[] explosions;
    bool menuDropped = false;


    //Remember to set to false
    bool waitForStartPress = false;

    private void Start()
    {
        foreach (ParticleSystem ps in explosions)
        {
            ps.Stop();
        }
    }

    void Update()
    {
        if (waitForStartPress) //Starts the game
        {
            if (Input.GetMouseButtonDown(0))
            {
                foreach (Image img in logo.GetComponentsInChildren<Image>())
                {
                    Rigidbody2D r = img.gameObject.AddComponent<Rigidbody2D>();
                    r.gravityScale = 1;
                    r.AddForce(new Vector2(Random.Range(-2, 2), Random.Range(5, 10)), ForceMode2D.Impulse);
                    r.AddTorque(Random.Range(-2, 2), ForceMode2D.Impulse);
                }
                Rigidbody2D rb = pressToStart.AddComponent<Rigidbody2D>();
                rb.gravityScale = 1;
                rb.AddForce(new Vector2(0, 5), ForceMode2D.Impulse);

                foreach (ParticleSystem ps in explosions)
                {
                    ps.Play();
                }

                StartCoroutine(DropMenu());
                StartCoroutine(LoadScene());
            }
        }
        else
        {
            if (!fadedIn) //First fade in to the screen
            {
                fade -= Time.deltaTime * fadeInSpeed;
                fadeIn.color = new Color(0f, 0f, 0f, smoothing.Evaluate(Mathf.Clamp01(fade)));
                if (fade <= 0f)
                {
                    fadedIn = true;
                    fade = 0f;
                    fadeIn.color = new Color(0f, 0f, 0f, fade);
                }
            }
            else if(!shownTip)
            {
                info.ShowTip(0);
                StartCoroutine(PrologueTime());
                shownTip = true;
            }
            else if (startMovieFade)
            {
                movieFade -= Time.deltaTime * movieFadeSpeed;
                movieScreenFade.alpha = smoothing.Evaluate(Mathf.Clamp01(movieFade));
                if(movieFade <= 0f)
                {
                    movieScreenFade.alpha = 0f;
                    waitForStartPress = true;
                }
            }
        }

        if (startMovieFade)
        {
            rightThingy.transform.Translate(Vector2.right * Time.deltaTime * thingySpeed);
            leftThingy.transform.Translate(Vector2.left * Time.deltaTime * thingySpeed);
        }
    }

    WaitForSeconds prologue = new WaitForSeconds(39f);
    IEnumerator PrologueTime()
    {
        yield return prologue;
        startMovieFade = true;
    }

    WaitForSecondsRealtime wait = new WaitForSecondsRealtime(3f);
    IEnumerator DropMenu()
    {
        yield return wait;
        menuDropped = true;
    }

    IEnumerator LoadScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Level1");
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (menuDropped)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
