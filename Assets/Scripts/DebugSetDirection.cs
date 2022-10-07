using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSetDirection : MonoBehaviour
{
    public enum Direction
    {
        Down,
        Left,
        Right,
        Up
    }

    public Direction direction = Direction.Down;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (direction)
        {
            case Direction.Down:
                animator.SetFloat("MoveX", 0);
                animator.SetFloat("MoveY", -1);
                break;
            case Direction.Left:
                animator.SetFloat("MoveX", -1);
                animator.SetFloat("MoveY", 0);
                break;
            case Direction.Right:
                animator.SetFloat("MoveX", 1);
                animator.SetFloat("MoveY", 0);
                break;
            case Direction.Up:
                animator.SetFloat("MoveX", 0);
                animator.SetFloat("MoveY", 1);
                break;
        }
    }
}
