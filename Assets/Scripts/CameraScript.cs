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
    Coroutine holdNumeratorCar;

    [Header("For On Foot")]
    [SerializeField] Transform mainChar;
    public Transform freeLookPivotOnFoot;
    [SerializeField] Transform camPointOnFoot;
    [SerializeField] Vector3 offsetCharPivot;
    [SerializeField] Vector3 offsetCharFollow;
    float smoothTimeOnFoot = 0.15f;


    #region Mouse Inputs
    float mouseX;
    float mouseY;
    public static bool mouseMoved;
    Vector2 lastMousePos;
    #endregion

    void Start()
    {
        freeLookPivotCar.localPosition = offsetVehicle;
        mainChar.GetComponent<MainCharacter>().charMoving = false;
        if(GameManager.mainState == PlayerState.onFoot)
        {

        }
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
        if(Mathf.Abs((camPointOnFoot.eulerAngles.y - mainChar.eulerAngles.y)) < 180 && !mouseMoved)
        {
            camPointOnFoot.eulerAngles = Vector3.Lerp(camPointOnFoot.eulerAngles, mainChar.eulerAngles, 0.2f);
        }
        else
        {
            camPointOnFoot.eulerAngles = mainChar.eulerAngles;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            AdjustCameraPivot(CamState.pivot);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            AdjustCameraPivot(CamState.follow);
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            mouseX += Input.GetAxis("Mouse X") * GameManager.mouseSensitivity;
            mouseY -= Input.GetAxis("Mouse Y") * GameManager.mouseSensitivity;
            mouseY = Mathf.Clamp(mouseY, -30, 50);
            freeLookPivotOnFoot.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        }
        else
        {
            UncontrolledCameraFollowChar();
        }
    }
    public void AdjustCameraPivot()
    {
        if (GameManager.mainState == PlayerState.onFoot)
        {
            AdjustCameraPivot(CamState.follow);
        }
        else if (GameManager.mainState == PlayerState.inMainCar)
        {
             freeLookPivotCar.eulerAngles = transform.eulerAngles;
             transform.SetParent(freeLookPivotCar);
             transform.localEulerAngles = new Vector3(0, 0, 0);
             transform.localPosition = freeOffsetCar;
        }
    }
    public void AdjustCameraPivot(CamState stateCam)
    {
        //mouseX = 0; mouseY = 0;
        switch (stateCam)
        {
            case CamState.pivot:
                transform.SetParent(freeLookPivotOnFoot);
                transform.localPosition = offsetCharPivot;
                freeLookPivotOnFoot.eulerAngles = transform.eulerAngles;
                transform.localEulerAngles = Vector3.zero;
                break;
            case CamState.follow:
                camPointOnFoot.eulerAngles = transform.eulerAngles;
                transform.SetParent(camPointOnFoot);
                transform.localPosition = offsetCharFollow;
                break;
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
    void UncontrolledCameraFollowChar()
    {
        mouseX += Input.GetAxis("Mouse X") * GameManager.mouseSensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * GameManager.mouseSensitivity;
        mouseY = Mathf.Clamp(mouseY, -30, 50);
        camPointOnFoot.rotation = Quaternion.Euler(mouseY, mouseX, 0);
    }
}
public enum CamState { pivot, follow}
