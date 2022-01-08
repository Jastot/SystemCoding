using System;
using Data;
using Mechanics;
using UnityEngine;

namespace Main
{
    [Serializable]
    public class PlanetStruct
    {
        public string PlanetName;
        public PlanetOrbitData PlanetOrbitData;
        public Material Material;
        public Vector2 Position;
        public int Radius;
        public float Speed;
    }
}