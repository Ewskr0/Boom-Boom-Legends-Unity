﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace SuperTiled2Unity.Editor
{
    public abstract class SuperImporterEditor<T> : ScriptedImporterEditor where T : SuperImporter
    {
        private bool m_ShowDependencies;
        private bool m_ShowReferences;

        public T TargetAssetImporter
        {
            get { return serializedObject.targetObject as T; }
        }

        protected abstract string EditorLabel { get; }
        protected abstract string EditorDefinition { get; }

        public override sealed void OnInspectorGUI()
        {
            if (assetTarget != null)
            {
                // If we have importer errors then they should be front and center
                DisplayErrorsAndWarnings();
                DisplayTagManagerErrors();

                EditorGUILayout.LabelField(EditorLabel, EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(EditorDefinition, MessageType.None);
                EditorGUILayout.Separator();

                InternalOnInspectorGUI();
                DisplayDependencies();
            }
            else
            {
                // Force Unity to stop OnGUI calls for this editor
                GUIUtility.ExitGUI();
            }
        }

        protected U GetAssetTarget<U>() where U : UnityEngine.Object
        {
            foreach (var target in assetTargets)
            {
                U u = target as U;
                if (u != null)
                {
                    return u;
                }
            }

            return null;
        }


        protected void ToggleFromInt(SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            int intValue = EditorGUILayout.Toggle(label, property.intValue > 0) ? 1 : 0;
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = intValue;
            }
        }

        protected abstract void InternalOnInspectorGUI();

        private void DisplayErrorsAndWarnings()
        {
            var background = GetBackgroundColor();
            using (new GuiScopedBackgroundColor(background))
            {
                if (TargetAssetImporter.Errors.Any())
                {
                    EditorGUILayout.LabelField("There were errors importing " + this.TargetAssetImporter.assetPath, EditorStyles.boldLabel);
                    EditorGUILayout.HelpBox(string.Join("\n\n", this.TargetAssetImporter.Errors.Take(10).ToArray()), MessageType.Error);
                    EditorGUILayout.Separator();
                }

                if (TargetAssetImporter.Warnings.Any())
                {
                    EditorGUILayout.LabelField("There were warnings importing " + this.TargetAssetImporter.assetPath, EditorStyles.boldLabel);
                    EditorGUILayout.HelpBox(string.Join("\n\n", this.TargetAssetImporter.Warnings.Take(10).ToArray()), MessageType.Warning);
                    EditorGUILayout.Separator();
                }
            }
        }

        private void DisplayTagManagerErrors()
        {
            bool missingSortingLayers = TargetAssetImporter.MissingSortingLayers.Any();
            bool missingLayers = TargetAssetImporter.MissingLayers.Any();

            if (!missingSortingLayers && !missingLayers)
            {
                return;
            }

            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);

            using (new GuiScopedBackgroundColor(Color.yellow))
            {
                if (missingSortingLayers)
                {
                    EditorGUILayout.LabelField("Missing Sorting Layers!", EditorStyles.boldLabel);

                    using (new GuiScopedIndent())
                    {
                        StringBuilder message = new StringBuilder("Sorting Layers are missing in your project settings. Open the Tag Manager, add these missing sorting layer, and reimport:");
                        message.AppendLine();
                        message.AppendLine();

                        foreach (var layer in TargetAssetImporter.MissingSortingLayers)
                        {
                            message.AppendFormat("    {0}\n", layer);
                        }

                        EditorGUILayout.HelpBox(message.ToString(), MessageType.Warning);
                    }
                }

                if (missingLayers)
                {
                    EditorGUILayout.LabelField("Missing Layers!", EditorStyles.boldLabel);

                    using (new GuiScopedIndent())
                    {
                        StringBuilder message = new StringBuilder("Layers are missing in your project settings. Open the Tag Manager, add these missing layers, and reimport:");
                        message.AppendLine();
                        message.AppendLine();

                        foreach (var layer in TargetAssetImporter.MissingLayers)
                        {
                            message.AppendFormat("    {0}\n", layer);
                        }

                        EditorGUILayout.HelpBox(message.ToString(), MessageType.Warning);
                    }
                }
            }

            using (new GuiScopedHorizontal())
            {
                if (GUILayout.Button("Open Tag Manager"))
                {
                    EditorApplication.ExecuteMenuItem("Edit/Project Settings/Tags and Layers");
                }

                if (GUILayout.Button("Reimport"))
                {
                    ApplyAndImport();
                }
            }

            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
        }

        private void DisplayDependencies()
        {
            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.LabelField("[Dependencies are not diplayed while Editor is in Play Mode]", EditorStyles.boldLabel);
                return;
            }

            AssetDependencies depends;
            if (!TiledAssetDependencies.Instance.GetAssetDependencies(TargetAssetImporter.assetPath, out depends))
            {
                return;
            }

            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField("Additional Tiled Asset Information", EditorStyles.boldLabel);

            using (new GuiScopedIndent())
            {
                using (new GuiScopedIndent())
                {
                    var title = string.Format("Dependencies ({0})", depends.Dependencies.Count());
                    var tip = "This asset will be automatically reimported when any of these other assets are updated.";
                    var content = new GUIContent(title, tip);
                    m_ShowDependencies = EditorGUILayout.Foldout(m_ShowDependencies, content);

                    if (m_ShowDependencies)
                    {
                        foreach (var asset in depends.Dependencies)
                        {
                            EditorGUILayout.LabelField(asset);

                            // We can reimport a dependency with right click
                            var clickArea = GUILayoutUtility.GetLastRect();
                            var current = Event.current;
                            if (clickArea.Contains(current.mousePosition) && current.type == EventType.ContextClick)
                            {
                                var text = string.Format("Remport '{0}'", Path.GetFileName(asset));

                                var menu = new GenericMenu();
                                menu.AddItem(new GUIContent(text), false, MenuCallbackReimport, asset);
                                menu.ShowAsContext();
                                current.Use();
                            }
                        }
                    }
                }

                using (new GuiScopedIndent())
                {
                    var title = string.Format("References ({0})", depends.References.Count());
                    var tip = "This asset is used by this list of other assets.";
                    var content = new GUIContent(title, tip);
                    m_ShowReferences = EditorGUILayout.Foldout(m_ShowReferences, content);

                    if (m_ShowReferences)
                    {
                        foreach (var asset in depends.References)
                        {
                            EditorGUILayout.LabelField(asset);
                        }
                    }
                }
            }
        }

        private Color GetBackgroundColor()
        {
            if (TargetAssetImporter.Errors.Any())
            {
                return Color.red;
            }
            else if (TargetAssetImporter.Warnings.Any())
            {
                return Color.yellow;
            }

            return GUI.backgroundColor;
        }

        private void MenuCallbackReimport(object asset)
        {
            string assetPath = asset.ToString();
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }
    }
}
