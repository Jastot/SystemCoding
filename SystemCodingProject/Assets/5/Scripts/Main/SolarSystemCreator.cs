using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Main
{
    public class SolarSystemCreator: MonoBehaviour
    {
        [SerializeField] private string _some;
        [SerializeField,NonReorderable] private PlanetStruct[] _planetStructs;
        
        public void AddNewPoint(int index)
        {
             _planetStructs = ReSorting(index,_planetStructs.ToList());
        }

        private PlanetStruct[] ReSorting(int index, List<PlanetStruct> copyArray)
        {
            PlanetStruct[] additional = new PlanetStruct[copyArray.Count];
            copyArray.CopyTo(additional);
            copyArray.Add(new PlanetStruct());
            for (int i = index; i < copyArray.Count - 1; i++)
            {
                copyArray[i + 1] = additional[i];
            }
            copyArray[index+1] = new PlanetStruct();
            return copyArray.ToArray();
        }
    
        public void DeletePoint(int index)
        {
            var copyArray = _planetStructs.ToList();
            copyArray.RemoveAt(index);
            _planetStructs = copyArray.ToArray();
        }

        public void MovePointDown(int index)
        {
            if (index != _planetStructs.Length - 1)
            {
                var copyArray = _planetStructs.ToList();
                PlanetStruct[] additional = new PlanetStruct[copyArray.Count];
                copyArray.CopyTo(additional);
                copyArray[index + 1] = additional[index];
                copyArray[index] = additional[index + 1];
                _planetStructs = copyArray.ToArray();
            }
        }

        public void MovePointUp(int index)
        {
            if (index!=0)
            {
                var copyArray = _planetStructs.ToList();
                PlanetStruct[] additional = new PlanetStruct[copyArray.Count];
                copyArray.CopyTo(additional);
                copyArray[index - 1] = additional[index];
                copyArray[index] = additional[index - 1];
                _planetStructs = copyArray.ToArray();
            }
        }

        public void SetBasePlanet()
        {
            if (_planetStructs==null)
                _planetStructs = new PlanetStruct[]{new PlanetStruct(){PlanetOrbitData = null, Position = Vector3.zero,Speed = 0.0f}};
        }
    }
}