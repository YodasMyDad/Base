// Copyright © 2017 SOFTINUX. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

using System.Collections.Generic;
using Barebone.ViewModels.Shared.Script;

namespace Barebone.ViewModels.Shared.Scripts
{
    public class ScriptsViewModel
    {
        /// <summary>
        /// The scripts to use on page.
        /// </summary>
        public IEnumerable<ScriptViewModel> Scripts { get; set; }

        /// <summary>
        /// All the sections used by scripts.
        /// </summary>
        public IEnumerable<string> Sections { get; set; }
    }
}