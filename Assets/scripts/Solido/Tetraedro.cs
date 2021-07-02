using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

public class Tetraedro : MonoBehaviour
{
    #region variables

    //variables
    private Node[] _nodes; //deben ser 4
    private int[] indicesNodos;
    public float volumen;

    private List<NodoMalla> skin;

    //normales
    //0 - (123) 1-(124) 2- (234) 3-(134)
    private Vector3[] normals;

    //lista de los muelles pero con sus indices
    private SpringIndex[] _springs;
    private List<Spring> _muelles;

    //constructor

    #endregion

    public Tetraedro(Node[] nodos, int[] indices)
    {
        _muelles = new List<Spring>();
        indicesNodos = indices;
        _nodes = nodos;
        _springs = new SpringIndex[6];
        normals = new Vector3 [4];
        skin = new List<NodoMalla>();
        createSprings();
        calculateNormals();
        calculateThisVolume();
    }


    public void calculateMass(float densidad)
    {
        foreach (Node n in _nodes)
        {
            n.mass += densidad * volumen / 4;
            n.numTetra++;
        }
    }

    public void addMuelle(Spring s)
    {
        _muelles.Add(s);
    }

    public Spring[] getSpringsToArray()
    {
        return _muelles.ToArray();
    }

    public void calculatePos(NodoMalla p)
    {
        Vector3 aux = Vector3.zero;
        for (int i = 0; i < 4; i++)
        {
            aux += p.w[i] * _nodes[i].getPos();
        }

        p.setPos(aux);
    }


    public void calculateAllPos()
    {
        foreach (NodoMalla n in skin)
        {
            calculatePos(n);
        }
    }

    private float calculateVolume(Vector3 o, Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 w = c - o;
        Vector3 v = b - o;
        Vector3 u = a - o;
        float calc = Math.Abs(Vector3.Dot(u, (Vector3.Cross(v, w)))) / 6;
        return calc;
    }


    public void computeTetraNodeForces(Vector3 gravedad, float dump)
    {
        calculateThisVolume();
        foreach (Node n in _nodes)
        {
            n.computeforces(gravedad, dump);
        }
    }

    public void computeTetraSpringForces(float k, float d)
    {
        //calculateThisVolume();
        foreach (Spring s in _muelles)
        {
            s.computeForces(k, d, volumen / 6);
        }
    }

    public void calculateThisVolume()
    {
        volumen = calculateVolume(_nodes[0].getPos(), _nodes[1].getPos(), _nodes[2].getPos(), _nodes[3].getPos());
    }

    private void calculateWheight(NodoMalla n)
    {
        for (int i = 0; i < 4; i++)
        {
            float vol = calculateVolume(n.getPos(), _nodes[(i + 1) % 4].getPos(), _nodes[(i + 2) % 4].getPos(),
                _nodes[(i + 3) % 4].getPos());
            n.w[i] = vol / volumen;
        }

    }

    void createSprings()
    {
        _springs[0] = new SpringIndex(indicesNodos[0], indicesNodos[1]);
        _springs[1] = new SpringIndex(indicesNodos[0], indicesNodos[2]);
        _springs[2] = new SpringIndex(indicesNodos[0], indicesNodos[3]);
        _springs[3] = new SpringIndex(indicesNodos[1], indicesNodos[2]);
        _springs[4] = new SpringIndex(indicesNodos[1], indicesNodos[3]);
        _springs[5] = new SpringIndex(indicesNodos[2], indicesNodos[3]);
    }

    private void calculateNormals()
    {
        //(1,2)x(3,2)
        Vector3 a = _nodes[0].getPos() - _nodes[1].getPos();
        Vector3 b = _nodes[2].getPos() - _nodes[1].getPos();
        normals[0] = calculateNormal(a, b);
        //(1,2)x(1,4)
        a = _nodes[1].getPos() - _nodes[0].getPos();
        b = _nodes[3].getPos() - _nodes[0].getPos();
        normals[1] = calculateNormal(a, b);
        //(2,3)x(2,4)
        a = _nodes[2].getPos() - _nodes[1].getPos();
        b = _nodes[3].getPos() - _nodes[1].getPos();
        normals[2] = calculateNormal(a, b);
        //(1,4)x(1,3)
        a = _nodes[3].getPos() - _nodes[0].getPos();
        b = _nodes[2].getPos() - _nodes[0].getPos();
        normals[3] = calculateNormal(a, b);
    }

    private Vector3 calculateNormal(Vector3 a, Vector3 b)
    {
        Vector3 aux = Vector3.Cross(a, b);
        return aux.normalized;
    }

    public void isInsideAndAdd(NodoMalla p)
    {
        float dist1 = Vector3.Dot(normals[0], p.getPos() - _nodes[1].getPos()); //123
        float dist2 = Vector3.Dot(normals[1], p.getPos() - _nodes[0].getPos()); //124
        float dist3 = Vector3.Dot(normals[2], p.getPos() - _nodes[1].getPos()); //234
        float dist4 = Vector3.Dot(normals[3], p.getPos() - _nodes[0].getPos()); //134

        if (dist1 <= 0 && dist2 <= 0 && dist3 <= 0 && dist4 <= 0)
        {
            if (!p.skinned)
            {
                skin.Add(p);
                p.skinned = true;
                calculateWheight(p);
            }
        }
    }


    public SpringIndex[] springsToArray()
    {
        return _springs;
    }
}