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
		yield return null;
		Assert.IsTrue(eventFired, "Event should fire after number is changed");

		yield return null;
	}
}