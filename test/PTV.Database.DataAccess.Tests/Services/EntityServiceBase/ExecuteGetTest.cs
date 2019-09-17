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

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.DataAccess.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.EntityServiceBase
{
    /// <summary>
    /// Contains PTV CacheManager implementation tests.
    /// </summary>
    /// <remarks>
    /// <para>IClassFixture is used to share the mock and created CacheManager between tests.</para>
    /// <para>Tests test that when we create CacheManager instance the properties are set in CacheManager and the instances are the same as passed in constructor.</para>
    /// </remarks>
    public class EntityServiceBaseTests : TestBase // : IClassFixture<EntityServiceBaseFixture>
    {
        private EntityServiceBaseFixture entityServiceBaseFixture;
        private Mock<IValidationManager> validationManagerMock;
        private ItemListModelGenerator listModelGenerator;
        private Mock<ICommonServiceInternal> commonServiceMock;
        
        private Mock<IServiceUtilities> utilitiesMock;

        public EntityServiceBaseTests()
        {
            listModelGenerator = new ItemListModelGenerator();

            var translationManagerToVm = new Mock<ITranslationEntity>();
            var translationManagerToEntity = new Mock<ITranslationViewModel>();
            var userOrganizationChecker = new Mock<IUserOrganizationChecker>();
            var versioningManager = new Mock<IVersioningManager>();

            commonServiceMock = new Mock<ICommonServiceInternal>(MockBehavior.Strict);
            validationManagerMock = new Mock<IValidationManager>(MockBehavior.Strict);

            utilitiesMock = new Mock<IServiceUtilities>(MockBehavior.Strict);
            
            entityServiceBaseFixture = new EntityServiceBaseFixture
            (
                translationManagerToVm.Object,
                translationManagerToEntity.Object,
                CacheManager.PublishingStatusCache,
                userOrganizationChecker.Object,
                contextManagerMock.Object,
                utilitiesMock.Object,
                commonServiceMock.Object,
                validationManagerMock.Object,
                versioningManager.Object
            );
        }

        private void SetupContextManager<TWriterOut>()
        {
            SetupContextManager<TWriterOut,VmEntityHeaderBase>();
        }
        private void VerifyContextManagerCalls<TWriterOut>(Times reader, Times writer)
        {
            VerifyContextManagerCalls<TWriterOut,VmEntityHeaderBase>(reader, writer);
        }

        private EntityServiceBaseFixture TestFixture { get => entityServiceBaseFixture; }

        [Theory]
        [InlineData(false, "", "")]
        [InlineData(false, "fi", "en")]
        [InlineData(false, "fi", "fi")]
        [InlineData(true, "fi", "")]
        [InlineData(true, "fi", "en")]
        [InlineData(true, "fi", "fi")]
        public void ExecuteGet(bool includeValidation, string languageErrors, string publishedScheduled)
        {
            var modelMock = new Mock<IVmEntityGet>();
            modelMock.Setup(x => x.IncludeValidation).Returns(includeValidation);
            modelMock.SetupGet(x => x.Id).Returns("checkId".GetGuid());
            validationManagerMock.Setup(x => x.CheckEntity<TestEntityVersioned>(It.IsAny<Guid>(),
                    It.IsAny<IUnitOfWork>(), It.IsAny<ILanguagesAvailabilities>()))
                .Returns((Guid id, IUnitOfWork unitOfWork, ILanguagesAvailabilities la) =>
                {
                    return listModelGenerator
                        .CreateList(languageErrors, l => new List<ValidationMessage> {new ValidationMessage {Key = l}})
                        .ToDictionary(x => x.First().Key.GetGuid());
                });

            VmEntityHeaderBase loadedResult = null;

            commonServiceMock
                .Setup(x => x.GetValidatedHeader(It.IsAny<VmEntityHeaderBase>(),
                    It.IsAny<Dictionary<Guid, List<ValidationMessage>>>()))
                .Returns((VmEntityHeaderBase model, Dictionary<Guid, List<ValidationMessage>> messages) => model);
            SetupContextManager<object>();
                
            var result = TestFixture.CallExecuteGet(modelMock.Object, (unitOfWork, m) =>
                loadedResult = new VmEntityHeaderBase
                {
                    Id = m.Id,
                    LanguagesAvailabilities = listModelGenerator
                        .CreateList(publishedScheduled, l => new VmLanguageAvailabilityInfo { ValidFrom = 4564, LanguageId = l.GetGuid()})
                        .ToList()
                });
        
            // verify
            result.Should().NotBeNull();
            result.Should().Be(loadedResult);
            validationManagerMock.Verify(
                x => x.CheckEntity<TestEntityVersioned>(It.IsAny<Guid>(), It.IsAny<IUnitOfWork>(), It.IsAny<ILanguagesAvailabilities>()),
                includeValidation || !string.IsNullOrEmpty(publishedScheduled) ? Times.Once() : Times.Never()
            );
            commonServiceMock.Verify(
                x => x.GetValidatedHeader(It.IsAny<VmEntityHeaderBase>(), It.IsAny<Dictionary<Guid, List<ValidationMessage>>>()),
                includeValidation ||
                      !string.IsNullOrEmpty(publishedScheduled) && publishedScheduled.Split(";").Intersect(languageErrors.Split(";")).Any() ?
                    Times.Once() :
                    Times.Never()
            );
            VerifyContextManagerCalls<object>(Times.Once(), Times.Never());
        }
        
        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void ExecuteGetModelCheck(bool isModelNull, bool idEmpty)
        {
            VmEntityBasic model = isModelNull ? null : new VmEntityBasic { Id = idEmpty ? Guid.Empty : (Guid?) null};

            Action call = () => TestFixture.CallExecuteGet<VmEntityHeaderBase>(model, null);
        
            // verify
            call.Should().Throw<EntityNotFoundException>();
        }

    
        [Theory]
        [InlineData(ActionTypeEnum.Save, "", "", null)]
        [InlineData(ActionTypeEnum.Save, "fi", "en", null)]
        [InlineData(ActionTypeEnum.Save, "fi", "fi", "dss")]
        [InlineData(ActionTypeEnum.SaveAndValidate, "fi", "", "dss")]
        [InlineData(ActionTypeEnum.SaveAndValidate, "fi", "en", "dss")]
        [InlineData(ActionTypeEnum.SaveAndValidate, "fi", "fi", null)]
        [InlineData(ActionTypeEnum.SaveAndPublish, "fi", "", "dss")]
        [InlineData(ActionTypeEnum.SaveAndPublish, "fi", "en", "sda")]
        [InlineData(ActionTypeEnum.SaveAndPublish, "fi", "fi", null)]
        public void ExecuteSave(ActionTypeEnum action, string languageErrors, string publishedScheduled, string inputId)
        {
            var inputModel = new VmEntityHeaderBase
            {
                Id = string.IsNullOrEmpty(inputId) ? (Guid?) null : inputId.GetGuid(),
                Action = action
            };
            validationManagerMock.Setup(x => x.CheckEntity<TestEntityVersioned>(It.IsAny<Guid>(),
                    It.IsAny<IUnitOfWork>(), It.IsAny<ILanguagesAvailabilities>()))
                .Returns((Guid id, IUnitOfWork unitOfWork, ILanguagesAvailabilities la) =>
                {
                    return listModelGenerator
                        .CreateList(languageErrors, l => new List<ValidationMessage> {new ValidationMessage {Key = l}})
                        .ToDictionary(x => x.First().Key.GetGuid());
                });

            VmEntityHeaderBase loadedResult = null;

            commonServiceMock
                .Setup(x => x.GetValidatedHeader(It.IsAny<VmEntityHeaderBase>(),
                    It.IsAny<Dictionary<Guid, List<ValidationMessage>>>()))
                .Returns((VmEntityHeaderBase model, Dictionary<Guid, List<ValidationMessage>> messages) => model);

            utilitiesMock.Setup(x => x.LockEntityVersioned<TestEntityVersioned, TestEntityRoot>(It.IsAny<Guid>(), It.IsAny<bool>()))
                .Returns((Guid id, bool isLockDisAllowedForArchived) => new Mock<IVmEntityBase>().Object);
            
            SetupContextManager<IEntityIdentifier>();
                
            // call
            var result = TestFixture.CallExecuteSave(
                inputModel,
                (unitOfWorkWritable) => new TestEntityVersioned { Id = "newId".GetGuid() }, 
                (unitOfWork, m) =>
                    loadedResult = new VmEntityHeaderBase
                    {
                        Id = m.Id,
                        LanguagesAvailabilities = listModelGenerator
                            .CreateList(publishedScheduled, l => new VmLanguageAvailabilityInfo { ValidFrom = 4564, LanguageId = l.GetGuid()})
                            .ToList()
                    }
            );

            var includeValidation = action != ActionTypeEnum.Save;
            // verify
            result.Should().NotBeNull();
            result.Should().Be(loadedResult);
            result.Action.Should().Be(action);
            validationManagerMock.Verify(
                x => x.CheckEntity<TestEntityVersioned>(It.IsAny<Guid>(), It.IsAny<IUnitOfWork>(), It.IsAny<ILanguagesAvailabilities>()),
                includeValidation || !string.IsNullOrEmpty(publishedScheduled) ? Times.Once() : Times.Never()
            );
            commonServiceMock.Verify(
                x => x.GetValidatedHeader(It.IsAny<VmEntityHeaderBase>(), It.IsAny<Dictionary<Guid, List<ValidationMessage>>>()),
                includeValidation ||
                      !string.IsNullOrEmpty(publishedScheduled) && publishedScheduled.Split(";").Intersect(languageErrors.Split(";")).Any() ?
                    Times.Once() :
                    Times.Never()
            );
            
            utilitiesMock.Verify(
                x => x.LockEntityVersioned<TestEntityVersioned, TestEntityRoot>(It.IsAny<Guid>(), It.IsAny<bool>()),
                () => !string.IsNullOrEmpty(inputId) && action == ActionTypeEnum.SaveAndPublish ? Times.Once() : Times.Never()
            );
            
            VerifyContextManagerCalls<IEntityIdentifier>(Times.Once(), Times.Once());
        }
        
        [Theory]
        [InlineData(PublishActionTypeEnum.SchedulePublish, "", "")]
        [InlineData(PublishActionTypeEnum.SchedulePublish, "fi", "en")]
        [InlineData(PublishActionTypeEnum.SchedulePublish, "fi", "fi")]
        [InlineData(PublishActionTypeEnum.ScheduleArchive, "fi", "")]
        [InlineData(PublishActionTypeEnum.ScheduleArchive, "fi", "en")]
        [InlineData(PublishActionTypeEnum.ScheduleArchive, "fi", "fi")]
        public void ExecuteScheduleEntity(PublishActionTypeEnum action, string languageErrors, string publishedScheduled)
        {
            var inputModelMock = new Mock<IVmLocalizedEntityModel>();
            inputModelMock.SetupGet(x => x.PublishAction).Returns(action);
            inputModelMock.SetupGet(x => x.Id).Returns("inputId".GetGuid());
            
            validationManagerMock.Setup(x => x.CheckEntity<TestEntityVersioned>(It.IsAny<Guid>(),
                    It.IsAny<IUnitOfWork>(), It.IsAny<ILanguagesAvailabilities>()))
                .Returns((Guid id, IUnitOfWork unitOfWork, ILanguagesAvailabilities la) =>
                {
                    return listModelGenerator
                        .CreateList(languageErrors, l => new List<ValidationMessage> {new ValidationMessage {Key = l}})
                        .ToDictionary(x => x.First().Key.GetGuid());
                });

            VmEntityHeaderBase loadedResult = null;

            commonServiceMock
                .Setup(x => x.SchedulePublishArchiveEntity<TestEntityVersioned,TestLanguageAvail>(It.IsAny<IUnitOfWorkWritable>(), inputModelMock.Object, It.IsAny<bool>()))
                .Returns((IUnitOfWorkWritable unitOfWorkMock, IVmLocalizedEntityModel model, bool updateHistory) => new PublishingResult
                {
                    Id = model.Id
                });
            commonServiceMock
                .Setup(x => x.GetValidatedHeader(It.IsAny<VmEntityHeaderBase>(),
                    It.IsAny<Dictionary<Guid, List<ValidationMessage>>>()))
                .Returns((VmEntityHeaderBase model, Dictionary<Guid, List<ValidationMessage>> messages) => model);
            
            SetupContextManager<PublishingResult>();
            
            // Call
            if (action == PublishActionTypeEnum.SchedulePublish && !string.IsNullOrEmpty(languageErrors))
            {
                Action call = () => 
                    TestFixture.CallExecuteScheduleEntity<VmEntityHeaderBase>(inputModelMock.Object, (iu, model) => null);
                call.Should().Throw<SchedulePublishException>();
                validationManagerMock.Verify(
                    x => x.CheckEntity<TestEntityVersioned>(It.IsAny<Guid>(), It.IsAny<IUnitOfWork>(), It.IsAny<ILanguagesAvailabilities>()),
                    Times.Once()
                );
                return;
            }
            VmEntityHeaderBase result = TestFixture.CallExecuteScheduleEntity(
                inputModelMock.Object, 
                (unitOfWork, m) =>
                    loadedResult = new VmEntityHeaderBase
                    {
                        Id = m.Id,
                        LanguagesAvailabilities = listModelGenerator
                            .CreateList(publishedScheduled, l => new VmLanguageAvailabilityInfo { ValidFrom = 4564, LanguageId = l.GetGuid()})
                            .ToList()
                    }
            );

            // verify
            result.Should().NotBeNull();
            result.Should().Be(loadedResult);
            validationManagerMock.Verify(
                x => x.CheckEntity<TestEntityVersioned>(It.IsAny<Guid>(), It.IsAny<IUnitOfWork>(), It.IsAny<ILanguagesAvailabilities>()),
                action == PublishActionTypeEnum.SchedulePublish || !string.IsNullOrEmpty(publishedScheduled) ? Times.Once() : Times.Never()
            );
            commonServiceMock.Verify(
                x => x.GetValidatedHeader(It.IsAny<VmEntityHeaderBase>(), It.IsAny<Dictionary<Guid, List<ValidationMessage>>>()),
                !string.IsNullOrEmpty(publishedScheduled) && publishedScheduled.Split(";").Intersect(languageErrors.Split(";")).Any() ?
                    Times.Once() :
                    Times.Never()
            );
            
            VerifyContextManagerCalls<PublishingResult>(Times.Once(), Times.Once());
        }
        
        [Fact]
        public void ExecuteScheduleEntityNullId()
        {
            var inputModelMock = new Mock<IVmLocalizedEntityModel>();
            inputModelMock.SetupGet(x => x.Id).Returns(Guid.Empty);
            
            // Call
            VmEntityHeaderBase result = TestFixture.CallExecuteScheduleEntity<VmEntityHeaderBase>(inputModelMock.Object, null);

            // verify
            result.Should().BeNull();
        }
        
        [Theory]
        [InlineData("", "", false)]
        [InlineData("fi", "en", true)]
        [InlineData("fi", "fi", true)]
        public void ExecuteValidate(string languageErrors, string publishedScheduled, bool lockActionDefined)
        {
            var modelMock = new Mock<IVmEntityGet>();
            modelMock.SetupGet(x => x.Id).Returns("checkId".GetGuid());
            validationManagerMock.Setup(x => x.CheckEntity<TestEntityVersioned>(It.IsAny<Guid>(),
                    It.IsAny<IUnitOfWork>(), It.IsAny<ILanguagesAvailabilities>()))
                .Returns((Guid id, IUnitOfWork unitOfWork, ILanguagesAvailabilities la) =>
                {
                    return listModelGenerator
                        .CreateList(languageErrors, l => new List<ValidationMessage> {new ValidationMessage {Key = l}})
                        .ToDictionary(x => x.First().Key.GetGuid());
                });

            VmEntityHeaderBase loadedResult = null;

            commonServiceMock
                .Setup(x => x.GetValidatedHeader(It.IsAny<VmEntityHeaderBase>(),
                    It.IsAny<Dictionary<Guid, List<ValidationMessage>>>()))
                .Returns((VmEntityHeaderBase model, Dictionary<Guid, List<ValidationMessage>> messages) => model);
            SetupContextManager<object>();

            bool lockActionCalled = false;
            Action lockAction = lockActionDefined ? () => lockActionCalled = true : (Action)null;  
                
            var result = TestFixture.CallExecuteValidate(lockAction, (unitOfWork) =>
                loadedResult = new VmEntityHeaderBase
                {
                    Id = Guid.NewGuid(),
                    LanguagesAvailabilities = listModelGenerator
                        .CreateList(publishedScheduled, l => new VmLanguageAvailabilityInfo { ValidFrom = 4564, LanguageId = l.GetGuid()})
                        .ToList()
                });
        
            // verify
            result.Should().NotBeNull();
            result.Should().Be(loadedResult);
            lockActionCalled.Should().Be(lockActionDefined);
            validationManagerMock.Verify(
                x => x.CheckEntity<TestEntityVersioned>(It.IsAny<Guid>(), It.IsAny<IUnitOfWork>(), It.IsAny<ILanguagesAvailabilities>()),
                Times.Once()
            );
            commonServiceMock.Verify(
                x => x.GetValidatedHeader(It.IsAny<VmEntityHeaderBase>(), It.IsAny<Dictionary<Guid, List<ValidationMessage>>>()),
                Times.Once()
            );
            VerifyContextManagerCalls<object>(Times.Once(), Times.Never());
        }


    }
}
