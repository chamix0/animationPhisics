using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Parser : MonoBehaviour
{
    public TextAsset textoNodos;
    public TextAsset textoTetra;
    public TextAsset textoTri;

    private List<Node> _nodes;
    private List<Tetraedro> _tetraedros;
    private List<int> _triangles;


    public Node[] getNodes()
    {
        return _nodes.ToArray();
    }

    public Tetraedro[] getTetra()
    {
        return _tetraedros.ToArray();
    }
    public int[] getTris()
    {
        return _triangles.ToArray();
    }


    public void getNodesAndTetra()
    {
        //obtencion de nodos
        string[] textString = textoNodos.text.Split(
            new string[] {" ", "\n", "\r"}, StringSplitOptions.RemoveEmptyEntries);

        _nodes = new List<Node>();
        int maxVertices = int.Parse(textString[0]) + 1;
        Vector3 off = Vector3.zero;
        for (int i = 4; i < maxVertices * 4; i += 4)
        {
            //print(textString[i]);
            Vector3 aux = new Vector3(
                float.Parse(textString[i + 1], CultureInfo.InvariantCulture),
                float.Parse(textString[i + 2], CultureInfo.InvariantCulture),
                float.Parse(textString[i + 3], CultureInfo.InvariantCulture));

            //es importante que los hijos del game object esten en el (0,0,0)
            Node n = new Node(transform.TransformPoint(aux));
            _nodes.Add(n);
        }

        //obtencion de tetraedros
        string[] textString2 = textoTetra.text.Split(
            new string[] {" ", "\n", "\r"}, StringSplitOptions.RemoveEmptyEntries);
        _tetraedros = new List<Tetraedro>();
        int maxtetra = int.Parse(textString2[0]);

        for (int i = 3; i < maxtetra * 5; i += 5)
        {
            Node[] aux =
            {
                _nodes[int.Parse(textString2[i + 1], CultureInfo.InvariantCulture) - 1],
                _nodes[int.Parse(textString2[i + 2], CultureInfo.InvariantCulture) - 1],
                _nodes[int.Parse(textString2[i + 3], CultureInfo.InvariantCulture) - 1],
                _nodes[int.Parse(textString2[i + 4], CultureInfo.InvariantCulture) - 1]
            };
            int[] aux2 =
            {
                int.Parse(textString2[i + 1], CultureInfo.InvariantCulture) - 1,
                int.Parse(textString2[i + 2], CultureInfo.InvariantCulture) - 1,
                int.Parse(textString2[i + 3], CultureInfo.InvariantCulture) - 1,
                int.Parse(textString2[i + 4], CultureInfo.InvariantCulture) - 1
            };
            Tetraedro tetra = new Tetraedro(aux, aux2);
            _tetraedros.Add(tetra);
        }

        //obtencion de triangulos
        string[] textString3 = textoTri.text.Split(
            new string[] {" ", "\n", "\r"}, StringSplitOptions.RemoveEmptyEntries);
        _triangles = new List<int>();
        int maxTri = int.Parse(textString3[0], CultureInfo.InvariantCulture);

        for (int i = 2; i < maxTri * 5; i += 5)
        {
            _triangles.Add(int.Parse(textString3[i + 1], CultureInfo.InvariantCulture) - 1);
            _triangles.Add(int.Parse(textString3[i + 2], CultureInfo.InvariantCulture) - 1);
            _triangles.Add(int.Parse(textString3[i + 3], CultureInfo.InvariantCulture) - 1);
        }
    }
}