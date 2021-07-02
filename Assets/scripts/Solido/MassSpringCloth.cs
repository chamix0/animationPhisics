using System;
using System.Collections;
using System.Collections.Generic;
using Source.Scripts;
using UnityEngine;

public class MassSpringCloth : MonoBehaviour
{
    #region InEditorVariables

    public bool Paused;
    public float TimeStep;
    public Vector3 Gravity, wind;
    public Integration IntegrationMethod;
    public int substeps;
    public float densidad, dampT, dampNodes; //masa de los nodos, constantes de amortiguamiento
    public float stiffnessTraccion; //rigidez de los nodos de traccion
    public float k, dViento; //constante que afecta a la fuerza de las colisiones

    #endregion

    #region estructuras

    //malla
    Mesh _mesh;
    Vector3[] _vertices;

    private List<NodoMalla> _nodosMalla;

    //nodos, muelles y tetraedros
    private List<Node> _nodos;
    private List<Spring> _muelles;
    private List<Tetraedro> _tetraedros;
    private List<int> triangles;

    //parser
    private Parser _parser;


    //colisiones
    private List<GameObject> _planos;
    private List<Plane> planes;

    #endregion


    //constructor
    public MassSpringCloth()
    {
        this.Paused = true;
        this.TimeStep = 0.01f;
        this.Gravity = new Vector3(0.0f, -9.81f, 0.0f);
        this.IntegrationMethod = Integration.Explicit;
    }

    //tipo de integracion
    public enum Integration
    {
        Explicit = 0,
        Symplectic = 1,
    };

    #region MonoBehaviour

    // Start is called before the first frame update
    void Awake()
    {
        //colisiones con planos
        _planos = new List<GameObject>();
        _planos.AddRange(GameObject.FindGameObjectsWithTag("PlanoColision"));
        planes = new List<Plane>();
        foreach (GameObject o in _planos)
        {
            planes.Add(new Plane(o.transform.up, o.transform.position));
        }

        //tetraedros
        _parser = GetComponent<Parser>();
        _parser.getNodesAndTetra();
        _tetraedros = new List<Tetraedro>(_parser.getTetra());
        _nodos = new List<Node>(_parser.getNodes());
        triangles = new List<int>(_parser.getTris());
        _muelles = new List<Spring>();
        springCreation();

        //inicializacion de la tela
        _mesh = GetComponentInChildren<MeshFilter>().mesh;
        _vertices = _mesh.vertices;
        _nodosMalla = new List<NodoMalla>();
        //transformacion de los vertices a coordenadas globales y creacion de nodos
        localesAGlobales();

        //asignacion de nodos de la malla a tetraedros
        foreach (Tetraedro t in _tetraedros)
        {
            t.calculateMass(densidad);

            foreach (NodoMalla n in _nodosMalla)
            {
                t.isInsideAndAdd(n);
            }
        }

        foreach (Node n in _nodos)
        {
            n.massAverage();
        }
    }


    // Update is called once per frame
    void Update()
    {
        //transformacion de los nodos a coordenadas locales y actualizacion de los vertices de la malla
        globalesALocales();
    }


    //para ver visualmente los nodos y los muelles
    private void OnDrawGizmos()
    {
        DrawMeshNodes();
        DrawEdges();
        DrawNodes();
    }

    public void FixedUpdate()
    {
        if (this.Paused)
            return; // Not simulating
        //implementacion de substeps
        int Aux = substeps;
        for (int i = 0; i < Aux; i++)
        {
            // Select integration method
            switch (this.IntegrationMethod)
            {
                case Integration.Explicit:
                    this.stepExplicit();
                    break;
                case Integration.Symplectic:
                    this.stepSymplectic();
                    break;
                default:
                    throw new System.Exception("[ERROR] Should never happen!");
            }
        }
    }

    #endregion

    #region Metodos

    private void stepExplicit()
    {
    }

    private void stepSymplectic()
    {
        //calcular fuerza del viento en cada triangulo


        foreach (Node n in _nodos)
        {
            n.setForce(Vector3.zero);
            n.setWindForce(Vector3.zero);
        }

        calculateWind();
        foreach (Node n in _nodos)
        {
            n.computeCollisions(planes, k);
        }

        foreach (Tetraedro t in _tetraedros)
        {
            t.computeTetraNodeForces(Gravity, dampNodes);
        }


        foreach (Tetraedro t in _tetraedros)
        {
            t.computeTetraSpringForces(stiffnessTraccion, dampT);
        }


        foreach (Node n in _nodos)
        {
            if (n.isMovable)
            {
                //actualizar velocidad: v(t+h)= v(t) + (h/m)*F
                n.setVel(n.getVel() + (TimeStep / n.mass) * n.getForce());
                //actualizar posicionx(t+h)=x(c) +h*v(t+h)
                n.setPos(n.getPos() + TimeStep * n.getVel());
            }
        }

        foreach (Spring s in _muelles)
        {
            s.UpdateLength();
        }

        foreach (Tetraedro t in _tetraedros)
        {
            t.calculateAllPos();
        }
    }

    private void calculateWind()
    {
        for (int i = 0; i < triangles.Count; i += 3)
        {
            Vector3 u = _nodos[triangles[i + 1]].getPos() - _nodos[triangles[i]].getPos();
            Vector3 v = _nodos[triangles[i + 2]].getPos() - _nodos[triangles[i]].getPos();
            float A = Vector3.Dot(u, v) / 2;
            Vector3 normal = -Vector3.Cross(u, v).normalized;
            Vector3 vMedia = (_nodos[triangles[i]].getVel() + _nodos[triangles[i + 1]].getVel() +
                              _nodos[triangles[i + 2]].getVel()) / 3;
            Vector3 calc = Vector3.Dot(normal, (wind - vMedia)) * normal;
            Vector3 aux = (dViento * A * calc.normalized) / 3;
            _nodos[triangles[i]].windForce += aux;
            _nodos[triangles[i + 1]].windForce += aux;
            _nodos[triangles[i + 2]].windForce += aux;
        }
    }

    private void springCreation()
    {
        SpringStructure springStructure = new SpringStructure();
        foreach (Tetraedro t in _tetraedros)
        {
            SpringIndex[] s = t.springsToArray();
            for (int j = 0; j < 6; j++)
            {
                SpringIndex spring = springStructure.add(s[j].a, s[j].b);
                if (spring != null)
                {
                    Spring aux = new Spring(_nodos[spring.a], _nodos[spring.b]);
                    _muelles.Add(aux);
                    t.addMuelle(aux); //insercion de los muelles en el tetraedro
                }
            }
        }
    }


    public Node[] getNodosSolido()
    {
        return _nodos.ToArray();
    }

    #region transformaciones de coordenadas

    public void localesAGlobales()
    {
//For simulation purposes, transform the points to global coordinates
        for (int i = 0;
            i < _mesh.vertexCount;
            i++)
        {
            Vector3 globPos = transform.TransformPoint(_vertices[i]);
            _nodosMalla.Add(new NodoMalla(globPos));
        }
    }

    public void globalesALocales()
    {
//Procedure to update vertex positions
        Vector3[] localPos = new Vector3[_nodosMalla.Count];
        for (int i = 0; i < _nodosMalla.Count; i++)
        {
            localPos[i] = transform.InverseTransformPoint(_nodosMalla[i].getPos());
        }

        _mesh.vertices = localPos;
    }

    #endregion

    #region debuggin

    public void DrawNodes()
    {
        if (_nodos != null)
        {
            foreach (Node n in _nodos)
            {
                if (n.inCollision)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(n.getPos(), 0.1f);
                }
                else
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(n.getPos(), 0.1f);
                }
            }
        }
    }

    public void DrawMeshNodes()
    {
        if (_nodosMalla != null)
        {
            foreach (NodoMalla n in _nodosMalla)
            {
                if (n.skinned)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(n.getPos(), 0.1f);
                }
            }
        }
    }


    public void DrawEdges()
    {
        if (_muelles != null)
        {
            foreach (Spring s in _muelles)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(s.nodeA.getPos(), s.nodeB.getPos());
            }
        }
    }

    #endregion

    #endregion
}