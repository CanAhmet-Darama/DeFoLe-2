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
    [SerializeField] Vector3 freeOffsetCar;
    float smoothTimeCar = 0.2f;
    float camFollowCooldown = 1.5f;
    bool camCanFollow = true;
    Vector3 velocity = Vector3.zero;
    float carPivotStartingDistance;
    Coroutine holdNumeratorCar;

    [Header("For On Foot")]
    [SerializeField] Transform mainChar;
    public Transform freeLookPivotOnFoot;
    [SerializeField] Transform camPointOnFoot;
    [SerializeField] Vector3 offsetChar;
    float smoothTimeOnFoot = 0.15f;


    #region Mouse Inputs
    float mouseX;
    float mouseY;
    public static bool mouseMoved;
    Vector2 lastMousePos;
    #endregion

    void Start()
    {
        GameManager.mainCam = transform;
        freeLookPivotCar.localPosition = offsetVehicle;
        carPivotStartingDistance = (freeLookPivotCar.position - transform.position).magnitude;
        mainChar.GetComponent<MainCharacter>().charMoving = false;
    }

    void Update()
    {
        DetectMouseMotion();
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
        if (!Input.GetKey(KeyCode.Mouse1))
        {
            transform.SetParent(null);
            pivot.localRotation = Quaternion.Euler(0, 0, 0);
            camCanFollow = true;
            mouseX = 0;
            mouseY = 0;
        }
    }
    void CamFollowMainCar()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            AdjustCameraPivot();
            if (holdNumeratorCar != null)
            {
                StopCoroutine(holdNumeratorCar);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            camCanFollow = false;
            holdNumeratorCar = StartCoroutine(ReadyToCamFollow(freeLookPivotCar, camFollowCooldown));
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            mouseX += Input.GetAxis("Mouse X") * GameManager.mouseSensitivity;
            mouseY += Input.GetAxis("Mouse Y") * GameManager.mouseSensitivity;
            freeLookPivotCar.localRotation = Quaternion.Euler(Mathf.Clamp(-mouseY, -15, 45), mouseX, 0);
        }
        else if (camCanFollow)
        {
            if ((mainCarTransform.position - camCarPointTransform.position).magnitude < (transform.position - mainCarTransform.position).magnitude)
            {
                transform.position = Vector3.SmoothDamp(transform.position, camCarPointTransform.position + offsetVehicle.magnitude * (transform.position - mainCarTransform.position).normalized, ref velocity, smoothTimeCar);
                transform.LookAt(camCarPoint2Transform.position + new Vector3(0, 0, 0));
            }

            if(transform.position.y < mainCarTransform.position.y - 1)
            {
                transform.position = new Vector3(transform.position.x,(mainCarTransform.position.y - 1),transform.position.z);
            }

        }
        camCarPoint2Transform.position = Vector3.Lerp(camCarPoint2Transform.position, mainCarTransform.position + new Vector3(0, 4.5f, 0), smoothTimeCar * 2);
    }
    void CamFollowMainCharacter()
    {
        camPointOnFoot.position = Vector3.SmoothDamp(camPointOnFoot.transform.position, mainChar.position, ref velocity, smoothTimeOnFoot);
        camPointOnFoot.eulerAngles = Vector3.Lerp(camPointOnFoot.eulerAngles, mainChar.eulerAngles, 0.2f);
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            AdjustCameraPivot();
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            mouseX += Input.GetAxis("Mouse X") * GameManager.mouseSensitivity;
            mouseY -= Input.GetAxis("Mouse Y") * GameManager.mouseSensitivity;
            freeLookPivotOnFoot.rotation = Quaternion.Euler(Mathf.Clamp(mouseY, -30, 50), mouseX, 0);
        }
    }
    public void AdjustCameraPivot()
    {
        if (GameManager.mainState == PlayerState.onFoot)
        {
            mouseX = 0; mouseY = 0;
            transform.SetParent(freeLookPivotOnFoot);
            transform.localPosition = offsetChar;
            freeLookPivotOnFoot.eulerAngles = transform.eulerAngles;
            transform.localEulerAngles = Vector3.zero;
        }
        else if (GameManager.mainState == PlayerState.inMainCar)
        {
             freeLookPivotCar.eulerAngles = transform.eulerAngles;
             transform.SetParent(freeLookPivotCar);
             transform.localEulerAngles = new Vector3(0, 0, 0);
             transform.localPosition = freeOffsetCar;
        }
    }
    public void DetectMouseMotion()
    {
        if(lastMousePos != new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")))
        {
            mouseMoved = true;
        }
        else
        {
            mouseMoved = false;
        }
        lastMousePos = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }
}
