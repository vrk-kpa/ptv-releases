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

using System;
using System.Diagnostics;
using Npgsql;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.DirectRaw;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services.V2
{
    [RegisterService(typeof(IGeoServerService), RegisterType.Transient)]
    internal class GeoServerService : ServiceBase, IGeoServerService
    {
        private readonly IDatabaseRawContext rawContext;

        public GeoServerService(
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IVersioningManager versioningManager,
            IDatabaseRawContext rawContext)
            : base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker, versioningManager)
        {
            this.rawContext = rawContext;
        }

        public string ExecuteCommand(string command)
        {
            var result = rawContext.ExecuteWriter(db =>
            {
                try
                {
                    var sw = new Stopwatch();
                    sw.Start();
                    db.Command(command, null);
                    db.Save();
                    sw.Stop();
                    return $"Command '{command}' successfully executed in {sw.ElapsedMilliseconds} ms";
                }
                catch (PostgresException ex)
                {
                    var hint = ex.Hint.IsNullOrEmpty()
                        ? string.Empty
                        : $"{Environment.NewLine}{ex.Hint}";
                    return $"Command '{command}' execution failed: '{ex.Message}{hint}'";
                }
            });

            return result;
        }
    }
}
