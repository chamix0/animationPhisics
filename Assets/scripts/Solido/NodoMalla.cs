using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodoMalla : MonoBehaviour
{
    #region variables

    private Vector3 pos;
    public float[] w;
    public bool skinned; //significa que ha sido insertado en algun tetraedro

    #endregion

    #region constructores

    public NodoMalla(Vector3 posN)
    {
        pos = posN;
        skinned = false;
        w = new float[4];
    }

    private void Update()
    {
        transform.position = pos;
    }

    #endregion

    #region metodos

    public float getTotalWeight()
    {
        float sum = 0;
        for (int i = 0; i < 4; i++)
        {
            sum += w[i];
        }

        return sum;
    }

    public void setPos(Vector3 newPos)
    {
        pos = newPos;
    }

    public Vector3 getPos()
    {
        return pos;
    }

    public override string ToString()
    {
        String cad = pos.ToString();
        return cad;
    }

    #endregion
}