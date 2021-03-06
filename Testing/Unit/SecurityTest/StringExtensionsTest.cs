// Copyright © 2017 SOFTINUX. All rights reserved.
// Licensed under the MIT License, Version 2.0. See LICENSE file in the project root for license information.

using Security.Extensions;
using Xunit;

namespace SecurityTest
{
    public class StringExtensionsTest
    {
        [Theory]
        [InlineData("thisisatest")]
        [InlineData("ThisIsaTest")]
        [InlineData("Thisisatest")]
        [InlineData("THISISATEST")]
        public void Test(string value)
        {
            Assert.Equal("Thisisatest", value.UppercaseFirst());
        }
    }
}