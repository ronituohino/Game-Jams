using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoTrigger : MonoBehaviour
{
    InfoManager info;
    public int infoIdToShow;
    bool shown = false;

    private void Start()
    {
        info = GameObject.Find("Player Controller").GetComponent<InfoManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name == "Player")
        {
            if (!shown)
            {
                info.ShowTip(infoIdToShow);
                shown = true;
            }
        }
    }
}
