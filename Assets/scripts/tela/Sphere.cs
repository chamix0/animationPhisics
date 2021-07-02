using System;
using UnityEngine;

namespace Source.Scripts
{
    public class Sphere: MonoBehaviour
    {
        //variables 
        private GameObject s;
        public Vector3 center;
        public SphereCollider _sphereCollider;
        public Bounds _bounds;
        public float radio;

        //constructor
        public Sphere(GameObject Object)
        {
            s = Object;
            center = s.transform.position;
            _sphereCollider =s.GetComponent<SphereCollider>();
            _bounds = _sphereCollider.bounds;
            radio = _sphereCollider.radius;
        }

        private void Update()
        {
            center = s.transform.position;
            radio = _sphereCollider.radius;
        }
    }
}