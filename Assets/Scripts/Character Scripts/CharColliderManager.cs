using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CharColliderManager : MonoBehaviour
{
    [Header("Holder Objects")]
    public GameObject _head;
    public GameObject _helmet;
    public GameObject _upperBody;
    public GameObject _lowerBody;
    public GameObject _leftShoulder;
    public GameObject _rightShoulder;
    public GameObject _leftArm;
    public GameObject _rightArm;
    public GameObject _leftHip;
    public GameObject _rightHip;
    public GameObject _leftLeg;
    public GameObject _rightLeg;

    [Header("Bones")]
    public GameObject head;
    public GameObject helmet;
    public GameObject upperBody;
    public GameObject lowerBody;
    public GameObject leftShoulder;
    public GameObject rightShoulder;
    public GameObject leftArm;
    public GameObject rightArm;
    public GameObject leftHip;
    public GameObject rightHip;
    public GameObject leftLeg;
    public GameObject rightLeg;

    [Header("Arrays")]
    GameObject[] holders;
    GameObject[] bones;

    void Awake()
    {
        GameObject[] _holders = {_head,_helmet, _upperBody, _lowerBody, _leftShoulder, _rightShoulder, _leftArm, _rightArm, _leftHip, _rightHip, _leftLeg, _rightLeg};
        holders = _holders;
        GameObject[] _bones = { head, helmet, upperBody, lowerBody, leftShoulder, rightShoulder, leftArm, rightArm, leftHip, rightHip, leftLeg, rightLeg };
        bones = _bones;
    }

    void Update()
    {
        MatchColliderAndMesh();
    }
    void MatchColliderAndMesh()
    {
        for(short i = (short)(holders.Length - 1); i >= 0; i--)
        {
            holders[i].transform.position = bones[i].transform.position;
            holders[i].transform.rotation = bones[i].transform.rotation;
            if (holders[i] == _leftHip || holders[i] == _rightHip || holders[i] == _leftLeg || holders[i] == _rightLeg)
            {
                holders[i].transform.Rotate(new Vector3(90, 0, 0), Space.Self);
            }
            else if (holders[i] == _head || holders[i] == _helmet || holders[i] == _upperBody || holders[i] == _lowerBody)
            {
                holders[i].transform.Rotate(new Vector3(-90, 0, 0), Space.Self);
                //holders[i].transform.position -= holders[i].transform.forward * 0.05f;

                if(holders[i] == _helmet) holders[i].transform.position -= holders[i].transform.forward * 0.11f;
                else if(holders[i] == _helmet) holders[i].transform.position -= holders[i].transform.forward * 0.05f;
                else if(holders[i] == _upperBody) holders[i].transform.position += holders[i].transform.forward * 0.1f;
            }
        }
    }
}
