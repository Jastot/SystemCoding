using Main;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "ScriptableObjects/Data", order = 1)]
public class MapData: ScriptableObject
{
    public PlanetStruct[] PlanetStructs = new PlanetStruct[] {};
}