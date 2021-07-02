using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringIndex:IEquatable<SpringIndex>
{
    public int a;
    public int b;


    public SpringIndex(int a, int b)
    {
        this.a = a;
        this.b = b;
    }

    public  bool Equals(SpringIndex obj)
    {
        if (obj.a == a && obj.b == b) return true;
        
                else return false;
    }

   
}