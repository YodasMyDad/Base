﻿// Copyright © 2017 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.

using ExtCore.Data.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Security.Data.Entities;

namespace Security.Data.EntityFramework
{
    public class EntityRegistrar : IEntityRegistrar
    {
        public void RegisterEntities(ModelBuilder modelBuilder_)
        {

            // non-WIF entities

            // Permission
            modelBuilder_.Entity<Permission>(etb_ =>
                {
                    etb_.HasKey(e_ => e_.Id);
                    etb_.Property(e_ => e_.Id).ValueGeneratedOnAdd();
                }
            );

            // RolePermission
            modelBuilder_.Entity<RolePermission>(etb_ =>
                {
                    etb_.HasKey(e_ => new { e_.RoleId, e_.PermissionId, e_.Scope });
                }
            );

            // UserPermission
            modelBuilder_.Entity<UserPermission>(etb_ =>
                {
                    etb_.HasKey(e_ => new { e_.UserId, e_.PermissionId, e_.Scope });
                }
            );
        }
    }
}
