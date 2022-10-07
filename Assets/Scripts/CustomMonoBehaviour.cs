using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomMonoBehaviour : MonoBehaviour
{
    private Boolean animationStatelocked = false;

    public Boolean IsAnimationStateLocked { get { return animationStatelocked; } }

    public void LockAnimationState(string resetParameter, Animator animator, String animation)
    {

        StartCoroutine(LockStateCoroutine(resetParameter));

        IEnumerator LockStateCoroutine(string resetParameter)
        {
            animationStatelocked = true;
            animator.SetBool(resetParameter, !animator.GetBool(resetParameter));

            yield return null;
            animator.SetBool(resetParameter, !animator.GetBool(resetParameter));

            float seconds = animator.runtimeAnimatorController.animationClips
                .Where<AnimationClip>((x) => x.name == animation)
                .First()
                .length;

            yield return new WaitForSeconds(seconds);

            animationStatelocked = false;

        }
    }
}
