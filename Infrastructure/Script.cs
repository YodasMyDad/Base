// Copyright © 2017 SOFTINUX. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

namespace Infrastructure
{
    /// <summary>
    /// Definition of a javascript script that should be included on pages (views.)
    /// </summary>
    public class Script
    {
        /// <summary>
        /// Absolute path relative to application root.
        /// The slashes excepted the first one have to be replaced by a dot because scripts
        /// are served as embedded resources.
        /// </summary>
        /// <returns></returns>
        public string Url {get; }

        /// <summary>
        /// Relative position among scripts.
        /// </summary>
        /// <returns></returns>
        public int Position {get;}

        /// <summary>
        /// Optional script section name.
        /// If not defined, the script will be included on all pages.
        /// </summary>
        /// <returns></returns>
        public string Section {get; }

        public Script(string url_, int position_, string section_ = null)
        {
            Url = url_;
            Position = position_;
            Section = section_;
        }
    }
}