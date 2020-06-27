using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    Level1 level;
    public bool playAudio = true;

    void Start()
    {
        level = GameObject.Find("Level").GetComponent<Level1>();
        StartCoroutine(wait());

        if (playAudio)
        {
            AudioSource audio = GetComponent<AudioSource>();
            audio.volume = VolumeCalculation(Vector2.Distance(transform.position, level.player.player.transform.position));
            audio.Play();
        }
    }

    WaitForSeconds waitTime = new WaitForSeconds(1f);

    IEnumerator wait()
    {
        yield return waitTime;
        Destroy(gameObject);
    }

    float VolumeCalculation(float distance)
    {
        return 1 - Mathf.Clamp(distance, 0, 20).Remap(0, 20, 0, (1 - 0.1f)) + 0.1f;
    }
}
