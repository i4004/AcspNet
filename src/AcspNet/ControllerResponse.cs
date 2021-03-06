﻿using AcspNet.Core;

namespace AcspNet
{
	/// <summary>
	/// Provides controllers responses base class
	/// </summary>
	public abstract class ControllerResponse : ActionModulesAccessor
	{
		/// <summary>
		/// Gets the response writer.
		/// </summary>
		/// <value>
		/// The response writer.
		/// </value>
		public virtual IResponseWriter ResponseWriter { get; internal set; }

		/// <summary>
		/// Processes this response
		/// </summary>
		public abstract ControllerResponseResult Process();
	}
}