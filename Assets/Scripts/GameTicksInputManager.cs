// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using TMPro;
using UnityEngine;

public class GameTicksInputManager : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField inputfield;

	[SerializeField]
	private GameManager gamemanager;

	public void OnInputFieldValueChanged()
	{
		if (string.IsNullOrEmpty(inputfield.text))
		{
			inputfield.text = gamemanager.SpeedValue.ToString();
			return;
		}

		try
		{
			int newInt = int.Parse(inputfield.text);
			gamemanager.SpeedValue = newInt;
		}
		catch { }
		finally
		{
			inputfield.text = gamemanager.SpeedValue.ToString();
		}
	}
}
