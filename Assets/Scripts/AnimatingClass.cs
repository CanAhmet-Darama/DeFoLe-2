using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimatingClass : MonoBehaviour
{
    MainCharacter character;
    [SerializeField] float blendLerpSpeed;
    float targetBlendX;
    float targetBlendY;
    void Start()
    {
        character = transform.parent.GetComponent<MainCharacter>();
        character.animStateSpeed = AnimStateSpeed.idle;
        character.animStatePriDir = AnimStatePriDir.none;
        character.animStateSecDir = AnimStateSecDir.none;
    }

    void Update()
    {
        switch (character.animStateSpeed)
        {
            //case AnimStateSpeed.walk: ManageAnims(false); break;
            //case AnimStateSpeed.run: ManageAnims(true); break;
            case AnimStateSpeed.walk: ManageAnimatorParameters(false); break;
            case AnimStateSpeed.run: ManageAnimatorParameters(true); break;
            default:
                character.blendAnimX = Mathf.Lerp(character.blendAnimX, 0, blendLerpSpeed);
                character.blendAnimY = Mathf.Lerp(character.blendAnimY, 0, blendLerpSpeed);
                break;
        }
        Debug.Log(/*character.blendAnimX + " " + character.blendAnimY + " " + */character.animator.GetFloat("blendVeloX") + " " + character.animator.GetFloat("blendVeloY"));
        character.animator.SetFloat("blendVeloX", character.blendAnimX);
        character.animator.SetFloat("blendVeloY", character.blendAnimY);
    }
    void SnapVaule(float whatTo, float whereTo)
    {
        if(Mathf.Abs(whatTo - whereTo) < 0.01f)
        {
            whatTo = whereTo;
        }
    }
    void ManageAnimatorParameters(bool isRunning)
    {
        if (isRunning)
        {
            switch (character.animStatePriDir)
            {
                case AnimStatePriDir.front:
                    switch (character.animStateSecDir)
                    {
                        case AnimStateSecDir.left: 
                            character.blendAnimX = Mathf.Lerp(character.blendAnimX, -0.7f, blendLerpSpeed);
                            character.blendAnimY = Mathf.Lerp(character.blendAnimY, 0.7f, blendLerpSpeed);
                            break;
                        case AnimStateSecDir.right:
                            character.blendAnimX = Mathf.Lerp(character.blendAnimX, 0.7f, blendLerpSpeed);
                            character.blendAnimY = Mathf.Lerp(character.blendAnimY, 0.7f, blendLerpSpeed);
                            break;
                        default:
                            character.blendAnimX = Mathf.Lerp(character.blendAnimX, 0, blendLerpSpeed);
                            character.blendAnimY = Mathf.Lerp(character.blendAnimY, 1, blendLerpSpeed);
                            break;
                    }
                    break;
                case AnimStatePriDir.back:
                    switch (character.animStateSecDir)
                    {
                        case AnimStateSecDir.left:
                            character.blendAnimX = Mathf.Lerp(character.blendAnimX, -0.7f, blendLerpSpeed);
                            character.blendAnimY = Mathf.Lerp(character.blendAnimY, -0.7f, blendLerpSpeed);
                            break;
                        case AnimStateSecDir.right:
                            character.blendAnimX = Mathf.Lerp(character.blendAnimX, 0.7f, blendLerpSpeed);
                            character.blendAnimY = Mathf.Lerp(character.blendAnimY, -0.7f, blendLerpSpeed);
                            break;
                        default:
                            character.blendAnimX = Mathf.Lerp(character.blendAnimX, 0, blendLerpSpeed);
                            character.blendAnimY = Mathf.Lerp(character.blendAnimY, -1, blendLerpSpeed);
                            break;
                    }
                    break;
                default:
                    switch (character.animStateSecDir)
                    {
                        case AnimStateSecDir.left:
                            character.blendAnimX = Mathf.Lerp(character.blendAnimX, -1, blendLerpSpeed);
                            character.blendAnimY = Mathf.Lerp(character.blendAnimY, 0, blendLerpSpeed);
                            break;
                        case AnimStateSecDir.right:
                            character.blendAnimX = Mathf.Lerp(character.blendAnimX, 1, blendLerpSpeed);
                            character.blendAnimY = Mathf.Lerp(character.blendAnimY, 0, blendLerpSpeed);
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
                            character.blendAnimX = Mathf.Lerp(character.blendAnimX, -0.35f, blendLerpSpeed);
                            character.blendAnimY = Mathf.Lerp(character.blendAnimY, 0.35f, blendLerpSpeed);
                            break;
                        case AnimStateSecDir.right:
                            character.blendAnimX = Mathf.Lerp(character.blendAnimX, 0.35f, blendLerpSpeed);
                            character.blendAnimY = Mathf.Lerp(character.blendAnimY, 0.35f, blendLerpSpeed);
                            break;
                        default:
                            character.blendAnimX = Mathf.Lerp(character.blendAnimX, 0, blendLerpSpeed);
                            character.blendAnimY = Mathf.Lerp(character.blendAnimY, 0.5f, blendLerpSpeed);
                            break;
                    }
                    break;
                case AnimStatePriDir.back:
                    switch (character.animStateSecDir)
                    {
                        case AnimStateSecDir.left:
                            character.blendAnimX = Mathf.Lerp(character.blendAnimX, -0.35f, blendLerpSpeed);
                            character.blendAnimY = Mathf.Lerp(character.blendAnimY, -0.35f, blendLerpSpeed);
                            break;
                        case AnimStateSecDir.right:
                            character.blendAnimX = Mathf.Lerp(character.blendAnimX, 0.35f, blendLerpSpeed);
                            character.blendAnimY = Mathf.Lerp(character.blendAnimY, -0.35f, blendLerpSpeed);
                            break;
                        default:
                            character.blendAnimX = Mathf.Lerp(character.blendAnimX, 0, blendLerpSpeed);
                            character.blendAnimY = Mathf.Lerp(character.blendAnimY, -0.5f, blendLerpSpeed);
                            break;
                    }
                    break;
                default:
                    switch (character.animStateSecDir)
                    {
                        case AnimStateSecDir.left:
                            character.blendAnimX = Mathf.Lerp(character.blendAnimX, -0.5f, blendLerpSpeed);
                            character.blendAnimY = Mathf.Lerp(character.blendAnimY, 0, blendLerpSpeed);
                            break;
                        case AnimStateSecDir.right:
                            character.blendAnimX = Mathf.Lerp(character.blendAnimX, 0.5f, blendLerpSpeed);
                            character.blendAnimY = Mathf.Lerp(character.blendAnimY, 0, blendLerpSpeed);
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