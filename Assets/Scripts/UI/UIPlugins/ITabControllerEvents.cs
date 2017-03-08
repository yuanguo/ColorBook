using UnityEngine;
using System.Collections;


namespace CEUI
{
	public interface ITabControllerEvents
	{
		bool willChangeTabItem(TabController _tabCtrl, int _newTabIdx, int _lastTabIdx);
		void didChangedTabItem(TabController _tabCtrl, RectTransform _containner, int _selectedTabIndex);
	}
}
