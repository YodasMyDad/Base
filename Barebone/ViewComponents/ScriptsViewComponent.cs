// Copyright © 2017 SOFTINUX. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Barebone.ViewModels.Shared.Scripts;
using Microsoft.Extensions.Logging;

namespace Barebone.ViewComponents
{
    public class ScriptsViewComponent : ViewComponentBase
    {
        public ScriptsViewComponent(ILoggerFactory loggerFactory_) : base(loggerFactory_)
        {
        }

        public Task<IViewComponentResult> InvokeAsync(string section_)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            ScriptsViewModel model = new ScriptsViewModelFactory().Create(section_);
            watch.Stop();
            LoggerFactory.CreateLogger<ScriptsViewComponent>().LogInformation("Time to build scripts list by ScriptsViewModelFactory: " + watch.ElapsedMilliseconds + " ms");
            // When asking for scripts not linked to a section, the scripts will be included in _Layout.cshtml so rendered as a list of scripts.
            // Else, they're surrounded by the section declaration.
            if (string.IsNullOrWhiteSpace(section_))
                return Task.FromResult<IViewComponentResult>(View(model));
            return Task.FromResult<IViewComponentResult>(View("_ScriptsSection", model));
        }


    }
}