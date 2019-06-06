using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringForFlee : Steering
{

    public GameObject target;
    
    public float fearDistance = 20;
    
    private Vector3 desireVelocity;

    private Vehicle m_vehicle;

    private float maxSpeed;
    // Start is called before the first frame update
    void Start()
    {
        m_vehicle = GetComponent<Vehicle>();
        maxSpeed = m_vehicle.maxSpeed;
    }

    public override Vector3 Force()
    {
        Vector3 tmpPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 tmpTargetPos = new Vector3(target.transform.position.x, 0, target.transform.position.z);

        if (Vector3.Distance(tmpPos, tmpTargetPos) > fearDistance)
        {
            return new Vector3(0, 0, 0);
        }

        desireVelocity = (transform.position - target.transform.position).normalized * maxSpeed;
        return (desireVelocity - m_vehicle.velocity);
    }
}
