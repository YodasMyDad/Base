// Copyright © 2017 SOFTINUX. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Barebone.ViewModels.Shared.Scripts;
using Microsoft.Extensions.Logging;

namespace Barebone.ViewComponents
{
    public class ScriptsRenderSectionViewComponent : ViewComponentBase
    {
        public ScriptsRenderSectionViewComponent(ILoggerFactory loggerFactory_) : base(loggerFactory_)
        {
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            ScriptsViewModel model = new ScriptsViewModelFactory().GetScriptsSections();
            return Task.FromResult<IViewComponentResult>(View(model));
        }


    }
}