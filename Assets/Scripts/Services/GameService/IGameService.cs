// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using System;
using UnityEngine;

public interface IGameService
{
	public enum GameModes
	{
		GameOfLife,
		ReactionDiffusion
	}

	public const int maxNumberOfFramesBetweenUpdates = 1200;
	public int NumberOfFramesBetweenUpdates { get; set; }

	public bool IsPlaying { get; }

	public event EventHandler OnPlayingChanged;

	public GameModes GameMode { get; }

	public event EventHandler OnGameModeChanged;

	public void ApplyRandomPixels();

	public void ClearPixels();

	public void Play();

	public void Pause();

	public void GoOneStep();

	public void SwitchPixel(Vector2 pixelPosition);
}