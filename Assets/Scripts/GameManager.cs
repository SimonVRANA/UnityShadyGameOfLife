// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[Header("Links")]
	[SerializeField]
	private Image gameImage;

	[SerializeField]
	private Toggle playToggle;

	[Header("GameOfLife")]
	[SerializeField]
	private Color deadColor;

	[SerializeField]
	private Color aliveColor;

	[Header("Shaders")]
	[SerializeField]
	private Material clearShader;

	[SerializeField]
	private Material gameOfLifeShader;

	public int MaxColumnsNumber => 1760;

	private int columnsNumber = 496;
	public int ColumnsNumber
	{
		get => columnsNumber;
		set
		{
			columnsNumber = AdjustColumnsNumber(value);

			Texture2D newTexture = new(columnsNumber, columnsNumber * 9 / 16)
			{
				filterMode = FilterMode.Point
			};
			Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), new Vector2(0.5f, 0.5f), 100);
			gameImage.sprite = newSprite;
			Clear();
			OnColumnsNumberChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public event EventHandler OnColumnsNumberChanged;

	public int MaxSpeedValue => 1200;

	private int speedValue = 2;
	public int SpeedValue
	{
		get => speedValue;
		set
		{
			speedValue = Mathf.Clamp(value, 0, MaxSpeedValue);
			framesLeftUntilApplyGameLogic = speedValue;
		}
	}

	private int framesLeftUntilApplyGameLogic;

	private void Awake()
	{
		Application.targetFrameRate = 60;
	}

	// Start is called before the first frame update
	void Start()
	{
		framesLeftUntilApplyGameLogic = speedValue;
		SetColumnsNumber(columnsNumber);
	}

	// Update is called once per frame
	void Update()
	{
		if (!playToggle.isOn)
		{
			if (framesLeftUntilApplyGameLogic != speedValue)
			{
				framesLeftUntilApplyGameLogic = speedValue;
			}
			return;
		}

		if (framesLeftUntilApplyGameLogic <= 0)
		{
			framesLeftUntilApplyGameLogic = speedValue;
			ApplyGameMechanic();
		}
		else
		{
			framesLeftUntilApplyGameLogic--;
		}
	}

	public void SetColumnsNumber(int newNumber)
	{
		ColumnsNumber = newNumber;
	}

	private int AdjustColumnsNumber(int newNumber)
	{
		int restult = Mathf.Clamp(newNumber, 16, MaxColumnsNumber);
		restult = (restult / 16) * 16;
		return restult;
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

	public void ApplyGameMechanic()
	{
		gameOfLifeShader.SetColor("_DeadColor", deadColor);
		gameOfLifeShader.SetColor("_AliveColor", aliveColor);
		ApplyMaterialToGameImage(gameOfLifeShader);
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
