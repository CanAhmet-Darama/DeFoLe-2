using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelScript : MonoBehaviour
{
    public GeneralVehicle ownerVehicle;
    public WheelCollider wheelCol;
    public bool isHittingGround;
    WheelHit wheelHit;

    void Update()
    {
        ManageWheelSounds();
    }

    void ManageWheelSounds()
    {
        isHittingGround = wheelCol.GetGroundHit(out wheelHit);
    }
}
