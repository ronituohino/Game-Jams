using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingRoom : MonoBehaviour
{
    bool fightRoomActive = false;
    bool startSpawning = false;
    int waveCounter = 0;
    bool waveSpawned = false;

    bool fightRoomCleared = false;

    public List<SpawnWave> spawns = new List<SpawnWave>();

    public EnemyController[] guysToClearAtFirst;

    bool doorsClosed = false;
    public GameObject[] doorsToCloseWhenActivated;
    public GameObject[] doorsToOpenOnceCleared;

    public Light[] lightsToActivate;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!fightRoomCleared)
        {
            if(collision.name == "Player")
            {
                fightRoomActive = true;
                if (!doorsClosed)
                {
                    foreach (GameObject g in doorsToCloseWhenActivated)
                    {
                        g.SetActive(true);
                    }
                    doorsClosed = true;

                    foreach (Light l in lightsToActivate)
                    {
                        l.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (fightRoomActive)
        {
            if (!startSpawning) //Clear out the already-there-dudes
            {
                bool guysCleared = true;
                foreach (EnemyController ec in guysToClearAtFirst)
                {
                    if (ec != null)
                    {
                        guysCleared = false;
                        break;
                    }
                }

                if (guysCleared)
                {
                    startSpawning = true;
                }
            } else
            {
                if (!waveSpawned) //Spawn the wave
                {
                    if(waveCounter == spawns.Count)
                    {
                        fightRoomCleared = true;
                        fightRoomActive = false;

                        foreach(GameObject g in doorsToOpenOnceCleared)
                        {
                            g.SetActive(false);
                        }
                    }
                    else
                    {
                        SpawnWave sw = spawns[waveCounter];
                        foreach (GameObject g in sw.spawns)
                        {
                            g.SetActive(true);
                            waveSpawned = true;
                        }
                    }
                }
                else //Wait for the player to clear them
                {
                    bool guysCleared = true;
                    foreach (EnemyController ec in spawns[waveCounter].controllers)
                    {
                        if (ec != null)
                        {
                            guysCleared = false;
                            break;
                        }
                    }

                    if (guysCleared)
                    {
                        waveSpawned = false;
                        waveCounter++;
                    }
                }
            }
        }
    }

    [System.Serializable]
    public struct SpawnWave
    {
        public GameObject[] spawns;
        public EnemyController[] controllers;
    }
}
