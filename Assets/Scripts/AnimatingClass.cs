using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimatingClass : GeneralCharacter
{
    void Start()
    {
        animStateSpeed = AnimStateSpeed.idle;
        animStatePriDir = AnimStatePriDir.none;
        animStateSecDir = AnimStateSecDir.none;
    }

    void Update()
    {
        switch (animStateSpeed)
        {
            case AnimStateSpeed.walk: ManageAnims(false); break;
            case AnimStateSpeed.run: ManageAnims(true); break;
            default: animator.Play("Skeleton|Idle"); break;
        }
    }
    void ManageAnims(bool isRunning)
    {
        if (isRunning)
        {
            AnimStateDirPirAndMoreChecker(animStatePriDir);
        }
        else
        {
            AnimStateDirPirAndMoreChecker(animStatePriDir);
        }


        void ManageSecAnimState(AnimStateSpeed speedState, AnimStatePriDir primState, AnimStateSecDir seconState)
        {
            switch (speedState)
            {
                case AnimStateSpeed.run:
                    switch (primState)
                    {
                        case AnimStatePriDir.front:
                            switch (seconState)
                            {
                                case AnimStateSecDir.none: animator.Play(""); break;
                                case AnimStateSecDir.left: animator.Play(""); break;
                                case AnimStateSecDir.right: animator.Play(""); break;
                            }
                            break;
                        case AnimStatePriDir.back:
                            switch (seconState)
                            {
                                case AnimStateSecDir.none: animator.Play(""); break;
                                case AnimStateSecDir.left: animator.Play(""); break;
                                case AnimStateSecDir.right: animator.Play(""); break;
                            }
                            break;
                        case AnimStatePriDir.none:
                            switch (seconState)
                            {
                                case AnimStateSecDir.none: animator.Play(""); break;
                                case AnimStateSecDir.left: animator.Play(""); break;
                                case AnimStateSecDir.right: animator.Play(""); break;
                            }
                            break;
                    }
                    break;
                case AnimStateSpeed.walk:
                    switch (primState)
                    {
                        case AnimStatePriDir.front:
                            switch (seconState)
                            {
                                case AnimStateSecDir.none: animator.Play("Skeleton|Walking"); break;
                                case AnimStateSecDir.left: animator.Play(""); break;
                                case AnimStateSecDir.right: animator.Play(""); break;
                            }
                            break;
                        case AnimStatePriDir.back:
                            switch (seconState)
                            {
                                case AnimStateSecDir.none: animator.Play(""); break;
                                case AnimStateSecDir.left: animator.Play(""); break;
                                case AnimStateSecDir.right: animator.Play(""); break;
                            }
                            break;
                        case AnimStatePriDir.none:
                            switch (seconState)
                            {
                                case AnimStateSecDir.none: animator.Play(""); break;
                                case AnimStateSecDir.left: animator.Play(""); break;
                                case AnimStateSecDir.right: animator.Play(""); break;
                            }
                            break;
                    }
                    break;
            }
        }
        void AnimStateDirPirAndMoreChecker(AnimStatePriDir _animStatePriDir)
        {
            switch (_animStatePriDir)
            {
                case AnimStatePriDir.front:
                    switch (animStateSecDir)
                    {
                        case AnimStateSecDir.left: ManageSecAnimState(AnimStateSpeed.run, AnimStatePriDir.front, AnimStateSecDir.left); break;
                        case AnimStateSecDir.right: ManageSecAnimState(AnimStateSpeed.run, AnimStatePriDir.front, AnimStateSecDir.right); break;
                        default: ManageSecAnimState(AnimStateSpeed.run, AnimStatePriDir.front, AnimStateSecDir.none); break;
                    }
                    break;
                case AnimStatePriDir.back:
                    switch (animStateSecDir)
                    {
                        case AnimStateSecDir.left: ManageSecAnimState(AnimStateSpeed.run, AnimStatePriDir.back, AnimStateSecDir.left); break;
                        case AnimStateSecDir.right: ManageSecAnimState(AnimStateSpeed.run, AnimStatePriDir.back, AnimStateSecDir.right); break;
                        default: ManageSecAnimState(AnimStateSpeed.run, AnimStatePriDir.back, AnimStateSecDir.none); break;
                    }
                    break;
                default:
                    switch (animStateSecDir)
                    {
                        case AnimStateSecDir.left: ManageSecAnimState(AnimStateSpeed.run, AnimStatePriDir.none, AnimStateSecDir.left); break;
                        case AnimStateSecDir.right: ManageSecAnimState(AnimStateSpeed.run, AnimStatePriDir.none, AnimStateSecDir.right); break;
                        default: ManageSecAnimState(AnimStateSpeed.run, AnimStatePriDir.none, AnimStateSecDir.none); break;
                    }
                    break;
            }

        }


    }
    public void SetAnimStates(AnimStateSpeed state1, AnimStatePriDir state2, AnimStateSecDir state3)
    {
        SetAnimStateSpeed(state1);
        SetAnimStatePri(state2);
        SetAnimStateSec(state3);

    }
    public void SetAnimStateSpeed(AnimStateSpeed state)
    {
        animStateSpeed = state;
    }
    public void SetAnimStatePri(AnimStatePriDir state)
    {
        animStatePriDir = state;
    }
    public void SetAnimStateSec(AnimStateSecDir state)
    {
        animStateSecDir = state;

    }
}