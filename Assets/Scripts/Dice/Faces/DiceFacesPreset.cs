using System.Collections.Generic;
using UnityEngine;

namespace Dice.Faces
{
    [CreateAssetMenu(fileName = "Dice Faces Preset", menuName = "Dice/Dice Faces Preset")]
    public sealed class DiceFacesPreset : ScriptableObject
    {
        [SerializeField, Min(1)] private int _faceCount = 12;
        [SerializeField] private List<int> _faceValues = new();

        public int FaceCount => _faceCount;
        public IReadOnlyList<int> FaceValues => _faceValues;


        private void OnValidate()
        {
            while (_faceValues.Count < _faceCount)
            {
                _faceValues.Add(_faceValues.Count + 1);
            }

            while (_faceValues.Count > _faceCount)
            {
                _faceValues.RemoveAt(_faceValues.Count - 1);
            }
        }
    }
}