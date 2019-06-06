using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILocomotion : Vehicle
{
    
    //AI角色的控制器
    private CharacterController controller;
    
    //AI角色的Rigidbody
    private Rigidbody theRigidbody;
    
    //AI角色每次移动距离
    private Vector3 moveDistance;
    
    // Start is called before the first frame update
    void Start()
    {
        //获得角色控制器
        controller = GetComponent<CharacterController>();
        
        //获得AI角色的Rigidbody(如果有的话)
        theRigidbody = GetComponent<Rigidbody>();
        
        moveDistance = new Vector3(0, 0, 0);
        
        //调用基类的Start()函数，进行所需的初始化
        base.Start();
    }

    private void FixedUpdate()
    {
        //计算速度
        velocity += acceleration * Time.fixedDeltaTime;
        
        //限制速度，要低于最大速度
        if (velocity.sqrMagnitude > sqrMaxSpeed)
            velocity = velocity.normalized * maxSpeed;
        
        //计算AI角色的移动距离
        moveDistance = velocity * Time.fixedDeltaTime;
        
        //如果要求AI角色在平面上移动，那么将y设置为0
        if (isPlaner)
        {
            velocity.y = 0;
            moveDistance.y = 0;
        }
        
        //如果已经为AI角色添加了角色控制器，那么利用角色控制器使其移动
        if (controller != null)
        {
            controller.SimpleMove(velocity);
        }
        //如果AI角色没有角色控制器和Rigidbody
        //或AI角色用Rigidbody，但是要由动力学方式控制它的运动
        else if (theRigidbody == null || theRigidbody.isKinematic)
            transform.position += moveDistance;
        //用Rigidbody控制AI角色的运动
        else
        {
            theRigidbody.MovePosition(theRigidbody.position + moveDistance);
        }
        
        //更新朝向，如果速度大于一个阀值(为了防止抖动)
        if (velocity.sqrMagnitude > 0.00001)
        {
            //通过当前朝向与速度方向的插值，计算新的朝向
            Vector3 newForward = Vector3.Slerp(transform.forward, velocity, damping * Time.deltaTime);
            if (isPlaner)
                newForward.y = 0;
            transform.forward = newForward;
        }

        //gameObject.GetComponent<Animation>().Play("walk");
    }
}
