using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool goalNode;
    public Transform nodeMask;

    float maskAngle;
    public float maskSpinSpeed;

    public List<Connection> connections = new List<Connection>();

    [HideInInspector]
    public bool inOrOut; //true in, false out
    float glow;
    [SerializeField ]float glowStep;
    Material m;

    private void Start()
    {
        m = gameObject.GetComponent<SpriteRenderer>().material;
        if(goalNode)
        {
            gameObject.tag = "Goal";
            nodeMask.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        } else
        {
            nodeMask.localScale = new Vector3(0f, 0f, 0f);
        }
    }

    private void Update()
    {
        maskAngle -= Time.deltaTime * maskSpinSpeed;

        if(inOrOut)
        {
            GlowIn();
        }
        else
        {
            GlowOut();
        }

        nodeMask.rotation = Quaternion.Euler(0, 0, maskAngle);
    }


    public void GlowIn()
    {
        if(glow < 0.3f)
        {
            glow += glowStep * Time.deltaTime;

            if(glow >= 0.3f)
            {
                glow = 0.3f;
            }

            m.SetFloat("_Glow", glow);
        }
    }

    public void GlowOut()
    {
        if (glow > 0f)
        {
            glow -= glowStep * Time.deltaTime;

            if (glow <= 0f)
            {
                glow = 0f;
            }

            
            m.SetFloat("_Glow", glow);
        }
    }
}