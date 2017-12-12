﻿// Copyright © 2017 SOFTINUX. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using Security;
using SecurityTest.Util;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Infrastructure.Enums;
using Security.Data.Abstractions;
using Security.Data.Entities;
using Security.Common;
using Security.Common.Enums;
using Xunit;
using Permission = Security.Data.Entities.Permission;

namespace SecurityTest
{
    [TestCaseOrderer("SecurityTest.PriorityOrderer", "SecurityTest")]
    [Collection("Database collection")]
    public class PermissionManagerTest : BaseTest
    {
        private const string CST_PERM_CODE_1 = "test_perm_1";
        private const string CST_PERM_CODE_2 = "test_perm_2";
        private const string CST_PERM_CODE_3 = "test_perm_3";
        private const string CST_ROLE_CODE_1 = "test_role_1";
        private const string CST_GROUP_CODE_1 = "test_group_1";
        private const string CST_GROUP_CODE_2 = "test_group_2";

        private static readonly string _assembly = typeof(PermissionManagerTest).Assembly.GetName().Name;

        // permission levels

        private PermissionLevel _writeLevel, _readLevel, _neverLevel, _noLevel;


        public PermissionManagerTest(DatabaseFixture fixture_)
        {
            _fixture = fixture_;
            IPermissionLevelRepository levelRepo = _fixture.GetRepository<IPermissionLevelRepository>();
            _writeLevel = levelRepo.FindBy(PermissionLevelValue.ReadWrite);
            _readLevel = levelRepo.FindBy(PermissionLevelValue.ReadOnly);
            _neverLevel = levelRepo.FindBy(PermissionLevelValue.Never);
            _noLevel = levelRepo.FindBy(PermissionLevelValue.No);

        }

        /// <summary>
        /// Same formatting as method that formats claim value from permission unique id and access level.
        /// </summary>
        /// <param name="permissionCode_"></param>
        /// <returns></returns>
        private string FormatExpectedClaimValue(string permissionCode_)
        {
            return $"{permissionCode_}|{_assembly}".ToLowerInvariant();
        }

        /// <summary>
        /// Test of GetFinalPermissions() passing permission-related data.
        /// A reference to the same permission is passed, first with R right level, second with RW.
        /// Expected: one claim with R right level, one claim with RW right level (one claim per permission).
        /// </summary>
        [Fact(Skip = "To be fixed later"), TestPriority(1)]
        public void TestGetFinalPermissionsRAndRwNoDatabase()
        {
            #region test data setup
            Permission roPerm = new Permission { Code = CST_PERM_CODE_1, OriginExtension = _assembly };
            Permission rwPerm = new Permission { Code = CST_PERM_CODE_1, OriginExtension = _assembly };
            PermissionValue roPv = new PermissionValue { UniqueId = roPerm.UniqueIdentifier, Level = (byte)PermissionLevelValue.ReadOnly };
            PermissionValue rwPv = new PermissionValue { UniqueId = rwPerm.UniqueIdentifier, Level = (byte)PermissionLevelValue.ReadWrite };

            #endregion
            IEnumerable<Claim> claims =
                new PermissionManager().GetFinalPermissions(new List<PermissionValue> { roPv, rwPv }, false);
            Assert.Equal(2, claims.Count());
            Assert.Equal(ClaimType.Permission, claims.First().Type);
            List<string> claimValues = new List<string>();
            foreach (Claim claim in claims)
            {
                claimValues.Add(claim.Value);
            }

            List<string> expectedClaimValues = new List<string>
            {
                FormatExpectedClaimValue(roPerm.Code)
            };

            claimValues.Sort();
            expectedClaimValues.Sort();
            Assert.Equal(expectedClaimValues, claimValues);
        }

        /// <summary>
        /// Test of GetFinalPermissions() passing permission-related data.
        /// A reference to a permission is passed, with R right level.
        /// Expected: one claim with R right level.
        /// </summary>
        [Fact(Skip = "To be fixed later"), TestPriority(2)]
        public void TestGetFinalPermissionsRNoDatabase()
        {
            #region test data setup
            Permission roPerm = new Permission { Code = CST_PERM_CODE_1, OriginExtension = _assembly };
            PermissionValue roPv = new PermissionValue { UniqueId = roPerm.UniqueIdentifier, Level = (byte)PermissionLevelValue.ReadOnly };

            #endregion
            IEnumerable<Claim> claims =
                new PermissionManager().GetFinalPermissions(new List<PermissionValue> { roPv }, false);
            Assert.Single(claims);
            Assert.Equal(ClaimType.Permission, claims.First().Type);
            List<string> claimValues = new List<string>();
            foreach (Claim claim in claims)
            {
                claimValues.Add(claim.Value);
            }

            List<string> expectedClaimValues = new List<string> { FormatExpectedClaimValue(roPerm.Code) };

            claimValues.Sort();
            expectedClaimValues.Sort();
            Assert.Equal(expectedClaimValues, claimValues);
        }

        /// <summary>
        /// Test of GetFinalPermissions() passing permission-related data.
        /// A reference to a permission is passed, with RW right level.
        /// Expected: one claim with R right level, one claim with RW right level (explicit claim for RW and implicit for R).
        /// </summary>
        [Fact(Skip = "To be fixed later"), TestPriority(3)]
        public void TestGetFinalPermissionsRwNoDatabase()
        {
            #region test data setup
            Permission rwPerm = new Permission { Code = CST_PERM_CODE_1, OriginExtension = _assembly };
            PermissionValue rwPv = new PermissionValue { UniqueId = rwPerm.UniqueIdentifier, Level = (byte)PermissionLevelValue.ReadWrite };

            #endregion
            IEnumerable<Claim> claims =
                new PermissionManager().GetFinalPermissions(new List<PermissionValue> { rwPv }, false);
            Assert.Equal(2, claims.Count());
            Assert.Equal(ClaimType.Permission, claims.First().Type);
            List<string> claimValues = new List<string>();
            foreach (Claim claim in claims)
            {
                claimValues.Add(claim.Value);
            }

            List<string> expectedClaimValues = new List<string>
            {
                FormatExpectedClaimValue(rwPerm.Code)
            };

            claimValues.Sort();
            expectedClaimValues.Sort();
            Assert.Equal(expectedClaimValues, claimValues);
        }

        /// <summary>
        /// Test of loading permissions from database, from roles and groups.
        /// A permission is attributed to a group, another one to a role. user is linked to these role and group.
        /// Expected : two permissions loaded.
        /// </summary>
        [Fact(Skip = "To be fixed later"), TestPriority(4)]
        public void TestLoadPermissions()
        {
            try
            {
                _fixture.OpenTransaction();

                #region test data setup
                // Permission 1, Role 1, Permission 2, Permission 3, Group 1, Group 2, User 1
                Permission perm1 = new Permission { Code = CST_PERM_CODE_1, Label = "Perm 1", OriginExtension = _assembly };
                Permission perm2 = new Permission { Code = CST_PERM_CODE_2, Label = "Perm 2", OriginExtension = _assembly };
                Permission perm3 = new Permission { Code = CST_PERM_CODE_3, Label = "Perm 3", OriginExtension = _assembly };

                Role role1 = new Role { Code = CST_ROLE_CODE_1, Label = "Role 1", OriginExtension = _assembly };

                Group group1 = new Group { Code = CST_GROUP_CODE_1, Label = "Group 1", OriginExtension = _assembly };
                Group group2 = new Group { Code = CST_GROUP_CODE_2, Label = "Group 2", OriginExtension = _assembly };

                User user1 = new User { DisplayName = "Test", FirstName = "Test", LastName = "Test" };

                IPermissionRepository permRepo = _fixture.GetRepository<IPermissionRepository>();
                permRepo.Create(perm1);
                permRepo.Create(perm2);
                permRepo.Create(perm3);

                _fixture.GetRepository<IRoleRepository>().Create(role1);

                IGroupRepository groupRepo = _fixture.GetRepository<IGroupRepository>();
                groupRepo.Create(group1);
                groupRepo.Create(group2);

                _fixture.GetRepository<IUserRepository>().Create(user1);

                _fixture.SaveChanges();

                // Link Permission 1 to Role 1, R
                _fixture.GetRepository<IRolePermissionRepository>().Create(new RolePermission
                {
                    PermissionId = perm1.Id,
                    RoleId = role1.Id,
                    PermissionLevelId = _readLevel.Id
                });

                // Link Permission 2 to Group 1, R
                _fixture.GetRepository<IGroupPermissionRepository>().Create(new GroupPermission
                {
                    PermissionId = perm2.Id,
                    GroupId = group1.Id,
                    PermissionLevelId = _readLevel.Id
                });

                // Link Permission 3 to Group 2, R
                _fixture.GetRepository<IGroupPermissionRepository>().Create(new GroupPermission
                {
                    PermissionId = perm3.Id,
                    GroupId = group2.Id,
                    PermissionLevelId = _readLevel.Id
                });

                // Link Role 1 and Group 1 to user 1
                _fixture.GetRepository<IUserRoleRepository>().Create(new UserRole
                {
                    RoleId = role1.Id,
                    UserId = user1.Id
                });

                _fixture.GetRepository<IGroupUserRepository>().Create(new GroupUser
                {
                    GroupId = group1.Id,
                    UserId = user1.Id
                });

                _fixture.SaveChanges();

                #endregion

                IEnumerable<PermissionValue> perms = new PermissionManager().LoadPermissionLevels(_fixture.DatabaseContext, user1);
                Assert.Equal(2, perms.Count());
                List<string> permCodes = new List<string>();
                foreach (PermissionValue perm in perms)
                {
                    permCodes.Add(perm.UniqueId);
                }

                Assert.Contains(perm1.UniqueIdentifier, permCodes);
                Assert.Contains(perm2.UniqueIdentifier, permCodes);
            }
            finally
            {
                _fixture.RollbackTransaction();
            }
        }

        /// <summary>
        /// Test of GetFinalPermissions() with permission loading and computation.
        /// The same permission is linked to role, group and user with different right level.
        /// Expected: The "Never" right level takes precedence, thus no claim.
        /// </summary>
        [Fact(Skip = "To be fixed later"), TestPriority(5)]
        public void TestGetFinalPermissionsWithLevels()
        {
            try
            {
                _fixture.OpenTransaction();

                #region test data setup

                // Permission 1, Role 1, User 1, Group 1
                Permission perm1 = new Permission { Code = CST_PERM_CODE_1, Label = "Perm 1", OriginExtension = _assembly };

                Role role1 = new Role { Code = CST_ROLE_CODE_1, Label = "Role 1", OriginExtension = _assembly };

                Group group1 = new Group { Code = CST_GROUP_CODE_1, Label = "Group 1", OriginExtension = _assembly };

                User user1 = new User { DisplayName = "Test", FirstName = "Test", LastName = "Test" };

                _fixture.GetRepository<IPermissionRepository>().Create(perm1);
                _fixture.GetRepository<IRoleRepository>().Create(role1);
                _fixture.GetRepository<IGroupRepository>().Create(group1);
                _fixture.GetRepository<IUserRepository>().Create(user1);

                _fixture.SaveChanges();

                // Link Permission 1 to Role 1, RO
                _fixture.GetRepository<IRolePermissionRepository>().Create(new RolePermission
                {
                    PermissionId = perm1.Id,
                    RoleId = role1.Id,
                    PermissionLevelId = _readLevel.Id
                });

                // Link Permission 1 to User 1, Never
                _fixture.GetRepository<IUserPermissionRepository>().Create(new UserPermission
                {
                    PermissionId = perm1.Id,
                    UserId = user1.Id,
                    PermissionLevelId = _neverLevel.Id
                });

                // Link Permission 1 to Group 1, RW
                _fixture.GetRepository<IGroupPermissionRepository>().Create(new GroupPermission
                {
                    PermissionId = perm1.Id,
                    GroupId = group1.Id,
                    PermissionLevelId = _writeLevel.Id
                });

                // Link Role 1 and Group 1 to user 1
                _fixture.GetRepository<IUserRoleRepository>().Create(new UserRole
                {
                    RoleId = role1.Id,
                    UserId = user1.Id
                });

                _fixture.GetRepository<IGroupUserRepository>().Create(new GroupUser
                {
                    GroupId = group1.Id,
                    UserId = user1.Id
                });

                _fixture.SaveChanges();

                #endregion

                IEnumerable<Claim> claims = new PermissionManager().GetFinalPermissions(_fixture.DatabaseContext, user1);
                // Expected no permission because permission is denied
                Assert.Empty(claims);
            }
            finally
            {
                _fixture.RollbackTransaction();
            }
        }

        /// <summary>
        /// Test of GetFinalPermissions() with permission loading and computation.
        /// A permission flagged as administrator-owner is attached to the admin role, linked to a user.
        /// This permission is removed for user's group ("never" right level).
        /// Expected : A permission flagged as administrator-owner can be ungranted to a superadmin user.
        /// </summary>
        [Fact(Skip = "To be fixed later"), TestPriority(6)]
        public void TestGetFinalPermissionsWithAdminOwnerFlagStillGranted()
        {
            try
            {
                _fixture.OpenTransaction();

                #region test data setup

                // Permission 1, User 1, Group 1
                Permission perm1 = new Permission { Code = CST_PERM_CODE_1, Label = "Perm 1", OriginExtension = _assembly, AdministratorOwner = true };

                Group group1 = new Group { Code = CST_GROUP_CODE_1, Label = "Group 1", OriginExtension = _assembly };

                User user1 = new User { DisplayName = "Test", FirstName = "Test", LastName = "Test" };

                Role role1 = _fixture.GetRepository<IRoleRepository>().FindBy(Roles.ROLE_CODE_ADMINISTRATOR_OWNER, "Security");

                IRolePermissionRepository rolePermRepo = _fixture.GetRepository<IRolePermissionRepository>();

                _fixture.GetRepository<IPermissionRepository>().Create(perm1);
                _fixture.GetRepository<IGroupRepository>().Create(group1);
                _fixture.GetRepository<IUserRepository>().Create(user1);

                _fixture.SaveChanges();

                // Link Permission 1 to Role, RW
                rolePermRepo.Create(new RolePermission
                {
                    PermissionId = perm1.Id,
                    RoleId = role1.Id,
                    PermissionLevelId = _writeLevel.Id
                });

                // Link Role 1 to user 1
                _fixture.GetRepository<IUserRoleRepository>().Create(new UserRole
                {
                    RoleId = role1.Id,
                    UserId = user1.Id
                });

                _fixture.SaveChanges();

                int initialClaimCount = new PermissionManager().GetFinalPermissions(_fixture.DatabaseContext, user1).Count();

                // Link Permission 1 to Group 1, Never
                _fixture.GetRepository<IGroupPermissionRepository>().Create(new GroupPermission
                {
                    PermissionId = perm1.Id,
                    GroupId = group1.Id,
                    PermissionLevelId = _neverLevel.Id
                });

                // Link Group 1 to user 1
                _fixture.GetRepository<IGroupUserRepository>().Create(new GroupUser
                {
                    GroupId = group1.Id,
                    UserId = user1.Id
                });

                _fixture.SaveChanges();

                #endregion

                IEnumerable<Claim> claims = new PermissionManager().GetFinalPermissions(_fixture.DatabaseContext, user1);
                // Expected permission to be still granted, RW and R
                Assert.Equal(initialClaimCount, claims.Count());
                Assert.NotNull(claims.FirstOrDefault(c_ => c_.Value == FormatExpectedClaimValue(perm1.Code)));

            }
            finally
            {
                _fixture.RollbackTransaction();
            }
        }

        /// <summary>
        /// Test of GetFinalPermissions() with permission loading and computation.
        /// A permission flagged as administrator-owner is attached to the superadmin role, linked to a user.
        /// The use has not superadmin but admin role.
        /// This permission is removed for user's group ("never" right level).
        /// Expected : A permission flagged as administrator-owner can be ungranted to a non superadmin user.
        /// </summary>
        [Fact(Skip = "To be fixed later"), TestPriority(7)]
        public void TestGetFinalPermissionsWithAdminOwnerFlagUngranted()
        {
            try
            {
                _fixture.OpenTransaction();

                #region test data setup

                // Permission 1, User 1, Group 1
                Permission perm1 = new Permission { Code = CST_PERM_CODE_1, Label = "Perm 1", OriginExtension = _assembly, AdministratorOwner = true };

                Group group1 = new Group { Code = CST_GROUP_CODE_1, Label = "Group 1", OriginExtension = _assembly };

                User user1 = new User { DisplayName = "Test", FirstName = "Test", LastName = "Test" };

                Role role1 = _fixture.GetRepository<IRoleRepository>().FindBy(Roles.ROLE_CODE_ADMINISTRATOR, "Security");

                IRolePermissionRepository rolePermRepo = _fixture.GetRepository<IRolePermissionRepository>();

                _fixture.GetRepository<IPermissionRepository>().Create(perm1);
                _fixture.GetRepository<IGroupRepository>().Create(group1);
                _fixture.GetRepository<IUserRepository>().Create(user1);

                _fixture.SaveChanges();

                // Link Permission 1 to Role, R
                rolePermRepo.Create(new RolePermission
                {
                    PermissionId = perm1.Id,
                    RoleId = role1.Id,
                    PermissionLevelId = _readLevel.Id
                });

                // Link Role 1 to user 1
                _fixture.GetRepository<IUserRoleRepository>().Create(new UserRole
                {
                    RoleId = role1.Id,
                    UserId = user1.Id
                });

                _fixture.SaveChanges();

                int initialClaimCount = new PermissionManager().GetFinalPermissions(_fixture.DatabaseContext, user1).Count();

                // Link Permission 1 to Group 1, Never
                _fixture.GetRepository<IGroupPermissionRepository>().Create(new GroupPermission
                {
                    PermissionId = perm1.Id,
                    GroupId = group1.Id,
                    PermissionLevelId = _neverLevel.Id
                });

                // Link Group 1 to user 1
                _fixture.GetRepository<IGroupUserRepository>().Create(new GroupUser
                {
                    GroupId = group1.Id,
                    UserId = user1.Id
                });

                _fixture.SaveChanges();

                #endregion

                IEnumerable<Claim> claims = new PermissionManager().GetFinalPermissions(_fixture.DatabaseContext, user1);
                // Expected permission to be ungranted
                Assert.Equal(initialClaimCount - 1, claims.Count());
                Assert.Null(claims.FirstOrDefault(c_ => c_.Value == FormatExpectedClaimValue(perm1.Code)));

            }
            finally
            {
                _fixture.RollbackTransaction();
            }
        }

        /// <summary>
        /// Test of GetFinalPermissions() with permission loading and computation.
        /// A permission flagged as administrator-owner is attached to a group, linked to a user.
        /// The use has superadmin role.
        /// This permission is removed for user ("never" right level).
        /// Expected : A permission flagged as administrator-owner cannot be ungranted to a superadmin user.
        /// </summary>
        [Fact(Skip = "To be fixed later"), TestPriority(8)]
        public void TestGetFinalPermissionsWithAdminOwnerFlagStillGrantedCaseTwo()
        {
            try
            {
                _fixture.OpenTransaction();

                #region test data setup

                // Permission 1, User 1, Group 1
                Permission perm1 = new Permission { Code = CST_PERM_CODE_1, Label = "Perm 1", OriginExtension = _assembly, AdministratorOwner = true };

                Group group1 = new Group { Code = CST_GROUP_CODE_1, Label = "Group 1", OriginExtension = _assembly };

                User user1 = new User { DisplayName = "Test", FirstName = "Test", LastName = "Test" };

                Role role1 = _fixture.GetRepository<IRoleRepository>().FindBy(Roles.ROLE_CODE_ADMINISTRATOR_OWNER, "Security");

                _fixture.GetRepository<IPermissionRepository>().Create(perm1);
                _fixture.GetRepository<IGroupRepository>().Create(group1);
                _fixture.GetRepository<IUserRepository>().Create(user1);

                _fixture.SaveChanges();

                // Link Permission 1 to Group, R
                _fixture.GetRepository<IGroupPermissionRepository>().Create(new GroupPermission
                {
                    PermissionId = perm1.Id,
                    GroupId = group1.Id,
                    PermissionLevelId = _readLevel.Id
                });

                // Link Group 1 to user 1
                _fixture.GetRepository<IGroupUserRepository>().Create(new GroupUser
                {
                    GroupId = group1.Id,
                    UserId = user1.Id
                });

                // Link Role 1 to user 1
                _fixture.GetRepository<IUserRoleRepository>().Create(new UserRole
                {
                    RoleId = role1.Id,
                    UserId = user1.Id
                });

                _fixture.SaveChanges();

                int initialClaimCount = new PermissionManager().GetFinalPermissions(_fixture.DatabaseContext, user1).Count();

                // Link Permission 1 to User 1, Never
                _fixture.GetRepository<IUserPermissionRepository>().Create(new UserPermission
                {
                    PermissionId = perm1.Id,
                    UserId = user1.Id,
                    PermissionLevelId = _neverLevel.Id
                });

                _fixture.SaveChanges();

                #endregion

                IEnumerable<Claim> claims = new PermissionManager().GetFinalPermissions(_fixture.DatabaseContext, user1);
                // Expected permission to be still granted
                Assert.Equal(initialClaimCount, claims.Count());
                Assert.NotNull(claims.FirstOrDefault(c_ => c_.Value == FormatExpectedClaimValue(perm1.Code)));
            }
            finally
            {
                _fixture.RollbackTransaction();
            }
        }

        /// <summary>
        /// Test of GetFinalPermissions() with permission loading and computation.
        /// A permission flagged as administrator-owner is attached to a group, linked to a user.
        /// The use has not superadmin but admin role.
        /// This permission is removed for user ("never" right level).
        /// Expected : A permission flagged as administrator-owner can be ungranted to a non-superadmin user.
        /// </summary>
        [Fact(Skip = "To be fixed later"), TestPriority(9)]
        public void TestGetFinalPermissionsWithAdminOwnerFlagUngrantedCaseTwo()
        {
            try
            {
                _fixture.OpenTransaction();

                #region test data setup

                // Permission 1, User 1, Group 1
                Permission perm1 = new Permission { Code = CST_PERM_CODE_1, Label = "Perm 1", OriginExtension = _assembly, AdministratorOwner = true };

                Group group1 = new Group { Code = CST_GROUP_CODE_1, Label = "Group 1", OriginExtension = _assembly };

                User user1 = new User { DisplayName = "Test", FirstName = "Test", LastName = "Test" };

                Role role1 = _fixture.GetRepository<IRoleRepository>().FindBy(Roles.ROLE_CODE_ADMINISTRATOR, "Security");

                _fixture.GetRepository<IPermissionRepository>().Create(perm1);
                _fixture.GetRepository<IGroupRepository>().Create(group1);
                _fixture.GetRepository<IUserRepository>().Create(user1);

                _fixture.SaveChanges();

                // Link Permission 1 to Group, R
                _fixture.GetRepository<IGroupPermissionRepository>().Create(new GroupPermission
                {
                    PermissionId = perm1.Id,
                    GroupId = group1.Id,
                    PermissionLevelId = _readLevel.Id
                });

                // Link Role 1 to user 1
                _fixture.GetRepository<IUserRoleRepository>().Create(new UserRole
                {
                    RoleId = role1.Id,
                    UserId = user1.Id
                });

                // Link Group 1 to user 1
                _fixture.GetRepository<IGroupUserRepository>().Create(new GroupUser
                {
                    GroupId = group1.Id,
                    UserId = user1.Id
                });

                _fixture.SaveChanges();

                int initialClaimCount = new PermissionManager().GetFinalPermissions(_fixture.DatabaseContext, user1).Count();

                // Link Permission 1 to User 1, Never
                _fixture.GetRepository<IUserPermissionRepository>().Create(new UserPermission
                {
                    PermissionId = perm1.Id,
                    UserId = user1.Id,
                    PermissionLevelId = _neverLevel.Id
                });

                _fixture.SaveChanges();

                #endregion

                IEnumerable<Claim> claims = new PermissionManager().GetFinalPermissions(_fixture.DatabaseContext, user1);
                // Expected permission to be ungranted
                Assert.Equal(initialClaimCount - 1, claims.Count());
                Assert.Null(claims.FirstOrDefault(c_ => c_.Value == FormatExpectedClaimValue(perm1.Code)));
            }
            finally
            {
                _fixture.RollbackTransaction();
            }
        }

    }
}
