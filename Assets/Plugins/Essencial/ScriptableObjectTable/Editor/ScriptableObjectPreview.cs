using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace EnlitGames.ScriptableObjectTable{
    public class ScriptableObjectPreview : EditorWindow{
        static readonly Color greyBackgroundColor = new(0.16f, 0.16f, 0.16f);
        static readonly Color defaultBackgroundColor = new(0.2f, 0.2f, 0.2f);
        static ScriptableObject selectedScriptableObject;
        static bool showWarningForUndisplayedFields;
        static bool hideReadOnlyFields;
        bool scale_swap = true;

        [MenuItem("Window/Scriptable Object Table")]
        public static void ShowExample(){
            var wnd = GetWindow<ScriptableObjectPreview>();
            wnd.titleContent = new GUIContent("Scriptable Object Table");
        }

        public void CreateGUI(){
            showWarningForUndisplayedFields = false;
            var root = rootVisualElement;
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Plugins/ScriptableObjectTable/Editor/ScriptableObjectPreview.uxml");
            VisualElement ScriptableObjectTable = visualTree.Instantiate();
            root.Add(ScriptableObjectTable);


            ObjectField ScriptableObjectSelection = root.Query<ObjectField>("ScriptableObjectSelection");
            ScriptableObjectSelection.RegisterValueChangedCallback(evt => {
                PopulateTable((ScriptableObject)evt.newValue);
            });
            ScriptableObjectSelection.value = selectedScriptableObject;

            Toggle HideReadOnlyFields = root.Query<Toggle>("HideReadOnlyFields");
            HideReadOnlyFields.RegisterValueChangedCallback(evt => { HideReadOnlyFieldsToggled(evt.newValue); });
            HideReadOnlyFields.value = hideReadOnlyFields;
        }

        void HideReadOnlyFieldsToggled(bool newValue){
            hideReadOnlyFields = newValue;
            PopulateTable(selectedScriptableObject);
            ForceUpdateScrollViewScale();
        }

        void PopulateTable(ScriptableObject newSelectedScriptableObject){
            var root = rootVisualElement;
            VisualElement scrollview = root.Query<ScrollView>("scroll-view-wrap-example");
            scrollview.Clear();
            if (newSelectedScriptableObject != null){
                var scriptableObject = newSelectedScriptableObject;
                selectedScriptableObject = scriptableObject;
                ShowSelectedScriptableObject(scriptableObject, scrollview);
            }
        }

        void ShowSelectedScriptableObject(ScriptableObject scriptableObject, VisualElement scrollview){
            var scriptableObjectDataList = GetScriptableObjectDataList(scriptableObject);

            var pathColumnWidth = ColumnWidthCalculator.FindScriptableObjectPathColumnWidth(scriptableObjectDataList);
            var columnWidths = ColumnWidthCalculator.FindColumnWidths(scriptableObjectDataList);

            if (showWarningForUndisplayedFields){
                ShowWarningForUndisplayedFields();
            }
            else{
                HideWarningForUndisplayedFields();
            }

            ShowHeader(scriptableObjectDataList[0], scrollview, pathColumnWidth, columnWidths);
            for (var i = 0; i < scriptableObjectDataList.Count; i++){
                var colorRowGrey = i % 2 == 0;
                ShowScriptableObjectInstance(scriptableObjectDataList[i], scrollview, pathColumnWidth, columnWidths,
                    colorRowGrey);
            }
        }

        void ShowWarningForUndisplayedFields(){
            Label warning = rootVisualElement.Query<Label>("Warning");
            warning.text =
                "Some fields are not displayed because they are not serializable. You can make them serializable by adding the [SerializeField] attribute to them.";
        }

        void HideWarningForUndisplayedFields(){
            Label warning = rootVisualElement.Query<Label>("Warning");
            warning.text = "";
        }

        void ShowHeader(ScriptableObjectData scriptableObjectData, VisualElement scrollview, float pathColumnWidth,
            List<float> columnWidths){
            var headerRow = new VisualElement();
            headerRow.style.flexDirection = FlexDirection.Row;
            scrollview.Add(headerRow);
            var pathHeader = new Label("File Path");
            pathHeader.style.width = pathColumnWidth;
            headerRow.Add(pathHeader);
            for (var i = 0; i < scriptableObjectData.fields.Count; i++){
                var fieldHeader = new Label(scriptableObjectData.fields[i].Name);
                fieldHeader.style.width = columnWidths[i];

                headerRow.Add(fieldHeader);
            }
        }

        void ShowScriptableObjectInstance(ScriptableObjectData scriptableObjectData, VisualElement scrollview,
            float columnWidth, List<float> columnWidths, bool colorRowGrey){
            var scriptableObjectInstanceRow = new VisualElement();
            scriptableObjectInstanceRow.style.flexDirection = FlexDirection.Row;
            scrollview.Add(scriptableObjectInstanceRow);
            var pathLabel = new Label(scriptableObjectData.path);
            if (colorRowGrey){
                pathLabel.style.backgroundColor = greyBackgroundColor;
            }
            else{
                pathLabel.style.backgroundColor = defaultBackgroundColor;
            }
            pathLabel.style.width = columnWidth;
            pathLabel.RegisterCallback<MouseUpEvent>(evt => {
                Selection.activeObject = scriptableObjectData.scriptableObjectInstance;
            });

            scriptableObjectInstanceRow.Add(pathLabel);
            for (var i = 0; i < scriptableObjectData.fields.Count; i++){
                var element = MakeVisualElementForValue(scriptableObjectData.fields[i]
                    .GetValue(scriptableObjectData.scriptableObjectInstance));
                var fieldName = scriptableObjectData.fields[i].Name;
                if (element is Label){
                    element.RegisterCallback<MouseUpEvent>(evt => {
                        Selection.activeObject = scriptableObjectData.scriptableObjectInstance;
                    });
                }
                element.tooltip = scriptableObjectData.fields[i].Name;

                var so = new SerializedObject(scriptableObjectData.scriptableObjectInstance);

                if (element is IBindable){
                    var property = so.FindProperty(scriptableObjectData.fields[i].Name);
                    if (property != null){
                        ((IBindable)element).BindProperty(property);
                    }
                    else{
                        Debug.LogWarning("Could not find property " + scriptableObjectData.fields[i].Name + " on " +
                                         scriptableObjectData.scriptableObjectInstance.name);
                    }
                }

                element.style.width = columnWidths[i];

                //set background of every second column to grey
                if (colorRowGrey){
                    element.style.backgroundColor = greyBackgroundColor;
                    //get children and change their background color
                    foreach (var child in element.Children()){
                        child.style.backgroundColor = greyBackgroundColor;
                        //get children and change their background color
                        foreach (var grandChild in child.Children())
                            grandChild.style.backgroundColor = greyBackgroundColor;
                    }
                }
                else{
                    element.style.backgroundColor = defaultBackgroundColor;
                    //get children and change their background color
                    foreach (var child in element.Children()){
                        child.style.backgroundColor = defaultBackgroundColor;
                        //get children and change their background color
                        foreach (var grandChild in child.Children())
                            grandChild.style.backgroundColor = defaultBackgroundColor;
                    }
                }

                scriptableObjectInstanceRow.Add(element);
            }
        }

        VisualElement MakeVisualElementForValue(dynamic value){
            VisualElement visualElement;
            if ((object)value != null){
                visualElement = new Label(value.ToString());
            }
            else{
                value = null;
                visualElement = new Label("null");
            }
            if (value.GetType() == typeof(Color) || value.GetType() == typeof(Color32)){
                visualElement = new ColorField();
                ((ColorField)visualElement).SetValueWithoutNotify(value);
            }
            if (value.GetType() == typeof(Vector2)){
                visualElement = new Vector2Field();
                ((Vector2Field)visualElement).SetValueWithoutNotify(value);
            }
            if (value.GetType() == typeof(Vector3)){
                visualElement = new Vector3Field();
                ((Vector3Field)visualElement).SetValueWithoutNotify(value);
            }
            if (value.GetType() == typeof(Vector4)){
                visualElement = new Vector4Field();
                ((Vector4Field)visualElement).SetValueWithoutNotify(value);
            }
            if (value.GetType() == typeof(Rect)){
                visualElement = new RectField();
                ((RectField)visualElement).SetValueWithoutNotify(value);
            }
            if (value.GetType() == typeof(Bounds)){
                visualElement = new BoundsField();
                ((BoundsField)visualElement).SetValueWithoutNotify(value);
            }
            if (value.GetType() == typeof(Transform) || value.GetType() == typeof(Object) ||
                value.GetType() == typeof(GameObject) || value.GetType() == typeof(Component) ||
                value.GetType().IsSubclassOf(typeof(ScriptableObject)) || value.GetType() == typeof(Sprite)){
                visualElement = new ObjectField();
                ((ObjectField)visualElement).objectType = value.GetType();
                ((ObjectField)visualElement).SetValueWithoutNotify(value);
                ((ObjectField)visualElement).allowSceneObjects = false;
            }
            if (value.GetType() == typeof(AnimationCurve)){
                visualElement = new CurveField();
                ((CurveField)visualElement).SetValueWithoutNotify(value);
            }
            if (value.GetType() == typeof(Gradient)){
                visualElement = new GradientField();
                ((GradientField)visualElement).SetValueWithoutNotify(value);
            }
            if (value.GetType() == typeof(LayerMask)){
                visualElement = new LayerMaskField();
                ((LayerMaskField)visualElement).SetValueWithoutNotify(value);
            }
            if (value.GetType() == typeof(RectInt)){
                visualElement = new RectIntField();
                ((RectIntField)visualElement).SetValueWithoutNotify(value);
            }
            if (value.GetType() == typeof(BoundsInt)){
                visualElement = new BoundsIntField();
                ((BoundsIntField)visualElement).SetValueWithoutNotify(value);
            }
            if (value.GetType() == typeof(Enum) || value.GetType().IsEnum){
                visualElement = new EnumField();
                ((EnumField)visualElement).SetValueWithoutNotify(value);
            }
            if (value.GetType() == typeof(bool)){
                visualElement = new Toggle();
                ((Toggle)visualElement).SetValueWithoutNotify(value);
            }
            if (value.GetType() == typeof(int)){
                visualElement = new IntegerField();
                ((IntegerField)visualElement).SetValueWithoutNotify(value);
            }
            if (value.GetType() == typeof(float)){
                visualElement = new FloatField();
                ((FloatField)visualElement).SetValueWithoutNotify(value);
            }
            if (value.GetType() == typeof(double)){
                visualElement = new DoubleField();
                ((DoubleField)visualElement).SetValueWithoutNotify(value);
            }
            if (value.GetType() == typeof(string) || value.GetType() == typeof(string) ||
                value.GetType() == typeof(char)){
                visualElement = new TextField();
                ((TextField)visualElement).SetValueWithoutNotify(value);
            }

            return visualElement;
        }


        public static Type GetTypeFromName(string name){
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies){
                var tt = assembly.GetType(name);
                if (tt != null){
                    return tt;
                }
            }
            Debug.LogError("Type not found: " + name);

            return null;
        }

        List<ScriptableObjectData> GetScriptableObjectDataList(ScriptableObject scriptableObject){
            var ScriptableObjectType = GetTypeFromName(scriptableObject.GetType().FullName);
            var fields = GetFieldsToDisplay(scriptableObject);

            var scriptableObjectDataList = new List<ScriptableObjectData>();
            var scriptableObjectPaths = AssetDatabase.FindAssets("t:" + scriptableObject.GetType().FullName);
            foreach (var scriptableObjectPath in scriptableObjectPaths){
                var scriptableObjectData = new ScriptableObjectData();
                scriptableObjectData.name = AssetDatabase.GUIDToAssetPath(scriptableObjectPath);
                scriptableObjectData.type = ScriptableObjectType.ToString();
                scriptableObjectData.path = AssetDatabase.GUIDToAssetPath(scriptableObjectPath);
                scriptableObjectData.scriptableObjectInstance =
                    (ScriptableObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(scriptableObjectPath),
                        ScriptableObjectType);

                scriptableObjectData.fields.AddRange(fields);

                scriptableObjectDataList.Add(scriptableObjectData);
            }

            return scriptableObjectDataList;
        }

        List<FieldInfo> GetFieldsToDisplay(ScriptableObject scriptableObject){
            var ScriptableObjectType = GetTypeFromName(scriptableObject.GetType().FullName);
            var scriptableObjectPaths = AssetDatabase.FindAssets("t:" + scriptableObject.GetType().FullName);
            var scriptableObjectInstance =
                (ScriptableObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(scriptableObjectPaths[0]),
                    ScriptableObjectType);

            var fields =
                new List<FieldInfo>(ScriptableObjectType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic |
                                                                   BindingFlags.Public));
            //remove fields that are not supported
            var invalidFieldsRemoved = new List<FieldInfo>(fields);
            showWarningForUndisplayedFields = false;
            foreach (var field in fields){
                var so = new SerializedObject(scriptableObjectInstance);
                var property = so.FindProperty(field.Name);
                if (property == null){
                    showWarningForUndisplayedFields = true;
                    invalidFieldsRemoved.Remove(field);
                }
                else if (hideReadOnlyFields){
                    if (field.IsInitOnly){
                        invalidFieldsRemoved.Remove(field);
                    }
                    if (MakeVisualElementForValue(field.GetValue(scriptableObjectInstance)) is Label){
                        invalidFieldsRemoved.Remove(field);
                    }
                }
            }

            return invalidFieldsRemoved;
        }

        void ForceUpdateScrollViewScale(){
            //this is a hack-fix for a bug in the scrollview where it doesn't update the scrollbars when the scale changes.
            //https://forum.unity.com/threads/how-to-refresh-scrollview-scrollbars-to-reflect-changed-content-width-and-height.1260920/
            //https://issuetracker.unity3d.com/issues/uitoolkit-scrollview-scroll-bars-arent-refreshed-after-changing-its-size
            var new_len = new StyleLength(Length.Percent(scale_swap ? 99.9f : 100f));
            var scroll_view = rootVisualElement.Q<ScrollView>();
            scroll_view.style.width = new_len;
            scroll_view.style.height = new_len;
            scale_swap = !scale_swap;
        }
    }
}