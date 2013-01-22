using UnityEngine;

using UnityEditor;

using System.Collections;

 

[CustomEditor(typeof(CombineChildrenAdvanced))]

public class CombineChildrenAdvancedEditor : Editor {

 

    CombineChildrenAdvanced _target;

    

    void OnEnable() {

        _target = (CombineChildrenAdvanced)target;

    }

    

    

    

    public override void OnInspectorGUI() {

        

        EditorGUIUtility.LookLikeInspector();

        

        _target.generateTriangleStrips = EditorGUILayout.Toggle("Generate Triangle Strips", _target.generateTriangleStrips);

        _target.generateLightmappingUVs = EditorGUILayout.Toggle ("Generate Lightmapping UVs", _target.generateLightmappingUVs);

        

        if (!_target.isCombined) {

            if (GUILayout.Button ("Combine now (May take a while!)")) {

                _target.Combine();

                

                if (_target.generateLightmappingUVs) {

                    GenerateLightmappingUVs();

                }

            }

        }

        

        if (_target.isCombined) {

            if (GUILayout.Button ("Generate Lightmap UVs")) {

                GenerateLightmappingUVs();

            }

            

            if (GUILayout.Button ("Split Mesh")) {

                DestroyImmediate(_target.GetComponent (typeof(MeshFilter)));

                DestroyImmediate(_target.GetComponent (typeof(MeshRenderer)));

                

                foreach (MeshRenderer mr in _target.GetComponentsInChildren(typeof(MeshRenderer))) {

                    mr.enabled = true;

                }

                

                _target.isCombined = false;

            }

        }

    }

    

    

    

    

    void GenerateLightmappingUVs() {

        MeshFilter mf = _target.GetComponent(typeof(MeshFilter)) as MeshFilter;

        

        // make null check because if not enough meshes are present no combined mesh would have been created!

        if (mf != null) {

            Unwrapping.GenerateSecondaryUVSet(mf.sharedMesh);

        }

    }

    

}