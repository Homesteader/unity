using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringForSeek : Steering
{
    
    //需要需找的目标物体
    public GameObject target;
    
    //预期速度
    private Vector3 desiredVelocity;
    
    //获得被操控AI角色，以便查询这个AI角色的最大速度等信息
    private Vehicle m_vehicle;
    
    //最大速度
    private float maxSpeed;
    
    //是否尽在二维平面运动
    private bool isPlanar;
    
    // Start is called before the first frame update
    void Start()
    {
        m_vehicle = GetComponent<Vehicle>();
        maxSpeed = m_vehicle.maxSpeed;
        isPlanar = m_vehicle.isPlaner;
    }
    
    //计算操控向量(操控力)
    public override Vector3 Force()
    {
        //计算预期速度
        desiredVelocity = (target.transform.position - transform.position).normalized * maxSpeed;
        if (isPlanar)
        {
            desiredVelocity.y = 0;
        }

        return (desiredVelocity - m_vehicle.velocity);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
