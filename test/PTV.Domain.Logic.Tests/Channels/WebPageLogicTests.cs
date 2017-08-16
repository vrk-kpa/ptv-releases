/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using FluentAssertions;
using PTV.Domain.Logic.Channels;
using PTV.Domain.Model.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PTV.Domain.Logic.Tests.Channels
{
    /// <summary>
    /// Tests PTV WebPageLogic
    /// </summary>
    public class WebPageLogicTests : IClassFixture<WebPageLogicFixture>
    {
        public WebPageLogicFixture TestFixture { get; private set; }

        public WebPageLogicTests(WebPageLogicFixture fixture)
        {
            TestFixture = fixture;
        }

        [Fact]
        public void IsFilledNullReference()
        {
            // the implementation should handle null
            Action act = () => TestFixture.WebPageLogic.IsFilled(null);
            act.ShouldNotThrow();
        }

        [Theory]
        [InlineData(null, "url")]
        [InlineData("", "url")]
        [InlineData("name", null)]
        [InlineData("name", "")]
        [InlineData("name", "url")]
        public void WebPageIsFilled(string name, string url)
        {
            var wp = new VmWebPage() { Name = name, UrlAddress = url };

            TestFixture.WebPageLogic.IsFilled(wp).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, " ")]
        [InlineData("", " ")]
        [InlineData(" ", null)]
        [InlineData(" ", "")]
        [InlineData(" ", " ")]
        public void WebPageIsNotFilled(string name, string url)
        {
            var wp = new VmWebPage() { Name = name, UrlAddress = url };

            TestFixture.WebPageLogic.IsFilled(wp).Should().BeFalse();
        }

        [Fact]
        public void PreFilterNullReference()
        {
            // the implementation should handle null
            Action act = () => TestFixture.WebPageLogic.PrefilterModel(null);
            act.ShouldNotThrow();
        }

        [Fact]
        public void PreFilterNullCollection()
        {
            WebPagesContainer cnt = new WebPagesContainer();
            // explicitly set the webpages to null
            cnt.WebPages = null;

            TestFixture.WebPageLogic.PrefilterModel(cnt);
            // just assume no exception and the list property is still null
            cnt.WebPages.Should().BeNull();
        }

        [Fact]
        public void PreFilterEmptyCollection()
        {
            WebPagesContainer cnt = new WebPagesContainer();

            TestFixture.WebPageLogic.PrefilterModel(cnt);
            // just assume no exception and the list is still empty
            cnt.WebPages.Should().BeEmpty();
        }

        [Fact]
        public void PreFilterCollection()
        {
            WebPagesContainer cnt = new WebPagesContainer();
            cnt.WebPages.Add(new VmWebPage() { Name = "TestOne", UrlAddress = "http://www.someurl.com" });
            cnt.WebPages.Add(new VmWebPage() { Name = "TestTwo", UrlAddress = "http://www.someurl2.com" });
            cnt.WebPages.Add(new VmWebPage() { Name = null, UrlAddress = null });

            TestFixture.WebPageLogic.PrefilterModel(cnt);
            // the instance with null values should have been removed
            cnt.WebPages.Should().HaveCount(2);
        }
    }

    public class WebPageLogicFixture
    {
        public WebPageLogic WebPageLogic { get; private set; }

        public WebPageLogicFixture()
        {
            WebPageLogic = new WebPageLogic();
        }
    }

    public class WebPagesContainer : Model.Models.Interfaces.IWebPages
    {
        public WebPagesContainer()
        {
            WebPages = new List<VmWebPage>();
        }

        public List<VmWebPage> WebPages { get; set; }
    }
}
