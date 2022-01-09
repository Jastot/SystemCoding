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
        public int Material;
        public Vector2 Position;
        public int Radius;
        public float Speed;
    }
}