// Copyright © 2017 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtCore.Data.Abstractions;
using ExtCore.Infrastructure;
using Infrastructure.Extensions;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Security.Common;
using Security.Common.Attributes;
using Security.Data.Abstractions;
using Security.Data.Entities;
using Security.Extensions;
using Security.Tools;
using Security.ViewModels.Permissions;
using ControllerBase = Infrastructure.ControllerBase;
using Permission = Security.Common.Enums.Permission;

namespace Security.Controllers
{
    [PermissionRequirement(Common.Enums.Permission.Admin)]
    public class GrantPermissionsController : ControllerBase
    {
        private readonly RoleManager<IdentityRole<string>> _roleManager;

        public GrantPermissionsController(IStorage storage_, RoleManager<IdentityRole<string>> roleManager_) : base(storage_)
        {
            _roleManager = roleManager_;
        }

        [PermissionRequirement(Permission.Admin)]
        [Route("administration/grantpermissions")]
        [HttpGet]
        public IActionResult Index()
        {
           
            // Create a dictionary with all roles for injecting as json into grant permission page
            Dictionary<string, IdentityRole<string>> rolesList = new Dictionary<string, IdentityRole<string>>();

            // Create a dictionary with role id and name, since we will use role name in GrantViewModel
            // and we have role id in RolePermission table.
            Dictionary<string, string> roleNameByRoleId = new Dictionary<string, string>();

            foreach (var role in _roleManager.Roles)
            {
                roleNameByRoleId.Add(role.Id, role.Name);
                rolesList.Add(role.Id, role);
            }

            ViewBag.RolesList = rolesList;

            var model = ReadGrants.ReadAll(_roleManager, Storage, roleNameByRoleId);
            return View(model);
        }

        /// <summary>
        /// Update a record indicating with which permission this role is linked to an extension.
        /// </summary>
        /// <param name="roleName_">Role name</param>
        /// <param name="permissionId_">New permission level to save</param>
        /// <param name="scope_">Scope</param>
        /// <returns>JSON with "true" when it succeeded</returns>
        [PermissionRequirement(Permission.Admin)]
        [Route("administration/updaterolepermission")]
        [HttpGet] // TODO change to POST
        public async Task<IActionResult> UpdateRolePermission(string roleName_, string permissionId_, string scope_)
        {
            string roleId = (await _roleManager.FindByNameAsync(roleName_)).Id;
            IRolePermissionRepository repo = Storage.GetRepository<IRolePermissionRepository>();
            repo.Delete(roleId, scope_);
            if (!string.IsNullOrEmpty(permissionId_.ToLowerInvariant()))
                repo.Create(new RolePermission { RoleId = roleId, PermissionId = permissionId_.UppercaseFirst(), Scope = scope_ });
            Storage.Save();
            return new JsonResult(true);
        }

        /// <summary>
        /// Delete the record indicating that a role is linked to an extension.
        /// </summary>
        /// <param name="roleName_"></param>
        /// <param name="scope_"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteRoleExtensionLink(string roleName_, string scope_)
        {
            string roleId = (await _roleManager.FindByNameAsync(roleName_)).Id;
            IRolePermissionRepository repo = Storage.GetRepository<IRolePermissionRepository>();
            if (repo.FindBy(roleId, scope_) != null)
            {
                repo.Delete(roleId, scope_);
                Storage.Save();
                return new JsonResult(true);
            }
            return new JsonResult(false);
        }

        /// <summary>
        /// Create a role. Then create a set of records indicating to which extensions with which permission this role is linked to.
        /// </summary>
        /// <param name="model_"></param>
        /// <returns></returns>
        [Route("administration/savenewrole")]
        [HttpPost]
        public async Task<ObjectResult> SaveNewRoleAndItsPermissions(SaveNewRoleAndGrantsViewModel model_)
        {
            string error = await CreateRoleAndGrants.CheckAndSaveNewRoleAndGrants(model_, _roleManager, Storage);
            return StatusCode(string.IsNullOrEmpty(error) ? 201 : 400, error);
        }

        /// <summary>
        /// Return role for edition.
        /// </summary>
        /// <param name="roleID_"></param>
        /// <returns>JSON role object</returns>
        [PermissionRequirement(Permission.Admin)]
        [Route("administration/findrole")]
        [HttpGet]
        public async Task<IActionResult> GetRole(string roleId_)
        {
            if (string.IsNullOrWhiteSpace(roleId_) || string.IsNullOrEmpty(roleId_))
                return StatusCode(400, Json("No given role for edition."));

            object _role = await _roleManager.FindByIdAsync(roleId_);

            if (_role == null)
                return StatusCode(400, Json("No such role for edit."));

            return StatusCode(200, Json(_role));
        }

        /// <summary>
        /// Update role name into database.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [PermissionRequirement(Permission.Admin)]
        [Route("administration/updaterolename")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRoleName(string name, string id){
            return Json("Not yet implemented");
        }
    }
}