using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class LockAnimationState
{

    private Boolean locked = false;

    public Boolean IsLocked { get { return locked; } }

    public void Lock (string resetParameter, Animator animator, MonoBehaviour behaviour)
    {

            behaviour.StartCoroutine(LockStateCoroutine(resetParameter));

            IEnumerator LockStateCoroutine(string resetParameter)
            {
                locked = true;
                animator.SetBool(resetParameter, !animator.GetBool(resetParameter));

                yield return null;
                animator.SetBool(resetParameter, !animator.GetBool(resetParameter));

                float seconds = animator.runtimeAnimatorController.animationClips
                    .Where<AnimationClip>((x) => x.name == "down" + char.ToUpper(resetParameter[0]) + resetParameter.Substring(1))
                    .First()
                    .length;

                yield return new WaitForSeconds(seconds);

                locked = false;

        }
    }

}
