using System;
using System.Collections;
using System.Collections.Generic;
using Source.Scripts;
using UnityEngine;

public class NodeTela : MonoBehaviour
{
    #region varables

    private Vector3 pos;
    private Vector3 force;
    public Vector3 windForce;
    private Vector3 vel;
    public Vector3 fixerLocalPos;
    public bool inCollision;
    public bool isMovable;

    #endregion

    private void Update()
    {
        transform.position = pos;
    }

    #region constructores

    public NodeTela(Vector3 posN, bool M)
    {
        fixerLocalPos = Vector3.zero;
        isMovable = M;
        pos = posN;
        force = Vector3.zero;
        vel = Vector3.zero;
        inCollision = false;
        windForce = Vector3.zero;
    }


    public NodeTela(Vector3 posN)
    {
        isMovable = true;
        pos = posN;
        force = Vector3.zero;
        vel = Vector3.zero;
        inCollision = false;
        windForce = Vector3.zero;
    }

    #endregion

    #region metodos

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

    //comprobacion de colisiones con cada esfera
    public void computeCollisions(List<Sphere> spheres, float k)
    {
        inCollision = false;
        foreach (Sphere s in spheres)
        {
            Vector3 closestPoint = s._sphereCollider.ClosestPoint(getPos());
            float pen = (pos - closestPoint).magnitude;
            if (pen < 0.5f)
            {
                inCollision = true;
                Vector3 normal = (pos - s.center).normalized;
                Vector3 calc = k * normal / pen;
                if (!float.IsInfinity(calc.magnitude))
                {
                    //n.computeforces(mass, calc, Vector3.zero, 0);
                    addForce(calc);
                }
            }
        }
    }

    public void computeforces(float mass, Vector3 Gravity, Vector3 wind, float d)
    {
        Vector3 f = mass * Gravity;
        Vector3 fD = -d * mass * vel; //amortiguamiento
        force += f + fD + wind;
    }

    #endregion
}