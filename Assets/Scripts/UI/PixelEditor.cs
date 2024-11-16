// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class PixelEditor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	[Inject]
	private readonly IImageService imageService;

	[Inject]
	private readonly IGameService gameService;

	[SerializeField]
	private GameObject settings;

	private bool isEditing = false;

	private Vector2 lastPixelEdited = new(-1, -1);

	public void OnPointerDown(PointerEventData eventData)
	{
		isEditing = true;
		lastPixelEdited = new(-1, -1);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		isEditing = false;
	}

	private void Update()
	{
		if (!isEditing
			|| settings.activeSelf)
		{
			return;
		}

		EditPixelAtMousePotision();
	}

	private void EditPixelAtMousePotision()
	{
		Vector2 imagePosition = ScreenToImagePosition(new Vector2(Input.mousePosition.x, Input.mousePosition.y));

		if (imagePosition.y > 1)
		{
			return;
		}

		Vector2 pixelPosition = imageService.ImagePositionToPixelPosition(imagePosition);

		if (pixelPosition != lastPixelEdited)
		{
			gameService.SwitchPixel(pixelPosition);
			lastPixelEdited = pixelPosition;
		}
	}

	private Vector2 ScreenToImagePosition(Vector2 screenPosition)
	{
		float x = screenPosition.x / Screen.width;

		// Part of the screen is used for menus, we need to remove it to get the image height.
		float windowReferenceHeight = 1080; // the reference screen is 1920x1080
		float menuReferenceHeight = 120; // on a 1920x1080 screen, the menu have a height of 120
		float imageHeight = Screen.height * ((windowReferenceHeight - menuReferenceHeight) / windowReferenceHeight);

		float y = screenPosition.y / imageHeight;

		return new Vector2(x, y);
	}
}