using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TakeDamage : MonoBehaviour
{

    private Animator animator;
    private bool stateLocked = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private int hitCount = 0;
    public void Damage()
    {
        hitCount += 1;
        if(!stateLocked) { LockState("hurt"); }
        if(hitCount > 2)
        {
            LockState("death");
            
        }
    }

    // Used to ensure the state doesn't change before an animation is completed.
    private void LockState(string resetParameter)
    {
        StartCoroutine(LockStateCoroutine(resetParameter));

        IEnumerator LockStateCoroutine(string resetParameter)
        {
            stateLocked = true;
            animator.SetBool(resetParameter, !animator.GetBool(resetParameter));

            yield return null;
            animator.SetBool(resetParameter, !animator.GetBool(resetParameter));

            float seconds = animator.runtimeAnimatorController.animationClips
                .Where<AnimationClip>((x) => x.name == "down" + char.ToUpper(resetParameter[0]) + resetParameter.Substring(1))
                .First()
                .length;

            yield return new WaitForSeconds(seconds);
            if(resetParameter == "death")
            {
                this.gameObject.SetActive(false);
            }
            stateLocked = false;

        }
    }

}
