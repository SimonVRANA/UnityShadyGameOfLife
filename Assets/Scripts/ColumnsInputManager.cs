// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColumnsInputManager : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField inputfield;

	[SerializeField]
	private Slider slider;

	[SerializeField]
	private GameManager gamemanager;

	public void OnInputFieldValueChanged()
	{
		if (string.IsNullOrEmpty(inputfield.text))
		{
			inputfield.text = gamemanager.ColumnsNumber.ToString();
			return;
		}

		try
		{
			int newInt = int.Parse(inputfield.text);
			gamemanager.ColumnsNumber = newInt;
		}
		catch { }
		finally
		{
			UpdateText();
			UpdateSlider();
		}
	}

	public void OnSliderValueChanged()
	{
		try
		{
			gamemanager.ColumnsNumber = (int)slider.value * 16;
		}
		catch { }
		finally
		{
			UpdateText();
			UpdateSlider();
		}
	}

	public void OnColumnsNumberChanged(object sender, EventArgs eventArgs)
	{
		UpdateText();
		UpdateSlider();
	}

	private void UpdateText()
	{
		inputfield.text = gamemanager.ColumnsNumber.ToString();
	}

	private void UpdateSlider()
	{
		slider.value = gamemanager.ColumnsNumber / 16;
	}

	private void OnEnable()
	{
		slider.maxValue = gamemanager.MaxColumnsNumber / 16;

		UpdateText();
		UpdateSlider();

		gamemanager.OnColumnsNumberChanged += OnColumnsNumberChanged;
	}

	private void OnDisable()
	{
		gamemanager.OnColumnsNumberChanged -= OnColumnsNumberChanged;
	}
}
