using System;
using System.Reflection;
using Assets.AstarPathfindingProject.Core.Misc;
using UnityEditor;
using UnityEngine;

namespace Assets.AstarPathfindingProject.Editor.Misc
{
    [CustomPropertyDrawer(typeof(AstarEnumFlagAttribute))]
    public class AstarEnumFlagDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            AstarEnumFlagAttribute flagSettings = (AstarEnumFlagAttribute)attribute;
            Enum targetEnum = GetBaseProperty<Enum>(property);
		
            string propName = flagSettings.enumName;
            if (string.IsNullOrEmpty(propName))
                propName = ObjectNames.NicifyVariableName (property.name);

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            Enum enumNew = EditorGUI.EnumMaskField(position, propName, targetEnum);
            bool changed = EditorGUI.EndChangeCheck();
            if (!property.hasMultipleDifferentValues || changed) {
                property.intValue = (int) Convert.ChangeType(enumNew, targetEnum.GetType());
            }
            EditorGUI.EndProperty();
        }
	
        static T GetBaseProperty<T>(SerializedProperty prop)
        {
            // Separate the steps it takes to get to this property
            string[] separatedPaths = prop.propertyPath.Split('.');
		
            // Go down to the root of this serialized property
            System.Object reflectionTarget = prop.serializedObject.targetObject as object;
            // Walk down the path to get the target object
            foreach (var path in separatedPaths)
            {
                FieldInfo fieldInfo = reflectionTarget.GetType().GetField(path);
                reflectionTarget = fieldInfo.GetValue(reflectionTarget);
            }
            return (T) reflectionTarget;
        }
    }
}

