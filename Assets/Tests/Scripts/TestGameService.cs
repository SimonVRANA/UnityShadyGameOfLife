// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using NUnit.Framework;
using SGOL.GameModes;
using SGOL.Services.Game;
using SGOL.Services.Image;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Zenject;

public class TestIGameService
{
	private IGameService gameService;

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
		container.Resolve<IImageService>().NumberOfColumns = 60;
		gameService = container.Resolve<IGameService>();
		gameService.NumberOfFramesBetweenUpdates = 0;

		GameObject imageGameObject = GameObject.Find("Image");
		imageComponent = imageGameObject.GetComponent<UnityEngine.UI.Image>();

		yield return null;
	}

	[TearDown]
	public void TearDown()
	{
		if (gameService != null)
		{
			gameService = null;
		}
		if (imageComponent != null)
		{
			imageComponent = null;
		}
		SceneManager.UnloadSceneAsync("TestScene");
	}

	[Test]
	public void TestNumberOfFramesBetweenUpdates()
	{
		gameService.NumberOfFramesBetweenUpdates = 0;
		Assert.AreEqual(0, gameService.NumberOfFramesBetweenUpdates, "Number of frames between update can be 0.");
		gameService.NumberOfFramesBetweenUpdates = (int)Mathf.FloorToInt(IGameService.maxNumberOfFramesBetweenUpdates / 2);
		Assert.AreEqual((int)Mathf.FloorToInt(IGameService.maxNumberOfFramesBetweenUpdates / 2), gameService.NumberOfFramesBetweenUpdates, "Number of frames between update need to be able to change.");

		gameService.NumberOfFramesBetweenUpdates = IGameService.maxNumberOfFramesBetweenUpdates;
		Assert.AreEqual(IGameService.maxNumberOfFramesBetweenUpdates, gameService.NumberOfFramesBetweenUpdates, "Number of frames between update can be max value.");

		gameService.NumberOfFramesBetweenUpdates = -10;
		Assert.AreEqual(0, gameService.NumberOfFramesBetweenUpdates, "Number of frames between update cannot be negative.");

		gameService.NumberOfFramesBetweenUpdates = IGameService.maxNumberOfFramesBetweenUpdates + 1;
		Assert.AreEqual(IGameService.maxNumberOfFramesBetweenUpdates, gameService.NumberOfFramesBetweenUpdates, "Number of frames between update cannot be greater than max value.");
	}

	[Test]
	public void TestGetGameModes()
	{
		string[] gameModes = gameService.GetGameModes();

		Assert.IsNotNull(gameModes, "Game modes should not be null.");
		Assert.IsNotEmpty(gameModes, "Game modes should not be empty.");
		Assert.Contains("BlinkTest", gameModes, "BlinkTest mode should be in the list of game modes.");
		Assert.Contains("Game of Life", gameModes, "Game of Life mode should be in the list of game modes.");
	}

	[Test]
	public void TestSetGameModes()
	{
		bool eventTriggered = false;
		gameService.OnGameModeChanged += (sender, args) => eventTriggered = true;

		string newGameMode = "BlinkTest";
		gameService.ChangeGameMode(newGameMode);
		Assert.AreEqual(newGameMode, gameService.GameMode, "Game mode should be changed to BlinkTest.");

		newGameMode = "Game of Life";
		eventTriggered = false;
		gameService.ChangeGameMode(newGameMode);
		Assert.AreEqual(newGameMode, gameService.GameMode, "Game mode should be changed to Game of Life.");
		Assert.IsTrue(eventTriggered, "OnGameModeChanged event should be triggered when changing game mode.");

		newGameMode = "BlinkTest";
		eventTriggered = false;
		gameService.ChangeGameMode(newGameMode);
		Assert.AreEqual(newGameMode, gameService.GameMode, "Game mode should be changed to BlinkTest again.");
		Assert.IsTrue(eventTriggered, "OnGameModeChanged event should be triggered when changing game mode again.");

		newGameMode = "BlinkTest";
		eventTriggered = false;
		gameService.ChangeGameMode(newGameMode);
		Assert.AreEqual(newGameMode, gameService.GameMode, "Game mode should still be BlinkTest.");
		Assert.IsFalse(eventTriggered, "OnGameModeChanged event should not be triggered when changing to the same game mode.");

		newGameMode = "NonExistentMode";
		eventTriggered = false;
		Assert.Throws<System.ArgumentException>(() => gameService.ChangeGameMode(newGameMode), "Changing to a non-existent game mode should throw an exception.");
		Assert.IsFalse(eventTriggered, "OnGameModeChanged event should not be triggered when changing to a non-existent game mode.");
	}

	[Test]
	public void TestApplyRandomPixels()
	{
		Texture2D texture = imageComponent.sprite.texture;
		Color[] pixels = new Color[texture.width * texture.height];
		for (int i = 0; i < pixels.Length; i++)
		{
			pixels[i] = Color.black;
		}
		texture.SetPixels(pixels);
		texture.Apply();

		DeadAliveGameModeParameters blinkParametters = AssetDatabase.LoadAssetAtPath<DeadAliveGameModeParameters>("Assets/Tests/ScriptableObjects/TestBlinkGameModeParameters.asset");

		gameService.ChangeGameMode("BlinkTest");
		gameService.ApplyRandomPixels();

		int aliveCount = 0;
		int deadCount = 0;
		for (int x = 0; x < imageComponent.sprite.rect.width; x++)
		{
			for (int y = 0; y < imageComponent.sprite.rect.height; y++)
			{
				Color pixelColor = imageComponent.sprite.texture.GetPixel(x, y);
				if (pixelColor == blinkParametters.AliveColor)
				{
					aliveCount++;
				}
				else if (pixelColor == blinkParametters.DeadColor)
				{
					deadCount++;
				}
			}
		}

		float totalPixels = imageComponent.sprite.rect.width * imageComponent.sprite.rect.height;
		float expectedAliveCount = totalPixels * blinkParametters.AliveRatio;
		Assert.IsTrue(Mathf.Abs(aliveCount - expectedAliveCount) < totalPixels * 0.1f, $"Alive pixels should be approximately equal to the expected ratio ({blinkParametters.AliveRatio}).");
		Assert.IsTrue(Mathf.Abs(deadCount - (totalPixels - expectedAliveCount)) < totalPixels * 0.1f, $"Dead pixels should be approximately equal to the expected ratio ({1 - blinkParametters.AliveRatio}).");
	}

	[Test]
	public void TestClearPixels()
	{
		gameService.ChangeGameMode("BlinkTest");

		Texture2D texture = imageComponent.sprite.texture;
		Color[] pixels = new Color[texture.width * texture.height];
		for (int i = 0; i < pixels.Length; i++)
		{
			pixels[i] = Color.white;
		}
		texture.SetPixels(pixels);
		texture.Apply();

		gameService.ClearPixels();
		AssertAllPixelsAreColor(Color.black, "All pixels should be cleared to black after calling ClearPixels().");
	}

	[Test]
	public void TestSwitchPixel()
	{
		gameService.ChangeGameMode("BlinkTest");

		DeadAliveGameModeParameters blinkParametters = AssetDatabase.LoadAssetAtPath<DeadAliveGameModeParameters>("Assets/Tests/ScriptableObjects/TestBlinkGameModeParameters.asset");

		Texture2D texture = imageComponent.sprite.texture;
		Color[] pixels = new Color[texture.width * texture.height];
		for (int i = 0; i < pixels.Length; i++)
		{
			pixels[i] = blinkParametters.DeadColor;
		}
		texture.SetPixels(pixels);
		texture.Apply();

		Vector2 pixelPosition = new Vector2(10, 10);
		gameService.SwitchPixel(pixelPosition);
		Color pixelColor = texture.GetPixel((int)pixelPosition.x, (int)pixelPosition.y);
		Assert.AreEqual(blinkParametters.AliveColor, pixelColor, "Pixel at the specified position should be switched to alive color.");

		pixelPosition = new Vector2(10, 10);
		gameService.SwitchPixel(pixelPosition);
		pixelColor = texture.GetPixel((int)pixelPosition.x, (int)pixelPosition.y);
		Assert.AreEqual(blinkParametters.DeadColor, pixelColor, "Pixel at the specified position should be switched back to dead color.");

		pixelPosition = new Vector2(-1, 10);
		Assert.Throws<System.ArgumentOutOfRangeException>(() => gameService.SwitchPixel(pixelPosition), "Switching pixel at an out-of-bounds position should throw an exception.");

		pixelPosition = new Vector2(10, -1);
		Assert.Throws<System.ArgumentOutOfRangeException>(() => gameService.SwitchPixel(pixelPosition), "Switching pixel at an out-of-bounds position should throw an exception.");

		pixelPosition = new Vector2(texture.width + 1, 10);
		Assert.Throws<System.ArgumentOutOfRangeException>(() => gameService.SwitchPixel(pixelPosition), "Switching pixel at an out-of-bounds position should throw an exception.");

		pixelPosition = new Vector2(10, texture.height + 1);
		Assert.Throws<System.ArgumentOutOfRangeException>(() => gameService.SwitchPixel(pixelPosition), "Switching pixel at an out-of-bounds position should throw an exception.");
	}

	[UnityTest]
	public IEnumerator TestPlayPauseGoOneStep()
	{
		Assert.IsFalse(gameService.IsPlaying, "Game should not be playing initially.");
		gameService.Play();
		Assert.IsTrue(gameService.IsPlaying, "Game should be playing after calling Play().");
		gameService.Pause();
		Assert.IsFalse(gameService.IsPlaying, "Game should not be playing after calling Pause().");
		gameService.GoOneStep();
		Assert.IsFalse(gameService.IsPlaying, "Game should not be playing after calling GoOneStep().");

		gameService.NumberOfFramesBetweenUpdates = 0;
		gameService.ChangeGameMode("BlinkTest");
		gameService.ClearPixels();
		DeadAliveGameModeParameters blinkParametters = AssetDatabase.LoadAssetAtPath<DeadAliveGameModeParameters>("Assets/Tests/ScriptableObjects/TestBlinkGameModeParameters.asset");

		AssertAllPixelsAreColor(blinkParametters.DeadColor, "All pixels should be cleared to black before starting test.");

		gameService.GoOneStep();
		yield return null;
		AssertAllPixelsAreColor(blinkParametters.AliveColor, "All pixels should be alive after one step in BlinkTest mode.");

		gameService.GoOneStep();
		yield return null;
		AssertAllPixelsAreColor(blinkParametters.DeadColor, "All pixels should be dead after two steps in BlinkTest mode.");

		gameService.Play();
		yield return null;
		AssertAllPixelsAreColor(blinkParametters.AliveColor, "All pixels should be alive after starting to play in BlinkTest mode.");
		yield return null;
		AssertAllPixelsAreColor(blinkParametters.DeadColor, "All pixels should be dead after two frames in BlinkTest mode while playing.");

		gameService.Pause();
		yield return null;
		AssertAllPixelsAreColor(blinkParametters.DeadColor, "All pixels should remain dead after pausing in BlinkTest mode.");
		yield return null;
		AssertAllPixelsAreColor(blinkParametters.DeadColor, "All pixels should still be dead after another frame while paused in BlinkTest mode.");

		gameService.Play();
		yield return null;
		AssertAllPixelsAreColor(blinkParametters.AliveColor, "All pixels should be alive after resuming play in BlinkTest mode.");

		gameService.NumberOfFramesBetweenUpdates = 2;
		yield return null;
		AssertAllPixelsAreColor(blinkParametters.AliveColor, "All pixels should be alive after zero frame in BlinkTest mode while playing with 2 frames between updates.");
		yield return null;
		AssertAllPixelsAreColor(blinkParametters.AliveColor, "All pixels should be alive after one frame in BlinkTest mode while playing with 2 frames between updates.");
		yield return null;
		AssertAllPixelsAreColor(blinkParametters.DeadColor, "All pixels should be dead after two frames in BlinkTest mode while playing with 2 frames between updates.");

		gameService.NumberOfFramesBetweenUpdates = 10;
		yield return null;
		AssertAllPixelsAreColor(blinkParametters.DeadColor, "All pixels should be dead after zero frame in BlinkTest mode while playing with 10 frames between updates.");
		yield return null;
		AssertAllPixelsAreColor(blinkParametters.DeadColor, "All pixels should still be dead after one frame in BlinkTest mode while playing with 10 frames between updates.");
		yield return null;
		AssertAllPixelsAreColor(blinkParametters.DeadColor, "All pixels should still be dead after two frames in BlinkTest mode while playing with 10 frames between updates (because changing NumberOfFramesBetweenUpdates resets the counter).");
	}

	private void AssertAllPixelsAreColor(Color expectedColor, string message)
	{
		for (int x = 0; x < imageComponent.sprite.rect.width; x++)
		{
			for (int y = 0; y < imageComponent.sprite.rect.height; y++)
			{
				Color pixelColor = imageComponent.sprite.texture.GetPixel(x, y);
				Assert.AreEqual(expectedColor, pixelColor, message);
			}
		}
	}
}