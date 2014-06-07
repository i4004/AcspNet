﻿namespace AcspNet.Modules
{
	/// <summary>
	/// Represent current language controller and information container
	/// </summary>
	public interface ILanguageManager : IHideObjectMembers
	{
		/// <summary>
		/// Site current language, for example: "en", "ru", "de" etc.
		/// </summary>
		string Language { get; }
		
		/// <summary>
		/// Set site cookie language value
		/// </summary>
		/// <param name="language">Language code</param>
		void SetCookieLanguage(string language);

		/// <summary>
		/// Set language for current request
		/// </summary>
		/// <param name="language">Language code</param>
		void SetCurrentLanguage(string language);
	}
}