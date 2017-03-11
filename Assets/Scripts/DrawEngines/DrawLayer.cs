using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;


[Serializable]
public class DrawLayer {
	static TextureFormat textureFormat = TextureFormat.ARGB32;

	protected Mesh mesh;
	protected Texture2D texture;
	protected IntVector2 size;
	protected Quaternion quaternion;
	protected Material material;
	public DrawLayer(IntVector2 size, Shader shader){
		material = new Material(shader);
		quaternion = Quaternion.identity;
		this.size = new IntVector2( size);
		mesh = MeshUtil.createPlaneMesh(size);
	}

	public void setTexture(Texture2D texture){
		this.texture = texture;
		material.mainTexture = texture;
	}

	public Texture2D getTexture(){
		return texture;
	}

	public Texture2D setBlank(Color32[] colors){
		if (texture == null){
			this.texture = new Texture2D(size.x, size.y, textureFormat, false);
			this.texture.filterMode = FilterMode.Point;
		}
		material.mainTexture = texture;
		texture.SetPixels32(colors);
		texture.Apply();
		return texture;
	}

	public IEnumerator updateColors(Color32[] colors, bool onNextFrame=false){
		if (onNextFrame)
			yield return null;		

		if (texture == null){
			setBlank(colors);
		} else {
			texture.SetPixels32(colors);
			texture.Apply();
		}
	}
}

