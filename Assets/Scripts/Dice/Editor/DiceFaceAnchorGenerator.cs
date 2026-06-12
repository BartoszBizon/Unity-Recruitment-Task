using System.Collections.Generic;
using Dice.Faces;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dice.Editor
{
    public class DiceFaceAnchorGeneratorWindow : EditorWindow
    {
        [SerializeField] private DiceFaces _diceFacesObject;
        [SerializeField, Min(1)] private int _expectedFaceCount = 12;
        [SerializeField, Min(0.001f)] private float _normalTolerance = 0.02f;
        [SerializeField, Min(0f)] private float _surfaceOffset = 0.01f;
        [SerializeField] private string _anchorsRootName = "Face Anchors";

        [SerializeField] private bool _createLabels = true;
        [SerializeField] private float _labelFontSize = 5f;
        [SerializeField] private Vector2 _labelSize = new(1f, 1f);

        [MenuItem("Tools/Dice/Face Anchor Generator")]
        private static void Open()
        {
            GetWindow<DiceFaceAnchorGeneratorWindow>("Face Anchor Generator");
        }

        private void OnGUI()
        {
            _diceFacesObject = (DiceFaces)EditorGUILayout.ObjectField("Target Object", _diceFacesObject, typeof(DiceFaces), true);

            _expectedFaceCount = EditorGUILayout.IntField("Expected Face Count", _expectedFaceCount);
            _normalTolerance = EditorGUILayout.FloatField("Normal Tolerance", _normalTolerance);
            _surfaceOffset = EditorGUILayout.FloatField("Surface Offset", _surfaceOffset);
            _anchorsRootName = EditorGUILayout.TextField("Anchors Root Name", _anchorsRootName);

            EditorGUILayout.Space();

            _createLabels = EditorGUILayout.Toggle("Create Labels", _createLabels);
            _labelFontSize = EditorGUILayout.FloatField("Label Font Size", _labelFontSize);
            _labelSize = EditorGUILayout.Vector2Field("Label Size", _labelSize);

            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(_diceFacesObject == null))
            {
                if (GUILayout.Button("Generate Face Anchors"))
                    Generate();
            }
        }

        private void Generate()
        {
            if (_diceFacesObject == null)
            {
                Debug.LogError("Missing target object.");
                return;
            }

            GenerateForObject(_diceFacesObject.gameObject);
        }

        private void GenerateForObject(GameObject targetObject)
        {
            MeshFilter meshFilter = targetObject.GetComponentInChildren<MeshFilter>();

            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                Debug.LogError("Missing MeshFilter or Mesh.", targetObject);
                return;
            }

            Undo.RegisterFullObjectHierarchyUndo(targetObject, "Generate Dice Face Anchors");

            ClearExistingAnchors(targetObject.transform);

            Mesh mesh = meshFilter.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            List<FaceGroup> groups = new();

            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 a = ToTargetLocal(targetObject.transform, meshFilter.transform, vertices[triangles[i]]);
                Vector3 b = ToTargetLocal(targetObject.transform, meshFilter.transform, vertices[triangles[i + 1]]);
                Vector3 c = ToTargetLocal(targetObject.transform, meshFilter.transform, vertices[triangles[i + 2]]);

                Vector3 normal = Vector3.Cross(b - a, c - a).normalized;
                Vector3 center = (a + b + c) / 3f;
                float area = Vector3.Cross(b - a, c - a).magnitude * 0.5f;

                FaceGroup group = FindMatchingGroup(groups, normal);

                if (group == null)
                {
                    group = new FaceGroup(normal);
                    groups.Add(group);
                }

                group.AddTriangle(center, area);
            }

            groups.Sort((a, b) => b.TotalArea.CompareTo(a.TotalArea));

            while (groups.Count > _expectedFaceCount)
                groups.RemoveAt(groups.Count - 1);

            Transform root = new GameObject(_anchorsRootName).transform;
            Undo.RegisterCreatedObjectUndo(root.gameObject, "Create Face Anchors Root");
            root.SetParent(targetObject.transform, false);

            for (int i = 0; i < groups.Count; i++)
            {
                FaceGroup group = groups[i];

                Vector3 localPosition = group.Center + group.Normal * _surfaceOffset;
                Quaternion localRotation = Quaternion.LookRotation(group.Normal, Vector3.up);

                GameObject anchorObject = new($"Face Anchor {i + 1:00}");
                Undo.RegisterCreatedObjectUndo(anchorObject, "Create Face Anchor");

                anchorObject.transform.SetParent(root, false);
                anchorObject.transform.localPosition = localPosition;
                anchorObject.transform.localRotation = localRotation;

                DiceFaceAnchor anchor = anchorObject.AddComponent<DiceFaceAnchor>();
                anchor.SetValue(i + 1);

                if (_createLabels)
                    CreateLabel(anchorObject.transform, i + 1, anchor);
            }

            _diceFacesObject.ApplyPreset();
            EditorUtility.SetDirty(targetObject);

            Debug.Log($"Generated {groups.Count} face anchors.", targetObject);
        }

        private Vector3 ToTargetLocal(Transform target, Transform meshTransform, Vector3 meshLocalPosition)
        {
            Vector3 worldPosition = meshTransform.TransformPoint(meshLocalPosition);
            return target.InverseTransformPoint(worldPosition);
        }

        private FaceGroup FindMatchingGroup(List<FaceGroup> groups, Vector3 normal)
        {
            for (int i = 0; i < groups.Count; i++)
            {
                if (Vector3.Distance(groups[i].Normal, normal) <= _normalTolerance)
                    return groups[i];
            }

            return null;
        }

        private void CreateLabel(Transform parent, int value, DiceFaceAnchor anchor)
        {
            GameObject labelObject = new("Face Label");
            Undo.RegisterCreatedObjectUndo(labelObject, "Create Face Label");

            labelObject.transform.SetParent(parent, false);
            labelObject.transform.localPosition = Vector3.zero;
            labelObject.transform.localRotation = Quaternion.identity;

            TextMeshPro label = labelObject.AddComponent<TextMeshPro>();
            label.text = value.ToString();
            label.fontSize = _labelFontSize;
            label.alignment = TextAlignmentOptions.Center;
            label.enableAutoSizing = false;
            label.rectTransform.sizeDelta = _labelSize;

            anchor.SetLabel(label);
        }

        private void ClearExistingAnchors(Transform target)
        {
            Transform existingRoot = target.Find(_anchorsRootName);

            if (existingRoot == null)
                return;

            Undo.DestroyObjectImmediate(existingRoot.gameObject);
        }

        private sealed class FaceGroup
        {
            private Vector3 _weightedCenter;
            private float _totalArea;

            public Vector3 Normal { get; }
            public float TotalArea => _totalArea;
            public Vector3 Center => _weightedCenter / _totalArea;

            public FaceGroup(Vector3 normal)
            {
                Normal = normal;
            }

            public void AddTriangle(Vector3 center, float area)
            {
                _weightedCenter += center * area;
                _totalArea += area;
            }
        }
    }
}