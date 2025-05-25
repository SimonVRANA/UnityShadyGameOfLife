// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using SGOL.Services.Image;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SGOL.UI
{
	public class ColumnsInputManager : MonoBehaviour
	{
		[Inject]
		private readonly IImageService imageService;

		[SerializeField]
		private TMP_InputField inputfield;

		[SerializeField]
		private Slider slider;

		private void OnEnable()
		{
			slider.maxValue = IImageService.maximumNumberOfColumns / IImageService.columnsIncrementNumber;

			UpdateText();
			UpdateSlider();

			imageService.NumberOfColumnsChanged += OnColumnsNumberChanged;
		}

		private void OnDisable()
		{
			imageService.NumberOfColumnsChanged -= OnColumnsNumberChanged;
		}

		public void OnInputFieldValueChanged()
		{
			if (string.IsNullOrEmpty(inputfield.text))
			{
				inputfield.text = imageService.NumberOfColumns.ToString();
				return;
			}

			try
			{
				int newInt = int.Parse(inputfield.text);
				imageService.NumberOfColumns = newInt;
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
				imageService.NumberOfColumns = (int)slider.value * IImageService.columnsIncrementNumber;
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
			inputfield.text = imageService.NumberOfColumns.ToString();
		}

		private void UpdateSlider()
		{
			slider.value = imageService.NumberOfColumns / IImageService.columnsIncrementNumber;
		}
	}
}