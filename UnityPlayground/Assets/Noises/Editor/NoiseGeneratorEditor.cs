using System;
using UnityEditor;
using UnityEngine;

namespace Playground.Noises
{
    public partial class NoiseGenerator
    {
        [CustomEditor(typeof(NoiseGenerator))]
        private class NoiseGeneratorEditor : Editor
        {
            private NoiseGenerator generator = null;
            
            public override void OnInspectorGUI()
            {
                EditorGUI.BeginChangeCheck();
                DrawDefaultInspector();
                if (EditorGUI.EndChangeCheck() && Application.isPlaying)
                {
                   RefreshCreator();
                }
            }

            private void OnEnable()
            {
                generator = (target as NoiseGenerator);
                Undo.undoRedoPerformed += RefreshCreator;
            }

            private void OnDisable()
            {
                Undo.undoRedoPerformed -= RefreshCreator;
            }

            private void RefreshCreator()
            {
                generator.GenerateNoise();
            }
        }
    }
}