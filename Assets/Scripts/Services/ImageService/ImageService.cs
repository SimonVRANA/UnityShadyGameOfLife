// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ImageService : IImageService
{
	[Inject]
	private readonly IGameService gameService;

	private readonly Image gameImage;

	private int numberOfColumns = 900;

	public event EventHandler OnNumberOfColumnsChanged;

	public int NumberOfColumns
	{
		get => numberOfColumns;
		set
		{
			numberOfColumns = AdjustColumnsNumber(value);

			Texture2D newTexture = new(numberOfColumns, (int)Math.Floor(numberOfColumns * IImageService.columnsToRowsRatio))
			{
				filterMode = FilterMode.Point
			};
			Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), new Vector2(0.5f, 0.5f), 100);
			gameImage.sprite = newSprite;
			gameService.ClearPixels();
			OnNumberOfColumnsChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private int AdjustColumnsNumber(int newNumber)
	{
		int restult = Mathf.Clamp(newNumber, IImageService.minimumNumberOfColumns, IImageService.maximumNumberOfColumns);
		restult = (restult / IImageService.columnsIncrementNumber) * IImageService.columnsIncrementNumber;
		return restult;
	}

	public ImageService(Image image)
	{
		gameImage = image;

		gameImage.color = Color.white;
	}

	public void ApplyShader(Material material)
	{
		RenderTexture tmpTexture = RenderTexture.GetTemporary(gameImage.sprite.texture.width, gameImage.sprite.texture.height);
		Graphics.Blit(gameImage.sprite.texture, tmpTexture, material);
		RenderTexture.active = tmpTexture;
		gameImage.sprite.texture.ReadPixels(new Rect(0, 0, tmpTexture.width, tmpTexture.height), 0, 0);
		gameImage.sprite.texture.Apply();
		RenderTexture.ReleaseTemporary(tmpTexture);
	}

	public void RandomizePixels(Color deadColor, Color aliveColor, float aliveRatio)
	{
		System.Random random = new();

		for (int widthIndex = 0; widthIndex < gameImage.sprite.texture.width; widthIndex++)
		{
			for (int heightIndex = 0; heightIndex < gameImage.sprite.texture.height; heightIndex++)
			{
				gameImage.sprite.texture.SetPixel(widthIndex, heightIndex, random.NextDouble() < aliveRatio ? aliveColor : deadColor);
			}
		}
		gameImage.sprite.texture.Apply();
	}

	public Vector2 ImagePositionToPixelPosition(Vector2 imagePosition)
	{
		int x = (int)Math.Floor(imagePosition.x * NumberOfColumns);
		int numberOfRows = (int)Math.Floor(numberOfColumns * IImageService.columnsToRowsRatio);
		float y = (int)Math.Floor(imagePosition.y * numberOfRows);
		return new Vector2(x, y);
	}

	public Color GetPixelColor(Vector2 pixelPosition)
	{
		return gameImage.sprite.texture.GetPixel((int)Math.Floor(pixelPosition.x), (int)Math.Floor(pixelPosition.y));
	}

	public void SetPixelColor(Vector2 pixelPosition, Color color)
	{
		gameImage.sprite.texture.SetPixel((int)Math.Floor(pixelPosition.x), (int)Math.Floor(pixelPosition.y), color);
		gameImage.sprite.texture.Apply();
	}
}