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
    public float maxSpeed;
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
        vehicleRb.centerOfMass += centerOfMassOffset;
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
            SetWheelPosRot(axles[i - 1].leftCol, axles[i - 1].leftMesh.transform);
            SetWheelPosRot(axles[i - 1].rightCol, axles[i - 1].rightMesh.transform);
        }
    }
    void SetWheelPosRot(WheelCollider wheelCol, Transform wheelMesh)
    {
        Vector3 position;
        Quaternion rotation;
        wheelCol.GetWorldPose(out position, out rotation);
        wheelMesh.position = position;
        wheelMesh.rotation = rotation;
    }

}




[Serializable]
public class Axle{
    public WheelCollider leftCol;
    public WheelCollider rightCol;
    public GameObject leftMesh;
    public GameObject rightMesh;
}

