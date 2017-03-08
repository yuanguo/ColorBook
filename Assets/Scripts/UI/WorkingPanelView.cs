using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

public class WorkingPanelView : MonoBehaviour {

	public List<Image> m_images = new List<Image>();

	public Color32 m_selectedColor;

	public void OnClickColorBtn(Image btn)
	{
		Image selectedButtonImage = btn.GetComponentInChildren<Image>();
		if (selectedButtonImage != null)
		{
			m_selectedColor = selectedButtonImage.color;

			PropertiesSingleton.instance.colorProperties.activeColor = m_selectedColor;
		}
	}
}
