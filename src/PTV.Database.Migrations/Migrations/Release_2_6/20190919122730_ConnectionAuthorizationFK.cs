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
using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;

namespace PTV.Database.Migrations.Migrations.Release_2_6
{
    public partial class ConnectionAuthorizationFK : Migration
    {
        private readonly MigrateHelper migrateHelper;

        public ConnectionAuthorizationFK()
        {
            migrateHelper = new MigrateHelper();            
        }
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var file = Path.Combine("SqlMigrations", "PTV_2_6", "1ConnectionAuthorizationFK.sql");
            migrateHelper.AddSqlScript(migrationBuilder, file);
            /*             
            delete from "ServiceServiceChannelDigitalAuthorization"
            where 
            ("ServiceServiceChannelDigitalAuthorization"."DigitalAuthorizationId",  
            "ServiceServiceChannelDigitalAuthorization"."ServiceChannelId",
            "ServiceServiceChannelDigitalAuthorization"."ServiceId")
            in (
            select 
                sd."DigitalAuthorizationId",
                sd."ServiceChannelId",
                sd."ServiceId"
            from "ServiceServiceChannelDigitalAuthorization" AS sd
            left join "ServiceServiceChannel" AS ss On ss."ServiceId" = sd."ServiceId" AND ss."ServiceChannelId" = sd."ServiceChannelId"
            where ss."ServiceId" is null)
            
            ALTER TABLE public."ServiceServiceChannelDigitalAuthorization"
            ADD CONSTRAINT "FK_SerSerChaDesDigAut_SerChaId" FOREIGN KEY ("ServiceChannelId", "ServiceId")
            REFERENCES public."ServiceServiceChannel" ("ServiceChannelId", "ServiceId") MATCH SIMPLE
            ON UPDATE NO ACTION
            ON DELETE CASCADE;
            */            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
