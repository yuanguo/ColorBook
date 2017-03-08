using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CategoryView : MonoBehaviour {

	public TweenPRS m_tween = null;
	public RectTransform m_rectTransform;

	public RectTransform m_workingPanel;


	private int panelIdx = 0; // 0: category view 1: workingPanel View

	public void OnTapImage()
	{
		panelIdx = 0;

		m_tween.SetFrom(TweenPRS.PRSType.Pos, Vector3.zero);
		m_tween.SetTo(TweenPRS.PRSType.Pos, new Vector3(-(m_rectTransform.sizeDelta.x + 10), 0, 0));
		m_tween.setEnable(TweenPRS.PRSType.Pos, true);
		m_tween.PlayForward();

		m_workingPanel.gameObject.SetActive(true);
	}

	public void OnBackFormWorkingPanelView()
	{
		panelIdx = 1;
		m_tween.PlayReverse();
	}

	public void OnFinishAnimation()
	{
		if (panelIdx == 1)
		{
			m_workingPanel.gameObject.SetActive(false);
		}
	}
}
