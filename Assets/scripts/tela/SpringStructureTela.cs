using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public class SpringStructureTela
{
    //datos
    private List<Edge> muelles;


    //constructor
    public SpringStructureTela()
    {
        muelles = new List<Edge>();
    }

    #region metodos

    public Edge add(int a, int b, int c)
    {
        bool success = false;
        int max = a > b ? a : b;
        int min = a < b ? a : b;

        Edge edge = new Edge(max, min, c, true);

        if (!muelles.Contains(edge))
        {
            muelles.Add(edge);
            return edge;
        }
        else
        {
            Edge aux = muelles.Find(x => x.Equals(edge));
            int cOld = aux.c;
            Edge fEdge = new Edge(cOld, c, 0, false);
            muelles.Add(fEdge);
            return fEdge;
        }
    }

    #endregion
}