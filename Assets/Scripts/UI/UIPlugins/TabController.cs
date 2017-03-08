using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;


namespace CEUI
{
	public class TabController : UIBehaviour
	{
		private const int NoneSelectedTabID = -1;
		public ITabControllerEvents delegateInst = null;

		public int selectedTabIdx
		{
			get { return m_nLastSelectedTabID; }
		}

		#region UI members
		public List<TabButton> tabButtons = new List<TabButton>();
		public List<RectTransform> tabContainers = new List<RectTransform>();
		#endregion

		#region private members
		protected int m_nLastSelectedTabID = NoneSelectedTabID;
		#endregion

		public void initTabController(ITabControllerEvents tabController = null)
		{
			if (tabButtons.Count > 0)
				OnChangeTabButton(0);

			delegateInst = tabController;
			m_nLastSelectedTabID = NoneSelectedTabID;
		}

		public void SetActiveTabButton(int _tabIdx)
		{
			OnChangeTabButton(_tabIdx);
		}

		public void SetInteractableTabButton(int _tabIdx, bool _interactable = true)
		{
			for (int nIdx = 0; nIdx < tabButtons.Count; nIdx++)
			{
				if (nIdx == _tabIdx)
				{
					tabButtons[nIdx].interactable = _interactable;
				}
			}
		}

		public void SetFirstShowingTab(int _index = 0, bool _show = true)
		{
			if (_index >= tabButtons.Count ||
				_index < 0)
				return;

			for (int idx = 0; idx < tabButtons.Count; idx++)
			{
				if (idx == _index)
				{
					if (idx >= tabContainers.Count)
						return;

					TabButton tabBtn = tabButtons[idx];
					tabBtn.gameObject.SetActive(_show);
					tabContainers[idx].gameObject.SetActive(_show);

					break;
				}
			}

			if (_index != 0 && tabButtons.Count > 0)
				OnChangeTabButton(0);
			else if (_index == 0 &&
				tabButtons.Count > 1)
				OnChangeTabButton(1);
		}

		public void SetHideTab(int _index = 0, bool _show = true)
		{

		}

		#region event methods
		public void OnTabButtonClick(TabButton _sender)
		{
			OnChangeTabButton(_sender);
		}

		void OnChangedTabContainer(RectTransform _containner)
		{

		}

		void OnChangeTabButton(TabButton _sender)
		{
			//RectTransform containner = null;

			//foreach (Button btn in tabButtons)
			if (tabButtons != null)
			{
				int tabIdx = tabButtons.IndexOf(_sender);

				OnChangeTabButton(tabIdx);
			}
		}

		void OnChangeTabButton(int _tabIdx)
		{
			if (_tabIdx > -1 &&
				_tabIdx != m_nLastSelectedTabID)
			{
				bool canBeChangedTab = true;

				if (delegateInst != null)
					canBeChangedTab = delegateInst.willChangeTabItem(this, _tabIdx, m_nLastSelectedTabID);

				if (canBeChangedTab)
				{
					RectTransform containner = null;

					for (int nIdx = 0; nIdx < tabButtons.Count; nIdx++)
					{
						if (nIdx == _tabIdx)
						{
							m_nLastSelectedTabID = nIdx;

							if (tabButtons[nIdx] != null)
								tabButtons[nIdx].curState = TabButton.StatableCase.Selected;

							if (nIdx < tabContainers.Count)
							{
								if (tabContainers[nIdx] != null)
									tabContainers[nIdx].gameObject.SetActive(true);
								containner = tabContainers[nIdx];
							}
						}
						else
						{
							if (nIdx < tabContainers.Count)
							{
								if (tabContainers[nIdx] != null)
									tabContainers[nIdx].gameObject.SetActive(false);
							}

							if (tabButtons[nIdx] != null)
								tabButtons[nIdx].curState = TabButton.StatableCase.Normal;
						}
					}

					if (delegateInst != null)
						delegateInst.didChangedTabItem(this, containner, _tabIdx);
					OnChangedTabContainer(containner);
				}
			}
		}

		#endregion event methods

		#region override methods
		protected override void OnEnable()
		{
			base.OnEnable();
			SetActiveTabButton(0);
		}
		#endregion
	}
}

