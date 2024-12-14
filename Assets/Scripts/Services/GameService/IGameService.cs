// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using System;
using UnityEngine;

public interface IGameService
{
	public const int maxNumberOfFramesBetweenUpdates = 1200;
	public int NumberOfFramesBetweenUpdates { get; set; }

	public bool IsPlaying { get; }

	public event EventHandler OnPlayingChanged;

	public event EventHandler OnGameModeChanged;

	public void ApplyRandomPixels();

	public void ClearPixels();

	public void Play();

	public void Pause();

	public void GoOneStep();

	public void SwitchPixel(Vector2 pixelPosition);

	public string GameMode { get; }

	public string[] GetGameModes();

	public void ChangeGameMode(string newGameMode);
}