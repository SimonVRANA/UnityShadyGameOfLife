// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using TMPro;
using UnityEngine;

public class ColumnsInputManager : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField inputfield;

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
			int adjustedNewInt = gamemanager.AdjustColumnsNumber(newInt);
			gamemanager.ColumnsNumber = adjustedNewInt;
		}
		catch { }
		finally
		{
			inputfield.text = gamemanager.ColumnsNumber.ToString();
		}
	}
}
