using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class FinalScripts : MonoBehaviour
{
    public Material emissive;
    public MeshRenderer mr;
    public VisualEffect vfx;

    public Animator fadeAnimator;

    public void UpdateMaterial()
    {
        mr.sharedMaterial = emissive;
    }

    bool target = false;
    public void StartTargetingPlayer()
    {
        target = true;
        StartCoroutine(FinalFade());
    }

    private void Update()
    {
        if (target)
        {
            Vector3 playerPos = PlayerMovement.Instance.transform.position;
            vfx.SetVector3("Direction", new Vector3(playerPos.x, playerPos.y - 0.7f, playerPos.z) - transform.position);
            PlayerMovement.Instance.invertGravity = true;
        }
    }

    IEnumerator FinalFade()
    {
        yield return new WaitForSeconds(10f);
        fadeAnimator.SetTrigger("EndGame");
        StartCoroutine(LittleWait());
    }

    IEnumerator LittleWait()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("End");
    }
}
