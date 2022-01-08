using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "PlanetOrbitData", menuName = "Geekbrains/Settings/PlanetOrbitData")]
    public class PlanetOrbitData: ScriptableObject
    {
        public float smoothTime = .3f;
        public float offsetSin = 1;
        public float offsetCos = 1;
    }
}