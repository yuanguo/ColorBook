using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TestDraw : MonoBehaviour {

	public Texture2D m_textJPEG;
	public Texture2D m_textPNG;
	public Texture2D m_textPSD;

	void Start()
	{
		TextureFormat format = m_textJPEG.format;
		m_textJPEG.Resize(m_textJPEG.width, m_textJPEG.height, TextureFormat.ARGB32, false);
		format = m_textPNG.format;
		format = m_textPNG.format;
	}
}
