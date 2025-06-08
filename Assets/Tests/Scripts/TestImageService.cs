// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using NUnit.Framework;
using SGOL.Services.Image;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Zenject;

public class TestImageService
{
	private IImageService imageService;

	private UnityEngine.UI.Image imageComponent;

	[UnitySetUp]
	public IEnumerator SetUp()

	{
		UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode("Assets/Tests/Scenes/TestScene.unity", new LoadSceneParameters(LoadSceneMode.Additive));
		yield return null;

		GameObject zenjectContextGameObject = GameObject.Find("ZenjectTest");
		SceneContext sceneContext = zenjectContextGameObject.GetComponent<SceneContext>();
		DiContainer container = sceneContext.Container;
		Assert.IsNotNull(container);
		imageService = container.Resolve<IImageService>();

		GameObject imageGameObject = GameObject.Find("Image");
		imageComponent = imageGameObject.GetComponent<UnityEngine.UI.Image>();

		yield return null;
	}

	[TearDown]
	public void TearDown()
	{
		if (imageService != null)
		{
			imageService = null;
		}
		if (imageComponent != null)
		{
			imageComponent = null;
		}
		SceneManager.UnloadSceneAsync("TestScene");
	}

	[UnityTest]
	public IEnumerator TestNumberOfColumns()
	{
		imageService.NumberOfColumns = 12;
		Assert.AreEqual(imageComponent.sprite.rect.width, 12, "Value between IImageService.minimumNumberOfColumns and IImageService.maximumNumberOfColumns should be accepted.");
		Assert.AreEqual(imageComponent.sprite.rect.height, (int)Math.Floor(12 * IImageService.columnsToRowsRatio), "Height should be set to width*IImageService.columnsToRowsRatio.");

		imageService.NumberOfColumns = -10;
		Assert.AreEqual(imageComponent.sprite.rect.width, IImageService.minimumNumberOfColumns, "Values lower that IImageService.minimumNumberOfColumns are not allowed.");

		imageService.NumberOfColumns = int.MaxValue;
		Assert.AreEqual(imageComponent.sprite.rect.width, IImageService.maximumNumberOfColumns, "Values higher that IImageService.maximumNumberOfColumns are not allowed.");

		if (IImageService.columnsIncrementNumber > 1)
		{
			imageService.NumberOfColumns = IImageService.minimumNumberOfColumns;
			imageService.NumberOfColumns += IImageService.columnsIncrementNumber / 2;

			Assert.AreEqual(imageComponent.sprite.rect.width % IImageService.columnsIncrementNumber, 0, "Number of columns should be a multiple of IImageService.columnsIncrementNumber.");
		}

		bool eventFired = false;
		imageService.NumberOfColumnsChanged += (sender, args) => eventFired = true;
		imageService.NumberOfColumns = IImageService.minimumNumberOfColumns;
		imageService.NumberOfColumns = IImageService.minimumNumberOfColumns + IImageService.columnsIncrementNumber;
		Assert.IsTrue(eventFired, "Event should fire after number of columns is changed");

		imageService.NumberOfColumns = IImageService.minimumNumberOfColumns;
		eventFired = false;
		imageService.NumberOfColumns = IImageService.minimumNumberOfColumns;
		Assert.IsFalse(eventFired, "Event should not fire after number of columns is set to same number");

		yield return null;
	}

	// This needs to be a Unity test to use the scene, even though it does not need to wait for a frame.
	[UnityTest]
	public IEnumerator TestApplyShader()
	{
		Material blackMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Tests/Materials/ColorToBlack.mat");
		Material whiteMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Tests/Materials/ColorToWhite.mat");

		Assert.IsNotNull(blackMaterial, "Missing Black material for test at Assets/Tests/Materials/ColorToBlack.mat.");
		Assert.IsNotNull(whiteMaterial, "Missing White material for test at Assets/Tests/Materials/ColorToWhite.mat.");

		imageService.NumberOfColumns = 24;

		imageService.ApplyShader(blackMaterial);
		for (int x = 0; x < imageComponent.sprite.rect.width; x++)
		{
			for (int y = 0; y < imageComponent.sprite.rect.height; y++)
			{
				Color pixelColor = imageComponent.sprite.texture.GetPixel(x, y);
				Assert.AreEqual(Color.black, pixelColor, $"Pixel at ({x}, {y}) should be black after applying black material.");
			}
		}

		imageService.ApplyShader(whiteMaterial);
		for (int x = 0; x < imageComponent.sprite.rect.width; x++)
		{
			for (int y = 0; y < imageComponent.sprite.rect.height; y++)
			{
				Color pixelColor = imageComponent.sprite.texture.GetPixel(x, y);
				Assert.AreEqual(Color.white, pixelColor, $"Pixel at ({x}, {y}) should be white after applying white material.");
			}
		}

		imageService.ApplyShader(blackMaterial);
		for (int x = 0; x < imageComponent.sprite.rect.width; x++)
		{
			for (int y = 0; y < imageComponent.sprite.rect.height; y++)
			{
				Color pixelColor = imageComponent.sprite.texture.GetPixel(x, y);
				Assert.AreEqual(Color.black, pixelColor, $"Pixel at ({x}, {y}) should be black after applying black material again.");
			}
		}

		Assert.Throws<ArgumentNullException>(() => imageService.ApplyShader(null), "Applying null material should throw ArgumentNullException.");

		yield return null;
	}

	[UnityTest]
	public IEnumerator TestRandomizePixels()
	{
		imageService.NumberOfColumns = 24;

		TestRatio(Color.black, Color.white, 0.5f);
		TestRatio(Color.black, Color.white, 0.3f);
		TestRatio(Color.black, Color.white, 0.8f);

		Assert.Throws<ArgumentOutOfRangeException>(() => imageService.RandomizePixels(Color.black, Color.white, -0.5f), "Alive ratio less than 0 should throw ArgumentOutOfRangeException.");

		Assert.Throws<ArgumentOutOfRangeException>(() => imageService.RandomizePixels(Color.black, Color.white, 1.5f), "Alive ratio greater than 1 should throw ArgumentOutOfRangeException.");

		yield return null;
	}

	private void TestRatio(Color deadColor, Color aliveColor, float aliveRatio)
	{
		Assert.IsTrue(aliveRatio >= 0 && aliveRatio <= 1, "TestRatio bad call: Alive ratio should be between 0 and 1.");

		imageService.RandomizePixels(deadColor, aliveColor, aliveRatio);

		int aliveCount = 0;
		int deadCount = 0;
		for (int x = 0; x < imageComponent.sprite.rect.width; x++)
		{
			for (int y = 0; y < imageComponent.sprite.rect.height; y++)
			{
				Color pixelColor = imageComponent.sprite.texture.GetPixel(x, y);
				if (pixelColor == aliveColor)
				{
					aliveCount++;
				}
				else if (pixelColor == deadColor)
				{
					deadCount++;
				}
			}
		}

		float totalPixels = imageComponent.sprite.rect.width * imageComponent.sprite.rect.height;
		float expectedAliveCount = totalPixels * aliveRatio;
		Assert.IsTrue(Mathf.Abs(aliveCount - expectedAliveCount) < totalPixels * 0.1f, $"Alive pixels should be approximately equal to the expected ratio ({aliveRatio}).");
		Assert.IsTrue(Mathf.Abs(deadCount - (totalPixels - expectedAliveCount)) < totalPixels * 0.1f, $"Dead pixels should be approximately equal to the expected ratio ({1 - aliveRatio}).");
	}

	[UnityTest]
	public IEnumerator TestImagePositionToPixelPosition()
	{
		imageService.NumberOfColumns = 24;

		Vector2 imagePosition = new(0.5f, 0.5f);
		Vector2 pixelPosition = imageService.ImagePositionToPixelPosition(imagePosition);
		int expectedX = (int)Math.Floor(imagePosition.x * (imageService.NumberOfColumns));
		int expectedY = (int)Math.Floor(imagePosition.y * ((int)Math.Floor(imageService.NumberOfColumns * IImageService.columnsToRowsRatio)));
		Assert.AreEqual(new Vector2(expectedX, expectedY), pixelPosition, $"Image position (0.5, 0.5) should map to pixel position ({expectedX}, {expectedY}) for 24 columns.");

		imagePosition = new Vector2(0f, 0f);
		pixelPosition = imageService.ImagePositionToPixelPosition(imagePosition);
		Assert.AreEqual(Vector2.zero, pixelPosition, "Image position (0, 0) should map to pixel position (0, 0).");

		imagePosition = new Vector2(1f, 1f);
		pixelPosition = imageService.ImagePositionToPixelPosition(imagePosition);
		Assert.AreEqual(new Vector2(imageService.NumberOfColumns - 1, (int)Math.Floor(imageService.NumberOfColumns * IImageService.columnsToRowsRatio) - 1), pixelPosition, "Image position (1, 1) should map to pixel position (23, 11) for 24 columns.");

		imagePosition = new Vector2(-0.5f, 0.5f);
		Assert.Throws<ArgumentOutOfRangeException>(() => imageService.ImagePositionToPixelPosition(imagePosition), "Image position with negative x should throw ArgumentOutOfRangeException.");

		imagePosition = new Vector2(1.5f, 0.5f);
		Assert.Throws<ArgumentOutOfRangeException>(() => imageService.ImagePositionToPixelPosition(imagePosition), "Image position with x greater than 1 should throw ArgumentOutOfRangeException.");

		imagePosition = new Vector2(0.5f, -0.5f);
		Assert.Throws<ArgumentOutOfRangeException>(() => imageService.ImagePositionToPixelPosition(imagePosition), "Image position with negative y should throw ArgumentOutOfRangeException.");

		imagePosition = new Vector2(0.5f, 1.5f);
		Assert.Throws<ArgumentOutOfRangeException>(() => imageService.ImagePositionToPixelPosition(imagePosition), "Image position with y greater than 1 should throw ArgumentOutOfRangeException.");

		yield return null;
	}

	[UnityTest]
	public IEnumerator TestGetPixelColor()
	{
		imageService.NumberOfColumns = 24;

		Vector2 pixelPosition = new(5, 5);
		Color expectedColor = Color.red;
		imageComponent.sprite.texture.SetPixel((int)pixelPosition.x, (int)pixelPosition.y, expectedColor);
		imageComponent.sprite.texture.Apply();
		Color pixelColor = imageService.GetPixelColor(pixelPosition);
		Assert.AreEqual(expectedColor, pixelColor, $"Pixel color at ({pixelPosition.x}, {pixelPosition.y}) should be {expectedColor}.");

		pixelPosition = new Vector2(0, 0);
		expectedColor = Color.green;
		imageComponent.sprite.texture.SetPixel((int)pixelPosition.x, (int)pixelPosition.y, expectedColor);
		imageComponent.sprite.texture.Apply();
		pixelColor = imageService.GetPixelColor(pixelPosition);
		Assert.AreEqual(expectedColor, pixelColor, $"Pixel color at ({pixelPosition.x}, {pixelPosition.y}) should be {expectedColor}.");

		pixelPosition = new Vector2(-10, 0);
		Assert.Throws<ArgumentOutOfRangeException>(() => imageService.GetPixelColor(pixelPosition), "Pixel position with negative x should throw ArgumentOutOfRangeException.");

		pixelPosition = new Vector2(50, 0);
		Assert.Throws<ArgumentOutOfRangeException>(() => imageService.GetPixelColor(pixelPosition), "Pixel position with x greater than texture width should throw ArgumentOutOfRangeException.");

		pixelPosition = new Vector2(0, -10);
		Assert.Throws<ArgumentOutOfRangeException>(() => imageService.GetPixelColor(pixelPosition), "Pixel position with negative y should throw ArgumentOutOfRangeException.");

		pixelPosition = new Vector2(0, 2 * (int)Math.Floor(imageService.NumberOfColumns * IImageService.columnsToRowsRatio));
		Assert.Throws<ArgumentOutOfRangeException>(() => imageService.GetPixelColor(pixelPosition), "Pixel position with y greater than texture height should throw ArgumentOutOfRangeException.");

		yield return null;
	}

	[UnityTest]
	public IEnumerator TestSetPixelColor()
	{
		imageService.NumberOfColumns = 24;

		Vector2 pixelPosition = new(5, 5);
		Color expectedColor = Color.blue;
		imageService.SetPixelColor(pixelPosition, expectedColor);
		Color pixelColor = imageComponent.sprite.texture.GetPixel((int)pixelPosition.x, (int)pixelPosition.y);
		Assert.AreEqual(expectedColor, pixelColor, $"Pixel color at ({pixelPosition.x}, {pixelPosition.y}) should be set to {expectedColor}.");

		pixelPosition = new Vector2(0, 0);
		expectedColor = Color.yellow;
		imageService.SetPixelColor(pixelPosition, expectedColor);
		pixelColor = imageComponent.sprite.texture.GetPixel((int)pixelPosition.x, (int)pixelPosition.y);
		Assert.AreEqual(expectedColor, pixelColor, $"Pixel color at ({pixelPosition.x}, {pixelPosition.y}) should be set to {expectedColor}.");

		pixelPosition = new Vector2(-10, 0);
		Assert.Throws<ArgumentOutOfRangeException>(() => imageService.SetPixelColor(pixelPosition, Color.white), "Setting pixel color with negative x should throw ArgumentOutOfRangeException.");

		pixelPosition = new Vector2(50, 0);
		Assert.Throws<ArgumentOutOfRangeException>(() => imageService.SetPixelColor(pixelPosition, Color.white), "Setting pixel color with x greater than texture width should throw ArgumentOutOfRangeException.");

		pixelPosition = new Vector2(0, -10);
		Assert.Throws<ArgumentOutOfRangeException>(() => imageService.SetPixelColor(pixelPosition, Color.white), "Setting pixel color with negative y should throw ArgumentOutOfRangeException.");

		pixelPosition = new Vector2(0, 2 * (int)Math.Floor(imageService.NumberOfColumns * IImageService.columnsToRowsRatio));
		Assert.Throws<ArgumentOutOfRangeException>(() => imageService.SetPixelColor(pixelPosition, Color.white), "Setting pixel color with y greater than texture height should throw ArgumentOutOfRangeException.");

		yield return null;
	}
}