using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UnityEngine.Serialization;


using System;
using System.Collections;

namespace CEUI
{
	[Serializable]
	public class CEStatable : UIBehaviour
	{
		public enum StatableCase
		{
			Normal,
			Selected,
			Pressed,
			Disabled
		}

		public enum TransitionType
		{
			None,
			ColorTint,
			SpriteSwap,
		}

		public TransitionType trasitionType
		{
			get { return m_Transition; }
			set { m_Transition = value; SetStatableCase(m_curCase, value); }
		}

		public Graphic targetGrphic
		{
			get { return m_targetGraphic; }
			set { m_targetGraphic = value; SetStatableCase(m_curCase, m_Transition); }
		}

		/// <summary>
		/// Transit oldCase to new case
		/// </summary>
		[Serializable]
		public class CEStableTransitionEvent : UnityEvent<StatableCase /*oldState*/, StatableCase /*newState*/, CEStatable/*sender*/> { }

		[SerializeField] protected Graphic m_targetGraphic;

		[SerializeField] protected CEStatableGroup m_statableGroup = null;

		[SerializeField] protected StatableCase m_curCase = StatableCase.Normal;

	
		// Type of the transition that occurs when the button state changes.
		[SerializeField] private TransitionType m_Transition = TransitionType.ColorTint;

		// Colors used for a color tint-based transition.
		[SerializeField] private ColorBlock m_Colors = ColorBlock.defaultColorBlock;

		// Sprites used for a Image swap-based transition.
		[SerializeField] private SpriteState m_SpriteState;

		[SerializeField] protected CEStableTransitionEvent m_statableTransitionEvent = new CEStableTransitionEvent();



		protected Image image { 
			get { return m_targetGraphic as Image; }
			set { m_targetGraphic = value; }
		}


		public StatableCase curState { 
			get { return m_curCase; }
			set { SetStatableCase(value); }
		}



		public StatableCase SetStatableCase(StatableCase _newCase, TransitionType _type)
		{
			return DoStateTransition(_newCase, _type);
		}

		public StatableCase SetStatableCase(StatableCase _newCase)
		{
			return DoStateTransition(_newCase, m_Transition);
		}

	
		protected override void Awake()
		{
			if (m_targetGraphic == null)
				m_targetGraphic = GetComponent<Graphic>();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			if (m_statableGroup != null)
				m_statableGroup.RegisterCtrl(this);
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			if (m_statableGroup != null)
				m_statableGroup.UnRegisterCtrl(this);
		}


		protected virtual void InstantClearState()
		{
			switch (m_Transition)
			{
				case TransitionType.ColorTint:
					StartColorTween(Color.white);
					break;
				case TransitionType.SpriteSwap:
					DoSpriteSwap(null);
					break;
				default:
					m_targetGraphic.canvasRenderer.SetColor(m_targetGraphic.color);
					if (image != null)
						image.overrideSprite = null;
					break;
			}
		}

		protected virtual StatableCase DoStateTransition(StatableCase _newCase, TransitionType _type)
		{
			Color tintColor;
			Sprite transitionSprite;

			StatableCase oldCase = m_curCase;
			m_curCase = _newCase;

			m_Transition = _type;

			switch (_newCase)
			{
				case StatableCase.Normal:
					tintColor = m_Colors.normalColor;
					transitionSprite = null;
					break;
				case StatableCase.Selected:
					tintColor = m_Colors.highlightedColor;
					transitionSprite = m_SpriteState.highlightedSprite;
					break;
				case StatableCase.Pressed:
					tintColor = m_Colors.pressedColor;
					transitionSprite = m_SpriteState.pressedSprite;
					break;
				case StatableCase.Disabled:
					tintColor = m_Colors.disabledColor;
					transitionSprite = m_SpriteState.disabledSprite;
					break;
				default:
					tintColor = Color.black;
					transitionSprite = null;
					break;
			}

			if (gameObject.activeInHierarchy)
			{
				switch (m_Transition)
				{
					case TransitionType.ColorTint:
						StartColorTween(tintColor * m_Colors.colorMultiplier);
						break;
					case TransitionType.SpriteSwap:
						DoSpriteSwap(transitionSprite);
						break;
					default:
						m_targetGraphic.canvasRenderer.SetColor(m_targetGraphic.color);
						if (image != null)
							image.overrideSprite = null;
						break;
				}
			}

			if (m_statableTransitionEvent != null)
				m_statableTransitionEvent.Invoke(oldCase, _newCase, this);

			return oldCase;
		}

		void StartColorTween(Color targetColor)
		{
			if (m_targetGraphic == null)
				return;

			m_targetGraphic.canvasRenderer.SetColor(targetColor);
		}

		void DoSpriteSwap(Sprite newSprite)
		{
			if (image == null)
				return;
			image.canvasRenderer.SetColor(image.color);
			image.overrideSprite = newSprite;
		}


	}


}
