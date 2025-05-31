using UnityEngine;
using UnityEditor;

namespace UI.Tables.Editor
{
    public class TableLayoutWizard : EditorWindow
    {
        private int numberOfRows = 3;
        private int numberOfColumns = 3;


        [MenuItem("GameObject/UI/TableLayout/Add New Table")]
        private static void AddTableLayout()
        {
            var window = GetWindow<TableLayoutWizard>();
            window.Show();

            var width = 380f;
            var height = 110f;

            window.titleContent = new GUIContent("Add New TableLayout");
            window.position = new Rect((Screen.currentResolution.width - width) / 2f,
                (Screen.currentResolution.height - height) / 2f,
                width,
                height);
        }

        private void OnGUI()
        {
            var style = new GUIStyle();
            style.margin = new RectOffset(10, 10, 10, 10);

            GUILayout.BeginVertical(style);

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Number of Rows");
            numberOfRows = EditorGUI.IntSlider(EditorGUILayout.GetControlRect(), numberOfRows, 0, 32);
            GUILayout.EndHorizontal();

            if (numberOfRows == 0) EditorGUI.BeginDisabledGroup(true);
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Number of Columns");
            numberOfColumns = EditorGUI.IntSlider(EditorGUILayout.GetControlRect(), numberOfColumns, 0, 32);
            GUILayout.EndHorizontal();
            if (numberOfRows == 0) EditorGUI.EndDisabledGroup();

            GUILayout.Space(16);

            if (GUILayout.Button("Add Table Layout"))
            {
                CreateTable(numberOfRows, numberOfColumns);
                Close();
            }

            if (GUILayout.Button("Cancel")) Close();
            GUILayout.EndVertical();
        }

        private void CreateTable(int rows, int columns)
        {
            var gameObject = TableLayoutUtilities.InstantiatePrefab("TableLayout/TableLayout");
            gameObject.name = "TableLayout";

            var tableLayout = gameObject.GetComponent<TableLayout>();

            for (var x = 0; x < rows; x++) tableLayout.AddRow(columns);

            Selection.activeObject = gameObject;
        }
    }
}