using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixerTela : MonoBehaviour
{
    #region variables privadas

    private List<NodeTela> affectedNodes; //nodos afectados por el fixer
    private List<GameObject> cloths; //lista de objetos de tela
    private List<MassSpringTela> _clothsManagers; //lista de componentes Mass Spring 

    //deteccion de colisiones
    private MeshCollider _collider; //collider del objeto. es imoportante que el objeto tenga un meshcollider
    private Bounds bounds;

    #endregion

    // Possibilities of the Fixer
    void Start()
    {
        //inicializacion de listas y componentes
        _collider = GetComponent<MeshCollider>();
        bounds = _collider.bounds;
        cloths = new List<GameObject>();
        cloths.AddRange(GameObject.FindGameObjectsWithTag("Tela1"));
        _clothsManagers = new List<MassSpringTela>();
        affectedNodes = new List<NodeTela>();

        foreach (GameObject M in cloths)
        {
            MassSpringTela comp = M.GetComponent<MassSpringTela>();
            _clothsManagers.Add(comp);
        }

        /*comprobación de los vertices que estan dentro del fixer y aquellos que lo esten se bloquearan y se añadiran a
        la lista de nodos afectados*/
        for (int i = 0; i < _clothsManagers.Count; i++)
        {
            NodeTela[] auxNodes = _clothsManagers[i].getNodos();
            foreach (NodeTela n in auxNodes)
            {
                bool isInside = bounds.Contains(n.getPos());
                n.isMovable = !isInside;
                if (isInside) affectedNodes.Add(n);
            }
        }

        //para calcular el movimiento se guarda en el nodo la posición local de los nodos respecto de los objetos fixer
        foreach (NodeTela n in affectedNodes)
        {
            n.fixerLocalPos = transform.InverseTransformPoint(n.getPos());
        }
    }

    private void Update()
    {
        //para que la transsformacion sea efectiva, tendre que actualizar constantemente la posicion del objeto
        foreach (NodeTela n in affectedNodes)
        {
            Vector3 globalPos = transform.TransformPoint(n.fixerLocalPos);
            n.setPos(globalPos);
        }
    }
}