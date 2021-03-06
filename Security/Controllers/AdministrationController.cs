﻿// Copyright © 2017 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.

using ExtCore.Data.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Security.Common.Attributes;
using Security.Common.Enums;
using ControllerBase = Infrastructure.ControllerBase;

namespace Security.Controllers
{
    [PermissionRequirement(Permission.Admin)]
    public class AdministrationController : ControllerBase
    {
        private ILogger _logger;

        public AdministrationController(IStorage storage_, ILoggerFactory loggerFactory_) : base(storage_, loggerFactory_)
        {
            _logger = _loggerFactory.CreateLogger(GetType().FullName);
            _logger.LogInformation("oups");
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("Administration");
        }
    }
}