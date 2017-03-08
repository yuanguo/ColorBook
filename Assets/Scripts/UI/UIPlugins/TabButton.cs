using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

using System;
using System.Collections;

namespace CEUI
{

	public class TabButton : UIBehaviour, IPointerClickHandler
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

		[Serializable]
		public class TabButtonClickEvent : UnityEvent<TabButton/*sender*/> 
		{ 
		}


		[FormerlySerializedAs("interactable")]
		[SerializeField]
		protected bool m_interactable = true;

		[FormerlySerializedAs("Current Case")]
		[SerializeField]
		protected StatableCase m_curCase = StatableCase.Normal;

		[FormerlySerializedAs("Image")]
		[SerializeField]
		protected Graphic m_targetGraphicForImage;
		// Type of the transition that occurs when the button state changes.
		[FormerlySerializedAs("TransitionTypeForImage")]
		[SerializeField]
		private TransitionType m_TransitionForImage = TransitionType.ColorTint;
		// Colors used for a color tint-based transition.
		[FormerlySerializedAs("ColorsForImage")]
		[SerializeField]
		private ColorBlock m_ColorsForImage = ColorBlock.defaultColorBlock;
		// Sprites used for a Image swap-based transition.
		[FormerlySerializedAs("SpritesForImage")]
		[SerializeField] 
		private SpriteState m_SpriteState;

		[FormerlySerializedAs("TextComponent")]
		[SerializeField]
		protected Graphic m_targetGraphicForText;
		// Colors used for a color tint-based transition.
		[FormerlySerializedAs("ColorsForText")]
		[SerializeField]
		private ColorBlock m_ColorsForText = ColorBlock.defaultColorBlock;

		[SerializeField]
		protected TabButtonClickEvent m_tabButtonClickEvent = new TabButtonClickEvent();


		private StatableCase m_oldState = StatableCase.Normal;

		public bool interactable
		{
			get { return m_interactable; }
			set 
			{ 
				m_interactable = false;
				m_oldState = m_curCase;
				SetStatableCase(m_interactable == false ? StatableCase.Disabled : m_oldState); 
			}
		}

		protected Image image
		{
			get { return m_targetGraphicForImage as Image; }
			set { m_targetGraphicForImage = value; }
		}

		protected Text textComponent
		{
			get { return m_targetGraphicForText as Text; }
			set { m_targetGraphicForText = value; }
		}


		public StatableCase curState
		{
			get { return m_curCase; }
			set { SetStatableCase(value); }
		}

		public TransitionType trasitionTypeForImage
		{
			get { return m_TransitionForImage; }
			set { m_TransitionForImage = value; SetStatableCase(m_curCase, value); }
		}

		public Graphic targetGrphicForImage
		{
			get { return m_targetGraphicForImage; }
			set { m_targetGraphicForImage = value; SetStatableCase(m_curCase, m_TransitionForImage); }
		}



		public StatableCase SetStatableCase(StatableCase _newCase, TransitionType _type)
		{
			return DoStateTransition(_newCase, _type);
		}

		public StatableCase SetStatableCase(StatableCase _newCase)
		{
			return DoStateTransition(_newCase, m_TransitionForImage);
		}


		protected virtual void InstantClearState()
		{
			switch (m_TransitionForImage)
			{
				case TransitionType.ColorTint:
					StartColorTween(Color.white, Color.white);
					break;
				case TransitionType.SpriteSwap:
					DoSpriteSwap(null);
					break;
				default:
					m_targetGraphicForImage.canvasRenderer.SetColor(m_targetGraphicForImage.color);
					m_targetGraphicForText.canvasRenderer.SetColor(m_targetGraphicForText.color);
					if (image != null)
						image.overrideSprite = null;
					break;
			}
		}

		protected virtual StatableCase DoStateTransition(StatableCase _newCase, TransitionType _type)
		{
			Color tintColorForImage;
			Color tintColorForText;
			Sprite transitionSprite;

			StatableCase oldCase = m_curCase;
			m_curCase = _newCase;

			m_TransitionForImage = _type;

			switch (_newCase)
			{
				case StatableCase.Normal:
					tintColorForImage = m_ColorsForImage.normalColor;
					tintColorForText = m_ColorsForText.normalColor;
					transitionSprite = null;
					break;
				case StatableCase.Selected:
					tintColorForImage = m_ColorsForImage.highlightedColor;
					tintColorForText = m_ColorsForText.highlightedColor;
					transitionSprite = m_SpriteState.highlightedSprite;
					break;
				case StatableCase.Pressed:
					tintColorForImage = m_ColorsForImage.pressedColor;
					tintColorForText = m_ColorsForText.pressedColor;
					transitionSprite = m_SpriteState.pressedSprite;
					break;
				case StatableCase.Disabled:
					tintColorForImage = m_ColorsForImage.disabledColor;
					tintColorForText = m_ColorsForText.disabledColor;
					transitionSprite = m_SpriteState.disabledSprite;
					break;
				default:
					tintColorForImage = Color.black;
					tintColorForText = Color.black;
					transitionSprite = null;
					break;
			}

			if (gameObject.activeInHierarchy)
			{
				switch (m_TransitionForImage)
				{
					case TransitionType.ColorTint:
						StartColorTween(tintColorForImage * m_ColorsForImage.colorMultiplier, tintColorForText);
						break;
					case TransitionType.SpriteSwap:
						DoSpriteSwap(transitionSprite);

						if (textComponent != null)
							m_targetGraphicForText.canvasRenderer.SetColor(tintColorForText);
						break;
					default:
						m_targetGraphicForImage.canvasRenderer.SetColor(m_targetGraphicForImage.color);
						m_targetGraphicForText.canvasRenderer.SetColor(m_targetGraphicForText.color);
						
						if (image != null)
							image.overrideSprite = null;
						break;
				}
			}

			return oldCase;
		}

		void StartColorTween(Color targetColorForImage, Color targetColorForText)
		{
			if (m_targetGraphicForImage != null)
				m_targetGraphicForImage.canvasRenderer.SetColor(targetColorForImage);
			if (m_targetGraphicForText != null)
				m_targetGraphicForText.canvasRenderer.SetColor(targetColorForText);
		}

		void DoSpriteSwap(Sprite newSprite)
		{
			if (image != null)
			{
				image.canvasRenderer.SetColor(image.color);
				image.overrideSprite = newSprite;
			}
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (interactable == false)
				return;

			if (m_tabButtonClickEvent != null)
				m_tabButtonClickEvent.Invoke(this);
		}
	}
}
