// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using SGOL.GameModes;
using SGOL.Services.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace SGOL.Services.Game
{
	public class GameService : MonoBehaviour, IGameService
	{
		private IImageService imageService;

		private Dictionary<string, DeadAliveGameMode> gameModes;

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

		public string GameMode { get; private set; } = "Game of Life";

		public event EventHandler OnPlayingChanged;

		public event EventHandler OnGameModeChanged;

		private int framesLeftUntilApplyGameLogic;

		public void Initialize(Dictionary<string, DeadAliveGameMode> gameModes)
		{
			this.gameModes = gameModes;

			if (gameModes.Count <= 0)
			{
				Debug.LogError("No Game modes provided !");
			}

			if (!gameModes.ContainsKey(GameMode))
			{
				GameMode = gameModes.Keys.First();
			}
		}

		[Inject]
		private void Inject(IImageService imageService)
		{
			this.imageService = imageService;
			imageService.NumberOfColumnsChanged += ImageService_OnNumberOfColumnsChanged;
		}

		private void OnDestroy()
		{
			imageService.NumberOfColumnsChanged -= ImageService_OnNumberOfColumnsChanged;
		}

		private void ImageService_OnNumberOfColumnsChanged(object sender, EventArgs args)
		{
			ClearPixels();
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

			imageService.RandomizePixels(GetCurrentGameMode().DeadColor, GetCurrentGameMode().AliveColor, GetCurrentGameMode().AliveRatio);
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
			if (pixelPosition.x < 0 || pixelPosition.y < 0
				|| pixelPosition.x >= imageService.NumberOfColumns || pixelPosition.y >= (int)MathF.Floor(imageService.NumberOfColumns * IImageService.columnsToRowsRatio))
			{
				throw new ArgumentOutOfRangeException(nameof(pixelPosition), "Pixel position must be within the bounds of the image.");
			}

			Color pixelColor = imageService.GetPixelColor(pixelPosition);

			Color newColor = pixelColor == GetCurrentGameMode().DeadColor ? GetCurrentGameMode().AliveColor
																		  : GetCurrentGameMode().DeadColor;
			imageService.SetPixelColor(pixelPosition, newColor);
		}

		public string[] GetGameModes()
		{
			return gameModes.Keys.ToArray();
		}

		public void ChangeGameMode(string newGameMode)
		{
			if (newGameMode != GameMode)
			{
				if (gameModes.ContainsKey(newGameMode))
				{
					GameMode = newGameMode;
					OnGameModeChanged?.Invoke(this, EventArgs.Empty);
					ClearPixels();
				}
				else
				{
					throw new ArgumentException($"Game mode '{newGameMode}' does not exist in the available game modes.");
				}
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
}