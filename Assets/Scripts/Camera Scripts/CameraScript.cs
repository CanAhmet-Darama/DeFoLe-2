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
    MainCharacter mainCharScr;
    float smoothTimeOnFoot = 0.1f;

    [Header("Sound Stuff")]
    public AudioSource cameraAudioSource;
    public AudioClip[] bulletWhizSounds;
    public SphereCollider cameraRangeCollider;

    [Header("General")]
    public Camera CamScript;
    Vector3 targetObjectPos;
    PlayerState camPlayerState;
    public CamState camOwnState = CamState.follow;
    bool setToDefaultAlready;
    float maxCastDistance;
    public float defaultFOV;

    #region Mouse Inputs
    float mouseX;
    float mouseY;
    public static bool mouseMoved;
    Vector2 lastMousePos;
    static public float scrollInput;
    #endregion

    void Start()
    {
        freeLookPivotCar.localPosition = offsetVehicle;
        mainCharScr = mainChar.GetComponent<MainCharacter>();
        mainCharScr.charMoving = false;
        CamScript = GetComponent<Camera>();
        defaultFOV = CamScript.fieldOfView;

        if(GameManager.mainState == PlayerState.onFoot)
        {

        }
    }

    void Update()
    {
        DetectMouseMotion();
        LevelOfDetailManager.camForward2D = new Vector2(transform.forward.x, transform.forward.z);
    }
    void LateUpdate()
    {
        if(!GameManager.isGamePaused)
        {
            if(camOwnState != CamState.zoomScope)
            {
                if(GameManager.mainState == PlayerState.onFoot)
                {
                    CamFollowMainCharacter();
                    targetObjectPos = mainChar.position;
                    CheckObjectBetweenTarget();
                }
                else if (GameManager.mainState == PlayerState.inMainCar)
                {
                    CamFollowMainCar();
                    targetObjectPos = mainCarTransform.position;
                    CheckObjectBetweenTarget();
                }
            }
            else
            {
                CamMovementOnSniperZoom();
            }
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
            transform.LookAt(camCarPoint2Transform.position + new Vector3(0, -1, 0));

            if(transform.position.y < mainCarTransform.position.y - 1)
            {
                transform.position = new Vector3(transform.position.x,(mainCarTransform.position.y - 1),transform.position.z);
            }

        }
        camCarPoint2Transform.position = Vector3.Lerp(camCarPoint2Transform.position, mainCarTransform.position + new Vector3(0, 4.75f, 0), smoothTimeCar * 2);
    }
    void CamFollowMainCharacter()
    {
        Vector3 crouchOrNotOffset = Vector3.zero;
        if (mainCharScr.isCrouching)
        {
            crouchOrNotOffset = new(0, -0.75f, 0);
        }
        camPointOnFoot.position = Vector3.SmoothDamp(camPointOnFoot.transform.position, mainChar.position + crouchOrNotOffset, ref velocity, smoothTimeOnFoot);
        
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
    void CamMovementOnSniperZoom()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            mouseX += Input.GetAxis("Mouse X") * (GameManager.mouseSensitivity /4);
            mouseY -= Input.GetAxis("Mouse Y") * (GameManager.mouseSensitivity /4);
            mouseY = Mathf.Clamp(mouseY, -40, 60);
            freeLookPivotOnFoot.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        }
    }


    public void AdjustCameraPivotOrFollow(PlayerState pState,CamState stateCam)
    {
        camPlayerState = pState;
        camOwnState = stateCam;
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
                case CamState.zoomScope:

                    break;
            }
            if (holdNumeratorCar != null)
            {
                StopCoroutine(holdNumeratorCar);
                camCanFollow = true;
            }
            maxCastDistance = Mathf.Clamp((mainChar.position - transform.position).magnitude,0, 5);
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
                    transform.LookAt(camCarPoint2Transform.position);
                    break;
            }
            maxCastDistance = Mathf.Clamp((mainChar.position - transform.position).magnitude, 0, 15);
        }
    }

    void CheckObjectBetweenTarget()
    {
        RaycastHit hitInfo;
        Ray rayForCam = new Ray(targetObjectPos, (transform.position- targetObjectPos).normalized);
        bool isHit = Physics.Raycast(rayForCam,out hitInfo, maxCastDistance, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore);

        if (isHit && !(hitInfo.collider.CompareTag("Vehicle") && GameManager.mainState == PlayerState.inMainCar))
        {
            transform.position = targetObjectPos + (rayForCam.direction * (hitInfo.distance - 0.01f));
            setToDefaultAlready = false;
        }
        else if (!setToDefaultAlready)
        {
            setToDefaultAlready = true;
            CheckDefaultLocalPos();
        }

    }
    void CheckDefaultLocalPos()
    {
        if (camPlayerState == PlayerState.onFoot)
        {
            if (camOwnState == CamState.pivot)
            {
                transform.localPosition = offsetCharPivot;
            }
            else if(camOwnState == CamState.follow)
            {
                transform.localPosition = offsetCharFollow;
            }
        }
        else if (camPlayerState == PlayerState.inMainCar)
        {
            if (camOwnState == CamState.pivot)
            {
                transform.localPosition = freeOffsetCar;
            }
            else if (camOwnState == CamState.follow)
            {
                transform.position = camCarPointTransform.position + offsetVehicle.magnitude * (transform.position - mainCarTransform.position).normalized;
            }
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

        scrollInput = Input.GetAxis("Mouse ScrollWheel");
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
public enum CamState { pivot, follow, zoomScope}
