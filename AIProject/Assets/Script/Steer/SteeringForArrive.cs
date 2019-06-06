using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringForArrive : Steering
{
    public bool isPlanar = true;

    public float arrivalDistance = 0.3f;

    public float characterRaius = 1.2f;
    
    //当与目标小于这个距离时，开始减速
    public float slowDownDistance;

    public GameObject target;

    private Vector3 desiredVelocity;

    private Vehicle m_vehicle;

    private float maxSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        m_vehicle = GetComponent<Vehicle>();
        maxSpeed = m_vehicle.maxSpeed;
        isPlanar = m_vehicle.isPlaner;
    }

    public override Vector3 Force()
    {
        
        //计算和目标之间的距离
        Vector3 toTarget = target.transform.position - transform.position;
        
        //预期速度
        Vector3 desiredVelocity;
        
        //返回的操控向量
        Vector3 returnForce;
        if (isPlanar)
        {
            toTarget.y = 0;
        }

        float distance = toTarget.magnitude;
        if (distance > slowDownDistance)
        {
            desiredVelocity = toTarget.normalized * maxSpeed;
            returnForce = desiredVelocity - m_vehicle.velocity;
        }
        else
        {
            desiredVelocity = toTarget - m_vehicle.velocity;
            returnForce = desiredVelocity - m_vehicle.velocity;
        }

        return returnForce;

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(target.transform.position, slowDownDistance);
    }
}
