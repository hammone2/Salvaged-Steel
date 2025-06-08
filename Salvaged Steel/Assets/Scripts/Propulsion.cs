using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propulsion : MonoBehaviour
{
    public float moveSpeed = 10f;
    private bool isMoving = false;
    [SerializeField] private Animator animator;

    public void WalkAnimation()
    {
        if (!animator)
            return;
        if (isMoving)
            return;
        isMoving = true;
        animator.SetTrigger("TriWalk");
    }

    public void IdleAnimation()
    {
        if (!animator)
            return;
        if (!isMoving)
            return;
        isMoving = false;
        animator.SetTrigger("TriIdle");
    }
}
