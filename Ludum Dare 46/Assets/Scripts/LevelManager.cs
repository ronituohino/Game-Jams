using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public int currentLevel = 0;
    public Transform connectionsParent;
    int levelAmount;

    [HideInInspector]
    public bool simulating = false;
    Triangle source;

    public GameObject endCredits;

    Vector2 startPos;
    Quaternion startAngle;

    public AudioSource audioSource;

    float level1Timer = 0f;
    public float helpThreshold = 10f;
    float fade = 0f;
    public SpriteRenderer sr;

    float level2Timer = 0f;
    float fade2 = 0f;
    public SpriteRenderer sr2;

    //private void Start()
    //{
    //    StartGame();
    //}

    public void StartGame()
    {
        currentLevel = 1;
        audioSource.Play();
        levelAmount = transform.childCount;

        StartCoroutine(LoadLevel(currentLevel, 1f));
    }

    void Update()
    {
        if (currentLevel == 1)
        {
            level1Timer += Time.deltaTime;
            if (level1Timer > helpThreshold)
            {
                if (fade < 1)
                {
                    fade += Time.deltaTime;
                }

                sr.color = new Color(1, 1, 1, fade);
            }
        }

        if (currentLevel == 2)
        {
            level2Timer += Time.deltaTime;
            if (level2Timer > helpThreshold)
            {
                if (fade2 < 1)
                {
                    fade2 += Time.deltaTime;
                }

                sr2.color = new Color(1, 1, 1, fade2);
            }
        }
    }

    IEnumerator LoadLevel(int level, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Transform parent = null;

        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform t = transform.GetChild(i);
            if (t.name == level.ToString())
            {
                parent = t;
            }
        }
        parent.gameObject.SetActive(true);

        source = parent.GetComponentInChildren<Triangle>();
        source.lerp = 0f;
        startPos = source.transform.position;
        startAngle = source.transform.rotation;

        SpriteRenderer[] spriteRenderers = parent.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            StartCoroutine(Controls.Instance.ApplyEffectToObject(sr.gameObject, true, Controls.Instance.FadeEffectIn, false, 1f, source));
        }

        LineRenderer[] lineRenderers = parent.GetComponentsInChildren<LineRenderer>();
        foreach (LineRenderer lr in lineRenderers)
        {
            StartCoroutine(Controls.Instance.ApplyEffectToObject(lr.gameObject, false, Controls.Instance.FadeEffectIn, false, 1f));
        }
    }

    IEnumerator DestroyLevel(int level, float secs)
    {
        Transform parent = null;

        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform t = transform.GetChild(i);
            if (t.name == level.ToString())
            {
                parent = t;
            }
        }

        SpriteRenderer[] spriteRenderers = parent.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            StartCoroutine(Controls.Instance.ApplyEffectToObject(sr.gameObject, true, Controls.Instance.FadeEffectOut, false, 1f));
        }

        LineRenderer[] lineRenderers = parent.GetComponentsInChildren<LineRenderer>();
        foreach (LineRenderer lr in lineRenderers)
        {
            StartCoroutine(Controls.Instance.ApplyEffectToObject(lr.gameObject, false, Controls.Instance.FadeEffectOut, false, 1f));
        }

        int count = connectionsParent.childCount;
        for (int i = 0; i < count; i++)
        {
            Controls.Instance.RemoveConnection(connectionsParent.GetChild(i).gameObject);
        }

        yield return new WaitForSeconds(secs);
        parent.gameObject.SetActive(false);
    }

    public void Launch()
    {
        simulating = true;
        source.rb.velocity = source.transform.up * source.velocity;
    }

    public void Fail()
    {
        simulating = false;
        source.rb.velocity = Vector2.zero;
        source.rb.MovePosition(startPos);
        source.sprite.rotation = startAngle;
        source.rb.SetRotation(startAngle.eulerAngles.z);
    }

    public void Finish()
    {
        StartCoroutine(DestroyLevel(currentLevel, 1f));

        currentLevel++;
        if (currentLevel <= levelAmount)
        {
            StartCoroutine(LoadLevel(currentLevel, 2f));
            if (currentLevel == levelAmount)
            {
                endCredits.SetActive(true);
            }
        }
        else
        {
            Debug.Log("Finished!");
        }
    }
}
