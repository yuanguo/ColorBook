using UnityEngine;
using UnityEngine.UI;

using UnityEditor;
using UnityEditor.AnimatedValues;

using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using CEUI;


namespace CEUIEditor
{
	[CustomEditor(typeof(TabButton), true)]
	[CanEditMultipleObjects]
	public class TabButtonEditor : Editor
	{
		SerializedProperty m_Script;

		SerializedProperty m_interactableProperty;

		SerializedProperty m_curCaseProperty;

		SerializedProperty m_GraphicForImageProperty;
		SerializedProperty m_TransitionForImageProperty;
		SerializedProperty m_ColorBlockForImageProperty;
		SerializedProperty m_SpriteStateForImageProperty;


		SerializedProperty m_GraphicForTextProperty;
		SerializedProperty m_ColorBlockForTextProperty;
		
		AnimBool m_ShowColorTint = new AnimBool();
		AnimBool m_ShowSpriteTrasition = new AnimBool();


		//SerializedProperty m_statableTransitionEvent;

		// Whenever adding new SerializedProperties to the Selectable and SelectableEditor
		// Also update this guy in OnEnable. This makes the inherited classes from Selectable not require a CustomEditor.
		private string[] m_PropertyPathToExcludeForChildClasses;

		// 		CEStatable.StatableCase m_curCase = CEStatable.StatableCase.Normal;

		protected virtual void OnEnable()
		{
			m_Script = serializedObject.FindProperty("m_Script");

			m_interactableProperty = serializedObject.FindProperty("m_interactable");

			m_curCaseProperty = serializedObject.FindProperty("m_curCase");

			m_GraphicForImageProperty = serializedObject.FindProperty("m_targetGraphicForImage");
			m_TransitionForImageProperty = serializedObject.FindProperty("m_TransitionForImage");
			m_ColorBlockForImageProperty = serializedObject.FindProperty("m_ColorsForImage");
			m_SpriteStateForImageProperty = serializedObject.FindProperty("m_SpriteState");

			m_GraphicForTextProperty = serializedObject.FindProperty("m_targetGraphicForText");
			m_ColorBlockForTextProperty = serializedObject.FindProperty("m_ColorsForText");

			//m_statableTransitionEvent = serializedObject.FindProperty("m_statableTransitionEvent");


			m_PropertyPathToExcludeForChildClasses = new[]
			{
				m_Script.propertyPath,
				m_interactableProperty.propertyPath,
				m_curCaseProperty.propertyPath,

				m_TransitionForImageProperty.propertyPath,
				m_ColorBlockForImageProperty.propertyPath,
				m_SpriteStateForImageProperty.propertyPath,
				m_GraphicForImageProperty.propertyPath,

				m_GraphicForTextProperty.propertyPath,
				m_ColorBlockForTextProperty.propertyPath,
			};

			var trans = GetTransition(m_TransitionForImageProperty);
			m_ShowColorTint.value = (trans == TabButton.TransitionType.ColorTint);
			m_ShowSpriteTrasition.value = (trans == TabButton.TransitionType.SpriteSwap);

			m_ShowColorTint.valueChanged.AddListener(Repaint);
			m_ShowSpriteTrasition.valueChanged.AddListener(Repaint);
		}

		protected virtual void OnDisable()
		{
			m_ShowColorTint.valueChanged.RemoveListener(Repaint);
			m_ShowSpriteTrasition.valueChanged.RemoveListener(Repaint);
		}


		static TabButton.TransitionType GetTransition(SerializedProperty transition)
		{
			return (TabButton.TransitionType)transition.enumValueIndex;
		}

		static TabButton.StatableCase GetStatableCase(SerializedProperty statable)
		{
			return (TabButton.StatableCase)statable.enumValueIndex;
		}

		public override void OnInspectorGUI()
		{
			foreach (Object targetTemp in targets)
			{
				serializedObject.Update();
				var trans = GetTransition(m_TransitionForImageProperty);

				EditorGUILayout.PropertyField(m_interactableProperty);
				EditorGUILayout.Space();

				EditorGUILayout.PropertyField(m_curCaseProperty);
				EditorGUILayout.Space();
				EditorGUI.BeginChangeCheck();

				var graphic = m_GraphicForImageProperty.objectReferenceValue as Graphic;
				if (graphic == null)
					graphic = (targetTemp as TabButton).GetComponent<Graphic>();

				m_ShowColorTint.target = (!m_TransitionForImageProperty.hasMultipleDifferentValues && trans == TabButton.TransitionType.ColorTint);
				m_ShowSpriteTrasition.target = (!m_TransitionForImageProperty.hasMultipleDifferentValues && trans == TabButton.TransitionType.SpriteSwap);

				EditorGUILayout.PropertyField(m_GraphicForImageProperty);
				switch (trans)
				{
					case TabButton.TransitionType.ColorTint:
						if (graphic == null)
							EditorGUILayout.HelpBox("You must have a Graphic target in order to use a color transition.", MessageType.Warning);
						break;

					case TabButton.TransitionType.SpriteSwap:
						if (graphic as Image == null)
							EditorGUILayout.HelpBox("You must have a Image target in order to use a sprite swap transition.", MessageType.Warning);
						break;
				}

				++EditorGUI.indentLevel;
				{
					EditorGUILayout.PropertyField(m_TransitionForImageProperty);


					if (EditorGUILayout.BeginFadeGroup(m_ShowColorTint.faded))
					{
						EditorGUILayout.PropertyField(m_ColorBlockForImageProperty);
					}
					EditorGUILayout.EndFadeGroup();

					if (EditorGUILayout.BeginFadeGroup(m_ShowSpriteTrasition.faded))
					{
						EditorGUILayout.PropertyField(m_SpriteStateForImageProperty);
					}
					EditorGUILayout.EndFadeGroup();
				}
				--EditorGUI.indentLevel;

				EditorGUILayout.Space();

				EditorGUILayout.PropertyField(m_GraphicForTextProperty);
				graphic = m_GraphicForTextProperty.objectReferenceValue as Graphic;

				if (graphic == null)
					EditorGUILayout.HelpBox("You must have a Graphic target in order to use a color transition.", MessageType.Warning);
				else
				{
					++EditorGUI.indentLevel;
					{
						EditorGUILayout.PropertyField(m_ColorBlockForTextProperty);
					} --EditorGUI.indentLevel;


					EditorGUI.BeginChangeCheck();
					Rect toggleRect = EditorGUILayout.GetControlRect();
					toggleRect.xMin += EditorGUIUtility.labelWidth;
				}

				var statableCase = GetStatableCase(m_curCaseProperty);
				{
					TabButton statableCtrl = targetTemp as TabButton;
					if (statableCtrl != null)
						statableCtrl.SetStatableCase(statableCase, trans);
				}

				EditorGUILayout.Space();

				// We do this here to avoid requiring the user to also write a Editor for their Selectable-derived classes.
				// This way if we are on a derived class we dont draw anything else, otherwise draw the remaining properties.
				ChildClassPropertiesGUI();

				serializedObject.ApplyModifiedProperties();
			}

		}

		// Draw the extra SerializedProperties of the child class.
		// We need to make sure that m_PropertyPathToExcludeForChildClasses has all the Selectable properties and in the correct order.
		// TODO: find a nicer way of doing this. (creating a InheritedEditor class that automagically does this)
		private void ChildClassPropertiesGUI()
		{
			if (IsDerivedSelectableEditor())
				return;

			DrawPropertiesExcluding(serializedObject, m_PropertyPathToExcludeForChildClasses);
		}

		private bool IsDerivedSelectableEditor()
		{
			return GetType() != typeof(TabButtonEditor);
		}
	}
}
