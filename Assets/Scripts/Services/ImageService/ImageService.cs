// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using System;
using UnityEngine;

namespace SGOL.Services.Image
{
	public class ImageService : IImageService
	{
		private readonly UnityEngine.UI.Image gameImage;

		private int numberOfColumns = 0;

		public event EventHandler NumberOfColumnsChanged;

		public int NumberOfColumns
		{
			get => numberOfColumns;
			set
			{
				int adjustedValue = AdjustColumnsNumber(value);
				if (adjustedValue == numberOfColumns)
				{
					return;
				}

				numberOfColumns = adjustedValue;

				Texture2D newTexture = new(numberOfColumns, (int)Math.Floor(numberOfColumns * IImageService.columnsToRowsRatio))
				{
					filterMode = FilterMode.Point
				};
				Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), new Vector2(0.5f, 0.5f), 100);
				gameImage.sprite = newSprite;
				NumberOfColumnsChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		private int AdjustColumnsNumber(int newNumber)
		{
			int restult = Mathf.Clamp(newNumber, IImageService.minimumNumberOfColumns, IImageService.maximumNumberOfColumns);
			restult = (restult / IImageService.columnsIncrementNumber) * IImageService.columnsIncrementNumber;
			return restult;
		}

		public ImageService(UnityEngine.UI.Image image)
		{
			if (image == null)
			{
				throw new ArgumentNullException(nameof(image), "Image cannot be null.");
			}

			gameImage = image;

			gameImage.color = Color.white;

			NumberOfColumns = IImageService.minimumNumberOfColumns;
		}

		public void ApplyShader(Material material)
		{
			if (material == null)
			{
				throw new ArgumentNullException(nameof(material), "Material cannot be null.");
			}

			RenderTexture tmpTexture = RenderTexture.GetTemporary(gameImage.sprite.texture.width, gameImage.sprite.texture.height);
			Graphics.Blit(gameImage.sprite.texture, tmpTexture, material);
			RenderTexture.active = tmpTexture;
			gameImage.sprite.texture.ReadPixels(new Rect(0, 0, tmpTexture.width, tmpTexture.height), 0, 0);
			gameImage.sprite.texture.Apply();
			RenderTexture.ReleaseTemporary(tmpTexture);
		}

		public void RandomizePixels(Color deadColor, Color aliveColor, float aliveRatio)
		{
			if (aliveRatio < 0 || aliveRatio > 1)
			{
				throw new ArgumentOutOfRangeException(nameof(aliveRatio), "Alive ratio must be between 0 and 1.");
			}

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
			if (imagePosition.x < 0 || imagePosition.x > 1
			   || imagePosition.y < 0 || imagePosition.y > 1)
			{
				throw new ArgumentOutOfRangeException(nameof(imagePosition), "Image position must be between 0 and 1 for both x and y.");
			}

			int x = (int)Math.Floor(imagePosition.x * (NumberOfColumns - 1));
			int numberOfRows = (int)Math.Floor(numberOfColumns * IImageService.columnsToRowsRatio);
			float y = (int)Math.Floor(imagePosition.y * (numberOfRows - 1));
			return new Vector2(x, y);
		}

		public Color GetPixelColor(Vector2 pixelPosition)
		{
			if (pixelPosition.x < 0 || pixelPosition.x >= gameImage.sprite.texture.width
			   || pixelPosition.y < 0 || pixelPosition.y >= gameImage.sprite.texture.height)
			{
				throw new ArgumentOutOfRangeException(nameof(pixelPosition), "Pixel position must be within the texture bounds.");
			}

			return gameImage.sprite.texture.GetPixel((int)Math.Floor(pixelPosition.x), (int)Math.Floor(pixelPosition.y));
		}

		public void SetPixelColor(Vector2 pixelPosition, Color color)
		{
			if (pixelPosition.x < 0 || pixelPosition.x >= gameImage.sprite.texture.width
			   || pixelPosition.y < 0 || pixelPosition.y >= gameImage.sprite.texture.height)
			{
				throw new ArgumentOutOfRangeException(nameof(pixelPosition), "Pixel position must be within the texture bounds.");
			}

			gameImage.sprite.texture.SetPixel((int)Math.Floor(pixelPosition.x), (int)Math.Floor(pixelPosition.y), color);
			gameImage.sprite.texture.Apply();
		}
	}
}