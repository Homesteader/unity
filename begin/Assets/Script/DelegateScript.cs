using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DelegateScript : MonoBehaviour
{
    delegate void MyDelegate(int num);
    
    // Start is called before the first frame update
    void Start()
    {
        MyDelegate myDelegate1 = new MyDelegate(this.PrintNum);
        MyDelegate myDelegate2 = new MyDelegate(this.PrintDoubleNum);
        MyDelegate myDelegate3 = new MyDelegate(this.PrintTripleNum);

        MyDelegate myDelegates = null;
        myDelegates = (MyDelegate)Delegate.Combine(myDelegates, myDelegate1);
        myDelegates = (MyDelegate)Delegate.Combine(myDelegates, myDelegate2);
        myDelegates = (MyDelegate)Delegate.Combine(myDelegates, myDelegate3);
        this.Print(10,myDelegates);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Print(int value, MyDelegate md)
    {
        if (md != null)
        {
            md(value);
        }
        else
        {
            Debug.Log("Print:" + value);
        }
    }
    
    void PrintNum(int num)
    {
        Debug.Log("PrintNum:" + num);
    }

    void PrintDoubleNum(int num)
    {
        Debug.Log("PrintDoubleNum:" + num * 2);
    }

    void PrintTripleNum(int num)
    {
        Debug.Log("PrintDoubleNum:" + num * 3);
    }
}
