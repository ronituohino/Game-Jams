using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Singleton<PlayerMovement>
{
    public GameObject player;
    public Camera cam;
    public CharacterController cc;
    public GameObject respawn;

    [Header("Controls")]

    public float movementSpeed;
    public float mouseSensitivity;

    [Space]

    public float minVertical;
    public float maxVertical;

    [HideInInspector]
    public bool moving = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    bool respawned = false;
    public bool invertGravity = false;

    // Update is called once per frame
    void Update()
    {
        //Respawning
        if(transform.position.y < -20 && !respawned)
        {
            cc.enabled = false;

            transform.position = respawn.transform.position;
            StartCoroutine(DialogueSystem.Instance.WriteDialogue(null, true, DialogueSystem.Instance.general[DialogueSystem.Instance.general.Length - 1]));
            respawned = true;

            cc.enabled = true;
        } else
        {
            respawned = false;
        }

        moving = false;

        //Basic movement
        if (Input.GetKey(KeyCode.W))
        {
            moving = true;
            cc.Move(player.transform.forward * movementSpeed * Time.deltaTime);
            //cc.SimpleMove(player.transform.forward * movementSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            moving = true;
            cc.Move(-player.transform.right * movementSpeed * Time.deltaTime);
            //cc.SimpleMove(-player.transform.right * movementSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            moving = true;
            cc.Move(-player.transform.forward * movementSpeed * Time.deltaTime);
            //cc.SimpleMove(-player.transform.forward * movementSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            moving = true;
            cc.Move(player.transform.right * movementSpeed * Time.deltaTime);
            //cc.SimpleMove(player.transform.right * movementSpeed * Time.deltaTime);
        }

        //Adjust sensitivity
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if(mouseSensitivity + 0.15f <= 8f)
            {
                mouseSensitivity += 0.15f;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            if((mouseSensitivity - 0.15f) >= 0f)
            {
                mouseSensitivity -= 0.15f;
            }
        }

        if (!invertGravity)
        {
            cc.SimpleMove(Physics.gravity * Time.deltaTime);
        } else
        {
            cc.enabled = false;
            transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime * 0.5f, transform.position.z);
        }
        

        if (Input.GetKey(KeyCode.UpArrow))
        {
            Time.timeScale += 0.05f;
            Debug.Log(Time.timeScale);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Time.timeScale -= 0.05f;
            Debug.Log(Time.timeScale);
        }

        //Mouse deltas
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        //Player and camera rotation with limits
        if(mouseX != 0f)
        {
            player.transform.rotation *= Quaternion.Euler(new Vector3(0, mouseX * mouseSensitivity, 0));
        }

        if(mouseY != 0f)
        {
            float angle = cam.transform.rotation.eulerAngles.x;
            bool camUp = angle > 270f;

            if (camUp)
            {
                Quaternion newRotation = cam.transform.rotation * Quaternion.Euler(new Vector3(-mouseY * mouseSensitivity, 0, 0));
                if (angle > maxVertical && newRotation.eulerAngles.x > maxVertical)
                {
                    cam.transform.rotation = newRotation;
                }
                else if (mouseY < 0f)
                {
                    cam.transform.rotation *= Quaternion.Euler(new Vector3(-mouseY * mouseSensitivity, 0, 0));
                }
                else
                {
                    cam.transform.rotation = Quaternion.Euler(new Vector3(maxVertical, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z));
                }
            }
            else
            {
                Quaternion newRotation = cam.transform.rotation * Quaternion.Euler(new Vector3(-mouseY * mouseSensitivity, 0, 0));
                if (angle < minVertical && newRotation.eulerAngles.x < minVertical)
                {
                    cam.transform.rotation = newRotation;
                }
                else if (mouseY > 0f)
                {
                    cam.transform.rotation *= Quaternion.Euler(new Vector3(-mouseY * mouseSensitivity, 0, 0));
                }
                else
                {
                    cam.transform.rotation = Quaternion.Euler(new Vector3(minVertical, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z));
                }
            }
        }
    }
}
