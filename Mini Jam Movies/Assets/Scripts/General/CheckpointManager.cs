using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles the checkpoint system
public class CheckpointManager : Singleton<CheckpointManager>
{
    public int checkPointReached = -1; 
    public Checkpoint furthestCheckpoint = null;
    public Vector2 currentSpawnPoint = new Vector2(-0.48f, -0.22f);

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void ReachedCheckPoint(Checkpoint point)
    {
        if(point.number > checkPointReached)
        {
            furthestCheckpoint = point;
            checkPointReached = point.number;
            currentSpawnPoint = point.spawnPoint;
        }
    }
}
