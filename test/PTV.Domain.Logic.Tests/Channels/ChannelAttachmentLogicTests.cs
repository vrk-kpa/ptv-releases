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
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace PTV.Domain.Logic.Tests.Channels
{
    /// <summary>
    /// Tests PTV ChannelAttachmentLogic
    /// </summary>
    public class ChannelAttachmentLogicTests : IClassFixture<ChannelAttachmentLogicFixture>
    {
        public ChannelAttachmentLogicFixture TestFixture { get; private set; }

        public ChannelAttachmentLogicTests(ChannelAttachmentLogicFixture fixture)
        {
            TestFixture = fixture;
        }

        [Fact]
        public void IsFilledNullReference()
        {
            using (var client = new HttpClient())
            {
                var a = WebRequest.Create(
                    new Uri("https://llso-test.lingsoft.fi/fetchFile/v1/?work=d84cfc8bfad735e6763ffea0e29e5eaa&c=a5bc63d3ff3022e1bfd6b9aa75f25630b3b5111665d9c855486e60e2eab13708"));

                client.DefaultRequestHeaders.Add("User-Agent", "ssdsad");
                var dataaa = client.GetStringAsync(new Uri("https://llso-test.lingsoft.fi/fetchFile/v1/?work=d84cfc8bfad735e6763ffea0e29e5eaa&c=a5bc63d3ff3022e1bfd6b9aa75f25630b3b5111665d9c855486e60e2eab13708")).Result;
                using (WebResponse response = a.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string result = reader.ReadToEnd();
                        }
                    }
                }

            }
            // the implementation should handle null
            Action act = () => TestFixture.ChannelAttachmentLogic.IsFilled(null);
            act.ShouldNotThrow();
        }

        [Theory]
        [InlineData(null, null, "description")]
        [InlineData(null, "url", null)]
        [InlineData("name", null, null)]
        [InlineData("", "", "description")]
        [InlineData("", "url", "")]
        [InlineData("name", "", "")]
        [InlineData("", null, "description")]
        [InlineData(null, "url", "")]
        [InlineData("name", null, null)]
        [InlineData(" ", null, null)] // currently accepts also whitespaces, change if implementation is changed, move these to NotFilled test
        [InlineData(null, " ", null)]
        [InlineData(null, null, " ")]
        public void AttachmentIsFilled(string name, string url, string description)
        {
            var attachment = new Model.Models.VmChannelAttachment() { Name = name, UrlAddress = url, Description = description };

            TestFixture.ChannelAttachmentLogic.IsFilled(attachment).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData(null, "", null)]
        [InlineData(null, "", "")]
        [InlineData(null, null, "")]
        [InlineData("", null, "")]
        public void AttachmentIsNotFilled(string name, string url, string description)
        {
            var attachment = new Model.Models.VmChannelAttachment() { Name = name, UrlAddress = url, Description = description };

            TestFixture.ChannelAttachmentLogic.IsFilled(attachment).Should().BeFalse();
        }

        [Fact]
        public void PrefilterModelNullReference()
        {
            // should handle nullreference
            Action act = () => TestFixture.ChannelAttachmentLogic.PrefilterModel(null);
            act.ShouldNotThrow();
        }

        [Fact]
        public void PrefilterModelNullCollection()
        {
            // should return null enumerable
            TestFixture.ChannelAttachmentLogic.PrefilterModel(null).Should().BeNull();
        }

        [Fact]
        public void PrefilterModelEmptyCollection()
        {
            List<Model.Models.VmChannelAttachment> emptyList = new List<Model.Models.VmChannelAttachment>();
            TestFixture.ChannelAttachmentLogic.PrefilterModel(emptyList).Should().HaveCount(0);
        }

        [Fact]
        public void PrefilterModelCollection()
        {
            List<Model.Models.VmChannelAttachment> attachments = new List<Model.Models.VmChannelAttachment>();
            attachments.Add(new Model.Models.VmChannelAttachment() { Description = "Some description", Name = "Some name", UrlAddress = "" });
            attachments.Add(new Model.Models.VmChannelAttachment() { Description = null, Name = "Some name two", UrlAddress = "" });
            attachments.Add(new Model.Models.VmChannelAttachment() { Description = "Some description", Name = "", UrlAddress = "" });
            attachments.Add(new Model.Models.VmChannelAttachment() { Description = "Some description", Name = "Some name three", UrlAddress = "" });
            attachments.Add(new Model.Models.VmChannelAttachment() { Id = Guid.NewGuid() }); // this should be removed
            attachments.Add(new Model.Models.VmChannelAttachment() { Description = " ", Name = " ", UrlAddress = "" }); // this shouldn't currently be removed because of whitespaces

            // the original collection shouldn't change
            attachments.Should().HaveCount(6);

            // one item should be removed from the reurned enumerable
            TestFixture.ChannelAttachmentLogic.PrefilterModel(attachments).Should().HaveCount(5);
        }
    }

    public class ChannelAttachmentLogicFixture
    {
        public ChannelAttachmentLogic ChannelAttachmentLogic { get; private set; }

        public ChannelAttachmentLogicFixture()
        {
            ChannelAttachmentLogic = new ChannelAttachmentLogic();
        }
    }
}
