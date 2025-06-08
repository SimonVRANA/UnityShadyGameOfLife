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

public class TestGameOfLifeShader
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
		container.Resolve<IImageService>().NumberOfColumns = 120;
		gameService = container.Resolve<IGameService>();
		gameService.NumberOfFramesBetweenUpdates = 0;
		gameService.ChangeGameMode("Game of Life");
		gameService.ClearPixels();

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
	public void TestGameOfLifeRules()
	{
		DeadAliveGameModeParameters golParametters = AssetDatabase.LoadAssetAtPath<DeadAliveGameModeParameters>("Assets/ScriptableObjects/GameOfLifeParameters.asset");

		Assert.IsTrue(AllPixselsAreOfColor(golParametters.DeadColor), "Test starts with all dead pixels, they should stay dead.");

		// Test birth: a pixel with exactly 3 alive neighbors should become alive
		gameService.SwitchPixel(new Vector2(30, 31));
		gameService.SwitchPixel(new Vector2(30, 29));
		gameService.SwitchPixel(new Vector2(29, 30));
		gameService.GoOneStep();
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(30, 30) == golParametters.AliveColor, "A pixel with exactly 3 alive neighbors should become alive.");
		gameService.ClearPixels();

		// Test survival: a pixel with 2 or 3 alive neighbors should stay alive
		gameService.SwitchPixel(new Vector2(30, 30));
		gameService.SwitchPixel(new Vector2(30, 31));
		gameService.SwitchPixel(new Vector2(30, 29));
		gameService.SwitchPixel(new Vector2(29, 30));
		gameService.GoOneStep();
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(30, 30) == golParametters.AliveColor, "A pixel with 3 alive neighbors should stay alive.");
		gameService.ClearPixels();
		gameService.SwitchPixel(new Vector2(30, 30));
		gameService.SwitchPixel(new Vector2(30, 31));
		gameService.SwitchPixel(new Vector2(30, 29));
		gameService.GoOneStep();
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(30, 30) == golParametters.AliveColor, "A pixel with 2 alive neighbors should stay alive.");
		gameService.ClearPixels();

		// Test overpopulation: a pixel with more than 3 alive neighbors should die
		gameService.SwitchPixel(new Vector2(30, 30));
		gameService.SwitchPixel(new Vector2(30, 31));
		gameService.SwitchPixel(new Vector2(30, 29));
		gameService.SwitchPixel(new Vector2(29, 30));
		gameService.SwitchPixel(new Vector2(29, 31));
		gameService.GoOneStep();
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(30, 30) == golParametters.DeadColor, "A pixel with 4 (>3) alive neighbors should die.");
		gameService.ClearPixels();
		gameService.SwitchPixel(new Vector2(30, 30));
		gameService.SwitchPixel(new Vector2(30, 31));
		gameService.SwitchPixel(new Vector2(30, 29));
		gameService.SwitchPixel(new Vector2(29, 30));
		gameService.SwitchPixel(new Vector2(29, 31));
		gameService.SwitchPixel(new Vector2(31, 31));
		gameService.GoOneStep();
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(30, 30) == golParametters.DeadColor, "A pixel with 5 (>3) alive neighbors should die.");
		gameService.ClearPixels();
		gameService.SwitchPixel(new Vector2(30, 30));
		gameService.SwitchPixel(new Vector2(30, 31));
		gameService.SwitchPixel(new Vector2(30, 29));
		gameService.SwitchPixel(new Vector2(29, 30));
		gameService.SwitchPixel(new Vector2(29, 31));
		gameService.SwitchPixel(new Vector2(31, 31));
		gameService.SwitchPixel(new Vector2(31, 30));
		gameService.GoOneStep();
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(30, 30) == golParametters.DeadColor, "A pixel with 6 (>3) alive neighbors should die.");
		gameService.ClearPixels();
		gameService.SwitchPixel(new Vector2(30, 30));
		gameService.SwitchPixel(new Vector2(30, 31));
		gameService.SwitchPixel(new Vector2(30, 29));
		gameService.SwitchPixel(new Vector2(29, 30));
		gameService.SwitchPixel(new Vector2(29, 31));
		gameService.SwitchPixel(new Vector2(31, 31));
		gameService.SwitchPixel(new Vector2(31, 30));
		gameService.SwitchPixel(new Vector2(31, 29));
		gameService.GoOneStep();
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(30, 30) == golParametters.DeadColor, "A pixel with 7 (>3) alive neighbors should die.");
		gameService.ClearPixels();
		gameService.SwitchPixel(new Vector2(30, 30));
		gameService.SwitchPixel(new Vector2(30, 31));
		gameService.SwitchPixel(new Vector2(30, 29));
		gameService.SwitchPixel(new Vector2(29, 30));
		gameService.SwitchPixel(new Vector2(29, 31));
		gameService.SwitchPixel(new Vector2(31, 31));
		gameService.SwitchPixel(new Vector2(31, 30));
		gameService.SwitchPixel(new Vector2(31, 29));
		gameService.SwitchPixel(new Vector2(29, 29));
		gameService.GoOneStep();
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(30, 30) == golParametters.DeadColor, "A pixel with 7 (>3) alive neighbors should die.");
		gameService.ClearPixels();

		// Test underpopulation: a pixel with less than 2 alive neighbors should die
		gameService.SwitchPixel(new Vector2(30, 30));
		gameService.SwitchPixel(new Vector2(30, 31));
		gameService.GoOneStep();
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(30, 30) == golParametters.DeadColor, "A pixel with 1 alive neighbor should die.");
		gameService.ClearPixels();
		gameService.SwitchPixel(new Vector2(30, 30));
		gameService.GoOneStep();
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(30, 30) == golParametters.DeadColor, "A pixel with no alive neighbor should die.");
		gameService.ClearPixels();
	}

	private bool AllPixselsAreOfColor(Color color)
	{
		Color[] pixels = imageComponent.sprite.texture.GetPixels();
		foreach (Color pixel in pixels)
		{
			if (pixel != color)
			{
				return false;
			}
		}
		return true;
	}

	[Test]
	public void TestKnownPaterns()
	{
		DeadAliveGameModeParameters golParametters = AssetDatabase.LoadAssetAtPath<DeadAliveGameModeParameters>("Assets/ScriptableObjects/GameOfLifeParameters.asset");
		Assert.IsTrue(AllPixselsAreOfColor(golParametters.DeadColor), "Test starts with all dead pixels, they should stay dead.");

		// Test Block
		gameService.SwitchPixel(new Vector2(30, 30));
		gameService.SwitchPixel(new Vector2(30, 31));
		gameService.SwitchPixel(new Vector2(31, 30));
		gameService.SwitchPixel(new Vector2(31, 31));
		gameService.GoOneStep();
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(30, 30) == golParametters.AliveColor, "Block should leave an alive pixel at (30, 30).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(30, 31) == golParametters.AliveColor, "Block should leave an alive pixel at (30, 31).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(31, 30) == golParametters.AliveColor, "Block should leave an alive pixel at (31, 30).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(31, 31) == golParametters.AliveColor, "Block should leave an alive pixel at (31, 31).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(29, 29) == golParametters.DeadColor, "Block should not create new pixel at (29, 29).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(29, 30) == golParametters.DeadColor, "Block should not create new pixel at (29, 30).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(29, 31) == golParametters.DeadColor, "Block should not create new pixel at (29, 31).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(29, 32) == golParametters.DeadColor, "Block should not create new pixel at (29, 32).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(30, 29) == golParametters.DeadColor, "Block should not create new pixel at (30, 29).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(30, 32) == golParametters.DeadColor, "Block should not create new pixel at (30, 32).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(31, 29) == golParametters.DeadColor, "Block should not create new pixel at (31, 29).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(31, 32) == golParametters.DeadColor, "Block should not create new pixel at (31, 32).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(32, 29) == golParametters.DeadColor, "Block should not create new pixel at (32, 29).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(32, 30) == golParametters.DeadColor, "Block should not create new pixel at (32, 30).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(32, 31) == golParametters.DeadColor, "Block should not create new pixel at (32, 31).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(32, 32) == golParametters.DeadColor, "Block should not create new pixel at (32, 32).");
		gameService.ClearPixels();

		// Test Blinker
		gameService.SwitchPixel(new Vector2(30, 29));
		gameService.SwitchPixel(new Vector2(30, 30));
		gameService.SwitchPixel(new Vector2(30, 31));
		for (int i = 0; i < 11; i++)
		{
			gameService.GoOneStep();
		}
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(30, 30) == golParametters.AliveColor, "Blinker should leave an alive pixel at (30, 30).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(30, 31) == golParametters.DeadColor, "Blinker should not leave a pixel at (30, 31).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(30, 29) == golParametters.DeadColor, "Blinker should not leave a pixel at (30, 29).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(29, 30) == golParametters.AliveColor, "Blinker should leave an alive pixel at (29, 30).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(31, 30) == golParametters.AliveColor, "Blinker should leave an alive pixel at (31, 30).");
		gameService.ClearPixels();

		// Test glider
		gameService.SwitchPixel(new Vector2(30, 30));
		gameService.SwitchPixel(new Vector2(31, 31));
		gameService.SwitchPixel(new Vector2(32, 31));
		gameService.SwitchPixel(new Vector2(32, 30));
		gameService.SwitchPixel(new Vector2(32, 29));
		for (int i = 0; i < 10; i++)
		{
			gameService.GoOneStep();
		}
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(30, 30) == golParametters.DeadColor, "Glider should leave no pixels behind (30, 30).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(31, 31) == golParametters.DeadColor, "Glider should leave no pixels behind (31, 31).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(32, 31) == golParametters.DeadColor, "Glider should leave no pixels behind (32, 31).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(32, 30) == golParametters.DeadColor, "Glider should leave no pixels behind (32, 30).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(32, 29) == golParametters.DeadColor, "Glider should leave no pixels behind (32, 29).");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(33, 33) == golParametters.AliveColor, "Glider should have moved (33, 33) is alive.");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(34, 33) == golParametters.AliveColor, "Glider should have moved (34, 33) is alive.");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(35, 33) == golParametters.AliveColor, "Glider should have moved (35, 33) is alive.");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(35, 32) == golParametters.AliveColor, "Glider should have moved (35, 32) is alive.");
		Assert.IsTrue(imageComponent.sprite.texture.GetPixel(34, 31) == golParametters.AliveColor, "Glider should have moved (34, 31) is alive.");
		gameService.ClearPixels();
	}
}