using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("For Vehicle")]
    [SerializeField] Vector3 offsetVehicle;
    [SerializeField] Transform mainCarTransform;
    [SerializeField] Transform camCarPointTransform;
    [SerializeField] Transform camCarPoint2Transform;
    [SerializeField] Transform freeLookPivotCar;
    float smoothTime = 0.25f;
    float camRotSpeed = 0.5f;
    float camFollowCooldown = 1.5f;
    bool camCanFollow = true;
    Vector3 velocity = Vector3.zero;
    Vector3 behindPoint;
    [Header("For On Foot")]
    [SerializeField] Transform mainChar;
    [SerializeField] Transform freeLookPivotOnFoot;
    [SerializeField] Vector3 offsetChar;


    #region Mouse Inputs
    float mouseX;
    float mouseY;
    #endregion

    void Start()
    {
        
    }

    void Update()
    {
    }
    void LateUpdate()
    {
        if(GameManager.mainState == PlayerState.onFoot)
        {
            CamFollowMainCharacter();
        }
        else if (GameManager.mainState == PlayerState.inMainCar)
        {
            CamFollowMainCar();
        }
    }
    IEnumerator ReadyToCamFollow(Transform pivot, float durat)
    {
        yield return new WaitForSeconds(durat);
        pivot.DetachChildren();
        pivot.localRotation = Quaternion.Euler(0, 0, 0);
        camCanFollow = true;
    }
    void CamFollowMainCar()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            StopCoroutine(ReadyToCamFollow(freeLookPivotCar,1.5f));
            gameObject.transform.SetParent(freeLookPivotCar);
            camCanFollow = false;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            mouseX = 0;
            mouseY = 0;
            camCanFollow = false;
            StartCoroutine(ReadyToCamFollow(freeLookPivotCar,1.5f));
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            mouseX += Input.GetAxis("Mouse X") * GameManager.mouseSensitivity;
            mouseY += Input.GetAxis("Mouse Y") * GameManager.mouseSensitivity;
            freeLookPivotCar.localRotation = Quaternion.Euler(Mathf.Clamp(-mouseY, -35, 45), mouseX, 0);
            if ((freeLookPivotCar.position - transform.position).magnitude > 6.034f)
            {
                transform.Translate((freeLookPivotCar.position - transform.position).normalized * Time.deltaTime);
            }
        }
        else if (camCanFollow)
        {
            if ((mainCarTransform.position - camCarPointTransform.position).magnitude < (transform.position - mainCarTransform.position).magnitude)
            {
                transform.position = Vector3.SmoothDamp(transform.position, camCarPointTransform.position + offsetVehicle.magnitude * (transform.position - mainCarTransform.position).normalized, ref velocity, smoothTime);
                camCarPoint2Transform.position = Vector3.Lerp(camCarPoint2Transform.position, mainCarTransform.position + new Vector3(0, 4.5f, 0), smoothTime * 2);
                transform.LookAt(camCarPoint2Transform.position + new Vector3(0, 0, 0));
            }
        }

    }
    void CamFollowMainCharacter()
    {

    }
}
