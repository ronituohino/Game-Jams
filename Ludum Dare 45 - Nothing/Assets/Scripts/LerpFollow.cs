using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpFollow : MonoBehaviour
{
    public void StartFollow()
    {
        NothingMovement.Instance.following = true;
    }
}
