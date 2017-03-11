using UnityEngine;

using System;
using System.Collections;

[Serializable]
public class DrawToolBucketLogic : DrawToolLogic
{
	public IntVector2 point;
	public DrawEngine drawEngine = null;

	public DrawToolBucketLogic(ToolType _type, DrawEngine _engin)
	{
		toolType = _type;
		drawEngine = _engin;
	}

	public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
	{
		point = new IntVector2(eventData.position);
		base.OnPointerDown(eventData);
	}

	public override void OnMove(UnityEngine.EventSystems.AxisEventData eventData)
	{
		base.OnMove(eventData);
	}

	public override void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
	}

	public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
	{
		base.OnPointerClick(eventData);
		point = new IntVector2(eventData.position);

		doFloodFill(point);
	}

	void doFloodFill(IntVector2 position)
	{
		Color32[] colors = drawEngine.fetchColors();
		if (drawEngine.persistentLayer[(int)(position.y * drawEngine.size.x + position.x)])
			return;

		bool[,] resultRegion = TextureUtil.floodFillLineGetRegion(position, 
			drawEngine.actualColors, 
			drawEngine.persistentLayer,
			(int)drawEngine.size.x, (int)drawEngine.size.y);

		Color32 activeColor = drawEngine.activeColor;
		int tCounter = 0;
		for (int yy = 0; yy < (int)drawEngine.size.y; yy++)
		{
			for (int xx = 0; xx < (int)drawEngine.size.x; xx++)
			{
				if (resultRegion[xx, yy])
				{
					colors[tCounter].r = activeColor.r;
					colors[tCounter].g = activeColor.g;
					colors[tCounter].b = activeColor.b;
					colors[tCounter].a = 255;
				}
				tCounter++;
			}
		}
		drawEngine.applyColors(colors);
	}

}
