// Copyright © 2017 SOFTINUX. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Barebone.ViewModels.Shared.Script;
using ExtCore.Infrastructure;
using Infrastructure.Interfaces;

namespace Barebone.ViewModels.Shared.Scripts
{
    public class ScriptsViewModelFactory
    {
        /// <summary>
        /// Builds the list of scripts from extensions metadata.
        /// </summary>
        /// <param name="section_">When defined, returns the scripts associated to this section, else the scripts without section</param>
        /// <returns></returns>
        public ScriptsViewModel Create(string section_)
        {
            List<Infrastructure.Script> scripts = new List<Infrastructure.Script>();

            foreach (IExtensionMetadata extensionMetadata in ExtensionManager.GetInstances<IExtensionMetadata>())
                scripts.AddRange(string.IsNullOrWhiteSpace(section_)
                    ? extensionMetadata.Scripts.Where(s_ => string.IsNullOrWhiteSpace(s_.Section))
                    : extensionMetadata.Scripts.Where(s_ => s_.Section == section_));

            return new ScriptsViewModel()
            {
                Scripts = scripts.OrderBy(s => s.Position).Select(s => new ScriptViewModelFactory().Create(s))
            };
        }

        public ScriptsViewModel GetScriptsSections()
        {
            HashSet<string> sections = new HashSet<string>();
            foreach (IExtensionMetadata extensionMetadata in ExtensionManager.GetInstances<IExtensionMetadata>())
                foreach (Infrastructure.Script script in extensionMetadata.Scripts.Where(s_ =>
                    !string.IsNullOrWhiteSpace(s_.Section)))
                    sections.Add(script.Section);
            return new ScriptsViewModel {Sections = sections.ToList()};
        }
    }
}