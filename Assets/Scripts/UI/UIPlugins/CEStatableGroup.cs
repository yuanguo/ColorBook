using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UnityEngine.Serialization;

using System;
using System.Collections;
using System.Collections.Generic;



namespace CEUI
{
	public class CEStatableGroup : UIBehaviour
	{
		/// <summary>
		/// Transit oldStatableCtrl to newStatableCtrl
		/// </summary>
		[Serializable]
		public class CEStatableGroupEvent : UnityEvent<CEStatable /*oldState*/, CEStatable /*newState*/> { }

		public CEStatable m_lastSelectedStatableCtrl = null;

		protected List<CEStatable> m_statableCtrls = new List<CEStatable>();


		protected override void Awake()
		{
			base.Awake();

			if (m_statableCtrls == null)
				m_statableCtrls = new List<CEStatable>();

			m_statableCtrls.Clear();
		}

		protected override void OnDestroy()
		{
			m_statableCtrls.Clear();

			base.OnDestroy();
		}



		public void RegisterCtrl(CEStatable _ctrl)
		{
			if (m_statableCtrls != null)
				m_statableCtrls.Add(_ctrl);
		}

		public void UnRegisterCtrl(CEStatable _ctrl)
		{
			if (m_statableCtrls != null)
				m_statableCtrls.Remove(_ctrl);
		}
	}
}
