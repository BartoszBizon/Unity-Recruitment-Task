using TMPro;
using UnityEngine;

namespace Dice.Faces
{
    public sealed class DiceFaceAnchor : MonoBehaviour
    {
        [SerializeField] private int _value;
        [SerializeField] private TMP_Text _label;

        public int Value => _value;
        public Transform FaceTransform => transform;

        public void SetValue(int newValue)
        {
            _value = newValue;
            RefreshLabel();
        }

        public void SetLabel(TMP_Text newLabel)
        {
            _label = newLabel;
            RefreshLabel();
        }

        private void OnValidate()
        {
            RefreshLabel();
        }

        private void RefreshLabel()
        {
            if (_label != null)
                _label.text = _value.ToString();
        }
    }
}