// Copyright © 2017 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Barebone.ViewModels.Shared.Script;
using ExtCore.Infrastructure;
using Infrastructure.Interfaces;

namespace Barebone.ViewModels.Shared.Scripts
{
    public class ScriptsViewModelFactory
    {
        public ScriptsViewModel Create()
        {
            List<Infrastructure.Script> scripts = new List<Infrastructure.Script>();

            foreach (IExtensionMetadata extensionMetadata in ExtensionManager.GetInstances<IExtensionMetadata>())
                if(extensionMetadata.Scripts != null)
                    scripts.AddRange(extensionMetadata.Scripts);

            return new ScriptsViewModel()
            {
                Scripts = scripts.OrderBy(s => s.Position).Select(s => new ScriptViewModelFactory().Create(s))
            };
        }
    }
}