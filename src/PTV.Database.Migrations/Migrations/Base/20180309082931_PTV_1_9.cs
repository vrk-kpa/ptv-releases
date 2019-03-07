﻿/**
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

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using PTV.Database.Migrations.Migrations.Base;
using PTV.Database.Migrations.Migrations.GeoServer;
using PTV.Database.Migrations.Migrations.Release_1_9;

namespace PTV.Database.Migrations.Migrations.Base
{
    public partial class PTV_1_9 : Migration
    {
        readonly List<IPartialMigration> migrations = new List<IPartialMigration>
        {
            new GeoServerViewsClean(),

            new TasksFilterTable(),
            new Add_Tasks_Configurtion_Table(),
            new NewColumnForTasksFilter(),
            new ChangeForeignKeyForTasksFilterTable(),
            new AddedGeneralDescriptionType(),
            new ServiceChannelAddress(),
            new serviceCollectionService(),
            new OrgRestrictionFilter(),
            new RestrictionFilterTypeChange(),
            new OpeningHoursDateFix(),
            new RestrictionFilterExtended(),
            
            new GeoServerViewsInit()
        };

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrations.ForEach(m =>
            {
                m.Up(migrationBuilder);
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrations.ForEach(m =>
            {
                m.Down(migrationBuilder);
            });
        }
    }
}
