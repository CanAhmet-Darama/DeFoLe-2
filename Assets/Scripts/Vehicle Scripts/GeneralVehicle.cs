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
    public WheelScript[] wheelScripts;
    public float currentRPM;
    public float maxRPM;
    public float gearRatio;

    [Header("Vehicle Effects")]
    public ParticleSystem halfDamagedSmoke;
    public ParticleSystem seriousDamagedSmoke;
    public ParticleSystem deadVehicleSmoke;
    public AudioSource vehicleAudioSource;
    public AudioSource wheelAudioSource;
    public AudioClip deadVehicleSound;
    public AudioSource crashAudioSource;
    public AudioClip crashSound;


    protected void VehicleStart()
    {
        vehicleRb = GetComponent<Rigidbody>();
        vehicleRb.centerOfMass += centerOfMassOffset;
        maxVehicleHealth = vehicleHealth;

        for(int i = axles.Length - 1; i>= 0; i--)
        {
            axles[i].leftCol.GetComponent<WheelScript>().ownerVehicle = this;
            axles[i].rightCol.GetComponent<WheelScript>().ownerVehicle = this;
        }
    }
    protected void VehicleUpdate()
    {
        UpdateWheelMeshes();
        VehicleHealthManage();
        VehicleSoundManage();
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
        if(vehicleHealth < (maxVehicleHealth*2) / 3 && !halfDamagedSmoke.isPlaying)
        {
            halfDamagedSmoke.Play();
            motorPower *= 0.75f;
            maxSpeed *= 0.75f;
            Debug.Log("Vehicle semi damaged");
            Debug.Log("MP : " + motorPower + ", MS : " + maxSpeed);
        }
        else if(vehicleHealth < (maxVehicleHealth / 3) && !seriousDamagedSmoke.isPlaying)
        {
            halfDamagedSmoke.Stop();
            seriousDamagedSmoke.Play();
            motorPower *= 0.75f;
            maxSpeed *= 0.75f;
            Debug.Log("Vehicle serious damaged");
            Debug.Log("MP : " + motorPower + ", MS : " + maxSpeed);
        }
        else if(vehicleHealth <= 0 && !deadVehicleSmoke.isPlaying)
        {
            seriousDamagedSmoke.Stop();
            deadVehicleSmoke.Play();
            motorPower = 0;
            maxSpeed = 0;
            vehicleAudioSource.Stop();
            wheelAudioSource.Stop();
            vehicleAudioSource.PlayOneShot(deadVehicleSound);
            Debug.Log("Vehicle broken");
            Debug.Log("MP : " + motorPower + ", MS : " + maxSpeed);
        }
    }
    void VehicleSoundManage()
    {
        if(GameManager.mainState == PlayerState.inMainCar)
        {
            currentRPM = vehicleRb.velocity.magnitude * 60 / (2 * Mathf.PI * axles[0].leftCol.radius);
            maxRPM = maxSpeed * 60 / (2 * Mathf.PI * axles[0].leftCol.radius);
            gearRatio = currentRPM / maxRPM;

            vehicleAudioSource.volume = 0.25f + gearRatio*0.75f;

            byte hittingWheels = 0;
            for(int i = wheelScripts.Length - 1; i >= 0; i--)
            {
                if (wheelScripts[i].isHittingGround)
                {
                    hittingWheels++;
                }
            }
            wheelAudioSource.volume = hittingWheels * gearRatio;

        }
    }
    public void DamageVehicle(short damage)
    {
        vehicleHealth -= damage;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(vehicleRb.velocity.sqrMagnitude > 25)
        {
            float velocityRate = vehicleRb.velocity.magnitude / maxSpeed;
            if(collision.gameObject == GameManager.mainTerrain ||collision.gameObject.GetComponent<EnvObject>() != null)
            {
                crashAudioSource.transform.position = collision.GetContact(0).point;
                crashAudioSource.PlayOneShot(crashSound,velocityRate);
                EnvObject envObj = collision.gameObject.GetComponent<EnvObject>();
                if (envObj.destroyable)
                {
                    envObj.ReduceObjHealth((short)(velocityRate * 1200));
                }
            }
            else if(collision.gameObject.GetComponent<EnemyScript>() != null)
            {
                ImpactMarkManager.MakeBloodImpactAndSound(crashAudioSource.transform.position, Vector3.up, true);
                EnemyScript.GiveDamage(collision.gameObject.GetComponent<EnemyScript>(), (short)(velocityRate * 1000));
            }
        }
    }
}




[Serializable]
public class Axle{
    public WheelCollider leftCol;
    public WheelCollider rightCol;
    public GameObject leftMesh;
    public GameObject rightMesh;
}

