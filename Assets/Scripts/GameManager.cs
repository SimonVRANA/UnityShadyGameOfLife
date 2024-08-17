// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[Header("Links")]
	[SerializeField]
	private Image gameImage;

	[Header("GameOfLife")]
	[SerializeField]
	private Color deadColor;

	[SerializeField]
	private Color aliveColor;

	[Header("Shaders")]
	[SerializeField]
	private Material clearShader;

	private int columnsNumber = 496;

	// Start is called before the first frame update
	void Start()
	{
		SetColumnsNumber(columnsNumber);
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void SetColumnsNumber(int newNumber)
	{
		columnsNumber = Mathf.Clamp(newNumber, 0, 1920);
		columnsNumber = (columnsNumber / 16) * 16;

		Texture2D newTexture = new(columnsNumber, columnsNumber * 9 / 16);
		newTexture.filterMode = FilterMode.Point;
		Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), new Vector2(0.5f, 0.5f), 100);
		gameImage.sprite = newSprite;
		Clear();
	}
	public void Random()
	{
		Clear();


		System.Random random = new();

		for (int widthIndex = 0; widthIndex < gameImage.sprite.texture.width; widthIndex++)
		{
			for (int heightIndex = 0; heightIndex < gameImage.sprite.texture.height; heightIndex++)
			{
				gameImage.sprite.texture.SetPixel(widthIndex, heightIndex, random.NextDouble() < 0.5 ? deadColor : aliveColor);
			}
		}
		gameImage.sprite.texture.Apply();
	}

	public void Clear()
	{
		gameImage.color = Color.white;
		clearShader.SetColor("_Color", deadColor);
		ApplyMaterialToGameImage(clearShader);
	}

	private void ApplyMaterialToGameImage(Material material)
	{
		RenderTexture tmpTexture = RenderTexture.GetTemporary(gameImage.sprite.texture.width, gameImage.sprite.texture.height);
		Graphics.Blit(gameImage.sprite.texture, tmpTexture, material);
		RenderTexture.active = tmpTexture;
		gameImage.sprite.texture.ReadPixels(new Rect(0, 0, tmpTexture.width, tmpTexture.height), 0, 0);
		gameImage.sprite.texture.Apply();
		RenderTexture.ReleaseTemporary(tmpTexture);
	}
}
