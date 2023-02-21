using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimManager : MonoBehaviour
{
    CameraScript mainCam;
    MainCharacter mainChar;
    Animator animator;
    [SerializeField]Transform aimTarget;

    bool targetHit;
    float aimCastDistance = 500;
    RaycastHit hitInfo;

    public void Start()
    {
        mainCam = GameManager.mainCam.GetComponent<CameraScript>();
        mainChar = GameManager.mainChar.GetComponent<MainCharacter>();
        animator = mainChar.GetComponent<Animator>();
    }

    void Update()
    {
        AimTargetPositioner();
    }

    void AimTargetPositioner()
    {
        Ray ray = mainCam.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        targetHit = Physics.Raycast(ray, out hitInfo,aimCastDistance, ~0, QueryTriggerInteraction.Ignore);
        if(targetHit && hitInfo.distance > 3) {
            aimTarget.position = Vector3.Lerp(aimTarget.position, hitInfo.point,0.04f);
        }
        else
        {
            aimTarget.localPosition =  Vector3.Lerp(aimTarget.localPosition,new Vector3(0, 0, 15), 0.04f);
        }
    }
}
