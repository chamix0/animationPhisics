using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

public class SpringStructure
{
    //datos
    private List<SpringIndex> muelles;


    //constructor
    public SpringStructure()
    {
        muelles = new List<SpringIndex>();
    }

    #region metodos

    public SpringIndex add(int a, int b)
    {
        int max = a > b ? a : b;
        int min = a < b ? a : b;

        SpringIndex aux = new SpringIndex(max, min);

        if (!muelles.Contains(aux))
        {
            muelles.Add(aux);
            return aux;
        }

        return null;
    }

    #endregion
}