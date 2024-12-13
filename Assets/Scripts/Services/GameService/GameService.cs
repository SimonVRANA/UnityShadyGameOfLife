// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static IGameService;

public class GameService : MonoBehaviour, IGameService
{
	[Inject]
	private readonly IImageService imageService;

	private Dictionary<GameModes, DeadAliveGameMode> gameModes;

	private int numberOfFramesBetweenUpdates = 2;

	public int NumberOfFramesBetweenUpdates
	{
		get => numberOfFramesBetweenUpdates;
		set
		{
			numberOfFramesBetweenUpdates = Mathf.Clamp(value, 0, IGameService.maxNumberOfFramesBetweenUpdates);
			framesLeftUntilApplyGameLogic = numberOfFramesBetweenUpdates;
		}
	}

	private bool isPlaying = false;

	public bool IsPlaying
	{
		get => isPlaying;
		private set
		{
			isPlaying = value;
			OnPlayingChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public GameModes GameMode { get; private set; } = GameModes.GameOfLife;

	public event EventHandler OnPlayingChanged;

	public event EventHandler OnGameModeChanged;

	private int framesLeftUntilApplyGameLogic;

	public void Initialize(Dictionary<GameModes, DeadAliveGameMode> gameModes)
	{
		this.gameModes = gameModes;
	}

	private void Awake()
	{
		Application.targetFrameRate = 60;
	}

	private void Start()
	{
		framesLeftUntilApplyGameLogic = NumberOfFramesBetweenUpdates;
		imageService.NumberOfColumns = imageService.NumberOfColumns;
	}

	// Update is called once per frame
	private void Update()
	{
		if (!IsPlaying)
		{
			if (framesLeftUntilApplyGameLogic != NumberOfFramesBetweenUpdates)
			{
				framesLeftUntilApplyGameLogic = NumberOfFramesBetweenUpdates;
			}
			return;
		}

		if (framesLeftUntilApplyGameLogic <= 0)
		{
			framesLeftUntilApplyGameLogic = NumberOfFramesBetweenUpdates;
			GoOneStep();
		}
		else
		{
			framesLeftUntilApplyGameLogic--;
		}
	}

	public void ApplyRandomPixels()
	{
		ClearPixels();

		imageService.RandomizePixels(GetCurrentGameMode().DeadColor, GetCurrentGameMode().AliveColor);
	}

	public void ClearPixels()
	{
		imageService.ApplyShader(GetCurrentGameMode().GetClearShader());
	}

	public void Play()
	{
		IsPlaying = true;
	}

	public void Pause()
	{
		IsPlaying = false;
	}

	public void GoOneStep()
	{
		imageService.ApplyShader(GetCurrentGameMode().GetGameShader());
	}

	public void SwitchPixel(Vector2 pixelPosition)
	{
		Color pixelColor = imageService.GetPixelColor(pixelPosition);

		Color newColor = pixelColor == GetCurrentGameMode().DeadColor ? GetCurrentGameMode().AliveColor
																	  : GetCurrentGameMode().DeadColor;
		imageService.SetPixelColor(pixelPosition, newColor);
	}

	public void ChangeGameMode(GameModes newGameMode)
	{
		if (newGameMode != GameMode)
		{
			GameMode = newGameMode;
			OnGameModeChanged?.Invoke(this, EventArgs.Empty);
			ClearPixels();
		}
	}

	private DeadAliveGameMode GetCurrentGameMode()
	{
		if (gameModes.ContainsKey(GameMode))
		{
			return gameModes[GameMode];
		}
		return null;
	}
}