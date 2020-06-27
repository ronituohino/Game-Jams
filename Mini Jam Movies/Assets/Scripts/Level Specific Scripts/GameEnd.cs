using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEnd : MonoBehaviour
{
    public float fadeSpeed = 1f;
    public Image fade;
    bool gameEnd = false;
    float fadeAmount = 0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!gameEnd)
        {
            if (collision.name == "Player")
            {
                gameEnd = true;
            }
        }
    }

    private void Update()
    {
        if (gameEnd)
        {
            fadeAmount += Time.deltaTime * fadeSpeed;
            fade.color = new Color(0f, 0f, 0f, Mathf.Clamp01(fadeAmount));

            if(fadeAmount > 1f)
            {
                SceneManager.LoadScene("Credits");
            }
        }
    }
}
