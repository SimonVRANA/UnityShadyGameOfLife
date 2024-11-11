// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using Helyn.DesignSystem;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Zenject;

public class SettingsMenu : MonoBehaviour
{
	[Inject]
	private readonly IGameService gameService;

	[Header("Buttons")]
	[SerializeField]
	private ToggleButton toggleButton;

	[SerializeField]
	private ToggleButton playButton;

	[SerializeField]
	private ButtonBase oneStepButton;

	[Header("Sub-menus")]
	[SerializeField]
	private GameObject settings;

	private void Start()
	{
		toggleButton.IsToggled = false;
		settings.SetActive(false);
	}

	private void OnEnable()
	{
		LocalizationSettings.SelectedLocaleChanged += OnLocalChanged;
		gameService.OnPlayingChanged += UpdateButtonsStates;
	}

	private void OnDisable()
	{
		LocalizationSettings.SelectedLocaleChanged -= OnLocalChanged;
		gameService.OnPlayingChanged -= UpdateButtonsStates;
	}

	public void OpenCloseSettings()
	{
		settings.SetActive(toggleButton.IsToggled);
		_ = ForeceRedoAllLayout();
	}

	public void OnLocalChanged(UnityEngine.Localization.Locale local)
	{
		_ = ForeceRedoAllLayout();
	}

	private async Task ForeceRedoAllLayout()
	{
		await Task.Yield();
		RecursivelyForceRebuildLayout((RectTransform)transform);
		await Task.Delay(300);
		RecursivelyForceRebuildLayout((RectTransform)transform);
		await Task.Delay(300);
		RecursivelyForceRebuildLayout((RectTransform)transform);
		await Task.Delay(300);
		RecursivelyForceRebuildLayout((RectTransform)transform);
	}

	private void RecursivelyForceRebuildLayout(RectTransform transformToRedo)
	{
		foreach (RectTransform childTransform in transformToRedo)
		{
			RecursivelyForceRebuildLayout(childTransform);
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(transformToRedo);
	}

	public void Clear()
	{
		gameService.ClearPixels();
	}

	public void Random()
	{
		gameService.ApplyRandomPixels();
	}

	public void OneStep()
	{
		gameService.GoOneStep();
	}

	public void PlayPause()
	{
		if (gameService.IsPlaying)
		{
			gameService.Pause();
		}
		else
		{
			gameService.Play();
		}
	}

	public void UpdateButtonsStates(object sender, EventArgs args)
	{
		playButton.IsToggled = gameService.IsPlaying;
		oneStepButton.interactable = !gameService.IsPlaying;
	}
}