using System;
using System.Collections;
using System.Collections.Generic;
using Source.Scripts;
using UnityEngine;

public class Node : MonoBehaviour
{
    #region variables

    private Vector3 pos;
    private Vector3 force;
    public Vector3 windForce;
    private Vector3 vel;
    public Vector3 fixerLocalPos;
    public float mass;
    public int numTetra;
    public float[] w;
    public bool inCollision;
    public bool isMovable;
    public bool skinned; //significa que ha sido insertado en algun tetraedro

    #endregion

    private void Update()
    {
        transform.position = pos;
    }


    #region constructores

    public Node(Vector3 posN, bool M)
    {
        numTetra = 0;
        mass = 0;
        isMovable = M;
        pos = posN;
        force = Vector3.zero;
        vel = Vector3.zero;
        inCollision = false;
        windForce = Vector3.zero;
        skinned = false;
        w = new float[4];
    }

    public Node(Vector3 posN)
    {
        numTetra = 0;
        mass = 0;
        isMovable = true;
        pos = posN;
        force = Vector3.zero;
        vel = Vector3.zero;
        inCollision = false;
        windForce = Vector3.zero;
        skinned = false;
        w = new float[4];
    }

    #endregion

    #region metodos

    #region getters y setters

    public void setPos(Vector3 newPos)
    {
        pos = newPos;
    }

    public Vector3 getPos()
    {
        return pos;
    }

    public void setForce(Vector3 newFor)
    {
        force = newFor;
    }
    public void setWindForce(Vector3 newFor)
    {
        windForce = newFor;
    }

    public void addForce(Vector3 newFor)
    {
        force += newFor;
    }

    public Vector3 getForce()
    {
        return force;
    }

    public void addVel(Vector3 newVel)
    {
        vel += newVel;
    }

    public void setVel(Vector3 newVel)
    {
        vel = newVel;
    }

    public Vector3 getVel()
    {
        return vel;
    }

    #endregion

    public void massAverage()
    {
        mass = mass / numTetra;
    }

    //comprobacion de colisiones con cada esfera
    public void computeCollisions(List<Plane> planes, float k)
    {
        inCollision = false;
        foreach (Plane p in planes)
        {
            Vector3 closestPoint = p.ClosestPointOnPlane(getPos());
            Vector3 normal = p.normal;

            float pen = Vector3.Dot(normal,
                getPos() - p.ClosestPointOnPlane(getPos())); //123            if (pen < 0.5f)
            if (pen <= 0)
            {
                inCollision = true;
                //Vector3 calc = -k * normal / pen;
                Vector3 calc = -k * pen * normal;
                force += calc;
            }
        }
    }

    public void computeforces(Vector3 Gravity, float d)
    {
        Vector3 f = mass * Gravity;
        Vector3 fD = -d * mass * vel; //amortiguamiento
        force += f + fD + windForce;
    }

    public override string ToString()
    {
        String cad = pos.ToString();
        return cad;
    }

    #endregion
}