using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Vehicle : MonoBehaviour
{
    //此AI对象包括的操作行为对象
    private Steering[] steerings;
    
    //设置AI角色能够达到的最大速度
    public float maxSpeed = 10;
    
    //设置对次对象能够加的力的最大值
    public float maxForce = 100;
    
    //最大速度的平方
    protected float sqrMaxSpeed;
    
    //AI角色的质量
    public float mass = 1;
    
    //AI角色的速度
    public Vector3 velocity;
    
    //控制转向的速度
    public float damping = 0.9f;
    
    //控制力的时间间隔
    public float computeInverval = 0.2f;
    
    //是否在二维平面上，用于计算距离
    public bool isPlaner = true;
    
    //计算得到的控制力
    private Vector3 steeringForce;
    
    //加速度
    protected Vector3 acceleration;
    
    //计时器
    private float timer;
    
    // Start is called before the first frame update
    protected void Start()
    {
        steeringForce = new Vector3(0, 0, 0);
        sqrMaxSpeed = maxSpeed * maxSpeed;
        timer = 0;
        steerings = GetComponents<Steering>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        steeringForce = new Vector3(0, 0, 0);
        if (timer > computeInverval)
        {
            //将操控行为列表中的所有操控行为对应的操控力进行带权重的求和
            foreach (Steering s in steerings)
            {
                if (s.enabled)
                {
                    steeringForce += s.Force() * s.weight;
                }
            }
            
            //控制力不大于maxForce
            steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);
            
            //求出加速度
            acceleration = steeringForce / mass;
            timer = 0;
        }
    }
  
}
