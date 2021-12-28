using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(TMP_ExpandEffectContainer))]
public class TMP_ExpandEffectContainerEditor : Editor
{
    private TMP_ExpandEffectContainer _target => target as TMP_ExpandEffectContainer;

    //通过ReorderableList实现自定义列表
    private ReorderableList _expandEffectArray;

    //提示文字的字体
    private readonly GUIStyle hintFontStyle = new GUIStyle();

    //根据名字排序
    private void Editor_ExpandEffectsSort()
    {
        _target.expandEffects.Sort((a, b) =>
        {
            var aLength = a == null ? 2147483647 : a.richTextName.Length;
            var bLength = b == null ? 2147483647 : b.richTextName.Length;
            return aLength.CompareTo(bLength);
        });
    }
    private void OnEnable()
    {
        
        hintFontStyle.normal.textColor = Color.yellow;
        hintFontStyle.fontSize = 14;
        hintFontStyle.fontStyle = FontStyle.Bold;

        _expandEffectArray = new ReorderableList(serializedObject, serializedObject.FindProperty("expandEffects")
            , true, true, true, true)
        {
            //自定义列表名称
            drawHeaderCallback = rect =>
            {
                GUI.Label(rect, "ExpandEffects");
                Editor_ExpandEffectsSort();
            },
            //定义元素的高度
            elementHeight = 20
        };

        //自定义绘制列表元素
        _expandEffectArray.drawElementCallback = (rect, index, selected, focused) =>
        {
            //根据index获取对应元素
            SerializedProperty item = _expandEffectArray.serializedProperty.GetArrayElementAtIndex(index);
            rect.height -= 4;
            rect.y += 2;
            string richTextName = "";
            if (_target.expandEffects.Count - 1 >= index && _target.expandEffects[index] != null)
            {
                richTextName = _target.expandEffects[index].richTextName;
            }

            EditorGUI.PropertyField(rect, item, new GUIContent("RichText : " + richTextName));
        };

        //当鼠标抬起时调用回调函数，对_ExpandEffects按照富文本名字排序
        _expandEffectArray.onMouseUpCallback = list =>
        {
            Editor_ExpandEffectsSort();
        };
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SerializedProperty iterator = serializedObject.GetIterator();
        for (var enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
        {
            using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
            {
                //其它属性照常绘制，当绘制TMP_BaseRichText数组时使用ReorderableList进行绘制
                if (iterator.isArray && iterator.arrayElementType == "PPtr<$"+typeof(TMP_BaseRichText)+">")
                {
                    EditorGUILayout.Space(10);
                    //自动布局绘制列表
                    _expandEffectArray.DoLayoutList();
                    EditorGUILayout.LabelField("注意:", hintFontStyle);
                    EditorGUILayout.LabelField("ExpandEffects中不能有空值或RichText名相同的", hintFontStyle);
                    EditorGUILayout.Space(10);
                }
                else
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
            }
        }    
        serializedObject.ApplyModifiedProperties();
    }
}

    
