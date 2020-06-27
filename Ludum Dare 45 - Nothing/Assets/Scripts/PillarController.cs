using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarController : Singleton<PillarController>
{
    public Transform[] pillars;
    public Vector3[] pos;

    public float diff = 0f;
    public float speed = 1f;
    int length;

    private void Awake()
    {
        length = pillars.Length;

        pos = new Vector3[length];

        for (int i = 0; i < length; i++)
        {
            pos[i] = pillars[i].position;
        }
    }

    private void Update()
    {
        if(DialogueSystem.Instance.dialoguePosition >= 14)
        {
            for (int i = 0; i < length; i++)
            {
                Transform t = pillars[i];

                if ((PlayerMovement.Instance.transform.position - pos[i]).magnitude < diff)
                {
                    Vector3 p = pos[i];
                    if (t.position.y != p.y)
                    {
                        t.position = new Vector3(p.x, t.position.y + Time.deltaTime * speed, p.z);
                        if (t.position.y >= p.y)
                        {
                            t.position = p;
                        }
                    }
                }
                else
                {
                    if (t.position.y > -50)
                    {
                        Vector3 p = pos[i];
                        if (t.position.y > -50)
                        {
                            t.position = new Vector3(p.x, t.position.y - Time.deltaTime * speed, p.z);
                        }
                    }
                }
            }
        }
    }
}
