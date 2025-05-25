// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using NUnit.Framework;
using SGOL.Services.Image;
using System;
using System.Collections;
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

		GameObject zenjectContextGameObject = GameObject.Find("Zenject");
		SceneContext sceneContext = zenjectContextGameObject.GetComponent<SceneContext>();
		DiContainer container = sceneContext.Container;
		Assert.IsNotNull(container);
		imageService = container.Resolve<IImageService>();

		GameObject imageGameObject = GameObject.Find("Image");
		imageComponent = imageGameObject.GetComponent<UnityEngine.UI.Image>();

		yield return null;
	}

	[UnityTest]
	public IEnumerator TestNumberOfColumns()
	{
		imageService.NumberOfColumns = 12;
		Assert.AreEqual(imageComponent.sprite.rect.width, 12);
		Assert.AreEqual(imageComponent.sprite.rect.height, (int)Math.Floor(12 * IImageService.columnsToRowsRatio));

		// Use the Assert class to test conditions.
		// Use yield to skip a frame.
		yield return null;
	}
}