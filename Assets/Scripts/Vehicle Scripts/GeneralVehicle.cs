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

    [Header("Health and Other")]
    public short vehicleHealth;
    short maxVehicleHealth;

    [Header("Wheels")]
    public Axle[] axles;

    [Header("Vehicle Effects")]
    public ParticleSystem halfDamagedSmoke;

    protected void VehicleStart()
    {
        vehicleRb = GetComponent<Rigidbody>();
        vehicleRb.centerOfMass += centerOfMassOffset;
        maxVehicleHealth = vehicleHealth;
    }
    protected void VehicleUpdate()
    {
        UpdateWheelMeshes();
        VehicleHealthManage();
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

    public void Brake()
    {
        for (int i = axles.Length; i > 0; i--)
        {
            axles[i - 1].leftCol.brakeTorque = brakePower;
            axles[i - 1].rightCol.brakeTorque = brakePower;
            axles[i - 1].leftCol.motorTorque = 0;
            axles[i - 1].rightCol.motorTorque = 0;
        }
    }
    public void ResetMotorTorque()
    {
        for (int i = axles.Length; i > 0; i--)
        {
            axles[i - 1].leftCol.motorTorque = 0;
            axles[i - 1].rightCol.motorTorque = 0;
        }
    }

    void VehicleHealthManage()
    {
        if(vehicleHealth < maxVehicleHealth*2 / 3 && !halfDamagedSmoke.isPlaying)
        {
            halfDamagedSmoke.Play();
        }
    }
    public void DamageVehicle(short damage)
    {
        vehicleHealth -= damage;
    }
}




[Serializable]
public class Axle{
    public WheelCollider leftCol;
    public WheelCollider rightCol;
    public GameObject leftMesh;
    public GameObject rightMesh;
}

