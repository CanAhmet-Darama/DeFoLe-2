using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimatingClass : MonoBehaviour
{
    GeneralCharacter character;
    [SerializeField] float blendLerpSpeed;
    float targetBlendX;
    float targetBlendY;
    void Start()
    {
        character = transform.parent.GetComponent<GeneralCharacter>();
        character.animStateSpeed = AnimStateSpeed.idle;
        character.animStatePriDir = AnimStatePriDir.none;
        character.animStateSecDir = AnimStateSecDir.none;
    }

    void Update()
    {
        switch (character.animStateSpeed)
        {
            case AnimStateSpeed.walk: ManageAnimatorParameters(false); break;
            case AnimStateSpeed.run: ManageAnimatorParameters(true); break;
            default:
                targetBlendX = 0;
                targetBlendY = 0;
                break;
        }
        character.blendAnimX = GameManager.LerpOrSnap(character.blendAnimX, targetBlendX, blendLerpSpeed);
        character.blendAnimY = GameManager.LerpOrSnap(character.blendAnimY, targetBlendY, blendLerpSpeed);

        character.animator.SetFloat("blendVeloX", character.blendAnimX);
        character.animator.SetFloat("blendVeloY", character.blendAnimY);

        //if(transform.parent.name == "Enemy 1")
        //{
        //    Debug.Log(character.animator.GetFloat("blendVeloX"));
        //    Debug.Log(character.animStateSpeed);
        //}

        character.animator.SetBool("isJumping", character.isJumping);
        character.animator.SetBool("isGrounded", character.isGrounded);

    }
    //static float SnapOrLerpVaule(float whatTo, float whereTo, float lerpSpeed)
    //{
    //    if(Mathf.Abs(whatTo - whereTo) > 0.01f)
    //    {
    //        return Mathf.Lerp(whatTo, whereTo, lerpSpeed);
    //    }
    //    else
    //    {
    //        return whereTo;
    //    }
    //}
    void ManageAnimatorParameters(bool isRunning)
    {
        if (isRunning)
        {
            /* setting values for blend tree in animator for walk run idle */
            switch (character.animStatePriDir)
            {
                case AnimStatePriDir.front:
                    switch (character.animStateSecDir)
                    {
                        case AnimStateSecDir.left:
                            targetBlendX = -0.7f;
                            targetBlendY = 0.7f;
                            break;
                        case AnimStateSecDir.right:
                            targetBlendX = 0.7f;
                            targetBlendY = 0.7f;
                            break;
                        default:
                            targetBlendX = 0;
                            targetBlendY = 1;
                            break;
                    }
                    break;
                case AnimStatePriDir.back:
                    switch (character.animStateSecDir)
                    {
                        case AnimStateSecDir.left:
                            targetBlendX = -0.7f;
                            targetBlendY = -0.7f;
                            break;
                        case AnimStateSecDir.right:
                            targetBlendX = 0.7f;
                            targetBlendY = -0.7f;
                            break;
                        default:
                            targetBlendX = 0;
                            targetBlendY = 1;
                            break;
                    }
                    break;
                default:
                    switch (character.animStateSecDir)
                    {
                        case AnimStateSecDir.left:
                            targetBlendX = -1;
                            targetBlendY = 0;
                            break;
                        case AnimStateSecDir.right:
                            targetBlendX = 1;
                            targetBlendY = 0;
                            break;
                    }
                    break;
            }
        }
        else
        {
            switch (character.animStatePriDir)
            {
                case AnimStatePriDir.front:
                    switch (character.animStateSecDir)
                    {
                        case AnimStateSecDir.left:
                            targetBlendX = -0.35f;
                            targetBlendY = 0.35f;
                            break;
                        case AnimStateSecDir.right:
                            targetBlendX = 0.35f;
                            targetBlendY = 0.35f;
                            break;
                        default:
                            targetBlendX = 0;
                            targetBlendY = 0.5f;
                            break;
                    }
                    break;
                case AnimStatePriDir.back:
                    switch (character.animStateSecDir)
                    {
                        case AnimStateSecDir.left:
                            targetBlendX = -0.35f;
                            targetBlendY = -0.35f;
                            break;
                        case AnimStateSecDir.right:
                            targetBlendX = 0.35f;
                            targetBlendY = -0.35f;
                            break;
                        default:
                            targetBlendX = 0;
                            targetBlendY = -0.5f;
                            break;
                    }
                    break;
                default:
                    switch (character.animStateSecDir)
                    {
                        case AnimStateSecDir.left:
                            targetBlendX = -0.5f;
                            targetBlendY = 0;
                            break;
                        case AnimStateSecDir.right:
                            targetBlendX = 0.5f;
                            targetBlendY = 0;
                            break;
                    }
                    break;
            }

        }

    }
    //void ManageAnims(bool isRunning)
    //{
    //        AnimStateDirPirAndMoreChecker(character.animStatePriDir, isRunning);
    //}
    //void ManageSecAnimState()
    //{
    //    switch (character.animStateSpeed)
    //    {
    //        case AnimStateSpeed.run:
    //            switch (character.animStatePriDir)
    //            {
    //                case AnimStatePriDir.front:
    //                    switch (character.animStateSecDir)
    //                    {
    //                        case AnimStateSecDir.none: character.animator.Play("Running"); break;
    //                        case AnimStateSecDir.left: character.animator.Play("Run Front Left"); break;
    //                        case AnimStateSecDir.right: character.animator.Play("Run Front Right"); break;
    //                    }
    //                    break;
    //                case AnimStatePriDir.back:
    //                    switch (character.animStateSecDir)
    //                    {
    //                        case AnimStateSecDir.none: character.animator.Play("Run Back"); break;
    //                        case AnimStateSecDir.left: character.animator.Play("Run Back Left"); break;
    //                        case AnimStateSecDir.right: character.animator.Play("Run Back Right"); break;
    //                    }
    //                    break;
    //                case AnimStatePriDir.none:
    //                    switch (character.animStateSecDir)
    //                    {
    //                        case AnimStateSecDir.left: character.animator.Play("Run Left"); break;
    //                        case AnimStateSecDir.right: character.animator.Play("Run Right"); break;
    //                    }
    //                    break;
    //            }
    //            break;
    //        case AnimStateSpeed.walk:
    //            switch (character.animStatePriDir)
    //            {
    //                case AnimStatePriDir.front:
    //                    switch (character.animStateSecDir)
    //                    {
    //                        case AnimStateSecDir.none: character.animator.Play("Walking"); break;
    //                        case AnimStateSecDir.left: character.animator.Play("Walk Front Left"); break;
    //                        case AnimStateSecDir.right: character.animator.Play("Walk Front Right"); break;
    //                    }
    //                    break;
    //                case AnimStatePriDir.back:
    //                    switch (character.animStateSecDir)
    //                    {
    //                        case AnimStateSecDir.none: character.animator.Play("Walk Back"); break;
    //                        case AnimStateSecDir.left: character.animator.Play("Walk Back Left"); break;
    //                        case AnimStateSecDir.right: character.animator.Play("Walk Back Right"); break;
    //                    }
    //                    break;
    //                case AnimStatePriDir.none:
    //                    switch (character.animStateSecDir)
    //                    {
    //                        case AnimStateSecDir.left: character.animator.Play("Walk Left"); break;
    //                        case AnimStateSecDir.right: character.animator.Play("Walk Right"); break;
    //                    }
    //                    break;
    //            }
    //            break;
    //    }
    //}
    //void AnimStateDirPirAndMoreChecker(AnimStatePriDir _animStatePriDir, bool isRunning)
    //{
    //    if (isRunning)
    //    {
    //        switch (_animStatePriDir)
    //        {
    //            case AnimStatePriDir.front:
    //                switch (character.animStateSecDir)
    //                {
    //                    case AnimStateSecDir.left: ManageSecAnimState(AnimStateSpeed.run, AnimStatePriDir.front, AnimStateSecDir.left); break;
    //                    case AnimStateSecDir.right: ManageSecAnimState(AnimStateSpeed.run, AnimStatePriDir.front, AnimStateSecDir.right); break;
    //                    default: ManageSecAnimState(AnimStateSpeed.run, AnimStatePriDir.front, AnimStateSecDir.none); break;
    //                }
    //                break;
    //            case AnimStatePriDir.back:
    //                switch (character.animStateSecDir)
    //                {
    //                    case AnimStateSecDir.left: ManageSecAnimState(AnimStateSpeed.run, AnimStatePriDir.back, AnimStateSecDir.left); break;
    //                    case AnimStateSecDir.right: ManageSecAnimState(AnimStateSpeed.run, AnimStatePriDir.back, AnimStateSecDir.right); break;
    //                    default: ManageSecAnimState(AnimStateSpeed.run, AnimStatePriDir.back, AnimStateSecDir.none); break;
    //                }
    //                break;
    //            default:
    //                switch (character.animStateSecDir)
    //                {
    //                    case AnimStateSecDir.left: ManageSecAnimState(AnimStateSpeed.run, AnimStatePriDir.none, AnimStateSecDir.left); break;
    //                    case AnimStateSecDir.right: ManageSecAnimState(AnimStateSpeed.run, AnimStatePriDir.none, AnimStateSecDir.right); break;
    //                    default: ManageSecAnimState(AnimStateSpeed.run, AnimStatePriDir.none, AnimStateSecDir.none); break;
    //                }
    //                break;
    //        }
    //    }
    //    else
    //    {
    //        switch (_animStatePriDir)
    //        {
    //            case AnimStatePriDir.front:
    //                switch (character.animStateSecDir)
    //                {
    //                    case AnimStateSecDir.left: ManageSecAnimState(AnimStateSpeed.walk, AnimStatePriDir.front, AnimStateSecDir.left); break;
    //                    case AnimStateSecDir.right: ManageSecAnimState(AnimStateSpeed.walk, AnimStatePriDir.front, AnimStateSecDir.right); break;
    //                    default: ManageSecAnimState(AnimStateSpeed.walk, AnimStatePriDir.front, AnimStateSecDir.none); break;
    //                }
    //                break;
    //            case AnimStatePriDir.back:
    //                switch (character.animStateSecDir)
    //                {
    //                    case AnimStateSecDir.left: ManageSecAnimState(AnimStateSpeed.walk, AnimStatePriDir.back, AnimStateSecDir.left); break;
    //                    case AnimStateSecDir.right: ManageSecAnimState(AnimStateSpeed.walk, AnimStatePriDir.back, AnimStateSecDir.right); break;
    //                    default: ManageSecAnimState(AnimStateSpeed.walk, AnimStatePriDir.back, AnimStateSecDir.none); break;
    //                }
    //                break;
    //            default:
    //                switch (character.animStateSecDir)
    //                {
    //                    case AnimStateSecDir.left: ManageSecAnimState(AnimStateSpeed.walk, AnimStatePriDir.none, AnimStateSecDir.left); break;
    //                    case AnimStateSecDir.right: ManageSecAnimState(AnimStateSpeed.walk, AnimStatePriDir.none, AnimStateSecDir.right); break;
    //                    default: ManageSecAnimState(AnimStateSpeed.walk, AnimStatePriDir.none, AnimStateSecDir.none); break;
    //                }
    //                break;
    //        }

    //    }

    //}

    public void SetAnimStates(AnimStateSpeed state1, AnimStatePriDir state2, AnimStateSecDir state3)
    {
        SetAnimStateSpeed(state1);
        SetAnimStatePri(state2);
        SetAnimStateSec(state3);
    }
    public void SetAnimStates(AnimStatePriDir state2, AnimStateSecDir state3)
    {
        SetAnimStatePri(state2);
        SetAnimStateSec(state3);
    }
    public void SetAnimStateSpeed(AnimStateSpeed state)
    {
        character.animStateSpeed = state;
    }
    public void SetAnimStatePri(AnimStatePriDir state)
    {
        character.animStatePriDir = state;
    }
    public void SetAnimStateSec(AnimStateSecDir state)
    {
        character.animStateSecDir = state;

    }
}