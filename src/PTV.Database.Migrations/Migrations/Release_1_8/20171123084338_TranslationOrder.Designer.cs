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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PTV.Database.DataAccess.ApplicationDbContext;

namespace PTV.Database.Migrations.Migrations.Release_1_8
{
    [DbContext(typeof(PtvDbContext))]
    [Migration("20171123084338_TranslationOrder")]
    partial class TranslationOrder
    {
        public void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("Npgsql:PostgresExtension:uuid-ossp", "'uuid-ossp', '', ''")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("PTV.Database.Model.Models.AccessRightName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_AccRigNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_AccRigNam_TypeId");

                    b.ToTable("AccessRightName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AccessRightType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_AccRigTyp_Id");

                    b.ToTable("AccessRightType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Address", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("CountryId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("CountryId")
                        .HasAnnotation("Npgsql:Name", "IX_Add_CountryId");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_Add_Id");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_Add_TypeId");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AddressAdditionalInformation", b =>
                {
                    b.Property<Guid>("AddressId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Text")
                        .HasMaxLength(150);

                    b.HasKey("AddressId", "LocalizationId");

                    b.HasIndex("AddressId")
                        .HasAnnotation("Npgsql:Name", "IX_AddAddInf_AddressId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_AddAddInf_LocalizationId");

                    b.ToTable("AddressAdditionalInformation");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AddressForeign", b =>
                {
                    b.Property<Guid>("AddressId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("AddressId");

                    b.HasIndex("AddressId")
                        .HasAnnotation("Npgsql:Name", "IX_AddFor_AddressId");

                    b.ToTable("AddressForeign");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AddressForeignTextName", b =>
                {
                    b.Property<Guid>("AddressForeignId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("AddressForeignId", "LocalizationId");

                    b.HasIndex("AddressForeignId")
                        .HasAnnotation("Npgsql:Name", "IX_AddForTexNam_AddressForeignId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_AddForTexNam_LocalizationId");

                    b.ToTable("AddressForeignTextName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AddressCharacter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_AddCha_Id");

                    b.ToTable("AddressCharacter");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AddressCharacterName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_AddChaNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_AddChaNam_TypeId");

                    b.ToTable("AddressCharacterName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AddressPostOfficeBox", b =>
                {
                    b.Property<Guid>("AddressId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid?>("MunicipalityId");

                    b.Property<Guid?>("PostalCodeId");

                    b.HasKey("AddressId");

                    b.HasIndex("AddressId")
                        .HasAnnotation("Npgsql:Name", "IX_AddPosOffBox_AddressId");

                    b.HasIndex("MunicipalityId")
                        .HasAnnotation("Npgsql:Name", "IX_AddPosOffBox_MunicipalityId");

                    b.HasIndex("PostalCodeId")
                        .HasAnnotation("Npgsql:Name", "IX_AddPosOffBox_PostalCodeId");

                    b.ToTable("AddressPostOfficeBox");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AddressStreet", b =>
                {
                    b.Property<Guid>("AddressId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid?>("MunicipalityId");

                    b.Property<Guid?>("PostalCodeId");

                    b.Property<string>("StreetNumber")
                        .HasMaxLength(30);

                    b.HasKey("AddressId");

                    b.HasIndex("AddressId")
                        .HasAnnotation("Npgsql:Name", "IX_AddStr_AddressId");

                    b.HasIndex("MunicipalityId")
                        .HasAnnotation("Npgsql:Name", "IX_AddStr_MunicipalityId");

                    b.HasIndex("PostalCodeId")
                        .HasAnnotation("Npgsql:Name", "IX_AddStr_PostalCodeId");

                    b.ToTable("AddressStreet");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AddressType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_AddTyp_Id");

                    b.ToTable("AddressType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AddressTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_AddTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_AddTypNam_TypeId");

                    b.ToTable("AddressTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AppEnvironmentData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("FreeText");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("TypeId");

                    b.Property<int>("Version");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_AppEnvDat_Id");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_AppEnvDat_TypeId");

                    b.ToTable("AppEnvironmentData");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AppEnvironmentDataType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_AppEnvDatTyp_Id");

                    b.ToTable("AppEnvironmentDataType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AppEnvironmentDataTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_AppEnvDatTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_AppEnvDatTypNam_TypeId");

                    b.ToTable("AppEnvironmentDataTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Area", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AreaTypeId");

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("IsValid");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("Id");

                    b.HasIndex("AreaTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_Are_AreaTypeId");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_Are_Id");

                    b.ToTable("Area");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AreaInformationType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_AreInfTyp_Id");

                    b.ToTable("AreaInformationType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AreaInformationTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_AreInfTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_AreInfTypNam_TypeId");

                    b.ToTable("AreaInformationTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AreaMunicipality", b =>
                {
                    b.Property<Guid>("AreaId");

                    b.Property<Guid>("MunicipalityId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("AreaId", "MunicipalityId");

                    b.HasIndex("AreaId")
                        .HasAnnotation("Npgsql:Name", "IX_AreMun_AreaId");

                    b.HasIndex("MunicipalityId")
                        .HasAnnotation("Npgsql:Name", "IX_AreMun_MunicipalityId");

                    b.ToTable("AreaMunicipality");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AreaName", b =>
                {
                    b.Property<Guid>("AreaId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("AreaId", "LocalizationId");

                    b.HasIndex("AreaId")
                        .HasAnnotation("Npgsql:Name", "IX_AreNam_AreaId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_AreNam_LocalizationId");

                    b.ToTable("AreaName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AreaType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_AreTyp_Id");

                    b.ToTable("AreaType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AreaTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_AreTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_AreTypNam_TypeId");

                    b.ToTable("AreaTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Attachment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description")
                        .HasMaxLength(150);

                    b.Property<string>("LastOperationId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<int?>("OrderNumber");

                    b.Property<Guid?>("TypeId");

                    b.Property<string>("Url")
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_Att_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_Att_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_Att_TypeId");

                    b.ToTable("Attachment");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AttachmentType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_AttTyp_Id");

                    b.ToTable("AttachmentType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AttachmentTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_AttTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_AttTypNam_TypeId");

                    b.ToTable("AttachmentTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.BugReport", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Actions");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("FinalState");

                    b.Property<string>("InitialState");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<string>("Version");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_BugRep_Id");

                    b.ToTable("BugReport");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Business", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .HasMaxLength(9);

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_Bus_Id");

                    b.ToTable("Business");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.CFGRequestFilter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ConcurrentRequests");

                    b.Property<string>("IPAddress");

                    b.Property<string>("Interface");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_CFGReqFil_Id");

                    b.ToTable("CFGRequestFilter");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Coordinate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AddressId");

                    b.Property<string>("CoordinateState");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("AddressId")
                        .HasAnnotation("Npgsql:Name", "IX_Coo_AddressId");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_Coo_Id");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_Coo_TypeId");

                    b.ToTable("Coordinate");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.CoordinateType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_CooTyp_Id");

                    b.ToTable("CoordinateType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.CoordinateTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_CooTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_CooTypNam_TypeId");

                    b.ToTable("CoordinateTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Country", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_Cou_Id");

                    b.ToTable("Country");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.CountryName", b =>
                {
                    b.Property<Guid>("CountryId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("CountryId", "LocalizationId");

                    b.HasIndex("CountryId")
                        .HasAnnotation("Npgsql:Name", "IX_CouNam_CountryId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_CouNam_LocalizationId");

                    b.ToTable("CountryName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.DailyOpeningTime", b =>
                {
                    b.Property<Guid>("OpeningHourId");

                    b.Property<int>("DayFrom");

                    b.Property<int>("Order");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<int?>("DayTo");

                    b.Property<TimeSpan>("From");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<TimeSpan>("To");

                    b.HasKey("OpeningHourId", "DayFrom", "Order");

                    b.HasIndex("OpeningHourId", "DayFrom", "Order")
                        .HasAnnotation("Npgsql:Name", "IX_DaiOpeTim_OpeningHourId_DayFrom_Order");

                    b.ToTable("DailyOpeningTime");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.DescriptionType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_DesTyp_Id");

                    b.ToTable("DescriptionType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.DescriptionTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_DesTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_DesTypNam_TypeId");

                    b.ToTable("DescriptionTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.DialCode", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<Guid>("CountryId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("Id");

                    b.HasIndex("CountryId")
                        .HasAnnotation("Npgsql:Name", "IX_DiaCod_CountryId");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_DiaCod_Id");

                    b.ToTable("DialCode");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.DigitalAuthorization", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Label");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("OntologyType");

                    b.Property<int?>("OrderNumber");

                    b.Property<Guid?>("ParentId");

                    b.Property<string>("ParentUri");

                    b.Property<string>("Uri");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_DigAut_Id");

                    b.HasIndex("ParentId")
                        .HasAnnotation("Npgsql:Name", "IX_DigAut_ParentId");

                    b.ToTable("DigitalAuthorization");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.DigitalAuthorizationName", b =>
                {
                    b.Property<Guid>("DigitalAuthorizationId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("DigitalAuthorizationId", "LocalizationId");

                    b.HasIndex("DigitalAuthorizationId")
                        .HasAnnotation("Npgsql:Name", "IX_DigAutNam_DigitalAuthorizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_DigAutNam_LocalizationId");

                    b.ToTable("DigitalAuthorizationName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ElectronicChannel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<bool>("RequiresAuthentication");

                    b.Property<bool>("RequiresSignature");

                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<int?>("SignatureQuantity");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_EleCha_Id");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_EleCha_ServiceChannelVersionedId");

                    b.ToTable("ElectronicChannel");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ElectronicChannelUrl", b =>
                {
                    b.Property<Guid>("ElectronicChannelId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Url");

                    b.HasKey("ElectronicChannelId", "LocalizationId");

                    b.HasIndex("ElectronicChannelId")
                        .HasAnnotation("Npgsql:Name", "IX_EleChaUrl_ElectronicChannelId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_EleChaUrl_LocalizationId");

                    b.ToTable("ElectronicChannelUrl");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Email", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description")
                        .HasMaxLength(100);

                    b.Property<string>("LastOperationId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.Property<string>("Value")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_Ema_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_Ema_LocalizationId");

                    b.ToTable("Email");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ExactMatch", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Uri");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_ExaMat_Id");

                    b.ToTable("ExactMatch");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ExceptionHoursStatusType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_ExcHouStaTyp_Id");

                    b.ToTable("ExceptionHoursStatusType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ExceptionHoursStatusTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_ExcHouStaTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_ExcHouStaTypNam_TypeId");

                    b.ToTable("ExceptionHoursStatusTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ExternalSource", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("ObjectType");

                    b.Property<Guid>("PTVId");

                    b.Property<string>("RelationId");

                    b.Property<string>("SourceId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_ExtSou_Id");

                    b.ToTable("ExternalSource");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ExtraSubType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("ExtraTypeId");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("ExtraTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_ExtSubTyp_ExtraTypeId");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_ExtSubTyp_Id");

                    b.ToTable("ExtraSubType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ExtraSubTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_ExtSubTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_ExtSubTypNam_TypeId");

                    b.ToTable("ExtraSubTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ExtraType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_ExtTyp_Id");

                    b.ToTable("ExtraType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ExtraTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_ExtTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_ExtTypNam_TypeId");

                    b.ToTable("ExtraTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Form", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("LastOperationId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_For_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_For_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_For_TypeId");

                    b.ToTable("Form");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.FormState", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("EntityId");

                    b.Property<string>("EntityType");

                    b.Property<string>("FormName");

                    b.Property<Guid>("LanguageId");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("State");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_ForSta_Id");

                    b.ToTable("FormState");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.FormType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_ForTyp_Id");

                    b.ToTable("FormType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.FormTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_ForTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_ForTypNam_TypeId");

                    b.ToTable("FormTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionBlockedAccessRight", b =>
                {
                    b.Property<Guid>("AccessBlockedId");

                    b.Property<Guid>("EntityId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("AccessBlockedId", "EntityId");

                    b.HasIndex("AccessBlockedId")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesBloAccRig_AccessBlockedId");

                    b.HasIndex("EntityId")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesBloAccRig_EntityId");

                    b.ToTable("GeneralDescriptionBlockedAccessRight");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionLanguageAvailability", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionVersionedId");

                    b.Property<Guid>("LanguageId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("StatusId");

                    b.HasKey("StatutoryServiceGeneralDescriptionVersionedId", "LanguageId");

                    b.HasIndex("LanguageId")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesLanAva_LanguageId");

                    b.HasIndex("StatusId")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesLanAva_StatusId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesLanAva_StatutoryServiceGeneralDescriptionVersionedId");

                    b.ToTable("GeneralDescriptionLanguageAvailability");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionServiceChannel", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionId");

                    b.Property<Guid>("ServiceChannelId");

                    b.Property<Guid?>("ChargeTypeId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("StatutoryServiceGeneralDescriptionId", "ServiceChannelId")
                        .HasAnnotation("Npgsql:Name", "PK_GenDesSerChan");

                    b.HasIndex("ChargeTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesSerCha_ChargeTypeId");

                    b.HasIndex("ServiceChannelId")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesSerCha_ServiceChannelId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesSerCha_StatutoryServiceGeneralDescriptionId");

                    b.ToTable("GeneralDescriptionServiceChannel");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionServiceChannelDescription", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<Guid>("ServiceChannelId");

                    b.Property<Guid>("StatutoryServiceGeneralDescriptionId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description")
                        .HasMaxLength(500);

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("TypeId", "LocalizationId", "ServiceChannelId", "StatutoryServiceGeneralDescriptionId")
                        .HasAnnotation("Npgsql:Name", "PK_GenDesSerChaDes");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesSerChaDes_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesSerChaDes_TypeId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId", "ServiceChannelId")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesSerChaDes_StatutoryServiceGeneralDescriptionId_ServiceChannelId");

                    b.ToTable("GeneralDescriptionServiceChannelDescription");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionServiceChannelDigitalAuthorization", b =>
                {
                    b.Property<Guid>("DigitalAuthorizationId");

                    b.Property<Guid>("StatutoryServiceGeneralDescriptionId");

                    b.Property<Guid>("ServiceChannelId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("DigitalAuthorizationId", "StatutoryServiceGeneralDescriptionId", "ServiceChannelId")
                        .HasAnnotation("Npgsql:Name", "PK_GenDesSerChaDesDigAut");

                    b.HasIndex("DigitalAuthorizationId")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesSerChaDigAut_DigitalAuthorizationId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId", "ServiceChannelId")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesSerChaDigAut_StatutoryServiceGeneralDescriptionId_ServiceChannelId");

                    b.ToTable("GeneralDescriptionServiceChannelDigitalAuthorization");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionServiceChannelExtraType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("ExtraSubTypeId");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("ServiceChannelId");

                    b.Property<Guid>("StatutoryServiceGeneralDescriptionId");

                    b.HasKey("Id")
                        .HasAnnotation("Npgsql:Name", "PK_GenDesSerChaExtTyp");

                    b.HasIndex("ExtraSubTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesSerChaExtTyp_ExtraSubTypeId");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesSerChaExtTyp_Id");

                    b.HasIndex("ServiceChannelId")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesSerChaExtTyp_ServiceChannelId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesSerChaExtTyp_StatutoryServiceGeneralDescriptionId");

                    b.ToTable("GeneralDescriptionServiceChannelExtraType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionServiceChannelExtraTypeDescription", b =>
                {
                    b.Property<Guid>("LocalizationId");

                    b.Property<Guid>("GeneralDescriptionServiceChannelExtraTypeId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description")
                        .HasMaxLength(500);

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("LocalizationId", "GeneralDescriptionServiceChannelExtraTypeId")
                        .HasAnnotation("Npgsql:Name", "PK_GenDesSerChaExtTypDes");

                    b.HasIndex("GeneralDescriptionServiceChannelExtraTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesSerChaExtTypDes_GeneralDescriptionServiceChannelExtraTypeId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesSerChaExtTypDes_LocalizationId");

                    b.ToTable("GeneralDescriptionServiceChannelExtraTypeDescription");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionTranslationOrder", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionVersionedId");

                    b.Property<Guid>("TranslationOrderId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("StatutoryServiceGeneralDescriptionVersionedId", "TranslationOrderId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesTraOrd_StatutoryServiceGeneralDescriptionVersionedId");

                    b.HasIndex("TranslationOrderId")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesTraOrd_TranslationOrderId");

                    b.ToTable("GeneralDescriptionTranslationOrder");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ChannelBlockedAccessRight", b =>
                {
                    b.Property<Guid>("AccessBlockedId");

                    b.Property<Guid>("EntityId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid?>("StatutoryServiceGeneralDescriptionId");

                    b.HasKey("AccessBlockedId", "EntityId");

                    b.HasIndex("AccessBlockedId")
                        .HasAnnotation("Npgsql:Name", "IX_ChaBloAccRig_AccessBlockedId");

                    b.HasIndex("EntityId")
                        .HasAnnotation("Npgsql:Name", "IX_ChaBloAccRig_EntityId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId")
                        .HasAnnotation("Npgsql:Name", "IX_ChaBloAccRig_StatutoryServiceGeneralDescriptionId");

                    b.ToTable("ChannelBlockedAccessRight");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.IndustrialClass", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Label");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("OntologyType");

                    b.Property<int?>("OrderNumber");

                    b.Property<Guid?>("ParentId");

                    b.Property<string>("ParentUri");

                    b.Property<string>("Uri");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_IndCla_Id");

                    b.HasIndex("ParentId")
                        .HasAnnotation("Npgsql:Name", "IX_IndCla_ParentId");

                    b.ToTable("IndustrialClass");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.IndustrialClassName", b =>
                {
                    b.Property<Guid>("IndustrialClassId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("IndustrialClassId", "LocalizationId");

                    b.HasIndex("IndustrialClassId")
                        .HasAnnotation("Npgsql:Name", "IX_IndClaNam_IndustrialClassId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_IndClaNam_LocalizationId");

                    b.ToTable("IndustrialClassName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Keyword", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("LastOperationId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_Key_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_Key_LocalizationId");

                    b.ToTable("Keyword");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Language", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("IsDefault");

                    b.Property<bool>("IsForData");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_Lan_Id");

                    b.ToTable("Language");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.LanguageName", b =>
                {
                    b.Property<Guid>("LanguageId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("LanguageId", "LocalizationId");

                    b.HasIndex("LanguageId")
                        .HasAnnotation("Npgsql:Name", "IX_LanNam_LanguageId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_LanNam_LocalizationId");

                    b.ToTable("LanguageName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Law", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_Law_Id");

                    b.ToTable("Law");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.LawName", b =>
                {
                    b.Property<Guid>("LawId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .HasMaxLength(500);

                    b.HasKey("LawId", "LocalizationId");

                    b.HasIndex("LawId")
                        .HasAnnotation("Npgsql:Name", "IX_LawNam_LawId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_LawNam_LocalizationId");

                    b.ToTable("LawName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.LawWebPage", b =>
                {
                    b.Property<Guid>("LawId");

                    b.Property<Guid>("WebPageId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("LawId", "WebPageId");

                    b.HasIndex("LawId")
                        .HasAnnotation("Npgsql:Name", "IX_LawWebPag_LawId");

                    b.HasIndex("WebPageId")
                        .HasAnnotation("Npgsql:Name", "IX_LawWebPag_WebPageId");

                    b.ToTable("LawWebPage");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.LifeEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Label");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("OntologyType");

                    b.Property<int?>("OrderNumber");

                    b.Property<Guid?>("ParentId");

                    b.Property<string>("ParentUri");

                    b.Property<string>("Uri");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_LifEve_Id");

                    b.HasIndex("ParentId")
                        .HasAnnotation("Npgsql:Name", "IX_LifEve_ParentId");

                    b.ToTable("LifeEvent");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.LifeEventName", b =>
                {
                    b.Property<Guid>("LifeEventId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("LifeEventId", "LocalizationId");

                    b.HasIndex("LifeEventId")
                        .HasAnnotation("Npgsql:Name", "IX_LifEveNam_LifeEventId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_LifEveNam_LocalizationId");

                    b.ToTable("LifeEventName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Locking", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("LockedAt");

                    b.Property<string>("LockedBy");

                    b.Property<Guid?>("LockedEntityId");

                    b.Property<string>("TableName");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_Loc_Id");

                    b.ToTable("Locking");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Municipality", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<bool>("IsValid");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_Mun_Id");

                    b.ToTable("Municipality");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.MunicipalityName", b =>
                {
                    b.Property<Guid>("MunicipalityId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("MunicipalityId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_MunNam_LocalizationId");

                    b.HasIndex("MunicipalityId")
                        .HasAnnotation("Npgsql:Name", "IX_MunNam_MunicipalityId");

                    b.ToTable("MunicipalityName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.NameType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_NamTyp_Id");

                    b.ToTable("NameType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.NameTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_NamTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_NamTypNam_TypeId");

                    b.ToTable("NameTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OntologyTerm", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Label");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("OntologyType");

                    b.Property<int?>("OrderNumber");

                    b.Property<string>("ParentUri");

                    b.Property<string>("Uri");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_OntTer_Id");

                    b.ToTable("OntologyTerm");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OntologyTermDescription", b =>
                {
                    b.Property<Guid>("OntologyTermId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("OntologyTermId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_OntTerDes_LocalizationId");

                    b.HasIndex("OntologyTermId")
                        .HasAnnotation("Npgsql:Name", "IX_OntTerDes_OntologyTermId");

                    b.ToTable("OntologyTermDescription");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OntologyTermExactMatch", b =>
                {
                    b.Property<Guid>("OntologyTermId");

                    b.Property<Guid>("ExactMatchId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("OntologyTermId", "ExactMatchId");

                    b.HasIndex("ExactMatchId")
                        .HasAnnotation("Npgsql:Name", "IX_OntTerExaMat_ExactMatchId");

                    b.HasIndex("OntologyTermId")
                        .HasAnnotation("Npgsql:Name", "IX_OntTerExaMat_OntologyTermId");

                    b.ToTable("OntologyTermExactMatch");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OntologyTermName", b =>
                {
                    b.Property<Guid>("OntologyTermId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("OntologyTermId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_OntTerNam_LocalizationId");

                    b.HasIndex("OntologyTermId")
                        .HasAnnotation("Npgsql:Name", "IX_OntTerNam_OntologyTermId");

                    b.ToTable("OntologyTermName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OntologyTermParent", b =>
                {
                    b.Property<Guid>("ParentId");

                    b.Property<Guid>("ChildId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ParentId", "ChildId");

                    b.HasIndex("ChildId")
                        .HasAnnotation("Npgsql:Name", "IX_OntTerPar_ChildId");

                    b.HasIndex("ParentId")
                        .HasAnnotation("Npgsql:Name", "IX_OntTerPar_ParentId");

                    b.ToTable("OntologyTermParent");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Organization", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_Org_Id");

                    b.ToTable("Organization");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationAddress", b =>
                {
                    b.Property<Guid>("OrganizationVersionedId");

                    b.Property<Guid>("AddressId");

                    b.Property<Guid>("CharacterId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("OrganizationVersionedId", "AddressId", "CharacterId");

                    b.HasIndex("AddressId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgAdd_AddressId");

                    b.HasIndex("CharacterId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgAdd_CharacterId");

                    b.HasIndex("OrganizationVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgAdd_OrganizationVersionedId");

                    b.ToTable("OrganizationAddress");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationArea", b =>
                {
                    b.Property<Guid>("OrganizationVersionedId");

                    b.Property<Guid>("AreaId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("OrganizationVersionedId", "AreaId");

                    b.HasIndex("AreaId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgAre_AreaId");

                    b.HasIndex("OrganizationVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgAre_OrganizationVersionedId");

                    b.ToTable("OrganizationArea");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationAreaMunicipality", b =>
                {
                    b.Property<Guid>("OrganizationVersionedId");

                    b.Property<Guid>("MunicipalityId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("OrganizationVersionedId", "MunicipalityId");

                    b.HasIndex("MunicipalityId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgAreMun_MunicipalityId");

                    b.HasIndex("OrganizationVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgAreMun_OrganizationVersionedId");

                    b.ToTable("OrganizationAreaMunicipality");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationBlockedAccessRight", b =>
                {
                    b.Property<Guid>("AccessBlockedId");

                    b.Property<Guid>("EntityId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("AccessBlockedId", "EntityId");

                    b.HasIndex("AccessBlockedId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgBloAccRig_AccessBlockedId");

                    b.HasIndex("EntityId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgBloAccRig_EntityId");

                    b.ToTable("OrganizationBlockedAccessRight");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationDescription", b =>
                {
                    b.Property<Guid>("OrganizationVersionedId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("OrganizationVersionedId", "TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgDes_LocalizationId");

                    b.HasIndex("OrganizationVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgDes_OrganizationVersionedId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgDes_TypeId");

                    b.ToTable("OrganizationDescription");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationDisplayNameType", b =>
                {
                    b.Property<Guid>("OrganizationVersionedId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("DisplayNameTypeId");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("OrganizationVersionedId", "LocalizationId");

                    b.HasIndex("DisplayNameTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgDisNamTyp_DisplayNameTypeId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgDisNamTyp_LocalizationId");

                    b.HasIndex("OrganizationVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgDisNamTyp_OrganizationVersionedId");

                    b.ToTable("OrganizationDisplayNameType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationEInvoicing", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<Guid>("OrganizationVersionedId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("ElectronicInvoicingAddress")
                        .HasMaxLength(110);

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("OperatorCode")
                        .HasMaxLength(110);

                    b.HasKey("Id", "OrganizationVersionedId");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_OrgEInv_Id");

                    b.HasIndex("OrganizationVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgEInv_OrganizationVersionedId");

                    b.ToTable("OrganizationEInvoicing");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationEInvoicingAdditionalInformation", b =>
                {
                    b.Property<Guid>("OrganizationEInvoicingId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Text")
                        .HasMaxLength(150);

                    b.HasKey("OrganizationEInvoicingId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgEInvAddInf_LocalizationId");

                    b.HasIndex("OrganizationEInvoicingId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgEInvAddInf_OrganizationEInvoicingId");

                    b.ToTable("OrganizationEInvoicingAdditionalInformation");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationEmail", b =>
                {
                    b.Property<Guid>("EmailId");

                    b.Property<Guid>("OrganizationVersionedId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("EmailId", "OrganizationVersionedId");

                    b.HasIndex("EmailId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgEma_EmailId");

                    b.HasIndex("OrganizationVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgEma_OrganizationVersionedId");

                    b.ToTable("OrganizationEmail");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationLanguageAvailability", b =>
                {
                    b.Property<Guid>("OrganizationVersionedId");

                    b.Property<Guid>("LanguageId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("StatusId");

                    b.HasKey("OrganizationVersionedId", "LanguageId");

                    b.HasIndex("LanguageId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgLanAva_LanguageId");

                    b.HasIndex("OrganizationVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgLanAva_OrganizationVersionedId");

                    b.HasIndex("StatusId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgLanAva_StatusId");

                    b.ToTable("OrganizationLanguageAvailability");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationName", b =>
                {
                    b.Property<Guid>("OrganizationVersionedId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.HasKey("OrganizationVersionedId", "TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgNam_LocalizationId");

                    b.HasIndex("OrganizationVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgNam_OrganizationVersionedId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgNam_TypeId");

                    b.ToTable("OrganizationName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationPhone", b =>
                {
                    b.Property<Guid>("PhoneId");

                    b.Property<Guid>("OrganizationVersionedId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("PhoneId", "OrganizationVersionedId");

                    b.HasIndex("OrganizationVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgPho_OrganizationVersionedId");

                    b.HasIndex("PhoneId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgPho_PhoneId");

                    b.ToTable("OrganizationPhone");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationService", b =>
                {
                    b.Property<Guid>("OrganizationId");

                    b.Property<Guid>("ServiceVersionedId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("OrganizationId", "ServiceVersionedId");

                    b.HasIndex("OrganizationId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgSer_OrganizationId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgSer_ServiceVersionedId");

                    b.ToTable("OrganizationService");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Label");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("OntologyType");

                    b.Property<int?>("OrderNumber");

                    b.Property<Guid?>("ParentId");

                    b.Property<string>("ParentUri");

                    b.Property<string>("Uri");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_OrgTyp_Id");

                    b.HasIndex("ParentId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgTyp_ParentId");

                    b.ToTable("OrganizationType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationTypeName", b =>
                {
                    b.Property<Guid>("OrganizationTypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("OrganizationTypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgTypNam_LocalizationId");

                    b.HasIndex("OrganizationTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgTypNam_OrganizationTypeId");

                    b.ToTable("OrganizationTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationVersioned", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AreaInformationTypeId");

                    b.Property<Guid?>("BusinessId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid?>("MunicipalityId");

                    b.Property<string>("Oid")
                        .HasMaxLength(100);

                    b.Property<Guid?>("ParentId");

                    b.Property<Guid>("PublishingStatusId");

                    b.Property<Guid?>("TypeId");

                    b.Property<Guid>("UnificRootId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.Property<Guid?>("VersioningId");

                    b.HasKey("Id");

                    b.HasIndex("AreaInformationTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgVer_AreaInformationTypeId");

                    b.HasIndex("BusinessId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgVer_BusinessId");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_OrgVer_Id");

                    b.HasIndex("MunicipalityId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgVer_MunicipalityId");

                    b.HasIndex("Oid")
                        .HasAnnotation("Npgsql:Name", "IX_OrgVer_Oid");

                    b.HasIndex("ParentId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgVer_ParentId");

                    b.HasIndex("PublishingStatusId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgVer_PublishingStatusId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgVer_TypeId");

                    b.HasIndex("UnificRootId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgVer_UnificRootId");

                    b.HasIndex("VersioningId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgVer_VersioningId");

                    b.ToTable("OrganizationVersioned");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationWebPage", b =>
                {
                    b.Property<Guid>("OrganizationVersionedId");

                    b.Property<Guid>("WebPageId");

                    b.Property<Guid>("TypeId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("OrganizationVersionedId", "WebPageId", "TypeId");

                    b.HasIndex("OrganizationVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgWebPag_OrganizationVersionedId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgWebPag_TypeId");

                    b.HasIndex("WebPageId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgWebPag_WebPageId");

                    b.ToTable("OrganizationWebPage");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Phone", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AdditionalInformation")
                        .HasMaxLength(150);

                    b.Property<string>("ChargeDescription")
                        .HasMaxLength(150);

                    b.Property<Guid>("ChargeTypeId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Number")
                        .HasMaxLength(20);

                    b.Property<int?>("OrderNumber");

                    b.Property<Guid?>("PrefixNumberId");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("ChargeTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_Pho_ChargeTypeId");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_Pho_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_Pho_LocalizationId");

                    b.HasIndex("PrefixNumberId")
                        .HasAnnotation("Npgsql:Name", "IX_Pho_PrefixNumberId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_Pho_TypeId");

                    b.ToTable("Phone");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PhoneNumberType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_PhoNumTyp_Id");

                    b.ToTable("PhoneNumberType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PhoneNumberTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_PhoNumTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_PhoNumTypNam_TypeId");

                    b.ToTable("PhoneNumberTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PostalCode", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .HasMaxLength(10);

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("IsValid");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid?>("MunicipalityId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_PosCod_Id");

                    b.HasIndex("MunicipalityId")
                        .HasAnnotation("Npgsql:Name", "IX_PosCod_MunicipalityId");

                    b.ToTable("PostalCode");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PostalCodeName", b =>
                {
                    b.Property<Guid>("PostalCodeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("PostalCodeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_PosCodNam_LocalizationId");

                    b.HasIndex("PostalCodeId")
                        .HasAnnotation("Npgsql:Name", "IX_PosCodNam_PostalCodeId");

                    b.ToTable("PostalCodeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PostOfficeBoxName", b =>
                {
                    b.Property<Guid>("AddressPostOfficeBoxId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.HasKey("AddressPostOfficeBoxId", "LocalizationId");

                    b.HasIndex("AddressPostOfficeBoxId")
                        .HasAnnotation("Npgsql:Name", "IX_PosOffBoxNam_AddressPostOfficeBoxId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_PosOffBoxNam_LocalizationId");

                    b.ToTable("PostOfficeBoxName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PrintableFormChannel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid?>("DeliveryAddressId");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.HasKey("Id");

                    b.HasIndex("DeliveryAddressId")
                        .HasAnnotation("Npgsql:Name", "IX_PriForCha_DeliveryAddressId");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_PriForCha_Id");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_PriForCha_ServiceChannelVersionedId");

                    b.ToTable("PrintableFormChannel");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PrintableFormChannelIdentifier", b =>
                {
                    b.Property<Guid>("PrintableFormChannelId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("FormIdentifier")
                        .HasMaxLength(100);

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("PrintableFormChannelId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_PriForChaIde_LocalizationId");

                    b.HasIndex("PrintableFormChannelId")
                        .HasAnnotation("Npgsql:Name", "IX_PriForChaIde_PrintableFormChannelId");

                    b.ToTable("PrintableFormChannelIdentifier");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PrintableFormChannelReceiver", b =>
                {
                    b.Property<Guid>("PrintableFormChannelId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("FormReceiver")
                        .HasMaxLength(100);

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("PrintableFormChannelId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_PriForChaRec_LocalizationId");

                    b.HasIndex("PrintableFormChannelId")
                        .HasAnnotation("Npgsql:Name", "IX_PriForChaRec_PrintableFormChannelId");

                    b.ToTable("PrintableFormChannelReceiver");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PrintableFormChannelUrl", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.Property<Guid>("PrintableFormChannelId");

                    b.Property<Guid>("TypeId");

                    b.Property<string>("Url")
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_PriForChaUrl_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_PriForChaUrl_LocalizationId");

                    b.HasIndex("PrintableFormChannelId")
                        .HasAnnotation("Npgsql:Name", "IX_PriForChaUrl_PrintableFormChannelId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_PriForChaUrl_TypeId");

                    b.ToTable("PrintableFormChannelUrl");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PrintableFormChannelUrlType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_PriForChaUrlTyp_Id");

                    b.ToTable("PrintableFormChannelUrlType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PrintableFormChannelUrlTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_PriForChaUrlTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_PriForChaUrlTypNam_TypeId");

                    b.ToTable("PrintableFormChannelUrlTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Privileges.UserAccessRight", b =>
                {
                    b.Property<Guid>("AccessRightId");

                    b.Property<Guid>("UserId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("AccessRightId", "UserId");

                    b.HasIndex("AccessRightId", "UserId")
                        .HasAnnotation("Npgsql:Name", "IX_UseAccRig_AccessRightId_UserId");

                    b.ToTable("UserAccessRight");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ProvisionType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_ProTyp_Id");

                    b.ToTable("ProvisionType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ProvisionTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_ProTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_ProTypNam_TypeId");

                    b.ToTable("ProvisionTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PublishingStatusType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.Property<int>("PriorityFallback");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_PubStaTyp_Id");

                    b.ToTable("PublishingStatusType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PublishingStatusTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_PubStaTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_PubStaTypNam_TypeId");

                    b.ToTable("PublishingStatusTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.SahaOrganizationInformation", b =>
                {
                    b.Property<Guid>("OrganizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<Guid>("SahaId");

                    b.Property<Guid>("SahaParentId");

                    b.HasKey("OrganizationId");

                    b.HasIndex("OrganizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SahOrgInf_OrganizationId");

                    b.ToTable("SahaOrganizationInformation");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Service", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_Ser_Id");

                    b.ToTable("Service");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceArea", b =>
                {
                    b.Property<Guid>("ServiceVersionedId");

                    b.Property<Guid>("AreaId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceVersionedId", "AreaId");

                    b.HasIndex("AreaId")
                        .HasAnnotation("Npgsql:Name", "IX_SerAre_AreaId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerAre_ServiceVersionedId");

                    b.ToTable("ServiceArea");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceAreaMunicipality", b =>
                {
                    b.Property<Guid>("ServiceVersionedId");

                    b.Property<Guid>("MunicipalityId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceVersionedId", "MunicipalityId");

                    b.HasIndex("MunicipalityId")
                        .HasAnnotation("Npgsql:Name", "IX_SerAreMun_MunicipalityId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerAreMun_ServiceVersionedId");

                    b.ToTable("ServiceAreaMunicipality");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceBlockedAccessRight", b =>
                {
                    b.Property<Guid>("AccessBlockedId");

                    b.Property<Guid>("EntityId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("AccessBlockedId", "EntityId");

                    b.HasIndex("AccessBlockedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerBloAccRig_AccessBlockedId");

                    b.HasIndex("EntityId")
                        .HasAnnotation("Npgsql:Name", "IX_SerBloAccRig_EntityId");

                    b.ToTable("ServiceBlockedAccessRight");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceClass", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Label");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("OntologyType");

                    b.Property<int?>("OrderNumber");

                    b.Property<Guid?>("ParentId");

                    b.Property<string>("ParentUri");

                    b.Property<string>("Uri");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerCla_Id");

                    b.HasIndex("ParentId")
                        .HasAnnotation("Npgsql:Name", "IX_SerCla_ParentId");

                    b.ToTable("ServiceClass");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceClassDescription", b =>
                {
                    b.Property<Guid>("ServiceClassId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceClassId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerClaDes_LocalizationId");

                    b.HasIndex("ServiceClassId")
                        .HasAnnotation("Npgsql:Name", "IX_SerClaDes_ServiceClassId");

                    b.ToTable("ServiceClassDescription");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceClassName", b =>
                {
                    b.Property<Guid>("ServiceClassId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("ServiceClassId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerClaNam_LocalizationId");

                    b.HasIndex("ServiceClassId")
                        .HasAnnotation("Npgsql:Name", "IX_SerClaNam_ServiceClassId");

                    b.ToTable("ServiceClassName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceCollection", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerCol_Id");

                    b.ToTable("ServiceCollection");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceCollectionBlockedAccessRight", b =>
                {
                    b.Property<Guid>("AccessBlockedId");

                    b.Property<Guid>("EntityId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("AccessBlockedId", "EntityId");

                    b.HasIndex("AccessBlockedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerColBloAccRig_AccessBlockedId");

                    b.HasIndex("EntityId")
                        .HasAnnotation("Npgsql:Name", "IX_SerColBloAccRig_EntityId");

                    b.ToTable("ServiceCollectionBlockedAccessRight");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceCollectionDescription", b =>
                {
                    b.Property<Guid>("ServiceCollectionVersionedId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceCollectionVersionedId", "TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerColDes_LocalizationId");

                    b.HasIndex("ServiceCollectionVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerColDes_ServiceCollectionVersionedId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerColDes_TypeId");

                    b.ToTable("ServiceCollectionDescription");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceCollectionLanguageAvailability", b =>
                {
                    b.Property<Guid>("ServiceCollectionVersionedId");

                    b.Property<Guid>("LanguageId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("StatusId");

                    b.HasKey("ServiceCollectionVersionedId", "LanguageId");

                    b.HasIndex("LanguageId")
                        .HasAnnotation("Npgsql:Name", "IX_SerColLanAva_LanguageId");

                    b.HasIndex("ServiceCollectionVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerColLanAva_ServiceCollectionVersionedId");

                    b.HasIndex("StatusId")
                        .HasAnnotation("Npgsql:Name", "IX_SerColLanAva_StatusId");

                    b.ToTable("ServiceCollectionLanguageAvailability");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceCollectionName", b =>
                {
                    b.Property<Guid>("ServiceCollectionVersionedId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.HasKey("ServiceCollectionVersionedId", "TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerColNam_LocalizationId");

                    b.HasIndex("ServiceCollectionVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerColNam_ServiceCollectionVersionedId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerColNam_TypeId");

                    b.ToTable("ServiceCollectionName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceCollectionService", b =>
                {
                    b.Property<Guid>("ServiceCollectionVersionedId");

                    b.Property<Guid>("ServiceId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceCollectionVersionedId", "ServiceId");

                    b.HasIndex("ServiceCollectionVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerColSer_ServiceCollectionVersionedId");

                    b.HasIndex("ServiceId")
                        .HasAnnotation("Npgsql:Name", "IX_SerColSer_ServiceId");

                    b.ToTable("ServiceCollectionService");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceCollectionVersioned", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("IsVisibleForAll");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("OrganizationId");

                    b.Property<Guid>("PublishingStatusId");

                    b.Property<Guid>("UnificRootId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.Property<Guid?>("VersioningId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerColVer_Id");

                    b.HasIndex("OrganizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerColVer_OrganizationId");

                    b.HasIndex("PublishingStatusId")
                        .HasAnnotation("Npgsql:Name", "IX_SerColVer_PublishingStatusId");

                    b.HasIndex("UnificRootId")
                        .HasAnnotation("Npgsql:Name", "IX_SerColVer_UnificRootId");

                    b.HasIndex("VersioningId")
                        .HasAnnotation("Npgsql:Name", "IX_SerColVer_VersioningId");

                    b.ToTable("ServiceCollectionVersioned");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceDescription", b =>
                {
                    b.Property<Guid>("ServiceVersionedId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceVersionedId", "TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerDes_LocalizationId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerDes_ServiceVersionedId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerDes_TypeId");

                    b.ToTable("ServiceDescription");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceElectronicCommunicationChannel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("ElectronicCommunicationChannel");

                    b.Property<string>("LastOperationId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("ServiceVersionedId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerEleComCha_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerEleComCha_LocalizationId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerEleComCha_ServiceVersionedId");

                    b.ToTable("ServiceElectronicCommunicationChannel");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceElectronicNotificationChannel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("ElectronicNotificationChannel");

                    b.Property<string>("LastOperationId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("ServiceVersionedId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerEleNotCha_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerEleNotCha_LocalizationId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerEleNotCha_ServiceVersionedId");

                    b.ToTable("ServiceElectronicNotificationChannel");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceFundingType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerFunTyp_Id");

                    b.ToTable("ServiceFundingType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceFundingTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerFunTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerFunTypNam_TypeId");

                    b.ToTable("ServiceFundingTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceHours", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("IsClosed");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<DateTime?>("OpeningHoursFrom");

                    b.Property<DateTime?>("OpeningHoursTo");

                    b.Property<int?>("OrderNumber");

                    b.Property<Guid>("ServiceHourTypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerHou_Id");

                    b.HasIndex("ServiceHourTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerHou_ServiceHourTypeId");

                    b.ToTable("ServiceHours");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceHoursAdditionalInformation", b =>
                {
                    b.Property<Guid>("ServiceHoursId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Text")
                        .HasMaxLength(100);

                    b.HasKey("ServiceHoursId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerHouAddInf_LocalizationId");

                    b.HasIndex("ServiceHoursId")
                        .HasAnnotation("Npgsql:Name", "IX_SerHouAddInf_ServiceHoursId");

                    b.ToTable("ServiceHoursAdditionalInformation");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceHourType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerHouTyp_Id");

                    b.ToTable("ServiceHourType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceHourTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerHouTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerHouTypNam_TypeId");

                    b.ToTable("ServiceHourTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerCha_Id");

                    b.ToTable("ServiceChannel");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelArea", b =>
                {
                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<Guid>("AreaId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceChannelVersionedId", "AreaId");

                    b.HasIndex("AreaId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaAre_AreaId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaAre_ServiceChannelVersionedId");

                    b.ToTable("ServiceChannelArea");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelAreaMunicipality", b =>
                {
                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<Guid>("MunicipalityId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceChannelVersionedId", "MunicipalityId");

                    b.HasIndex("MunicipalityId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaAreMun_MunicipalityId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaAreMun_ServiceChannelVersionedId");

                    b.ToTable("ServiceChannelAreaMunicipality");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelAttachment", b =>
                {
                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<Guid>("AttachmentId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceChannelVersionedId", "AttachmentId");

                    b.HasIndex("AttachmentId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaAtt_AttachmentId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaAtt_ServiceChannelVersionedId");

                    b.ToTable("ServiceChannelAttachment");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelConnectionType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaConTyp_Id");

                    b.ToTable("ServiceChannelConnectionType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelConnectionTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaConTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaConTypNam_TypeId");

                    b.ToTable("ServiceChannelConnectionTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelDescription", b =>
                {
                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceChannelVersionedId", "TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaDes_LocalizationId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaDes_ServiceChannelVersionedId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaDes_TypeId");

                    b.ToTable("ServiceChannelDescription");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelEmail", b =>
                {
                    b.Property<Guid>("EmailId");

                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("EmailId", "ServiceChannelVersionedId");

                    b.HasIndex("EmailId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaEma_EmailId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaEma_ServiceChannelVersionedId");

                    b.ToTable("ServiceChannelEmail");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelKeyword", b =>
                {
                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<Guid>("KeywordId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceChannelVersionedId", "KeywordId");

                    b.HasIndex("KeywordId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaKey_KeywordId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaKey_ServiceChannelVersionedId");

                    b.ToTable("ServiceChannelKeyword");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelLanguage", b =>
                {
                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<Guid>("LanguageId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("Order");

                    b.HasKey("ServiceChannelVersionedId", "LanguageId");

                    b.HasIndex("LanguageId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaLan_LanguageId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaLan_ServiceChannelVersionedId");

                    b.ToTable("ServiceChannelLanguage");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelLanguageAvailability", b =>
                {
                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<Guid>("LanguageId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("StatusId");

                    b.HasKey("ServiceChannelVersionedId", "LanguageId");

                    b.HasIndex("LanguageId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaLanAva_LanguageId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaLanAva_ServiceChannelVersionedId");

                    b.HasIndex("StatusId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaLanAva_StatusId");

                    b.ToTable("ServiceChannelLanguageAvailability");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelName", b =>
                {
                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.HasKey("ServiceChannelVersionedId", "TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaNam_LocalizationId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaNam_ServiceChannelVersionedId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaNam_TypeId");

                    b.ToTable("ServiceChannelName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelOntologyTerm", b =>
                {
                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<Guid>("OntologyTermId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceChannelVersionedId", "OntologyTermId");

                    b.HasIndex("OntologyTermId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaOntTer_OntologyTermId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaOntTer_ServiceChannelVersionedId");

                    b.ToTable("ServiceChannelOntologyTerm");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelPhone", b =>
                {
                    b.Property<Guid>("PhoneId");

                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("PhoneId", "ServiceChannelVersionedId");

                    b.HasIndex("PhoneId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaPho_PhoneId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaPho_ServiceChannelVersionedId");

                    b.ToTable("ServiceChannelPhone");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelServiceClass", b =>
                {
                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<Guid>("ServiceClassId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceChannelVersionedId", "ServiceClassId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaSerCla_ServiceChannelVersionedId");

                    b.HasIndex("ServiceClassId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaSerCla_ServiceClassId");

                    b.ToTable("ServiceChannelServiceClass");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelServiceHours", b =>
                {
                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<Guid>("ServiceHoursId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceChannelVersionedId", "ServiceHoursId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaSerHou_ServiceChannelVersionedId");

                    b.HasIndex("ServiceHoursId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaSerHou_ServiceHoursId");

                    b.ToTable("ServiceChannelServiceHours");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelTargetGroup", b =>
                {
                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<Guid>("TargetGroupId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceChannelVersionedId", "TargetGroupId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaTarGro_ServiceChannelVersionedId");

                    b.HasIndex("TargetGroupId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaTarGro_TargetGroupId");

                    b.ToTable("ServiceChannelTargetGroup");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelTranslationOrder", b =>
                {
                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<Guid>("TranslationOrderId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceChannelVersionedId", "TranslationOrderId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaTraOrd_ServiceChannelVersionedId");

                    b.HasIndex("TranslationOrderId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaTraOrd_TranslationOrderId");

                    b.ToTable("ServiceChannelTranslationOrder");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaTyp_Id");

                    b.ToTable("ServiceChannelType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaTypNam_TypeId");

                    b.ToTable("ServiceChannelTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelVersioned", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("AreaInformationTypeId");

                    b.Property<bool>("Charge");

                    b.Property<Guid>("ConnectionTypeId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("OrganizationId");

                    b.Property<Guid>("PublishingStatusId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("UnificRootId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.Property<Guid?>("VersioningId");

                    b.HasKey("Id");

                    b.HasIndex("AreaInformationTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaVer_AreaInformationTypeId");

                    b.HasIndex("ConnectionTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaVer_ConnectionTypeId");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaVer_Id");

                    b.HasIndex("OrganizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaVer_OrganizationId");

                    b.HasIndex("PublishingStatusId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaVer_PublishingStatusId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaVer_TypeId");

                    b.HasIndex("UnificRootId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaVer_UnificRootId");

                    b.HasIndex("VersioningId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaVer_VersioningId");

                    b.ToTable("ServiceChannelVersioned");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelWebPage", b =>
                {
                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<Guid>("WebPageId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("TypeId");

                    b.HasKey("ServiceChannelVersionedId", "WebPageId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaWebPag_ServiceChannelVersionedId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaWebPag_TypeId");

                    b.HasIndex("WebPageId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaWebPag_WebPageId");

                    b.ToTable("ServiceChannelWebPage");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChargeType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_ServCharType_Id");

                    b.ToTable("ServiceChargeType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChargeTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_ServCharTypeName_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_ServCharTypeName_TypeId");

                    b.ToTable("ServiceChargeTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceIndustrialClass", b =>
                {
                    b.Property<Guid>("ServiceVersionedId");

                    b.Property<Guid>("IndustrialClassId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceVersionedId", "IndustrialClassId");

                    b.HasIndex("IndustrialClassId")
                        .HasAnnotation("Npgsql:Name", "IX_SerIndCla_IndustrialClassId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerIndCla_ServiceVersionedId");

                    b.ToTable("ServiceIndustrialClass");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceKeyword", b =>
                {
                    b.Property<Guid>("ServiceVersionedId");

                    b.Property<Guid>("KeywordId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceVersionedId", "KeywordId");

                    b.HasIndex("KeywordId")
                        .HasAnnotation("Npgsql:Name", "IX_SerKey_KeywordId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerKey_ServiceVersionedId");

                    b.ToTable("ServiceKeyword");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceLanguage", b =>
                {
                    b.Property<Guid>("ServiceVersionedId");

                    b.Property<Guid>("LanguageId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("Order");

                    b.HasKey("ServiceVersionedId", "LanguageId");

                    b.HasIndex("LanguageId")
                        .HasAnnotation("Npgsql:Name", "IX_SerLan_LanguageId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerLan_ServiceVersionedId");

                    b.ToTable("ServiceLanguage");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceLanguageAvailability", b =>
                {
                    b.Property<Guid>("ServiceVersionedId");

                    b.Property<Guid>("LanguageId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("StatusId");

                    b.HasKey("ServiceVersionedId", "LanguageId");

                    b.HasIndex("LanguageId")
                        .HasAnnotation("Npgsql:Name", "IX_SerLanAva_LanguageId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerLanAva_ServiceVersionedId");

                    b.HasIndex("StatusId")
                        .HasAnnotation("Npgsql:Name", "IX_SerLanAva_StatusId");

                    b.ToTable("ServiceLanguageAvailability");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceLaw", b =>
                {
                    b.Property<Guid>("ServiceVersionedId");

                    b.Property<Guid>("LawId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceVersionedId", "LawId");

                    b.HasIndex("LawId")
                        .HasAnnotation("Npgsql:Name", "IX_SerLaw_LawId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerLaw_ServiceVersionedId");

                    b.ToTable("ServiceLaw");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceLifeEvent", b =>
                {
                    b.Property<Guid>("ServiceVersionedId");

                    b.Property<Guid>("LifeEventId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceVersionedId", "LifeEventId");

                    b.HasIndex("LifeEventId")
                        .HasAnnotation("Npgsql:Name", "IX_SerLifEve_LifeEventId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerLifEve_ServiceVersionedId");

                    b.ToTable("ServiceLifeEvent");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceLocationChannel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CoordinateSystem");

                    b.Property<bool>("CoordinatesSetManually");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<string>("Latitude");

                    b.Property<string>("Longitude");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<bool>("PhoneServiceCharge");

                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerLocCha_Id");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerLocCha_ServiceChannelVersionedId");

                    b.ToTable("ServiceLocationChannel");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceLocationChannelAddress", b =>
                {
                    b.Property<Guid>("ServiceLocationChannelId");

                    b.Property<Guid>("AddressId");

                    b.Property<Guid>("CharacterId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceLocationChannelId", "AddressId", "CharacterId");

                    b.HasIndex("AddressId")
                        .HasAnnotation("Npgsql:Name", "IX_SerLocChaAdd_AddressId");

                    b.HasIndex("CharacterId")
                        .HasAnnotation("Npgsql:Name", "IX_SerLocChaAdd_CharacterId");

                    b.HasIndex("ServiceLocationChannelId")
                        .HasAnnotation("Npgsql:Name", "IX_SerLocChaAdd_ServiceLocationChannelId");

                    b.ToTable("ServiceLocationChannelAddress");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceName", b =>
                {
                    b.Property<Guid>("ServiceVersionedId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.HasKey("ServiceVersionedId", "TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerNam_LocalizationId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerNam_ServiceVersionedId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerNam_TypeId");

                    b.ToTable("ServiceName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceOntologyTerm", b =>
                {
                    b.Property<Guid>("ServiceVersionedId");

                    b.Property<Guid>("OntologyTermId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceVersionedId", "OntologyTermId");

                    b.HasIndex("OntologyTermId")
                        .HasAnnotation("Npgsql:Name", "IX_SerOntTer_OntologyTermId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerOntTer_ServiceVersionedId");

                    b.ToTable("ServiceOntologyTerm");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceProducer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.Property<Guid>("ProvisionTypeId");

                    b.Property<Guid>("ServiceVersionedId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerPro_Id");

                    b.HasIndex("ProvisionTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerPro_ProvisionTypeId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerPro_ServiceVersionedId");

                    b.ToTable("ServiceProducer");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceProducerAdditionalInformation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("ServiceProducerId");

                    b.Property<string>("Text")
                        .HasMaxLength(150);

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerProAddInf_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerProAddInf_LocalizationId");

                    b.HasIndex("ServiceProducerId")
                        .HasAnnotation("Npgsql:Name", "IX_SerProAddInf_ServiceProducerId");

                    b.ToTable("ServiceProducerAdditionalInformation");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceProducerOrganization", b =>
                {
                    b.Property<Guid>("OrganizationId");

                    b.Property<Guid>("ServiceProducerId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("OrganizationId", "ServiceProducerId");

                    b.HasIndex("OrganizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerProOrg_OrganizationId");

                    b.HasIndex("ServiceProducerId")
                        .HasAnnotation("Npgsql:Name", "IX_SerProOrg_ServiceProducerId");

                    b.ToTable("ServiceProducerOrganization");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceRequirement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Requirement");

                    b.Property<Guid>("ServiceVersionedId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerReq_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerReq_LocalizationId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerReq_ServiceVersionedId");

                    b.ToTable("ServiceRequirement");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceClass", b =>
                {
                    b.Property<Guid>("ServiceVersionedId");

                    b.Property<Guid>("ServiceClassId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceVersionedId", "ServiceClassId");

                    b.HasIndex("ServiceClassId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerCla_ServiceClassId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerCla_ServiceVersionedId");

                    b.ToTable("ServiceServiceClass");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannel", b =>
                {
                    b.Property<Guid>("ServiceId");

                    b.Property<Guid>("ServiceChannelId");

                    b.Property<Guid?>("ChargeTypeId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("IsASTIConnection");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("ServiceId", "ServiceChannelId")
                        .HasName("PK_ServiceServiceChannel");

                    b.HasIndex("ChargeTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerCha_ChargeTypeId");

                    b.HasIndex("ServiceChannelId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerCha_ServiceChannelId");

                    b.HasIndex("ServiceId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerCha_ServiceId");

                    b.ToTable("ServiceServiceChannel");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannelAddress", b =>
                {
                    b.Property<Guid>("ServiceId");

                    b.Property<Guid>("ServiceChannelId");

                    b.Property<Guid>("AddressId");

                    b.Property<Guid>("CharacterId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceId", "ServiceChannelId", "AddressId");

                    b.HasIndex("AddressId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaAdd_AddressId");

                    b.HasIndex("CharacterId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaAdd_CharacterId");

                    b.HasIndex("ServiceId", "ServiceChannelId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaAdd_ServiceId_ServiceChannelId");

                    b.ToTable("ServiceServiceChannelAddress");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannelDescription", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<Guid>("ServiceChannelId");

                    b.Property<Guid>("ServiceId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description")
                        .HasMaxLength(500);

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("TypeId", "LocalizationId", "ServiceChannelId", "ServiceId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaDes_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaDes_TypeId");

                    b.HasIndex("ServiceId", "ServiceChannelId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaDes_ServiceId_ServiceChannelId");

                    b.ToTable("ServiceServiceChannelDescription");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannelDigitalAuthorization", b =>
                {
                    b.Property<Guid>("DigitalAuthorizationId");

                    b.Property<Guid>("ServiceId");

                    b.Property<Guid>("ServiceChannelId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("DigitalAuthorizationId", "ServiceId", "ServiceChannelId");

                    b.HasIndex("DigitalAuthorizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaDigAut_DigitalAuthorizationId");

                    b.HasIndex("ServiceId", "ServiceChannelId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaDigAut_ServiceId_ServiceChannelId");

                    b.ToTable("ServiceServiceChannelDigitalAuthorization");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannelEmail", b =>
                {
                    b.Property<Guid>("ServiceId");

                    b.Property<Guid>("ServiceChannelId");

                    b.Property<Guid>("EmailId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceId", "ServiceChannelId", "EmailId");

                    b.HasIndex("EmailId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaEma_EmailId");

                    b.HasIndex("ServiceId", "ServiceChannelId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaEma_ServiceId_ServiceChannelId");

                    b.ToTable("ServiceServiceChannelEmail");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannelExtraType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("ExtraSubTypeId");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("ServiceChannelId");

                    b.Property<Guid>("ServiceId");

                    b.HasKey("Id");

                    b.HasIndex("ExtraSubTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaExtTyp_ExtraSubTypeId");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaExtTyp_Id");

                    b.HasIndex("ServiceId", "ServiceChannelId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaExtTyp_ServiceId_ServiceChannelId");

                    b.ToTable("ServiceServiceChannelExtraType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannelExtraTypeDescription", b =>
                {
                    b.Property<Guid>("LocalizationId");

                    b.Property<Guid>("ServiceServiceChannelExtraTypeId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description")
                        .HasMaxLength(500);

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("LocalizationId", "ServiceServiceChannelExtraTypeId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaExtTypDes_LocalizationId");

                    b.HasIndex("ServiceServiceChannelExtraTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaExtTypDes_ServiceServiceChannelExtraTypeId");

                    b.ToTable("ServiceServiceChannelExtraTypeDescription");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannelPhone", b =>
                {
                    b.Property<Guid>("ServiceId");

                    b.Property<Guid>("ServiceChannelId");

                    b.Property<Guid>("PhoneId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceId", "ServiceChannelId", "PhoneId");

                    b.HasIndex("PhoneId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaPho_PhoneId");

                    b.HasIndex("ServiceId", "ServiceChannelId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaPho_ServiceId_ServiceChannelId");

                    b.ToTable("ServiceServiceChannelPhone");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannelServiceHours", b =>
                {
                    b.Property<Guid>("ServiceId");

                    b.Property<Guid>("ServiceChannelId");

                    b.Property<Guid>("ServiceHoursId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceId", "ServiceChannelId", "ServiceHoursId");

                    b.HasIndex("ServiceHoursId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaSerHou_ServiceHoursId");

                    b.HasIndex("ServiceId", "ServiceChannelId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaSerHou_ServiceId_ServiceChannelId");

                    b.ToTable("ServiceServiceChannelServiceHours");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannelWebPage", b =>
                {
                    b.Property<Guid>("ServiceId");

                    b.Property<Guid>("ServiceChannelId");

                    b.Property<Guid>("WebPageId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceId", "ServiceChannelId", "WebPageId");

                    b.HasIndex("WebPageId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaWebPag_WebPageId");

                    b.HasIndex("ServiceId", "ServiceChannelId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaWebPag_ServiceId_ServiceChannelId");

                    b.ToTable("ServiceServiceChannelWebPage");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceTargetGroup", b =>
                {
                    b.Property<Guid>("ServiceVersionedId");

                    b.Property<Guid>("TargetGroupId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<bool>("Override");

                    b.HasKey("ServiceVersionedId", "TargetGroupId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerTarGro_ServiceVersionedId");

                    b.HasIndex("TargetGroupId")
                        .HasAnnotation("Npgsql:Name", "IX_SerTarGro_TargetGroupId");

                    b.ToTable("ServiceTargetGroup");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceTranslationOrder", b =>
                {
                    b.Property<Guid>("ServiceVersionedId");

                    b.Property<Guid>("TranslationOrderId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceVersionedId", "TranslationOrderId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerTraOrd_ServiceVersionedId");

                    b.HasIndex("TranslationOrderId")
                        .HasAnnotation("Npgsql:Name", "IX_SerTraOrd_TranslationOrderId");

                    b.ToTable("ServiceTranslationOrder");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerTyp_Id");

                    b.ToTable("ServiceType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerTypNam_TypeId");

                    b.ToTable("ServiceTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceVersioned", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AreaInformationTypeId");

                    b.Property<Guid?>("ChargeTypeId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("ElectronicCommunication");

                    b.Property<bool>("ElectronicNotification");

                    b.Property<Guid?>("FundingTypeId");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("OrganizationId");

                    b.Property<Guid>("PublishingStatusId");

                    b.Property<Guid?>("StatutoryServiceGeneralDescriptionId");

                    b.Property<Guid?>("TypeId");

                    b.Property<Guid>("UnificRootId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.Property<Guid?>("VersioningId");

                    b.Property<bool>("WebPageInUse");

                    b.HasKey("Id");

                    b.HasIndex("AreaInformationTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerVer_AreaInformationTypeId");

                    b.HasIndex("ChargeTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerVer_ChargeTypeId");

                    b.HasIndex("FundingTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerVer_FundingTypeId");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerVer_Id");

                    b.HasIndex("OrganizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerVer_OrganizationId");

                    b.HasIndex("PublishingStatusId")
                        .HasAnnotation("Npgsql:Name", "IX_SerVer_PublishingStatusId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId")
                        .HasAnnotation("Npgsql:Name", "IX_SerVer_StatutoryServiceGeneralDescriptionId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerVer_TypeId");

                    b.HasIndex("UnificRootId")
                        .HasAnnotation("Npgsql:Name", "IX_SerVer_UnificRootId");

                    b.HasIndex("VersioningId")
                        .HasAnnotation("Npgsql:Name", "IX_SerVer_VersioningId");

                    b.ToTable("ServiceVersioned");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceWebPage", b =>
                {
                    b.Property<Guid>("ServiceVersionedId");

                    b.Property<Guid>("WebPageId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceVersionedId", "WebPageId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerWebPag_ServiceVersionedId");

                    b.HasIndex("WebPageId")
                        .HasAnnotation("Npgsql:Name", "IX_SerWebPag_WebPageId");

                    b.ToTable("ServiceWebPage");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceDescription", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionVersionedId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("StatutoryServiceGeneralDescriptionVersionedId", "TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerDes_LocalizationId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerDes_StatutoryServiceGeneralDescriptionVersionedId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerDes_TypeId");

                    b.ToTable("StatutoryServiceDescription");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerGenDes_Id");

                    b.ToTable("StatutoryServiceGeneralDescription");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceGeneralDescriptionVersioned", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("ChargeTypeId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("PublishingStatusId");

                    b.Property<string>("ReferenceCode");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("UnificRootId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.Property<Guid?>("VersioningId");

                    b.HasKey("Id");

                    b.HasIndex("ChargeTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerGenDesVer_ChargeTypeId");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerGenDesVer_Id");

                    b.HasIndex("PublishingStatusId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerGenDesVer_PublishingStatusId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerGenDesVer_TypeId");

                    b.HasIndex("UnificRootId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerGenDesVer_UnificRootId");

                    b.HasIndex("VersioningId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerGenDesVer_VersioningId");

                    b.ToTable("StatutoryServiceGeneralDescriptionVersioned");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceIndustrialClass", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionVersionedId");

                    b.Property<Guid>("IndustrialClassId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("StatutoryServiceGeneralDescriptionVersionedId", "IndustrialClassId");

                    b.HasIndex("IndustrialClassId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerIndCla_IndustrialClassId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerIndCla_StatutoryServiceGeneralDescriptionVersionedId");

                    b.ToTable("StatutoryServiceIndustrialClass");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceLanguage", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionVersionedId");

                    b.Property<Guid>("LanguageId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("StatutoryServiceGeneralDescriptionVersionedId", "LanguageId");

                    b.HasIndex("LanguageId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerLan_LanguageId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerLan_StatutoryServiceGeneralDescriptionVersionedId");

                    b.ToTable("StatutoryServiceLanguage");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceLaw", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionVersionedId");

                    b.Property<Guid>("LawId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("StatutoryServiceGeneralDescriptionVersionedId", "LawId");

                    b.HasIndex("LawId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerLaw_LawId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerLaw_StatutoryServiceGeneralDescriptionVersionedId");

                    b.ToTable("StatutoryServiceLaw");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceLifeEvent", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionVersionedId");

                    b.Property<Guid>("LifeEventId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("StatutoryServiceGeneralDescriptionVersionedId", "LifeEventId");

                    b.HasIndex("LifeEventId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerLifEve_LifeEventId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerLifEve_StatutoryServiceGeneralDescriptionVersionedId");

                    b.ToTable("StatutoryServiceLifeEvent");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceName", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionVersionedId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("StatutoryServiceGeneralDescriptionVersionedId", "TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerNam_LocalizationId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerNam_StatutoryServiceGeneralDescriptionVersionedId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerNam_TypeId");

                    b.ToTable("StatutoryServiceName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceOntologyTerm", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionVersionedId");

                    b.Property<Guid>("OntologyTermId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("StatutoryServiceGeneralDescriptionVersionedId", "OntologyTermId");

                    b.HasIndex("OntologyTermId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerOntTer_OntologyTermId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerOntTer_StatutoryServiceGeneralDescriptionVersionedId");

                    b.ToTable("StatutoryServiceOntologyTerm");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceRequirement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Requirement");

                    b.Property<Guid>("StatutoryServiceGeneralDescriptionVersionedId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerReq_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerReq_LocalizationId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerReq_StatutoryServiceGeneralDescriptionVersionedId");

                    b.ToTable("StatutoryServiceRequirement");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceServiceClass", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionVersionedId");

                    b.Property<Guid>("ServiceClassId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("StatutoryServiceGeneralDescriptionVersionedId", "ServiceClassId");

                    b.HasIndex("ServiceClassId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerSerCla_ServiceClassId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerSerCla_StatutoryServiceGeneralDescriptionVersionedId");

                    b.ToTable("StatutoryServiceServiceClass");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceTargetGroup", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionVersionedId");

                    b.Property<Guid>("TargetGroupId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("StatutoryServiceGeneralDescriptionVersionedId", "TargetGroupId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerTarGro_StatutoryServiceGeneralDescriptionVersionedId");

                    b.HasIndex("TargetGroupId")
                        .HasAnnotation("Npgsql:Name", "IX_StaSerTarGro_TargetGroupId");

                    b.ToTable("StatutoryServiceTargetGroup");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StreetName", b =>
                {
                    b.Property<Guid>("AddressStreetId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.HasKey("AddressStreetId", "LocalizationId");

                    b.HasIndex("AddressStreetId")
                        .HasAnnotation("Npgsql:Name", "IX_StrNam_AddressStreetId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_StrNam_LocalizationId");

                    b.ToTable("StreetName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TargetGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Label");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("OntologyType");

                    b.Property<int?>("OrderNumber");

                    b.Property<Guid?>("ParentId");

                    b.Property<string>("ParentUri");

                    b.Property<string>("Uri");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_TarGro_Id");

                    b.HasIndex("ParentId")
                        .HasAnnotation("Npgsql:Name", "IX_TarGro_ParentId");

                    b.ToTable("TargetGroup");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TargetGroupName", b =>
                {
                    b.Property<Guid>("TargetGroupId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TargetGroupId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_TarGroNam_LocalizationId");

                    b.HasIndex("TargetGroupId")
                        .HasAnnotation("Npgsql:Name", "IX_TarGroNam_TargetGroupId");

                    b.ToTable("TargetGroupName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TrackingServiceServiceChannel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ChannelId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("OperationType");

                    b.Property<Guid>("ServiceId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_TraSerSerCha_Id");

                    b.ToTable("TrackingServiceServiceChannel");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TranslationCompany", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description")
                        .HasMaxLength(2500);

                    b.Property<string>("Email")
                        .HasMaxLength(100);

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<string>("Url")
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_TraCom_Id");

                    b.ToTable("TranslationCompany");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TranslationOrder", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AdditionalInformation")
                        .HasMaxLength(2500);

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime?>("DeliverAt");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<long>("OrderIdentifier");

                    b.Property<string>("SenderEmail")
                        .HasMaxLength(100);

                    b.Property<string>("SenderName")
                        .HasMaxLength(100);

                    b.Property<string>("SourceLanguageData");

                    b.Property<string>("SourceLanguageDataHash");

                    b.Property<Guid>("SourceLanguageId");

                    b.Property<string>("TargetLanguageData");

                    b.Property<string>("TargetLanguageDataHash");

                    b.Property<Guid>("TargetLanguageId");

                    b.Property<Guid>("TranslationCompanyId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_TraOrd_Id");

                    b.HasIndex("SourceLanguageId")
                        .HasAnnotation("Npgsql:Name", "IX_TraOrd_SourceLanguageId");

                    b.HasIndex("TargetLanguageId")
                        .HasAnnotation("Npgsql:Name", "IX_TraOrd_TargetLanguageId");

                    b.HasIndex("TranslationCompanyId")
                        .HasAnnotation("Npgsql:Name", "IX_TraOrd_TranslationCompanyId");

                    b.ToTable("TranslationOrder");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TranslationOrderState", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("OperationDescription")
                        .HasMaxLength(2500);

                    b.Property<DateTime>("SendAt");

                    b.Property<Guid>("TranslationOrderId");

                    b.Property<Guid>("TranslationStateId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_TraOrdSta_Id");

                    b.HasIndex("TranslationOrderId")
                        .HasAnnotation("Npgsql:Name", "IX_TraOrdSta_TranslationOrderId");

                    b.HasIndex("TranslationStateId")
                        .HasAnnotation("Npgsql:Name", "IX_TraOrdSta_TranslationStateId");

                    b.ToTable("TranslationOrderState");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TranslationStateType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_TraStaTyp_Id");

                    b.ToTable("TranslationStateType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TranslationStateTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_TraStaTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_TraStaTypNam_TypeId");

                    b.ToTable("TranslationStateTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.UserOrganization", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("IsMain");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("OrganizationId");

                    b.Property<Guid>("RoleId");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_UseOrg_Id");

                    b.HasIndex("OrganizationId")
                        .HasAnnotation("Npgsql:Name", "IX_UseOrg_OrganizationId");

                    b.ToTable("UserOrganization");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Versioning", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid?>("PreviousVersionId");

                    b.Property<int>("VersionMajor");

                    b.Property<int>("VersionMinor");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_Ver_Id");

                    b.HasIndex("PreviousVersionId")
                        .HasAnnotation("Npgsql:Name", "IX_Ver_PreviousVersionId");

                    b.ToTable("Versioning");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.WebPage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<string>("LastOperationId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .HasMaxLength(110);

                    b.Property<int?>("OrderNumber");

                    b.Property<string>("Url")
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_WebPag_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_WebPag_LocalizationId");

                    b.ToTable("WebPage");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.WebpageChannel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_WebCha_Id");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_WebCha_ServiceChannelVersionedId");

                    b.ToTable("WebpageChannel");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.WebpageChannelUrl", b =>
                {
                    b.Property<Guid>("WebpageChannelId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Url");

                    b.HasKey("WebpageChannelId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_WebChaUrl_LocalizationId");

                    b.HasIndex("WebpageChannelId")
                        .HasAnnotation("Npgsql:Name", "IX_WebChaUrl_WebpageChannelId");

                    b.ToTable("WebpageChannelUrl");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.WebPageType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_WebPagTyp_Id");

                    b.ToTable("WebPageType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.WebPageTypeName", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_WebPagTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_WebPagTypNam_TypeId");

                    b.ToTable("WebPageTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AccessRightName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.AccessRightType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Address", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Country", "Country")
                        .WithMany("Addresses")
                        .HasForeignKey("CountryId");

                    b.HasOne("PTV.Database.Model.Models.AddressType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AddressAdditionalInformation", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Address", "Address")
                        .WithMany("AddressAdditionalInformations")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AddressForeign", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Address", "Address")
                        .WithMany("AddressForeigns")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AddressForeignTextName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AddressForeign", "AddressForeign")
                        .WithMany("ForeignTextNames")
                        .HasForeignKey("AddressForeignId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AddressCharacterName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.AddressCharacter", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AddressPostOfficeBox", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Address", "Address")
                        .WithMany("AddressPostOfficeBoxes")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Municipality", "Municipality")
                        .WithMany()
                        .HasForeignKey("MunicipalityId");

                    b.HasOne("PTV.Database.Model.Models.PostalCode", "PostalCode")
                        .WithMany()
                        .HasForeignKey("PostalCodeId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AddressStreet", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Address", "Address")
                        .WithMany("AddressStreets")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Municipality", "Municipality")
                        .WithMany()
                        .HasForeignKey("MunicipalityId");

                    b.HasOne("PTV.Database.Model.Models.PostalCode", "PostalCode")
                        .WithMany()
                        .HasForeignKey("PostalCodeId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AddressTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.AddressType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AppEnvironmentData", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AppEnvironmentDataType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AppEnvironmentDataTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.AppEnvironmentDataType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Area", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AreaType", "AreaType")
                        .WithMany()
                        .HasForeignKey("AreaTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AreaInformationTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.AreaInformationType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AreaMunicipality", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Area", "Area")
                        .WithMany("AreaMunicipalities")
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Municipality", "Municipality")
                        .WithMany()
                        .HasForeignKey("MunicipalityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AreaName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Area", "Area")
                        .WithMany("AreaNames")
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AreaTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.AreaType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Attachment", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.AttachmentType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AttachmentTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.AttachmentType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Coordinate", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Address", "Address")
                        .WithMany("Coordinates")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.CoordinateType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.CoordinateTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.CoordinateType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.CountryName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Country", "Country")
                        .WithMany("CountryNames")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.DailyOpeningTime", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceHours", "OpeningHour")
                        .WithMany("DailyOpeningTimes")
                        .HasForeignKey("OpeningHourId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.DescriptionTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.DescriptionType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.DialCode", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Country", "Country")
                        .WithMany("DialCodes")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.DigitalAuthorization", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.DigitalAuthorization", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.DigitalAuthorizationName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.DigitalAuthorization", "DigitalAuthorization")
                        .WithMany("Names")
                        .HasForeignKey("DigitalAuthorizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ElectronicChannel", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("ElectronicChannels")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ElectronicChannelUrl", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ElectronicChannel", "ElectronicChannel")
                        .WithMany("LocalizedUrls")
                        .HasForeignKey("ElectronicChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Email", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ExceptionHoursStatusTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ExceptionHoursStatusType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ExtraSubType", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ExtraType", "ExtraType")
                        .WithMany("ExtraSubType")
                        .HasForeignKey("ExtraTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ExtraSubTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ExtraSubType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ExtraTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ExtraType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Form", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.FormType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.FormTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.FormType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionBlockedAccessRight", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AccessRightType", "AccessBlocked")
                        .WithMany()
                        .HasForeignKey("AccessBlockedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", "Entity")
                        .WithMany()
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionLanguageAvailability", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescriptionVersioned", "StatutoryServiceGeneralDescriptionVersioned")
                        .WithMany("LanguageAvailabilities")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionServiceChannel", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChargeType", "ChargeType")
                        .WithMany()
                        .HasForeignKey("ChargeTypeId")
                        .HasAnnotation("Npgsql:Name", "FK_GenDesSerChan_ChaTypId");

                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("StatutoryServiceGeneralDescriptionServiceChannels")
                        .HasForeignKey("ServiceChannelId")
                        .HasAnnotation("Npgsql:Name", "FK_GenDesSerChan_SerChaId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", "StatutoryServiceGeneralDescription")
                        .WithMany("StatutoryServiceGeneralDescriptionServiceChannels")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId")
                        .HasAnnotation("Npgsql:Name", "FK_GenDesSerChan_GenDesId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionServiceChannelDescription", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "FK_GenDesSerChaDes_LocId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.DescriptionType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .HasAnnotation("Npgsql:Name", "FK_GenDesSerChaDes_TypId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.GeneralDescriptionServiceChannel", "GeneralDescriptionServiceChannel")
                        .WithMany("GeneralDescriptionServiceChannelDescriptions")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId", "ServiceChannelId")
                        .HasAnnotation("Npgsql:Name", "FK_GenDesSerChaDes_SerChaId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionServiceChannelDigitalAuthorization", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.DigitalAuthorization", "DigitalAuthorization")
                        .WithMany()
                        .HasForeignKey("DigitalAuthorizationId")
                        .HasAnnotation("Npgsql:Name", "FK_GenDesSerChaDesDigAut_DigAutId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.GeneralDescriptionServiceChannel", "GeneralDescriptionServiceChannel")
                        .WithMany("GeneralDescriptionServiceChannelDigitalAuthorizations")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId", "ServiceChannelId")
                        .HasAnnotation("Npgsql:Name", "FK_GenDesSerChaDesDigAut_SerChaId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionServiceChannelExtraType", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ExtraSubType", "ExtraSubType")
                        .WithMany()
                        .HasForeignKey("ExtraSubTypeId")
                        .HasAnnotation("Npgsql:Name", "FK_GenDesSerChaExtTyp_ExtSubTypId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("StatutoryServiceGeneralDescriptionServiceChannelExtraTypes")
                        .HasForeignKey("ServiceChannelId")
                        .HasAnnotation("Npgsql:Name", "FK_GenDesSerChaExtTyp_SerChaId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", "StatutoryServiceGeneralDescription")
                        .WithMany("StatutoryServiceGeneralDescriptionServiceChannelExtraTypes")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId")
                        .HasAnnotation("Npgsql:Name", "FK_GenDesSerChaExtTyp_GenDesId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionServiceChannelExtraTypeDescription", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.GeneralDescriptionServiceChannelExtraType", "GeneralDescriptionServiceChannelExtraType")
                        .WithMany("GeneralDescriptionServiceChannelExtraTypeDescriptions")
                        .HasForeignKey("GeneralDescriptionServiceChannelExtraTypeId")
                        .HasAnnotation("Npgsql:Name", "FK_GenDesSerChaExtTypDes_GenDesSerChaExtTypId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "FK_GenDesSerChaExtTypDes_LocId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionTranslationOrder", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescriptionVersioned", "StatutoryServiceGeneralDescriptionVersioned")
                        .WithMany("GeneralDescriptionTranslationOrders")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.TranslationOrder", "TranslationOrder")
                        .WithMany("GeneralDescriptionTranslationOrders")
                        .HasForeignKey("TranslationOrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ChannelBlockedAccessRight", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AccessRightType", "AccessBlocked")
                        .WithMany()
                        .HasForeignKey("AccessBlockedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "Entity")
                        .WithMany("BlockedAccessRights")
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription")
                        .WithMany("BlockedAccessRights")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.IndustrialClass", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.IndustrialClass", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.IndustrialClassName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.IndustrialClass", "IndustrialClass")
                        .WithMany("Names")
                        .HasForeignKey("IndustrialClassId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Keyword", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.LanguageName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Language")
                        .WithMany("Names")
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany("Localizations")
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.LawName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Law", "Law")
                        .WithMany("Names")
                        .HasForeignKey("LawId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.LawWebPage", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Law", "Law")
                        .WithMany("WebPages")
                        .HasForeignKey("LawId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.WebPage", "WebPage")
                        .WithMany()
                        .HasForeignKey("WebPageId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.LifeEvent", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.LifeEvent", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.LifeEventName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.LifeEvent", "LifeEvent")
                        .WithMany("Names")
                        .HasForeignKey("LifeEventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.MunicipalityName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Municipality", "Municipality")
                        .WithMany("MunicipalityNames")
                        .HasForeignKey("MunicipalityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.NameTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.NameType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OntologyTermDescription", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.OntologyTerm", "OntologyTerm")
                        .WithMany("Descriptions")
                        .HasForeignKey("OntologyTermId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OntologyTermExactMatch", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ExactMatch", "ExactMatch")
                        .WithMany("OntologyTermExactMatches")
                        .HasForeignKey("ExactMatchId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.OntologyTerm", "OntologyTerm")
                        .WithMany("OntologyTermExactMatches")
                        .HasForeignKey("OntologyTermId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OntologyTermName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.OntologyTerm", "OntologyTerm")
                        .WithMany("Names")
                        .HasForeignKey("OntologyTermId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OntologyTermParent", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.OntologyTerm", "Child")
                        .WithMany("Parents")
                        .HasForeignKey("ChildId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.OntologyTerm", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationAddress", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.AddressCharacter", "Character")
                        .WithMany()
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.OrganizationVersioned", "OrganizationVersioned")
                        .WithMany("OrganizationAddresses")
                        .HasForeignKey("OrganizationVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationArea", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Area", "Area")
                        .WithMany()
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.OrganizationVersioned", "OrganizationVersioned")
                        .WithMany("OrganizationAreas")
                        .HasForeignKey("OrganizationVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationAreaMunicipality", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Municipality", "Municipality")
                        .WithMany()
                        .HasForeignKey("MunicipalityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.OrganizationVersioned", "OrganizationVersioned")
                        .WithMany("OrganizationAreaMunicipalities")
                        .HasForeignKey("OrganizationVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationBlockedAccessRight", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AccessRightType", "AccessBlocked")
                        .WithMany()
                        .HasForeignKey("AccessBlockedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Organization", "Entity")
                        .WithMany("BlockedAccessRights")
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationDescription", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.OrganizationVersioned", "OrganizationVersioned")
                        .WithMany("OrganizationDescriptions")
                        .HasForeignKey("OrganizationVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.DescriptionType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationDisplayNameType", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.NameType", "DisplayNameType")
                        .WithMany()
                        .HasForeignKey("DisplayNameTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.OrganizationVersioned", "OrganizationVersioned")
                        .WithMany("OrganizationDisplayNameTypes")
                        .HasForeignKey("OrganizationVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationEInvoicing", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.OrganizationVersioned", "OrganizationVersioned")
                        .WithMany("OrganizationEInvoicings")
                        .HasForeignKey("OrganizationVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationEInvoicingAdditionalInformation", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.OrganizationEInvoicing", "OrganizationEInvoicing")
                        .WithMany("EInvoicingAdditionalInformations")
                        .HasForeignKey("OrganizationEInvoicingId")
                        .HasPrincipalKey("Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationEmail", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Email", "Email")
                        .WithMany()
                        .HasForeignKey("EmailId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.OrganizationVersioned", "OrganizationVersioned")
                        .WithMany("OrganizationEmails")
                        .HasForeignKey("OrganizationVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationLanguageAvailability", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.OrganizationVersioned", "OrganizationVersioned")
                        .WithMany("LanguageAvailabilities")
                        .HasForeignKey("OrganizationVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.OrganizationVersioned", "OrganizationVersioned")
                        .WithMany("OrganizationNames")
                        .HasForeignKey("OrganizationVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.NameType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationPhone", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.OrganizationVersioned", "OrganizationVersioned")
                        .WithMany("OrganizationPhones")
                        .HasForeignKey("OrganizationVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Phone", "Phone")
                        .WithMany()
                        .HasForeignKey("PhoneId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationService", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Organization", "Organization")
                        .WithMany("OrganizationServices")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "ServiceVersioned")
                        .WithMany("OrganizationServices")
                        .HasForeignKey("ServiceVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationType", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.OrganizationType", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.OrganizationType", "OrganizationType")
                        .WithMany("Names")
                        .HasForeignKey("OrganizationTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationVersioned", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AreaInformationType", "AreaInformationType")
                        .WithMany()
                        .HasForeignKey("AreaInformationTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Business", "Business")
                        .WithMany()
                        .HasForeignKey("BusinessId");

                    b.HasOne("PTV.Database.Model.Models.Municipality", "Municipality")
                        .WithMany("Organizations")
                        .HasForeignKey("MunicipalityId");

                    b.HasOne("PTV.Database.Model.Models.Organization", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "PublishingStatus")
                        .WithMany()
                        .HasForeignKey("PublishingStatusId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.OrganizationType", "Type")
                        .WithMany("Organization")
                        .HasForeignKey("TypeId");

                    b.HasOne("PTV.Database.Model.Models.Organization", "UnificRoot")
                        .WithMany("Versions")
                        .HasForeignKey("UnificRootId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Versioning", "Versioning")
                        .WithMany()
                        .HasForeignKey("VersioningId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationWebPage", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.OrganizationVersioned", "OrganizationVersioned")
                        .WithMany("OrganizationWebAddress")
                        .HasForeignKey("OrganizationVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.WebPageType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.WebPage", "WebPage")
                        .WithMany("OrganizationWebAddress")
                        .HasForeignKey("WebPageId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Phone", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChargeType", "ChargeType")
                        .WithMany()
                        .HasForeignKey("ChargeTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.DialCode", "PrefixNumber")
                        .WithMany()
                        .HasForeignKey("PrefixNumberId");

                    b.HasOne("PTV.Database.Model.Models.PhoneNumberType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PhoneNumberTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PhoneNumberType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PostalCode", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Municipality", "Municipality")
                        .WithMany()
                        .HasForeignKey("MunicipalityId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PostalCodeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PostalCode", "PostalCode")
                        .WithMany("PostalCodeNames")
                        .HasForeignKey("PostalCodeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PostOfficeBoxName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AddressPostOfficeBox", "AddressPostOfficeBox")
                        .WithMany("PostOfficeBoxNames")
                        .HasForeignKey("AddressPostOfficeBoxId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PrintableFormChannel", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Address", "DeliveryAddress")
                        .WithMany()
                        .HasForeignKey("DeliveryAddressId");

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("PrintableFormChannels")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PrintableFormChannelIdentifier", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PrintableFormChannel", "PrintableFormChannel")
                        .WithMany("FormIdentifiers")
                        .HasForeignKey("PrintableFormChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PrintableFormChannelReceiver", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PrintableFormChannel", "PrintableFormChannel")
                        .WithMany("FormReceivers")
                        .HasForeignKey("PrintableFormChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PrintableFormChannelUrl", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PrintableFormChannel", "PrintableFormChannel")
                        .WithMany("ChannelUrls")
                        .HasForeignKey("PrintableFormChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PrintableFormChannelUrlType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PrintableFormChannelUrlTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PrintableFormChannelUrlType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Privileges.UserAccessRight", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AccessRightType", "AccessRight")
                        .WithMany()
                        .HasForeignKey("AccessRightId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ProvisionTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ProvisionType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PublishingStatusTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.SahaOrganizationInformation", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Organization", "Organization")
                        .WithMany("SahaOrganizationInformations")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceArea", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Area", "Area")
                        .WithMany()
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "ServiceVersioned")
                        .WithMany("Areas")
                        .HasForeignKey("ServiceVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceAreaMunicipality", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Municipality", "Municipality")
                        .WithMany()
                        .HasForeignKey("MunicipalityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "ServiceVersioned")
                        .WithMany("AreaMunicipalities")
                        .HasForeignKey("ServiceVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceBlockedAccessRight", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AccessRightType", "AccessBlocked")
                        .WithMany()
                        .HasForeignKey("AccessBlockedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Service", "Entity")
                        .WithMany("BlockedAccessRights")
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceClass", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceClass", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceClassDescription", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceClass", "ServiceClass")
                        .WithMany("Descriptions")
                        .HasForeignKey("ServiceClassId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceClassName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceClass", "ServiceClass")
                        .WithMany("Names")
                        .HasForeignKey("ServiceClassId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceCollectionBlockedAccessRight", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AccessRightType", "AccessBlocked")
                        .WithMany()
                        .HasForeignKey("AccessBlockedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceCollection", "Entity")
                        .WithMany("BlockedAccessRights")
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceCollectionDescription", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceCollectionVersioned", "ServiceCollectionVersioned")
                        .WithMany("ServiceCollectionDescriptions")
                        .HasForeignKey("ServiceCollectionVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.DescriptionType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceCollectionLanguageAvailability", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceCollectionVersioned", "ServiceCollectionVersioned")
                        .WithMany("LanguageAvailabilities")
                        .HasForeignKey("ServiceCollectionVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceCollectionName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceCollectionVersioned", "ServiceCollectionVersioned")
                        .WithMany("ServiceCollectionNames")
                        .HasForeignKey("ServiceCollectionVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.NameType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceCollectionService", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceCollectionVersioned", "ServiceCollectionVersioned")
                        .WithMany("ServiceCollectionServices")
                        .HasForeignKey("ServiceCollectionVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Service", "Service")
                        .WithMany("ServiceCollectionServices")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceCollectionVersioned", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Organization", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "PublishingStatus")
                        .WithMany()
                        .HasForeignKey("PublishingStatusId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceCollection", "UnificRoot")
                        .WithMany("Versions")
                        .HasForeignKey("UnificRootId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Versioning", "Versioning")
                        .WithMany()
                        .HasForeignKey("VersioningId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceDescription", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "ServiceVersioned")
                        .WithMany("ServiceDescriptions")
                        .HasForeignKey("ServiceVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.DescriptionType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceElectronicCommunicationChannel", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "ServiceVersioned")
                        .WithMany("ServiceElectronicCommunicationChannels")
                        .HasForeignKey("ServiceVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceElectronicNotificationChannel", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "ServiceVersioned")
                        .WithMany("ServiceElectronicNotificationChannels")
                        .HasForeignKey("ServiceVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceFundingTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceFundingType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceHours", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceHourType", "ServiceHourType")
                        .WithMany()
                        .HasForeignKey("ServiceHourTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceHoursAdditionalInformation", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceHours", "ServiceHours")
                        .WithMany("AdditionalInformations")
                        .HasForeignKey("ServiceHoursId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceHourTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceHourType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelArea", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Area", "Area")
                        .WithMany()
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("Areas")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelAreaMunicipality", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Municipality", "Municipality")
                        .WithMany()
                        .HasForeignKey("MunicipalityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("AreaMunicipalities")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelAttachment", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Attachment", "Attachment")
                        .WithMany("ServiceChannelAttachments")
                        .HasForeignKey("AttachmentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("Attachments")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelConnectionTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelConnectionType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelDescription", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("ServiceChannelDescriptions")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.DescriptionType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelEmail", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Email", "Email")
                        .WithMany("ServiceChannelEmails")
                        .HasForeignKey("EmailId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("Emails")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelKeyword", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Keyword", "Keyword")
                        .WithMany()
                        .HasForeignKey("KeywordId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("Keywords")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelLanguage", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("Languages")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelLanguageAvailability", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("LanguageAvailabilities")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("ServiceChannelNames")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.NameType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelOntologyTerm", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.OntologyTerm", "OntologyTerm")
                        .WithMany("ServiceChannelOntologyTerms")
                        .HasForeignKey("OntologyTermId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("OntologyTerms")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelPhone", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Phone", "Phone")
                        .WithMany("ServiceChannelPhones")
                        .HasForeignKey("PhoneId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("Phones")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelServiceClass", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("ServiceClasses")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceClass", "ServiceClass")
                        .WithMany()
                        .HasForeignKey("ServiceClassId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelServiceHours", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("ServiceChannelServiceHours")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceHours", "ServiceHours")
                        .WithMany("ServiceChannelServiceHours")
                        .HasForeignKey("ServiceHoursId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelTargetGroup", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("TargetGroups")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.TargetGroup", "TargetGroup")
                        .WithMany()
                        .HasForeignKey("TargetGroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelTranslationOrder", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("ServiceChannelTranslationOrders")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.TranslationOrder", "TranslationOrder")
                        .WithMany("ServiceChannelTranslationOrders")
                        .HasForeignKey("TranslationOrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelVersioned", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AreaInformationType", "AreaInformationType")
                        .WithMany()
                        .HasForeignKey("AreaInformationTypeId");

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelConnectionType", "ConnectionType")
                        .WithMany()
                        .HasForeignKey("ConnectionTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Organization", "Organization")
                        .WithMany("OrganizationServiceChannelsVersioned")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "PublishingStatus")
                        .WithMany()
                        .HasForeignKey("PublishingStatusId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "UnificRoot")
                        .WithMany("Versions")
                        .HasForeignKey("UnificRootId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Versioning", "Versioning")
                        .WithMany()
                        .HasForeignKey("VersioningId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelWebPage", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("WebPages")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.WebPageType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.WebPage", "WebPage")
                        .WithMany("ServiceChannelWebPages")
                        .HasForeignKey("WebPageId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChargeTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChargeType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceIndustrialClass", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.IndustrialClass", "IndustrialClass")
                        .WithMany("ServiceServiceClasses")
                        .HasForeignKey("IndustrialClassId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "ServiceVersioned")
                        .WithMany("ServiceIndustrialClasses")
                        .HasForeignKey("ServiceVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceKeyword", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Keyword", "Keyword")
                        .WithMany("ServiceKeywords")
                        .HasForeignKey("KeywordId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "ServiceVersioned")
                        .WithMany("ServiceKeywords")
                        .HasForeignKey("ServiceVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceLanguage", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "ServiceVersioned")
                        .WithMany("ServiceLanguages")
                        .HasForeignKey("ServiceVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceLanguageAvailability", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "ServiceVersioned")
                        .WithMany("LanguageAvailabilities")
                        .HasForeignKey("ServiceVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceLaw", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Law", "Law")
                        .WithMany("ServiceLaws")
                        .HasForeignKey("LawId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "ServiceVersioned")
                        .WithMany("ServiceLaws")
                        .HasForeignKey("ServiceVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceLifeEvent", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.LifeEvent", "LifeEvent")
                        .WithMany("ServiceLifeEvents")
                        .HasForeignKey("LifeEventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "ServiceVersioned")
                        .WithMany("ServiceLifeEvents")
                        .HasForeignKey("ServiceVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceLocationChannel", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("ServiceLocationChannels")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceLocationChannelAddress", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.AddressCharacter", "Character")
                        .WithMany()
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceLocationChannel", "ServiceLocationChannel")
                        .WithMany("Addresses")
                        .HasForeignKey("ServiceLocationChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "ServiceVersioned")
                        .WithMany("ServiceNames")
                        .HasForeignKey("ServiceVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.NameType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceOntologyTerm", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.OntologyTerm", "OntologyTerm")
                        .WithMany("ServiceOntologyTerms")
                        .HasForeignKey("OntologyTermId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "ServiceVersioned")
                        .WithMany("ServiceOntologyTerms")
                        .HasForeignKey("ServiceVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceProducer", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ProvisionType", "ProvisionType")
                        .WithMany()
                        .HasForeignKey("ProvisionTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "ServiceVersioned")
                        .WithMany("ServiceProducers")
                        .HasForeignKey("ServiceVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceProducerAdditionalInformation", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceProducer", "ServiceProducer")
                        .WithMany("AdditionalInformations")
                        .HasForeignKey("ServiceProducerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceProducerOrganization", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Organization", "Organization")
                        .WithMany("ServiceProducerOrganizations")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceProducer", "ServiceProducer")
                        .WithMany("Organizations")
                        .HasForeignKey("ServiceProducerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceRequirement", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "ServiceVersioned")
                        .WithMany("ServiceRequirements")
                        .HasForeignKey("ServiceVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceClass", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceClass", "ServiceClass")
                        .WithMany("ServiceServiceClasses")
                        .HasForeignKey("ServiceClassId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "ServiceVersioned")
                        .WithMany("ServiceServiceClasses")
                        .HasForeignKey("ServiceVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannel", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChargeType", "ChargeType")
                        .WithMany()
                        .HasForeignKey("ChargeTypeId");

                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("ServiceServiceChannels")
                        .HasForeignKey("ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Service", "Service")
                        .WithMany("ServiceServiceChannels")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannelAddress", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.AddressCharacter", "Character")
                        .WithMany()
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceServiceChannel", "ServiceServiceChannel")
                        .WithMany("ServiceServiceChannelAddresses")
                        .HasForeignKey("ServiceId", "ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannelDescription", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.DescriptionType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceServiceChannel", "ServiceServiceChannel")
                        .WithMany("ServiceServiceChannelDescriptions")
                        .HasForeignKey("ServiceId", "ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannelDigitalAuthorization", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.DigitalAuthorization", "DigitalAuthorization")
                        .WithMany("ServiceServiceChannelDigitalAuthorizations")
                        .HasForeignKey("DigitalAuthorizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceServiceChannel", "ServiceServiceChannel")
                        .WithMany("ServiceServiceChannelDigitalAuthorizations")
                        .HasForeignKey("ServiceId", "ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannelEmail", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Email", "Email")
                        .WithMany("ServiceServiceChannelEmails")
                        .HasForeignKey("EmailId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceServiceChannel", "ServiceServiceChannel")
                        .WithMany("ServiceServiceChannelEmails")
                        .HasForeignKey("ServiceId", "ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannelExtraType", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ExtraSubType", "ExtraSubType")
                        .WithMany()
                        .HasForeignKey("ExtraSubTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceServiceChannel", "ServiceServiceChannel")
                        .WithMany("ServiceServiceChannelExtraTypes")
                        .HasForeignKey("ServiceId", "ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannelExtraTypeDescription", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceServiceChannelExtraType", "ServiceServiceChannelExtraType")
                        .WithMany("ServiceServiceChannelExtraTypeDescriptions")
                        .HasForeignKey("ServiceServiceChannelExtraTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannelPhone", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Phone", "Phone")
                        .WithMany("ServiceServiceChannelPhones")
                        .HasForeignKey("PhoneId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceServiceChannel", "ServiceServiceChannel")
                        .WithMany("ServiceServiceChannelPhones")
                        .HasForeignKey("ServiceId", "ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannelServiceHours", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceHours", "ServiceHours")
                        .WithMany("ServiceServiceChannelServiceHours")
                        .HasForeignKey("ServiceHoursId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceServiceChannel", "ServiceServiceChannel")
                        .WithMany("ServiceServiceChannelServiceHours")
                        .HasForeignKey("ServiceId", "ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannelWebPage", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.WebPage", "WebPage")
                        .WithMany("ServiceServiceChannelWebPages")
                        .HasForeignKey("WebPageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceServiceChannel", "ServiceServiceChannel")
                        .WithMany("ServiceServiceChannelWebPages")
                        .HasForeignKey("ServiceId", "ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceTargetGroup", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "ServiceVersioned")
                        .WithMany("ServiceTargetGroups")
                        .HasForeignKey("ServiceVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.TargetGroup", "TargetGroup")
                        .WithMany("ServiceTargetGroups")
                        .HasForeignKey("TargetGroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceTranslationOrder", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "ServiceVersioned")
                        .WithMany("ServiceTranslationOrders")
                        .HasForeignKey("ServiceVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.TranslationOrder", "TranslationOrder")
                        .WithMany("ServiceTranslationOrders")
                        .HasForeignKey("TranslationOrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceVersioned", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AreaInformationType", "AreaInformationType")
                        .WithMany()
                        .HasForeignKey("AreaInformationTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChargeType", "ChargeType")
                        .WithMany()
                        .HasForeignKey("ChargeTypeId");

                    b.HasOne("PTV.Database.Model.Models.ServiceFundingType", "FundingType")
                        .WithMany()
                        .HasForeignKey("FundingTypeId");

                    b.HasOne("PTV.Database.Model.Models.Organization", "Organization")
                        .WithMany("OrganizationServicesVersioned")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "PublishingStatus")
                        .WithMany()
                        .HasForeignKey("PublishingStatusId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", "StatutoryServiceGeneralDescription")
                        .WithMany()
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId");

                    b.HasOne("PTV.Database.Model.Models.ServiceType", "Type")
                        .WithMany("Service")
                        .HasForeignKey("TypeId");

                    b.HasOne("PTV.Database.Model.Models.Service", "UnificRoot")
                        .WithMany("Versions")
                        .HasForeignKey("UnificRootId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Versioning", "Versioning")
                        .WithMany()
                        .HasForeignKey("VersioningId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceWebPage", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "ServiceVersioned")
                        .WithMany("ServiceWebPages")
                        .HasForeignKey("ServiceVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.WebPage", "WebPage")
                        .WithMany("ServiceWebPages")
                        .HasForeignKey("WebPageId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceDescription", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescriptionVersioned", "StatutoryServiceGeneralDescriptionVersioned")
                        .WithMany("Descriptions")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.DescriptionType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceGeneralDescriptionVersioned", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChargeType", "ChargeType")
                        .WithMany()
                        .HasForeignKey("ChargeTypeId");

                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "PublishingStatus")
                        .WithMany()
                        .HasForeignKey("PublishingStatusId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", "UnificRoot")
                        .WithMany("Versions")
                        .HasForeignKey("UnificRootId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Versioning", "Versioning")
                        .WithMany()
                        .HasForeignKey("VersioningId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceIndustrialClass", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.IndustrialClass", "IndustrialClass")
                        .WithMany()
                        .HasForeignKey("IndustrialClassId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescriptionVersioned", "StatutoryServiceGeneralDescriptionVersioned")
                        .WithMany("IndustrialClasses")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceLanguage", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescriptionVersioned", "StatutoryServiceGeneralDescriptionVersioned")
                        .WithMany("Languages")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceLaw", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Law", "Law")
                        .WithMany("StatutoryServiceLaws")
                        .HasForeignKey("LawId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescriptionVersioned", "StatutoryServiceGeneralDescriptionVersioned")
                        .WithMany("StatutoryServiceLaws")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceLifeEvent", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.LifeEvent", "LifeEvent")
                        .WithMany("StatutoryServiceLifeEvents")
                        .HasForeignKey("LifeEventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescriptionVersioned", "StatutoryServiceGeneralDescriptionVersioned")
                        .WithMany("LifeEvents")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescriptionVersioned", "StatutoryServiceGeneralDescriptionVersioned")
                        .WithMany("Names")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.NameType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceOntologyTerm", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.OntologyTerm", "OntologyTerm")
                        .WithMany("StatutoryServiceOntologyTerms")
                        .HasForeignKey("OntologyTermId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescriptionVersioned", "StatutoryServiceGeneralDescriptionVersioned")
                        .WithMany("OntologyTerms")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceRequirement", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescriptionVersioned", "StatutoryServiceGeneralDescriptionVersioned")
                        .WithMany("StatutoryServiceRequirements")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceServiceClass", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceClass", "ServiceClass")
                        .WithMany("StatutoryServiceServiceClasses")
                        .HasForeignKey("ServiceClassId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescriptionVersioned", "StatutoryServiceGeneralDescriptionVersioned")
                        .WithMany("ServiceClasses")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceTargetGroup", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescriptionVersioned", "StatutoryServiceGeneralDescriptionVersioned")
                        .WithMany("TargetGroups")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.TargetGroup", "TargetGroup")
                        .WithMany("StatutoryServiceTargetGroups")
                        .HasForeignKey("TargetGroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StreetName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AddressStreet", "AddressStreet")
                        .WithMany("StreetNames")
                        .HasForeignKey("AddressStreetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TargetGroup", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.TargetGroup", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TargetGroupName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.TargetGroup", "TargetGroup")
                        .WithMany("Names")
                        .HasForeignKey("TargetGroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TranslationOrder", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "SourceLanguage")
                        .WithMany()
                        .HasForeignKey("SourceLanguageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "TargetLanguage")
                        .WithMany()
                        .HasForeignKey("TargetLanguageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.TranslationCompany", "TranslationCompany")
                        .WithMany("TranslationOrders")
                        .HasForeignKey("TranslationCompanyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TranslationOrderState", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.TranslationOrder", "TranslationOrder")
                        .WithMany("TranslationOrderStates")
                        .HasForeignKey("TranslationOrderId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.TranslationStateType", "TranslationStateType")
                        .WithMany("TranslationOrderStates")
                        .HasForeignKey("TranslationStateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TranslationStateTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.TranslationStateType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.UserOrganization", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Organization", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Versioning", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Versioning", "PreviousVersion")
                        .WithMany("ChildrenVersions")
                        .HasForeignKey("PreviousVersionId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.WebPage", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.WebpageChannel", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("WebpageChannels")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.WebpageChannelUrl", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.WebpageChannel", "WebpageChannel")
                        .WithMany("LocalizedUrls")
                        .HasForeignKey("WebpageChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.WebPageTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.WebPageType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
