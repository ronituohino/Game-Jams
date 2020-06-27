using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public LineRenderer line;
    public EdgeCollider2D edgeCollider;

    public bool updateCollider;

    private void Start()
    {
        AssignColliderPoints();
    }

    void AssignColliderPoints()
    {
        int count = line.positionCount;
        Vector2[] colliderPoints = new Vector2[count];

        for (int i = 0; i < count; i++)
        {
            colliderPoints[i] = line.GetPosition(i);
        }

        edgeCollider.points = colliderPoints;
    }

    private void Update()
    {
        if(updateCollider)
        {
            AssignColliderPoints();
        }
    }
}