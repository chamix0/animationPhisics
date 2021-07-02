using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SpringTela : MonoBehaviour
{
    [FormerlySerializedAs("nodeA")] public NodeTela nodeTelaA;
    [FormerlySerializedAs("nodeB")] public NodeTela nodeTelaB;
    public bool traccion;//indica si es un muelle de flexion o de traccion

    private float length;
    private float length0;


    //constructor
    public SpringTela(NodeTela nTelaA, NodeTela nTelaB, bool traccion)
    {
        this.traccion = traccion;
        nodeTelaA = nTelaA;
        nodeTelaB = nTelaB;
        length0 = (nTelaA.getPos() - nTelaB.getPos()).magnitude;
        length = length0;
        //print("SOY :"+ nodeA.getPos()+" "+nodeB.getPos());
    }

    #region metodos

    public void UpdateLength()
    {
        length = (nodeTelaA.getPos() - nodeTelaB.getPos()).magnitude;
    }

    public void computeForces(float stiffnessTraccion, float stiffnessFlexion, float dT, float dF)
    {
        if (traccion)
        {
            //muelle de traccion
            Vector3 u = (nodeTelaA.getPos() - nodeTelaB.getPos()).normalized;
            Vector3 fA = (-stiffnessTraccion * (length - length0) * u);
            //fuerza de amortiguamiento
            Vector3 fDamp = -dT * (nodeTelaA.getVel() - nodeTelaB.getVel());
            
            nodeTelaA.addForce(fA + fDamp);
            nodeTelaB.addForce(-fA - fDamp);
        }
        else
        {
            //muelle de flexion
            Vector3 u = (nodeTelaA.getPos() - nodeTelaB.getPos()).normalized;
            Vector3 fA = -stiffnessFlexion * (length - length0) * u;
            //fuerza de amortiguamiento
            Vector3 fDamp = -dF * (nodeTelaA.getVel() - nodeTelaB.getVel());
            
            nodeTelaA.addForce(fA + fDamp);
            nodeTelaB.addForce(-fA - fDamp);
        }
    }

    #endregion
}