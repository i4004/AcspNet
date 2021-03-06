﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AcspNet.Meta;
using AcspNet.Routing;

namespace AcspNet.Core
{
	/// <summary>
	/// Provides controllers agent
	/// </summary>
	public class ControllersAgent : IControllersAgent
	{
		private readonly IControllersMetaStore _controllersMetaStore;
		private readonly IRouteMatcher _routeMatcher;

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllersAgent" /> class.
		/// </summary>
		/// <param name="controllersMetaStore">The controllers meta store.</param>
		/// <param name="routeMatcher">The route matcher.</param>
		public ControllersAgent(IControllersMetaStore controllersMetaStore, IRouteMatcher routeMatcher)
		{
			_controllersMetaStore = controllersMetaStore;
			_routeMatcher = routeMatcher;
		}

		/// <summary>
		/// Gets the standard controllers meta data.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IControllerMetaData> GetStandardControllersMetaData()
		{
			return _controllersMetaStore.ControllersMetaData.Where(
				x =>
					x.Role == null || (x.Role.Is400Handler == false && x.Role.Is403Handler == false && x.Role.Is404Handler == false));
		}

		/// <summary>
		/// Matches the controller route.
		/// </summary>
		/// <param name="controllerMetaData">The controller meta data.</param>
		/// <param name="sourceRoute">The source route.</param>
		/// <param name="httpMethod">The HTTP method.</param>
		/// <returns></returns>
		public IRouteMatchResult MatchControllerRoute(IControllerMetaData controllerMetaData, string sourceRoute, string httpMethod)
		{
			if (controllerMetaData.ExecParameters == null || controllerMetaData.ExecParameters.RouteInfo == null)
				return _routeMatcher.Match(sourceRoute, null);

			switch (httpMethod)
			{
				case "GET":
					return controllerMetaData.ExecParameters.RouteInfo.GetRoute == null
						? null
						: _routeMatcher.Match(sourceRoute, controllerMetaData.ExecParameters.RouteInfo.GetRoute);

				case "POST":
					return controllerMetaData.ExecParameters.RouteInfo.PostRoute == null
						? null
						: _routeMatcher.Match(sourceRoute, controllerMetaData.ExecParameters.RouteInfo.PostRoute);

				case "PUT":
					return controllerMetaData.ExecParameters.RouteInfo.PutRoute == null
						? null
						: _routeMatcher.Match(sourceRoute, controllerMetaData.ExecParameters.RouteInfo.PutRoute);

				case "DELETE":
					return controllerMetaData.ExecParameters.RouteInfo.DeleteRoute == null
						? null
						: _routeMatcher.Match(sourceRoute, controllerMetaData.ExecParameters.RouteInfo.DeleteRoute);
			}

			return null;
		}

		/// <summary>
		/// Gets the handler controller.
		/// </summary>
		/// <param name="controllerType">Type of the controller.</param>
		/// <returns></returns>
		public IControllerMetaData GetHandlerController(HandlerControllerType controllerType)
		{
			IControllerMetaData metaData = null;

			switch (controllerType)
			{
				case HandlerControllerType.Http403Handler:
					metaData = _controllersMetaStore.ControllersMetaData.FirstOrDefault(x => x.Role != null && x.Role.Is403Handler);
					break;

				case HandlerControllerType.Http404Handler:
					metaData = _controllersMetaStore.ControllersMetaData.FirstOrDefault(x => x.Role != null && x.Role.Is404Handler);
					break;
			}

			return metaData;
		}

		/// <summary>
		/// Determines whether controller can be executed on any page.
		/// </summary>
		/// <param name="metaData">The controller meta data.</param>
		/// <returns></returns>
		public bool IsAnyPageController(IControllerMetaData metaData)
		{
			if(metaData.Role != null)
				if(metaData.Role.Is400Handler
					|| metaData.Role.Is403Handler
					|| metaData.Role.Is404Handler)
					return false;

			if(metaData.ExecParameters != null)
				if(metaData.ExecParameters.RouteInfo != null)
					if(!string.IsNullOrEmpty(metaData.ExecParameters.RouteInfo.GetRoute)
						|| !string.IsNullOrEmpty(metaData.ExecParameters.RouteInfo.PostRoute)
						|| !string.IsNullOrEmpty(metaData.ExecParameters.RouteInfo.PutRoute)
						|| !string.IsNullOrEmpty(metaData.ExecParameters.RouteInfo.DeleteRoute))
						return false;

			return true;
		}

		/// <summary>
		/// Determines whether controller security rules violated.
		/// </summary>
		/// <param name="metaData">The controller meta data.</param>
		/// <param name="user">The current request user.</param>
		/// <returns></returns>
		public SecurityRuleCheckResult IsSecurityRulesViolated(IControllerMetaData metaData, ClaimsPrincipal user)
		{
			if (metaData.Security == null)
				return SecurityRuleCheckResult.Ok;

			if (!metaData.Security.IsAuthorizationRequired)
				return SecurityRuleCheckResult.Ok;
			
			if (metaData.Security.AllowedUserRoles == null)
				return user == null || user.Identity == null || !user.Identity.IsAuthenticated ? SecurityRuleCheckResult.NotAuthenticated : SecurityRuleCheckResult.Ok;

			if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
				return SecurityRuleCheckResult.NotAuthenticated;

			return metaData.Security.AllowedUserRoles.Any(user.IsInRole) ? SecurityRuleCheckResult.Ok : SecurityRuleCheckResult.Forbidden;
		}
	}
}