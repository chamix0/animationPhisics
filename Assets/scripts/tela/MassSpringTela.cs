using System;
using System.Collections;
using System.Collections.Generic;
using Source.Scripts;
using UnityEngine;

public class MassSpringTela : MonoBehaviour
{
    #region InEditorVariables

    public bool Paused;
    public float TimeStep;
    public Vector3 Gravity, wind;
    public Integration IntegrationMethod;
    public int substeps;
    public float mass, dampT, dampF, dampNodes; //masa de los nodos, constantes de amortiguamiento
    public float stiffnessTraccion; //rigidez de los nodos de traccion
    public float stiffnessFlexion; //rigidez de los nodos de flexion
    public float k, dViento; //constante que afecta a la fuerza de las colisiones

    #endregion

    #region estructuras

    Mesh mesh; //malla
    Vector3[] vertices;
    int[] triangles;

    //nodos y muelles
    private List<NodeTela> _nodos;
    private List<SpringTela> _muelles;
    private SpringStructureTela auxMuelles;

    //colisiones
    private List<GameObject> esferas;
    private List<Sphere> _spheres;

    #endregion


    //constructor
    public MassSpringTela()
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
        //inicializacion de las esferas colisionables
        esferas = new List<GameObject>();
        esferas.AddRange(GameObject.FindGameObjectsWithTag("colisiones"));
        _spheres = new List<Sphere>();

        sphereCreation();

        //inicializacion de la tela
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        triangles = mesh.triangles;
        auxMuelles = new SpringStructureTela();
        _nodos = new List<NodeTela>();
        _muelles = new List<SpringTela>();

        //transformacion de los vertices a coordenadas globales y creacion de nodos
        localesAGlobales();
        //creacion de los muelles
        springCreation();
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
        // DrawEdges();
        // DrawNodes();
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
        //calcular fuerza del viento en cada triangulo
        calculateWind();

        foreach (NodeTela n in _nodos)
        {
            n.setForce(Vector3.zero);

            //añadir fuerza de gravedad
            n.computeforces(mass, Gravity, n.windForce, dampNodes);
            n.windForce = Vector3.zero;
            n.computeCollisions(_spheres, k);
        }

        foreach (SpringTela s in _muelles)
        {
            s.computeForces(stiffnessTraccion, stiffnessFlexion, dampT, dampF);
        }

        foreach (NodeTela n in _nodos)
        {
            if (n.isMovable)
            {
                //actualizar posicionx(t+h)=x(c) +h*v(t+h)
                n.setPos(n.getPos() + TimeStep * n.getVel());
                //actualizar velocidad: v(t+h)= v(t) + (h/m)*F
                n.setVel(n.getVel() + (TimeStep / mass) * n.getForce());
            }
        }

        foreach (SpringTela s in _muelles)
        {
            s.UpdateLength();
        }
    }

    private void stepSymplectic()
    {
        //calcular fuerza del viento en cada triangulo
        calculateWind();

        foreach (NodeTela n in _nodos)
        {
            n.setForce(Vector3.zero);

            //añadir fuerza de gravedad
            n.computeforces(mass, Gravity, n.windForce, dampNodes);
            n.windForce = Vector3.zero;
            n.computeCollisions(_spheres, k);
        }

        foreach (SpringTela s in _muelles)
        {
            s.computeForces(stiffnessTraccion, stiffnessFlexion, dampT, dampF);
        }

        foreach (NodeTela n in _nodos)
        {
            if (n.isMovable)
            {
                //actualizar velocidad: v(t+h)= v(t) + (h/m)*F
                n.setVel(n.getVel() + (TimeStep / mass) * n.getForce());
                //actualizar posicionx(t+h)=x(c) +h*v(t+h)
                n.setPos(n.getPos() + TimeStep * n.getVel());
            }
        }

        foreach (SpringTela s in _muelles)
        {
            s.UpdateLength();
        }
    }

    private void calculateWind()
    {
        for (int i = 0; i < triangles.Length; i += 3)
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

    private void sphereCreation()
    {
        foreach (GameObject O in esferas)
        {
            SphereCollider _sphereCollider = O.GetComponent<SphereCollider>();
            _spheres.Add(new Sphere(O));
        }
    }


    private void springCreation()
    {
        for (int i = 0; i < triangles.Length; i = i + 3)
        {
            Edge edge1 = auxMuelles.add(triangles[i], triangles[i + 1], triangles[i + 2]);
            if (edge1 != null)
            {
                if (edge1.traccion)
                    _muelles.Add(new SpringTela(_nodos[(int) edge1.a], _nodos[(int) edge1.b], true));
                else
                    _muelles.Add(new SpringTela(_nodos[(int) edge1.a], _nodos[(int) edge1.b], false));
            }

            Edge edge2 = auxMuelles.add(triangles[i], triangles[i + 2], triangles[i + 1]);
            if (edge2 != null)
            {
                if (edge2.traccion)
                    _muelles.Add(new SpringTela(_nodos[(int) edge2.a], _nodos[(int) edge2.b], true));
                else
                    _muelles.Add(new SpringTela(_nodos[(int) edge2.a], _nodos[(int) edge2.b], false));
            }

            Edge edge3 = auxMuelles.add(triangles[i + 1], triangles[i + 2], triangles[i]);
            if (edge3 != null)
            {
                if (edge3.traccion)
                    _muelles.Add(new SpringTela(_nodos[(int) edge3.a], _nodos[(int) edge3.b], true));
                else
                    _muelles.Add(new SpringTela(_nodos[(int) edge3.a], _nodos[(int) edge3.b], false));
            }
        }
    }

    public NodeTela[] getNodos()
    {
        return _nodos.ToArray();
    }

    #region transformaciones de coordenadas

    public void localesAGlobales()
    {
        //For simulation purposes, transform the points to global coordinates
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            Vector3 globPos = transform.TransformPoint(vertices[i]);
            _nodos.Add(new NodeTela(globPos));
        }
    }

    public void globalesALocales()
    {
        //Procedure to update vertex positions
        Vector3[] localPos = new Vector3[mesh.vertexCount];

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            localPos[i] = transform.InverseTransformPoint(_nodos[i].getPos());
        }

        mesh.vertices = localPos;
    }

    #endregion

    #region debuggin

    public void DrawNodes()
    {
        foreach (NodeTela n in _nodos)
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

    public void DrawEdges()
    {
        foreach (SpringTela s in _muelles)
        {
            if (s.traccion)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawLine(s.nodeTelaA.getPos(), s.nodeTelaB.getPos());
            }
            else
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(s.nodeTelaA.getPos(), s.nodeTelaB.getPos());
            }
        }
    }

    #endregion

    #endregion
}