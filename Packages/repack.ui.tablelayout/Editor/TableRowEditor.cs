using UnityEngine;
using UnityEditor;

namespace UI.Tables.Editor
{
    [CustomEditor(typeof(TableRow))]
    [CanEditMultipleObjects]
    public class TableRowEditor : UnityEditor.Editor
    {
        //private SerializedObject serializedObject;
        private TableRow TableRow;

        private SerializedProperty preferredHeight;
        private SerializedProperty dontUseTableRowBackground;


        public void OnEnable()
        {
            //serializedObject = new SerializedObject(target);
            TableRow = (TableRow)target;

            preferredHeight = serializedObject.FindProperty("preferredHeight");
            dontUseTableRowBackground = serializedObject.FindProperty("dontUseTableRowBackground");
        }

        public void OnDisable()
        {
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(preferredHeight);
            EditorGUILayout.PropertyField(dontUseTableRowBackground);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                TableRow.preferredHeight = preferredHeight.floatValue;
                TableRow.dontUseTableRowBackground = dontUseTableRowBackground.boolValue;

                Repaint();
            }

            if (GUILayout.Button("Add Cell")) TableRow.AddCell();

            TableRow.NotifyTableRowPropertiesChanged();
        }
    }
}