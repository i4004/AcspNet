﻿using AcspNet.Html;
using AcspNet.Identity;

namespace AcspNet
{
	/// <summary>
	/// AcspNet controllers and views base class
	/// </summary>
	public abstract class Container : IHideObjectMembers
	{
		/// <summary>
		/// Current HTTP and ACSP context
		/// </summary>
		public virtual IAcspContext Context { get; internal set; }

		/// <summary>
		/// Current request environment data.
		/// </summary>
		public virtual IEnvironment Environment { get; internal set; }

		/// <summary>
		/// Current language manager
		/// </summary>
		public virtual ILanguageManager LanguageManager { get; internal set; }

		/// <summary>
		/// Text and XML files loader.
		/// </summary>
		public virtual IFileReader FileReader { get; internal set; }

		/// <summary>
		/// Text templates loader.
		/// </summary>
		public virtual ITemplateFactory TemplateFactory { get; internal set; }

		/// <summary>
		/// Localizable text items string table.
		/// </summary>
		public virtual IStringTable StringTable { get; internal set; }

		/// <summary>
		/// Web-site master page data collector.
		/// </summary>
		public virtual IDataCollector DataCollector { get; internal set; }

		/// <summary>
		/// Various HTML generation classes
		/// </summary>
		public virtual IHtmlWrapper Html { get; internal set; }

		/// <summary>
		/// Controls users login/logout/autnenticate via cookie or session and stores current user name/password/id unformation
		/// </summary>
		public virtual IAuthentication Authentication { get; internal set; }
		
		/// <summary>
		/// Identifier processor that is used to parse and act on 'ID' field from request query string or form.
		/// </summary>
		public virtual IIdVerifier IdVerifier { get; internal set; }
	}
}