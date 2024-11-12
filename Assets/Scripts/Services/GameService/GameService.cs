// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using System;
using UnityEngine;
using Zenject;
using static IGameService;

public class GameService : MonoBehaviour, IGameService
{
	[Inject]
	private readonly IImageService imageService;

	private Color deadColor;
	private Color aliveColor;

	private Material clearShader;
	private Material gameOfLifeShader;

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

	public event EventHandler OnPlayingChanged;

	public GameModes GameMode { get; }

	public event EventHandler OnGameModeChanged;

	private int framesLeftUntilApplyGameLogic;

	public void Initialize(Color deadColor, Color aliveColor, Material clearShader, Material gameOfLifeShader)
	{
		this.deadColor = deadColor;
		this.aliveColor = aliveColor;
		this.clearShader = clearShader;
		this.gameOfLifeShader = gameOfLifeShader;
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

		imageService.RandomizePixels(deadColor, aliveColor);
	}

	public void ClearPixels()
	{
		clearShader.SetColor("_Color", deadColor);
		imageService.ApplyShader(clearShader);
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
		gameOfLifeShader.SetColor("_DeadColor", deadColor);
		gameOfLifeShader.SetColor("_AliveColor", aliveColor);
		imageService.ApplyShader(gameOfLifeShader);
	}
}