﻿using System;
using System.Collections.Generic;
using System.Linq;
using AcspNet.ModelBinding.Binders;
using AcspNet.ModelBinding.Validation;
using AcspNet.Modules;

namespace AcspNet.ModelBinding
{
	/// <summary>
	/// Provides model handling
	/// </summary>
	public class HttpModelHandler : IModelHandler
	{
		private static readonly IList<Type> ModelBindersTypesInstance = new List<Type>
		{
			typeof (HttpQueryModelBinder),
			typeof (HttpFormModelBinder)
		};

		private static readonly IList<Type> ModelValidatorTypesInstance = new List<Type>
		{
			typeof (ObjectPropertiesValidator)
		};

		private readonly IAcspNetContext _context;

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpModelHandler"/> class.
		/// </summary>
		/// <param name="context">The context.</param>
		public HttpModelHandler(IAcspNetContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Gets the model binders types.
		/// </summary>
		/// <value>
		/// The model binders types.
		/// </value>
		public static IList<Type> ModelBindersTypes
		{
			get { return ModelBindersTypesInstance; }
		}
		
		/// <summary>
		/// Gets the model validators types.
		/// </summary>
		/// <value>
		/// The model validators types.
		/// </value>
		public static IList<Type> ModelValidatorsTypes
		{
			get { return ModelValidatorTypesInstance; }
		}

		/// <summary>
		/// Registers the model binder.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public static void RegisterModelBinder<T>()
			where T : IModelBinder
		{
			ModelBindersTypes.Add(typeof(T));
		}

		/// <summary>
		/// Registers the model validator.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public static void RegisterModelValidator<T>()
			where T : IModelValidator
		{
			ModelValidatorsTypes.Add(typeof(T));
		}

		/// <summary>
		/// Parses object from data and validates it
		/// </summary>
		/// <typeparam name="T">Model type</typeparam>
		/// <returns></returns>
		/// <exception cref="ModelBindingException"></exception>
		public T Process<T>()
		{
			var args = new ModelBinderEventArgs<T>(_context);

			foreach (var binder in ModelBindersTypes.Select(binderType => (IModelBinder)Activator.CreateInstance(binderType)))
			{
				binder.Bind(args);

				if (!args.IsBinded) continue;

				foreach (var validator in ModelValidatorsTypes.Select(x => (IModelValidator)Activator.CreateInstance(x)))
					validator.Validate(args.Model);

				return args.Model;
			}

			throw new ModelBindingException(string.Format("Unrecognized request content type for binding: {0}", _context.Request.ContentType));
		}
	}
}