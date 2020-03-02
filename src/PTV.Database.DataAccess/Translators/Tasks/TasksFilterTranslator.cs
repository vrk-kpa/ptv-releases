/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
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

using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Logic;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Tasks
{
    [RegisterService(typeof(ITranslator<Model.Models.TasksFilter, VmTaskEntity>), RegisterType.Scope)]
    internal class TasksFilterTranslator : Translator<Model.Models.TasksFilter, VmTaskEntity>
    {
        private readonly IPahaTokenAccessor pahaTokenAccessor;
        public TasksFilterTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, IPahaTokenAccessor pahaTokenAccessor) : base(resolveManager, translationPrimitives)
        {
            this.pahaTokenAccessor = pahaTokenAccessor;
        }

        public override VmTaskEntity TranslateEntityToVm(TasksFilter entity)
        {
            throw new System.NotImplementedException();
        }

        public override TasksFilter TranslateVmToEntity(VmTaskEntity vModel)
        {
            var userGuid = pahaTokenAccessor.UserName.GetGuid();
            return CreateViewModelEntityDefinition<Model.Models.TasksFilter>(vModel)
                    .UseDataContextUpdate(i => true, i => o => i.GroupId == o.TypeId && i.UnificRootId == o.EntityId && userGuid == o.UserId, def => def.UseDataContextCreate(x => true))
                .AddSimple(input => input.GroupId, output => output.TypeId)
                .AddSimple(input => input.UnificRootId, output => output.EntityId)
                .AddSimple(input => input.Modified, output => output.EntityModified)
                .AddSimple(input => userGuid, output => output.UserId)
                .GetFinal();
        }
    }
}
