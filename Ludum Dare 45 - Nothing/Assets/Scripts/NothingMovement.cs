using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NothingMovement : Singleton<NothingMovement>
{
    public GameObject nothing;
    public Rigidbody nothingRb;

    public Camera cam;

    public GameObject lerp;
    public GameObject animationLerp;

    [Space]

    public float lerpingDelta;
    public bool following = false;

    public bool finalGrab = false;

    // Update is called once per frame
    void Update()
    {
        if (following)
        {
            //Lerp nothing toward lerping point
            nothing.transform.position = Vector3.Lerp(nothing.transform.position, lerp.transform.position, Time.deltaTime * lerpingDelta * (PlayerMovement.Instance.moving ? 2f : 1f) * (finalGrab ? 0.1f : 1f));
        }
        else
        {
            //Lerp nothing toward animation lerping point
            nothing.transform.position = Vector3.Lerp(nothing.transform.position, animationLerp.transform.position, Time.deltaTime * lerpingDelta * 0.5f);
        }

        nothingRb.AddForce(-nothingRb.velocity, ForceMode.Acceleration);
        nothingRb.AddTorque(-nothingRb.angularVelocity, ForceMode.Acceleration);
        
    }
}
