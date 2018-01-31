// Copyright © 2017 SOFTINUX. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for license information.

namespace Infrastructure
{
    /// <summary>
    /// Definition of a CSS stylesheet that should be included on pages (views.)
    /// </summary>
    public class StyleSheet
    {
        /// <summary>
        /// Absolute path relative to application root.
        /// The slashes excepted the first one have to be replaced by a dot because stylesheets
        /// are served as embedded resources.
        /// </summary>
        public string Url {get; }

        /// <summary>
        /// Relative position among stylesheets.
        /// </summary>
        /// <returns></returns>
        public int Position {get;}

        /// <summary>
        /// Optional stylesheet section name.
        /// If not defined, the stylesheet will be included on all pages.
        /// </summary>
        /// <returns></returns>
        public string Section {get; }

        public StyleSheet(string url_, int position_, string section_ = null)
        {
            Url = url_;
            Position = position_;
            Section = section_;
        }
    }
}