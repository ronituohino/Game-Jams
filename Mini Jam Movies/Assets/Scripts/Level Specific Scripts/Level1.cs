using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Level1 : MonoBehaviour
{
    public PlayerControls player;
    public InfoManager info;

    public Image fade;
    public AnimationCurve fadeCurve;
    float fadeAmount = 1f;
    public float fadeSpeed;
    bool startFade = false;

    public GameObject grenade;

    public GameObject[] jailDoors;
    public GameObject[] brokenJailParts;

    public Light[] lights;

    public SpriteRenderer[] badBois;
    public SpriteRenderer scientist;

    public GameObject pistol;

    public float guardMovementSpeed;

    AudioSource flashBang;
    public AudioSource alarm;

    Grenade g;
    Vector3 movement = new Vector3(-1, 0, 0);
    bool moveBadBois = false;


    public GameObject[] checkPointOneBlockers;
    public GameObject[] checkPointTwoBlockers;


    public void PlayStartStuff()
    {
        flashBang = GetComponent<AudioSource>();
        StartCoroutine(WaitForTip());
    }

    // Start is called before the first frame update
    void Start()
    {
        startFade = true;
    }

    bool stopped = false;

    // Update is called once per frame
    void Update()
    {
        if (player.dead && !stopped)
        {
            stopped = true;
            StopAllCoroutines();
        }

        if (startFade)
        {
            Color c = fade.color;
            fade.color = new Color(c.r, c.g, c.b, fadeCurve.Evaluate(fadeAmount));
            fadeAmount -= Time.deltaTime * fadeSpeed;

            if(fadeAmount <= 0f)
            {
                startFade = false;
                fadeAmount = 0f;
            }
        }

        if (moveBadBois)
        {
            foreach (SpriteRenderer sr in badBois)
            {
                sr.transform.Translate(movement * Time.deltaTime * guardMovementSpeed, Space.World);
            }
        }
    }

    WaitForSeconds waitForTip = new WaitForSeconds(4f);
    IEnumerator WaitForTip()
    {
        yield return waitForTip;
        info.ShowTip(0);
        StartCoroutine(Wait());
    }

    WaitForSeconds wait = new WaitForSeconds(6f);
    IEnumerator Wait()
    {
        yield return wait;
        moveBadBois = true;
        StartCoroutine(ThrowGrenade());
    }

    //We throw the grenade
    WaitForSeconds waitBeforeThrowing = new WaitForSeconds(16f);
    IEnumerator ThrowGrenade()
    {
        yield return waitBeforeThrowing;
        grenade.SetActive(true);
        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(0, -7), ForceMode2D.Impulse);
        rb.AddTorque(-1);

        g = rb.gameObject.AddComponent<Grenade>();
        g.playAudio = false;
        g.explosion = player.explosion;
        g.explosionParent = player.explosionParent;
        g.ThrowPin();

        StartCoroutine(StartGameWithExplosion());
    }

    //Wait for it to explode
    WaitForSeconds waitBeforeStart = new WaitForSeconds(2f);
    IEnumerator StartGameWithExplosion()
    {
        yield return waitBeforeStart;
        
        g.Explode();
        player.FlashCamera(2.5f);
        flashBang.Play();

        StartCoroutine(TinyWait());
    }

    //Destroy surroundings
    WaitForSeconds tinyWait = new WaitForSeconds(0.1f);
    IEnumerator TinyWait()
    {
        yield return tinyWait;
        foreach (GameObject g in jailDoors)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in brokenJailParts)
        {
            g.SetActive(true);
        }
        foreach(Light l in lights)
        {
            l.color = Color.red;
        }

        moveBadBois = false;

        foreach(SpriteRenderer sr in badBois)
        {
            Destroy(sr.gameObject);
        }
        Destroy(scientist.gameObject);

        pistol.SetActive(true);

        StartCoroutine(Alarm());
    }

    WaitForSeconds alarmWait = new WaitForSeconds(2f);
    IEnumerator Alarm()
    {
        yield return alarmWait;
        alarm.Play();
    }
}
