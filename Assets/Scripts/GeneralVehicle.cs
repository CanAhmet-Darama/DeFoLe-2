using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralVehicle : MonoBehaviour
{
    [Header("General")]
    public Rigidbody vehicleRb;
    public Vector3 centerOfMassOffset;

    [Header("Driving")]
    public float motorPower;
    public float steerAngle;
    public float steerSpeed;
    public float brakePower;
    float rotateOffset = -90;

    [Header("Health and Other")]
    public short vehicleHealth;

    [Header("Wheels")]
    public Axle[] axles;

    protected void VehicleStart()
    {
        vehicleRb = GetComponent<Rigidbody>();
        vehicleRb.centerOfMass = centerOfMassOffset;
        Debug.Log("VehicleStart works");
    }
    protected void VehicleUpdate()
    {
        UpdateWheelMeshes();
        Debug.Log("VehicleUpdate works");
        Debug.Log(vehicleRb.centerOfMass);
    }

    protected void UpdateWheelMeshes()
    {
        for (int i = axles.Length; i > 0; i--)
        {
            axles[i - 1].leftMesh.transform.rotation = OffsetQuaternion(axles[i - 1].leftCol.transform.rotation, rotateOffset);
            axles[i - 1].rightMesh.transform.rotation = OffsetQuaternion(axles[i - 1].rightCol.transform.rotation, rotateOffset);

        }
    }
    Quaternion OffsetQuaternion(Quaternion wheelQua, float yOffset)
    {
        return new Quaternion(wheelQua.x, wheelQua.y + yOffset, wheelQua.z, wheelQua.w);
    }

}




[Serializable]
public class Axle{
    public WheelCollider leftCol;
    public WheelCollider rightCol;
    public GameObject leftMesh;
    public GameObject rightMesh;
}

