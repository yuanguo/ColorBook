using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TweenColor))]
public class TweenColorEditor : TweenerEditor 
{
    public override void OnInspectorGUI()
    {
        GUILayout.Space(6f);
        UIEditorTools.SetLabelWidth(120f);

        TweenColor tw = target as TweenColor;

        GUI.changed = false;

        Color from = EditorGUILayout.ColorField("From", tw.From);
        Color to = EditorGUILayout.ColorField("To", tw.To);

        if (GUI.changed)
        {
            UIEditorTools.RegisterUndo("Tween Change", tw);
            tw.From = from;
            tw.To = to;

            UnityEditor.EditorUtility.SetDirty(tw);
        }

        DrawCommonProperties();
    }

}
