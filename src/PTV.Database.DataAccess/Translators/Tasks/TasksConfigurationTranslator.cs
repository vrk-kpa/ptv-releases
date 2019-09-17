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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Tasks
{
    [RegisterService(typeof(ITranslator<Model.Models.TasksConfiguration, VmJsonTasksConfiguration>), RegisterType.Scope)]
    internal class JsonTasksConfigurationTranslator : Translator<Model.Models.TasksConfiguration, VmJsonTasksConfiguration>
    {
        private readonly IPublishingStatusCache publishingStatusCache;
        public JsonTasksConfigurationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, IPublishingStatusCache publishingStatusCache) : base(resolveManager, translationPrimitives)
        {
            this.publishingStatusCache = publishingStatusCache;
        }

        public override VmJsonTasksConfiguration TranslateEntityToVm(TasksConfiguration entity)
        {
            throw new NotImplementedException();
        }

        public override TasksConfiguration TranslateVmToEntity(VmJsonTasksConfiguration vModel)
        {
            var publishingStatusId = publishingStatusCache.Get(vModel.PublishingStatus);
            var definition = CreateViewModelEntityDefinition<Model.Models.TasksConfiguration>(vModel)
                .UseDataContextUpdate(i => true, i => o => i.Code == o.Code, def => def.UseDataContextCreate(x => true, o => o.Id,
                    i => Guid.NewGuid()));
            return definition
                .AddSimple(input => publishingStatusId, output => output.PublishingStatusId)
                .AddNavigation(input => input.Interval, output => output.Interval)
                .AddNavigation(input => input.Code, output => output.Code)
                .AddNavigation(input => input.Entity, output => output.Entity)
                .GetFinal();
        }
    }
}
