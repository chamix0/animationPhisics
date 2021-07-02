using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge:IEquatable<Edge>
{
    public int a;
    public int b;
    public int c;
    public bool traccion;

    public Edge(int a, int b, int c, bool traccion)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.traccion = traccion;
    }
    
    public  bool Equals(Edge obj)
    {
        if (obj.a == a && obj.b == b) return true;

        else return false;
    }
}