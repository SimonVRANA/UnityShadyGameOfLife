// This code has been made by Simon VRANA.
// Please ask by email (simon.vrana.pro@gmail.com) before reusing for commercial purpose.

using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguagesButtons : MonoBehaviour
{
	public void ChangeLanguage(string languageKey)
	{
		UnityEngine.Localization.Locale local = LocalizationSettings.AvailableLocales.Locales.Where(local => local.Identifier == languageKey).FirstOrDefault();
		if (local != null)
		{
			LocalizationSettings.SelectedLocale = local;
		}
	}
}
