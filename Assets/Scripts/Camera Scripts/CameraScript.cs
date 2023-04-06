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
    float smoothTimeOnFoot = 0.1f;

    [Header("Sound Stuff")]
    public AudioSource cameraAudioSource;
    public AudioClip[] bulletWhizSounds;
    public SphereCollider cameraRangeCollider;

    [Header("General")]
    Vector3 targetObjectPos;
    float maxDistanceFromTarget;

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
        /* Waiting for durat. Unparenting cam. Making pivot rotation and mouse inputs zero. Lets
        the cam follow the car*/
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
            /* Set cam's parent to free look pivot. If there was a coroutine for making it follow
            the object without free look, stop it */
            AdjustCameraPivotOrFollow(PlayerState.inMainCar, CamState.pivot);
            if (holdNumeratorCar != null)
            {
                StopCoroutine(holdNumeratorCar);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            /* Set timer to make the cam follow the car */
            camCanFollow = false;
            holdNumeratorCar = StartCoroutine(ReadyToCamFollow(freeLookPivotCar, camFollowCooldown));
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            /* Add the mouse position to mouse input values and set the free look pivots rotation
             to it with that */
            mouseX += Input.GetAxis("Mouse X") * GameManager.mouseSensitivity;
            mouseY += Input.GetAxis("Mouse Y") * GameManager.mouseSensitivity;
            freeLookPivotCar.localRotation = Quaternion.Euler(Mathf.Clamp(-mouseY, -15, 45), mouseX, 0);
        }
        else if (camCanFollow)
        {
            transform.position = Vector3.SmoothDamp(transform.position, camCarPointTransform.position + offsetVehicle.magnitude * (transform.position - mainCarTransform.position).normalized, ref velocity, smoothTimeCar);
            transform.LookAt(camCarPoint2Transform.position + new Vector3(0, 0, 0));

            if(transform.position.y < mainCarTransform.position.y - 1)
            {
                transform.position = new Vector3(transform.position.x,(mainCarTransform.position.y - 1),transform.position.z);
            }

        }
        camCarPoint2Transform.position = Vector3.Lerp(camCarPoint2Transform.position, mainCarTransform.position + new Vector3(0, 4.75f, 0), smoothTimeCar * 2);
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
            AdjustCameraPivotOrFollow(PlayerState.onFoot, CamState.pivot);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            AdjustCameraPivotOrFollow(PlayerState.onFoot, CamState.follow);
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            mouseX += Input.GetAxis("Mouse X") * GameManager.mouseSensitivity;
            mouseY -= Input.GetAxis("Mouse Y") * GameManager.mouseSensitivity;
            mouseY = Mathf.Clamp(mouseY, -40, 60);
            freeLookPivotOnFoot.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        }
        else
        {
            UncontrolledCameraFollowChar();
        }
    }


    public void AdjustCameraPivotOrFollow(PlayerState pState,CamState stateCam)
    {
        if(pState == PlayerState.onFoot){

            switch (stateCam)
            {
                case CamState.pivot:
                    freeLookPivotOnFoot.eulerAngles = Vector3.zero;
                    transform.SetParent(freeLookPivotOnFoot);
                    transform.localPosition = offsetCharPivot;
                    transform.eulerAngles = Vector3.zero;
                    break;
                case CamState.follow:
                    camPointOnFoot.eulerAngles = transform.eulerAngles;
                    transform.SetParent(camPointOnFoot);
                    transform.localPosition = offsetCharFollow;
                    transform.localEulerAngles = Vector3.zero;
                    break;
            }
            if (holdNumeratorCar != null)
            {
                StopCoroutine(holdNumeratorCar);
                camCanFollow = true;
            }
            maxDistanceFromTarget = (GameManager.mainChar.position - transform.position).magnitude;
        }
        else if(pState == PlayerState.inMainCar)
        {
            switch (stateCam)
            {
                case CamState.pivot:
                    transform.SetParent(freeLookPivotCar);
                    transform.localPosition = freeOffsetCar;
                    freeLookPivotCar.eulerAngles = transform.eulerAngles;
                    transform.localEulerAngles = Vector3.zero;
                    break;
                case CamState.follow:
                    transform.position = camCarPointTransform.position + offsetVehicle.magnitude * (transform.position - mainCarTransform.position).normalized;
                    transform.LookAt(camCarPoint2Transform.position + new Vector3(0, 0, 0));
                    break;
            }
            maxDistanceFromTarget = (GameManager.mainCar.position - transform.position).magnitude;
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
    void CheckObjectBetweenTarget()
    {
        RaycastHit hitInfo;
        Vector3 extents = new Vector3(0.4f, 0.4f, 0.1f);
        //Vector3 dir = -transform.forward;
        Vector3 dir = transform.position - targetObjectPos;
        bool isHit = Physics.BoxCast(targetObjectPos, extents, dir, out hitInfo, Quaternion.LookRotation(transform.forward), maxDistanceFromTarget, 1);

    }
    void UncontrolledCameraFollowChar()
    {
        mouseX += Input.GetAxis("Mouse X") * GameManager.mouseSensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * GameManager.mouseSensitivity;
        mouseY = Mathf.Clamp(mouseY, -30, 50);
        camPointOnFoot.rotation = Quaternion.Euler(mouseY, mouseX, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            cameraAudioSource.transform.position = other.transform.position;
            byte whizIndex = (byte)Random.Range(0, bulletWhizSounds.Length);
            cameraAudioSource.PlayOneShot(bulletWhizSounds[whizIndex], 0.5f);
        }
    }
}
public enum CamState { pivot, follow}
