using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using System.Collections;

public class DrawEngine : MonoBehaviour, 
	IPointerClickHandler, 
	IPointerDownHandler,
	IPointerUpHandler,
	IDragHandler
{
	public Canvas m_mainCanvas;

	public RawImage m_workingTextureRawImage;
	
	public RawImage m_frontLayerRawImage;
	public RawImage m_backLayerRawImage;

	public Vector2 m_workingTextureSize = Vector2.zero;
	public Texture2D m_workingTextureOutLine;
	public Texture2D m_workingTextureBorder;

	public IntVector2 defaultBufferColorSize;
	public int bufferSize = 10;
	public UndoRedoBuffer undoRedoBuffer;

	public DrawLayer backLayer;
	public DrawLayer frontLayer;

	public Shader layersShader;
	
	public Vector2 size;

	public Color32 activeColor;

	public DrawToolBucketLogic toolBuckeLogic;

	Color32[] _actualColors;
	bool[] _persistentLayer;
	bool initialized = false;

	bool frontLayerNull = false;

	public Vector2 screenSize;


	public bool[] persistentLayer
	{
		get
		{
			return _persistentLayer;
		}
	}

	public Color32[] actualColors
	{
		get
		{
			return _actualColors;
		}
	}

	protected void Awake()
	{
		initialize(m_workingTextureOutLine);
	}

	public void initialize(Texture2D texture = null)
	{
		undoRedoBuffer = new UndoRedoBuffer(defaultBufferColorSize, bufferSize);

		backLayer = new DrawLayer(defaultBufferColorSize, layersShader);
		frontLayer = new DrawLayer(defaultBufferColorSize, layersShader);

		setNewPicture(m_workingTextureOutLine, m_workingTextureBorder);
		initialized = true;

		// zoom in / out
		//handleInnerEvents();

		m_frontLayerRawImage.material = new Material(layersShader);
		m_backLayerRawImage.material = new Material(layersShader);
	}

	void initializeSupportedLogicDictionary()
	{
		/*
		supportedLogic = new Dictionary<ToolType, ToolLogicContext>(){
			{ToolType.BUCKET     , new ToolLogicContext (new ToolBucketStrategyImpl ())},
			{ToolType.INK     , new ToolLogicContext (new LineStrategyImpl (
					PropertiesSingleton.instance.tools.Ink,
					new CatmullRomStrategy()))},
			{ToolType.BRUSH      , new ToolLogicContext (new LineStrategyImpl (
					PropertiesSingleton.instance.tools.brush,					
					new CatmullRomStrategy()))},		
			{ToolType.BRUSH_LARGE, new ToolLogicContext (new LineStrategyImpl (
					PropertiesSingleton.instance.tools.bigBrush,					
					new CatmullRomStrategy()))},			
			{ToolType.ROLLER, new ToolLogicContext (new LineStrategyImpl (
					PropertiesSingleton.instance.tools.Roller,					
					new LinearInterpolationStrategy()))},
			{ToolType.STAMP      , new ToolLogicContext (new ToolStampStrategyImpl  ())},
			{ToolType.HAND       , new ToolLogicContext (new ToolHandStrategyImpl   ())},
			{ToolType.PIPETTE    , new ToolLogicContext (new ToolPipetteStrategyImpl())}
		};*/
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="texture">outLine Texture</param>
	/// <param name="persistentBorder">Border Texture</param>
	/// <param name="_fileFormat">file format : 0(psd or png (with alpha chanel)
	///				1: JPEG (without alpha chanel))</param>
	public void setNewPicture(
		Texture2D texture, 
		Texture2D persistentBorder = null, 
		int _fileFormat = 0
		)
	{
		//int totalPixelSize = config.canvasSize.x * config.canvasSize.y;
		int totalPixelSize = texture.width * texture.height;
		size = new Vector2(texture.width, texture.height);

		if (_actualColors == null || _actualColors.Length != (totalPixelSize))
			_actualColors = new Color32[totalPixelSize];

		ColorUtil.setWhitePixels(_actualColors);
		backLayer.setBlank(_actualColors);

		if (persistentBorder != null){
			Color32[] colors  = persistentBorder.GetPixels32();
			if (_persistentLayer == null 
			    || _persistentLayer.Length != totalPixelSize )
				_persistentLayer = new bool[totalPixelSize];

			// adjust Texture's Color to be working
			// if the texture is JPEG or without alpha color's
			if (_fileFormat != 0)
			{
				for (int i = 0; i < _persistentLayer.Length; i++)
				{
					if (colors[i].r >= 250 &&
						colors[i].g >= 250 &&
						colors[i].b >= 250)
					{
						colors[i].a = 0;
					}
					else
					{
						colors[i].r = 0;
						colors[i].g = 0;
						colors[i].b = 0;
						colors[i].a = 255;
					}
				}

				persistentBorder.SetPixels32(colors);
				persistentBorder.Apply();
			}

			if (texture != null)
				frontLayer.setTexture(texture);

			frontLayerNull = (texture == null);


			for (int i = 0; i < _persistentLayer.Length; i++) {
				_persistentLayer[i] = colors[i].a > 253;
			}
		} 
		else 
		{
			if (_persistentLayer == null 
			    || _persistentLayer.Length != totalPixelSize )
				_persistentLayer = new bool[totalPixelSize];

			for (int i = 0; i < totalPixelSize; i++) {
				_persistentLayer[i] = false;
			}
		}

		undoRedoBuffer.resetUndoRedo();
	}

	public void setNewPicture(
		Texture2D texture,
		int _fileFormat = 0
	)
	{
		//int totalPixelSize = config.canvasSize.x * config.canvasSize.y;
		int totalPixelSize = texture.width * texture.height;

		size = new Vector2(texture.width, texture.height);

		if (_actualColors == null || _actualColors.Length != (totalPixelSize))
			_actualColors = new Color32[totalPixelSize];

		ColorUtil.setWhitePixels(_actualColors);
		backLayer.setBlank(_actualColors);

		if (texture != null)
		{
			Color32[] colors = texture.GetPixels32();
			if (_persistentLayer == null
				|| _persistentLayer.Length != totalPixelSize)
				_persistentLayer = new bool[totalPixelSize];

			// adjust Texture's Color to be working
			// if the texture is JPEG or without alpha color's
			if (_fileFormat != 0)
			{
				for (int i = 0; i < _persistentLayer.Length; i++)
				{
					if (colors[i].r >= 250 &&
						colors[i].g >= 250 &&
						colors[i].b >= 250)
					{
						colors[i].a = 0;
					}
					else
					{
						colors[i].r = 0;
						colors[i].g = 0;
						colors[i].b = 0;
						colors[i].a = 255;
					}
				}

				texture.SetPixels32(colors);
				texture.Apply();
			}

			if (texture != null)
				frontLayer.setTexture(texture);

			frontLayerNull = (texture == null);


			for (int i = 0; i < _persistentLayer.Length; i++)
			{
				_persistentLayer[i] = colors[i].a > 253;
			}
		}
		else
		{
			if (_persistentLayer == null
				|| _persistentLayer.Length != totalPixelSize)
				_persistentLayer = new bool[totalPixelSize];

			for (int i = 0; i < totalPixelSize; i++)
			{
				_persistentLayer[i] = false;
			}
		}

		undoRedoBuffer.resetUndoRedo();
	}


	//TODO - curently working with no alpha, if you need it, need to make delegates
	byte _r, _g, _b;
	public void applyColors(Color32[] colors)
	{
		for (int i = 0; i < colors.Length; i++)
		{
			if (colors[i].a != 0)
			{
				_r = _actualColors[i].r;
				_actualColors[i].r = colors[i].r;
				colors[i].r = _r;

				_g = _actualColors[i].g;
				_actualColors[i].g = colors[i].g;
				colors[i].g = _g;

				_b = _actualColors[i].b;
				_actualColors[i].b = colors[i].b;
				colors[i].b = _b;
			}
		}

		StartCoroutine(backLayer.updateColors(_actualColors));
	}

	public void clearActiveColors()
	{
		Color32[] colors = fetchColors();
		for (int i = 0; i < colors.Length; i++)
		{
			colors[i].r = 255;
			colors[i].g = 255;
			colors[i].b = 255;
			colors[i].a = 255;
		}
		
		applyColors(colors);
	}

	public Color32[] fetchColors()
	{
		return undoRedoBuffer.getArray();
	}

	public void undo()
	{
		Color32[] undoColors = undoRedoBuffer.getForUndo();
		if (undoColors != null)
			applyColors(undoColors);
	}

	public void redo()
	{
		Color32[] redoColors = undoRedoBuffer.getForRedo();
		if (redoColors != null)
			applyColors(redoColors);
	}

	public Texture2D getResultTexture()
	{
		Color32[] resultColors;
		if (frontLayerNull)
			resultColors = _actualColors;
		else
			resultColors = TextureUtil.mergetTextureAbovePixelArray(frontLayer.getTexture(), _actualColors);

		Texture2D result = new Texture2D((int)this.size.x,
										 (int)this.size.y,
										 TextureFormat.ARGB32,
										 false);
		result.SetPixels32(resultColors);
		result.Apply();
		resultColors = null;
		return result;
	}

	public void OnPointerClick(PointerEventData eventData) 
	{
		if (toolBuckeLogic != null)
			toolBuckeLogic.OnPointerClick(eventData);
	}
	public void OnPointerDown(PointerEventData eventData) { }
	public void OnPointerUp(PointerEventData eventData) { }
	public void OnDrag(PointerEventData eventData) { }

	void Update()
	{
		m_frontLayerRawImage.texture = frontLayer.getTexture();
		m_backLayerRawImage.texture = backLayer.getTexture();

		screenSize = m_mainCanvas.pixelRect.size;
	}
}
