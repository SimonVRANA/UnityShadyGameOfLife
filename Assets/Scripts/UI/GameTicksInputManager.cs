// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using SGOL.Services.Game;
using TMPro;
using UnityEngine;
using Zenject;

namespace SGOL.UI
{
	public class GameTicksInputManager : MonoBehaviour
	{
		[Inject]
		private readonly IGameService gameService;

		[SerializeField]
		private TMP_InputField inputfield;

		public void OnInputFieldValueChanged()
		{
			if (string.IsNullOrEmpty(inputfield.text))
			{
				inputfield.text = gameService.NumberOfFramesBetweenUpdates.ToString();
				return;
			}

			try
			{
				int newInt = int.Parse(inputfield.text);
				gameService.NumberOfFramesBetweenUpdates = newInt;
			}
			catch { }
			finally
			{
				inputfield.text = gameService.NumberOfFramesBetweenUpdates.ToString();
			}
		}
	}
}