// Copyright © 2017 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.

using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Security.Common.Enums;

namespace Security.Common.Attributes
{

    public class PermissionRequirementAttribute : ActionFilterAttribute
    {
        private Permission _permissionName;
        private string _scope;

        /// <summary>
        /// Permission unique identifier.
        /// </summary>
        public string PermissionIdentifier { get { return PermissionHelper.GetScopedPermissionIdentifier(_permissionName, _scope); } }

        /// <summary>
        /// Allows access when the user has the permission : a claim of type "Permission" with value
        /// defined by its level (Admin, Write, Read...) and its scope (Security, ExtensionX...).
        /// Or the user has a permission with same scope but higher level (Admin when Write is the minimum requested to be granted access).
        /// Default scope (assembly simple name) is global scope ("Security").
        /// </summary>
        public PermissionRequirementAttribute(Permission permissionName_, string extensionAssemblySimpleName_ = "Security")
        {
            _permissionName = permissionName_;
            _scope = extensionAssemblySimpleName_;
        }

        public override void OnActionExecuting(ActionExecutingContext context_)
        {
            bool accessGranted = false;
            // Get the user claim, if any, matching the scope of interest
            Claim claimOfLookupScope = context_.HttpContext.User.Claims.FirstOrDefault(c_ => c_.Type == ClaimType.Permission.ToString() && c_.Value.ToString().StartsWith($"{_scope}."));

            if(claimOfLookupScope != null)
            {
                Permission currentLevel = Enum.Parse<Permission>(PermissionHelper.GetPermissionLevel(claimOfLookupScope));
                if((int)currentLevel >= (int)_permissionName)
                {
                    // access granted
                    accessGranted = true;
                }
            }

             if (!accessGranted)
            {
                context_.Result = new ForbidResult();
            }
        }
    }
}
