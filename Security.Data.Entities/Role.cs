﻿// Copyright © 2017 SOFTINUX. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using System.Collections.Generic;
using ExtCore.Data.Entities.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Security.Data.Entities
{
    public class Role : IdentityRole<int>, IEntity
    {
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
