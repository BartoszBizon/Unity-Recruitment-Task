using System.Collections.Generic;
using System.Linq;
using Dice.Interfaces;
using NaughtyAttributes;
using UnityEngine;

namespace Dice.Faces
{
    public class DiceFaces : MonoBehaviour, IDiceFaceProvider
    {
        [InfoBox("Face values are configured through DiceFacesPreset. " +
                 "Use Tools/Dice/Face Anchor Generator to regenerate anchors when changing the mesh.")]
        [Expandable] [SerializeField] private DiceFacesPreset _diceFacesPreset;
        [SerializeField] private List<DiceFaceAnchor> _faces;

        private void OnValidate()
        {
            ApplyPreset();
        }

        [Button]
        public void ApplyPreset()
        {
            _faces.Clear();
            _faces = GetComponentsInChildren<DiceFaceAnchor>().ToList();

            if (_diceFacesPreset == null)
            {
                Debug.LogError("DiceFacesPreset is not assigned.", this);
                return;
            }

            if (_faces.Count != _diceFacesPreset.FaceCount)
            {
                Debug.LogError($"Face count mismatch: {_faces.Count} anchors but preset has {_diceFacesPreset.FaceCount} faces.", this);
                return;
            }

            for (int i = 0; i < _faces.Count; i++)
            {
                _faces[i].SetValue(_diceFacesPreset.FaceValues[i]);
            }
        }

        public DiceFaceAnchor GetTopDiceFace()
        {
            DiceFaceAnchor topFace = null;
            var highestY = float.MinValue;

            foreach (var face in _faces)
            {
                var y = face.transform.position.y;

                if (y > highestY)
                {
                    highestY = y;
                    topFace = face;
                }
            }

            return topFace;
        }
    }
}