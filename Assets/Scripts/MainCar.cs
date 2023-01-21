using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCar : GeneralVehicle
{

    void Start()
    {
        VehicleStart();
    }


    void FixedUpdate()
    {
        VehicleUpdate();
        Drive();
    }

    void Drive()
    {
        if((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && vehicleRb.velocity.magnitude <= maxSpeed)
        {
            for(int i = axles.Length; i > 0; i--)
            {
                axles[i-1].leftCol.motorTorque = motorPower;
                axles[i-1].rightCol.motorTorque = motorPower;
            }
        }
        else if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && vehicleRb.velocity.magnitude <= maxSpeed)
        {
            for (int i = axles.Length; i > 0; i--)
            {
                axles[i-1].leftCol.motorTorque = -motorPower;
                axles[i-1].rightCol.motorTorque = -motorPower;
            }
        }
        else
        {
            for (int i = axles.Length; i > 0; i--)
            {
                axles[i - 1].leftCol.motorTorque = 0;
                axles[i - 1].rightCol.motorTorque = 0;
            }
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            axles[0].leftCol.steerAngle = -steerAngle;
            axles[0].rightCol.steerAngle = -steerAngle;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            axles[0].leftCol.steerAngle = steerAngle;
            axles[0].rightCol.steerAngle = steerAngle;
        }
        else
        {
               axles[0].leftCol.steerAngle = 0;
               axles[0].rightCol.steerAngle = 0;
        }


        if (Input.GetKey(KeyCode.Space))
        {
            for (int i = axles.Length; i > 0; i--)
            {
                axles[i - 1].leftCol.brakeTorque = brakePower;
                axles[i - 1].rightCol.brakeTorque = brakePower;
                axles[i - 1].leftCol.motorTorque = 0;
                axles[i - 1].rightCol.motorTorque = 0;
            }
        }
        else
        {
            for (int i = axles.Length; i > 0; i--)
            {
                axles[i - 1].leftCol.brakeTorque = 0;
                axles[i - 1].rightCol.brakeTorque = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            gameObject.transform.rotation = new Quaternion(0,gameObject.transform.rotation.y + 90,0,0);
            vehicleRb.velocity = new Vector3(0, 0, 0);
            vehicleRb.angularVelocity = new Vector3(0, 0, 0);
            gameObject.transform.position = gameObject.transform.position + new Vector3(0, 4, 0);
            for (int i = axles.Length; i > 0; i--)
            {
                axles[i - 1].leftCol.motorTorque = 0;
                axles[i - 1].rightCol.motorTorque = 0;
            }
        }
    }

}
