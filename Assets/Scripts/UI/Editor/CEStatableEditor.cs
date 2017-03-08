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
	[CustomEditor(typeof(CEStatable), true)]
	public class CEStatableEditor : Editor
	{
		SerializedProperty m_Script;
		SerializedProperty m_TargetGraphicProperty;
		SerializedProperty m_StatableGroupProperty;
		SerializedProperty m_TransitionProperty;
		SerializedProperty m_ColorBlockProperty;
		SerializedProperty m_SpriteStateProperty;

		AnimBool m_ShowColorTint = new AnimBool();
		AnimBool m_ShowSpriteTrasition = new AnimBool();

		SerializedProperty m_curCaseProperty;

		//SerializedProperty m_statableTransitionEvent;

		// Whenever adding new SerializedProperties to the Selectable and SelectableEditor
		// Also update this guy in OnEnable. This makes the inherited classes from Selectable not require a CustomEditor.
		private string[] m_PropertyPathToExcludeForChildClasses;

// 		CEStatable.StatableCase m_curCase = CEStatable.StatableCase.Normal;

		protected virtual void OnEnable()
		{
			m_Script = serializedObject.FindProperty("m_Script");
			m_TargetGraphicProperty = serializedObject.FindProperty("m_targetGraphic");
			m_StatableGroupProperty = serializedObject.FindProperty("m_statableGroup");
			m_TransitionProperty = serializedObject.FindProperty("m_Transition");
			m_ColorBlockProperty = serializedObject.FindProperty("m_Colors");
			m_SpriteStateProperty = serializedObject.FindProperty("m_SpriteState");
			//m_statableTransitionEvent = serializedObject.FindProperty("m_statableTransitionEvent");

			m_curCaseProperty = serializedObject.FindProperty("m_curCase");

			m_PropertyPathToExcludeForChildClasses = new[]
			{
				m_Script.propertyPath,
				m_TransitionProperty.propertyPath,
				m_ColorBlockProperty.propertyPath,
				m_SpriteStateProperty.propertyPath,
				m_TargetGraphicProperty.propertyPath,

				m_curCaseProperty.propertyPath,
			};

			var trans = GetTransition(m_TransitionProperty);
			m_ShowColorTint.value = (trans == CEStatable.TransitionType.ColorTint);
			m_ShowSpriteTrasition.value = (trans == CEStatable.TransitionType.SpriteSwap);

			m_ShowColorTint.valueChanged.AddListener(Repaint);
			m_ShowSpriteTrasition.valueChanged.AddListener(Repaint);

			var statableCase = GetStatableCase(m_curCaseProperty);

			//m_curCase = statableCase;
		}

		protected virtual void OnDisable()
		{
			m_ShowColorTint.valueChanged.RemoveListener(Repaint);
			m_ShowSpriteTrasition.valueChanged.RemoveListener(Repaint);
		}


		static CEStatable.TransitionType GetTransition(SerializedProperty transition)
		{
			return (CEStatable.TransitionType)transition.enumValueIndex;
		}

		static CEStatable.StatableCase GetStatableCase(SerializedProperty statable)
		{
			return (CEStatable.StatableCase)statable.enumValueIndex;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			if (!IsDerivedSelectableEditor())
				EditorGUILayout.PropertyField(m_Script);

			var trans = GetTransition(m_TransitionProperty);

			var graphic = m_TargetGraphicProperty.objectReferenceValue as Graphic;
			if (graphic == null)
				graphic = (target as CEStatable).GetComponent<Graphic>();

			m_ShowColorTint.target = (!m_TransitionProperty.hasMultipleDifferentValues && trans == CEStatable.TransitionType.ColorTint);
			m_ShowSpriteTrasition.target = (!m_TransitionProperty.hasMultipleDifferentValues && trans == CEStatable.TransitionType.SpriteSwap);

			EditorGUILayout.PropertyField(m_TransitionProperty);

			++EditorGUI.indentLevel;
			{
				if (trans == CEStatable.TransitionType.ColorTint || trans == CEStatable.TransitionType.SpriteSwap)
				{
					EditorGUILayout.PropertyField(m_TargetGraphicProperty);
				}

				switch (trans)
				{
					case CEStatable.TransitionType.ColorTint:
						if (graphic == null)
							EditorGUILayout.HelpBox("You must have a Graphic target in order to use a color transition.", MessageType.Warning);
						break;

					case CEStatable.TransitionType.SpriteSwap:
						if (graphic as Image == null)
							EditorGUILayout.HelpBox("You must have a Image target in order to use a sprite swap transition.", MessageType.Warning);
						break;
				}

				EditorGUILayout.Space();

				EditorGUILayout.PropertyField(m_curCaseProperty);
				EditorGUI.BeginChangeCheck();
				var statableCase = GetStatableCase(m_curCaseProperty);
				{
					//if (m_curCase != statableCase)
					{
						CEStatable statableCtrl = target as CEStatable;
						if (statableCtrl != null)
							statableCtrl.SetStatableCase(statableCase, trans);

						//m_curCase = statableCase;
					}
				}


				if (EditorGUILayout.BeginFadeGroup(m_ShowColorTint.faded))
				{
					EditorGUILayout.PropertyField(m_ColorBlockProperty);
				}
				EditorGUILayout.EndFadeGroup();

				if (EditorGUILayout.BeginFadeGroup(m_ShowSpriteTrasition.faded))
				{
					EditorGUILayout.PropertyField(m_SpriteStateProperty);
				}
				EditorGUILayout.EndFadeGroup();
			}
			--EditorGUI.indentLevel;

			EditorGUILayout.Space();


			EditorGUI.BeginChangeCheck();
			Rect toggleRect = EditorGUILayout.GetControlRect();
			toggleRect.xMin += EditorGUIUtility.labelWidth;

			// We do this here to avoid requiring the user to also write a Editor for their Selectable-derived classes.
			// This way if we are on a derived class we dont draw anything else, otherwise draw the remaining properties.
			ChildClassPropertiesGUI();

			serializedObject.ApplyModifiedProperties();
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
			return GetType() != typeof(CEStatableEditor);
		}
	}
}

