using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeText : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void Change()
    {
        text.text = "HE IS NOTHING";
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("Main");
    }
}
