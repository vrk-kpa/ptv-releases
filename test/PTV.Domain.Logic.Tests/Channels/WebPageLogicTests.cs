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
