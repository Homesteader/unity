using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class BadGuy : IComparable<BadGuy>
{
    public string name;
    public int power;
    public BadGuy(string name,int power)
    {
        this.name = name;
        this.power = power;
    }
   
    public int CompareTo(BadGuy other)
    {
        if (other == null)
        {
            return 1;
        }

        return other.power-power;
    }

    public int Summon(int a, int b)
    {
        return a + b;
    }

    public int Del(int a, int b)
    {
        return a - b;
    }
}
