using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class StartScript : MonoBehaviour
{
    public Animator fade;
    public VideoPlayer vp;

    public Animator timer;

    public GameObject txt;
    public Prologue pr;

    public Animator flash;

    bool start = false;

    // Start is called before the first frame update
    void Start()
    {
        vp.url = System.IO.Path.Combine(Application.streamingAssetsPath, "intro.mp4");
        vp.loopPointReached += FlashText;
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            vp.Play();

            fade.SetBool("Fade", true);
            timer.SetBool("Play", true);

            if(vp.frame > vp.frameCount - (vp.frameCount / 8f))
            {
                txt.SetActive(false);
                fade.SetBool("Fade", false);
            }
        }
        else
        {
            if (Input.anyKey)
            {
                start = true;
            }
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
            {
                start = true;
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene("Main");
        }
    }

    void FlashText(VideoPlayer vp)
    {
        flash.SetTrigger("Trigger");
    }

    public void Pause()
    {
        vp.playbackSpeed = 0f;
        pr.NextText();
    }

    public void Continue()
    {
        vp.playbackSpeed = 1f;
    }

    public void Nothing() { }
}
