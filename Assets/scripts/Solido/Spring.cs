using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    public Node nodeA;
    public Node nodeB;

    private float length;
    private float length0;


    //constructor
    public Spring(Node nA, Node nB)
    {
        nodeA = nA;
        nodeB = nB;
        length0 = (nA.getPos() - nB.getPos()).magnitude;
        length = length0;
    }

    #region metodos

    public void UpdateLength()
    {
        length = (nodeA.getPos() - nodeB.getPos()).magnitude;
    }

    public void computeForces(float k, float dT, float vol)
    {
        //muelle de traccion
        Vector3 u = (nodeA.getPos() - nodeB.getPos()).normalized;
        // Vector3 fA = (-k * (length - length0) * u);
        float K = (vol / (length0 * length0)) * k;
        Vector3 fA = -K * (length - length0) * u;

        //fuerza de amortiguamiento
        fA -= dT * K * Vector3.Dot(u, (nodeA.getVel() - nodeB.getVel())) * u;

        nodeA.addForce(fA);
        nodeB.addForce(-fA);
    }

    #endregion
}