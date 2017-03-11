using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using System;
using System.Collections;

[Serializable]
public class DrawToolLogic : 
	IPointerClickHandler,
	IPointerDownHandler,
	IPointerUpHandler,
	IMoveHandler
{
	public ToolType toolType;

	public DrawToolLogic()
	{
		toolType = ToolType.NONE;
	}

	public DrawToolLogic(ToolType _toolType)
	{
		toolType = _toolType;
	}


	public virtual void OnMove(AxisEventData eventData) { }
	//
	// Summary:
	//     See IPointerClickHandler.OnPointerClick.
	//
	// Parameters:
	//   eventData:
	public virtual void OnPointerClick(PointerEventData eventData) { }
	//
	// Summary:
	//     See IPointerDownHandler.OnPointerDown.
	//
	// Parameters:
	//   eventData:
	public virtual void OnPointerDown(PointerEventData eventData) { }
	//
	// Summary:
	//     See IPointerUpHandler.OnPointerUp.
	//
	// Parameters:
	//   eventData:
	public virtual void OnPointerUp(PointerEventData eventData) { }
}
