using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TakeDamage : CustomMonoBehaviour
{

    private Animator animator;
    private int hitCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (hitCount > 2)
        {
            animator.SetBool("Death", true);

        } else
        {
            //animator.SetBool("Idle", true);
        }
    }

    
    public void Damage()
    {
        hitCount += 1;
        if(!IsAnimationStateLocked) { LockAnimationState("Hurt", animator, "downHurt"); }

    }

}
