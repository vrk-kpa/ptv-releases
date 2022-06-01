﻿/**
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
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PTV.Database.DataAccess.ApplicationDbContext;

namespace PTV.Database.Migrations.Migrations.Release_2_4
{
    [DbContext(typeof(PtvDbContext))]
    [Migration("20190315085714_DownloadAndImportCoordinates")]
    partial class DownloadAndImportCoordinates
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("Npgsql:PostgresExtension:postgis", ",,")
                .HasAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

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
                        .HasName("IX_AccRigNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_AccRigNam_TypeId");

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
                        .HasName("IX_AccRigTyp_Id");

                    b.ToTable("AccessRightType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AccessRightsOperationsUI", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AllowedAllOrganizations");

                    b.Property<Guid?>("OrganizationId");

                    b.Property<string>("Permission");

                    b.Property<string>("Role");

                    b.Property<long>("RulesAll");

                    b.Property<long>("RulesOwn");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_AccRigOpeUI_Id");

                    b.HasIndex("OrganizationId")
                        .HasName("IX_AccRigOpeUI_OrganizationId");

                    b.ToTable("AccessRightsOperationsUI");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AccessibilityClassification", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AccessibilityClassificationLevelTypeId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<int?>("OrderNumber");

                    b.Property<string>("Url")
                        .HasMaxLength(500);

                    b.Property<Guid?>("WcagLevelTypeId");

                    b.HasKey("Id");

                    b.HasIndex("AccessibilityClassificationLevelTypeId")
                        .HasName("IX_AccCla_AccessibilityClassificationLevelTypeId");

                    b.HasIndex("Id")
                        .HasName("IX_AccCla_Id");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_AccCla_LocalizationId");

                    b.HasIndex("WcagLevelTypeId")
                        .HasName("IX_AccCla_WcagLevelTypeId");

                    b.ToTable("AccessibilityClassification");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AccessibilityClassificationLevelType", b =>
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
                        .HasName("IX_AccClaLevTyp_Id");

                    b.ToTable("AccessibilityClassificationLevelType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AccessibilityClassificationLevelTypeName", b =>
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
                        .HasName("IX_AccClaLevTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_AccClaLevTypNam_TypeId");

                    b.ToTable("AccessibilityClassificationLevelTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AccessibilityRegister", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AddressId");

                    b.Property<Guid>("AddressLanguageId");

                    b.Property<string>("ContactEmail");

                    b.Property<string>("ContactPhone");

                    b.Property<string>("ContactUrl");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("IsValid");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("ServiceChannelId");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.HasIndex("AddressId")
                        .HasName("IX_AccReg_AddressId");

                    b.HasIndex("AddressLanguageId")
                        .HasName("IX_AccReg_AddressLanguageId");

                    b.HasIndex("Id")
                        .HasName("IX_AccReg_Id");

                    b.HasIndex("ServiceChannelId")
                        .IsUnique()
                        .HasName("IX_AccReg_ServiceChannelId");

                    b.ToTable("AccessibilityRegister");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AccessibilityRegisterEntrance", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AccessibilityRegisterId");

                    b.Property<Guid?>("AddressId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<int>("EntranceId");

                    b.Property<bool>("IsMain");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("AccessibilityRegisterId")
                        .HasName("IX_AccRegEnt_AccessibilityRegisterId");

                    b.HasIndex("AddressId")
                        .HasName("IX_AccRegEnt_AddressId");

                    b.HasIndex("Id")
                        .HasName("IX_AccRegEnt_Id");

                    b.ToTable("AccessibilityRegisterEntrance");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AccessibilityRegisterEntranceName", b =>
                {
                    b.Property<Guid>("AccessibilityRegisterEntranceId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("AccessibilityRegisterEntranceId", "LocalizationId");

                    b.HasIndex("AccessibilityRegisterEntranceId")
                        .HasName("IX_AccRegEntNam_AccessibilityRegisterEntranceId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_AccRegEntNam_LocalizationId");

                    b.ToTable("AccessibilityRegisterEntranceName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AccessibilityRegisterGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AccessibilityRegisterEntranceId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("AccessibilityRegisterEntranceId")
                        .HasName("IX_AccRegGro_AccessibilityRegisterEntranceId");

                    b.HasIndex("Id")
                        .HasName("IX_AccRegGro_Id");

                    b.ToTable("AccessibilityRegisterGroup");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AccessibilityRegisterGroupValue", b =>
                {
                    b.Property<Guid>("AccessibilityRegisterGroupId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Value");

                    b.HasKey("AccessibilityRegisterGroupId", "LocalizationId");

                    b.HasIndex("AccessibilityRegisterGroupId")
                        .HasName("IX_AccRegGroVal_AccessibilityRegisterGroupId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_AccRegGroVal_LocalizationId");

                    b.ToTable("AccessibilityRegisterGroupValue");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AccessibilityRegisterSentence", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.Property<Guid>("SentenceGroupId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_AccRegSen_Id");

                    b.HasIndex("SentenceGroupId")
                        .HasName("IX_AccRegSen_SentenceGroupId");

                    b.ToTable("AccessibilityRegisterSentence");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AccessibilityRegisterSentenceValue", b =>
                {
                    b.Property<Guid>("AccessibilityRegisterSentenceId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Value");

                    b.HasKey("AccessibilityRegisterSentenceId", "LocalizationId");

                    b.HasIndex("AccessibilityRegisterSentenceId")
                        .HasName("IX_AccRegSenVal_AccessibilityRegisterSentenceId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_AccRegSenVal_LocalizationId");

                    b.ToTable("AccessibilityRegisterSentenceValue");
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

                    b.Property<Guid>("UniqueId");

                    b.HasKey("Id");

                    b.HasIndex("CountryId")
                        .HasName("IX_Add_CountryId");

                    b.HasIndex("Id")
                        .HasName("IX_Add_Id");

                    b.HasIndex("TypeId")
                        .HasName("IX_Add_TypeId");

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
                        .HasName("IX_AddAddInf_AddressId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_AddAddInf_LocalizationId");

                    b.ToTable("AddressAdditionalInformation");
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
                        .HasName("IX_AddCha_Id");

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
                        .HasName("IX_AddChaNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_AddChaNam_TypeId");

                    b.ToTable("AddressCharacterName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AddressExtraType", b =>
                {
                    b.Property<Guid>("AddressId");

                    b.Property<Guid>("ExtraTypeId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("AddressId", "ExtraTypeId");

                    b.HasIndex("AddressId")
                        .HasName("IX_AddExtTyp_AddressId");

                    b.HasIndex("ExtraTypeId")
                        .HasName("IX_AddExtTyp_ExtraTypeId");

                    b.ToTable("AddressExtraType");
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
                        .HasName("IX_AddFor_AddressId");

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
                        .HasName("IX_AddForTexNam_AddressForeignId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_AddForTexNam_LocalizationId");

                    b.ToTable("AddressForeignTextName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AddressOther", b =>
                {
                    b.Property<Guid>("AddressId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid?>("PostalCodeId");

                    b.HasKey("AddressId");

                    b.HasIndex("AddressId")
                        .HasName("IX_AddOth_AddressId");

                    b.HasIndex("PostalCodeId")
                        .HasName("IX_AddOth_PostalCodeId");

                    b.ToTable("AddressOther");
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
                        .HasName("IX_AddPosOffBox_AddressId");

                    b.HasIndex("MunicipalityId")
                        .HasName("IX_AddPosOffBox_MunicipalityId");

                    b.HasIndex("PostalCodeId")
                        .HasName("IX_AddPosOffBox_PostalCodeId");

                    b.ToTable("AddressPostOfficeBox");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AddressReceiver", b =>
                {
                    b.Property<Guid>("AddressId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Receiver")
                        .HasMaxLength(100);

                    b.HasKey("AddressId", "LocalizationId");

                    b.HasIndex("AddressId")
                        .HasName("IX_AddRec_AddressId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_AddRec_LocalizationId");

                    b.ToTable("AddressReceiver");
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
                        .HasName("IX_AddStr_AddressId");

                    b.HasIndex("MunicipalityId")
                        .HasName("IX_AddStr_MunicipalityId");

                    b.HasIndex("PostalCodeId")
                        .HasName("IX_AddStr_PostalCodeId");

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
                        .HasName("IX_AddTyp_Id");

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
                        .HasName("IX_AddTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_AddTypNam_TypeId");

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
                        .HasName("IX_AppEnvDat_Id");

                    b.HasIndex("TypeId")
                        .HasName("IX_AppEnvDat_TypeId");

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
                        .HasName("IX_AppEnvDatTyp_Id");

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
                        .HasName("IX_AppEnvDatTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_AppEnvDatTypNam_TypeId");

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
                        .HasName("IX_Are_AreaTypeId");

                    b.HasIndex("Id")
                        .HasName("IX_Are_Id");

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
                        .HasName("IX_AreInfTyp_Id");

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
                        .HasName("IX_AreInfTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_AreInfTypNam_TypeId");

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
                        .HasName("IX_AreMun_AreaId");

                    b.HasIndex("MunicipalityId")
                        .HasName("IX_AreMun_MunicipalityId");

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
                        .HasName("IX_AreNam_AreaId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_AreNam_LocalizationId");

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
                        .HasName("IX_AreTyp_Id");

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
                        .HasName("IX_AreTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_AreTypNam_TypeId");

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
                        .HasName("IX_Att_Id");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_Att_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_Att_TypeId");

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
                        .HasName("IX_AttTyp_Id");

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
                        .HasName("IX_AttTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_AttTypNam_TypeId");

                    b.ToTable("AttachmentTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AuthorizationEntryPoint", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_AutEntPoi_Id");

                    b.ToTable("AuthorizationEntryPoint");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Base.EFMigrationsHistory", b =>
                {
                    b.Property<string>("MigrationId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AppVersion");

                    b.Property<DateTime?>("Applied");

                    b.Property<string>("ProductVersion");

                    b.HasKey("MigrationId");

                    b.HasIndex("MigrationId")
                        .HasName("IX___EFMigrationsHistory_MigrationId");

                    b.ToTable("__EFMigrationsHistory");
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
                        .HasName("IX_BugRep_Id");

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
                        .HasName("IX_Bus_Id");

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
                        .HasName("IX_CFGReqFil_Id");

                    b.ToTable("CFGRequestFilter");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ClsAddressPoint", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AddressId");

                    b.Property<Guid>("AddressStreetId");

                    b.Property<Guid?>("AddressStreetNumberId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("IsValid");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("MunicipalityId");

                    b.Property<Guid>("PostalCodeId");

                    b.Property<string>("StreetNumber")
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.HasIndex("AddressId")
                        .HasName("IX_ClsAddPoi_AddressId");

                    b.HasIndex("AddressStreetId")
                        .HasName("IX_ClsAddPoi_AddressStreetId");

                    b.HasIndex("AddressStreetNumberId")
                        .HasName("IX_ClsAddPoi_AddressStreetNumberId");

                    b.HasIndex("Id")
                        .HasName("IX_ClsAddPoi_Id");

                    b.HasIndex("MunicipalityId")
                        .HasName("IX_ClsAddPoi_MunicipalityId");

                    b.HasIndex("PostalCodeId")
                        .HasName("IX_ClsAddPoi_PostalCodeId");

                    b.ToTable("ClsAddressPoint");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ClsAddressStreet", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("IsValid");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("MunicipalityId");

                    b.Property<bool>("NonCls");

                    b.Property<Guid>("UpdateIdentifier");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_ClsAddStr_Id");

                    b.HasIndex("MunicipalityId")
                        .HasName("IX_ClsAddStr_MunicipalityId");

                    b.ToTable("ClsAddressStreet");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ClsAddressStreetName", b =>
                {
                    b.Property<Guid>("ClsAddressStreetId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<string>("Name3")
                        .HasMaxLength(3);

                    b.Property<bool>("NonCls");

                    b.Property<Guid>("UpdateIdentifier");

                    b.HasKey("ClsAddressStreetId", "LocalizationId");

                    b.HasIndex("ClsAddressStreetId")
                        .HasName("IX_ClsAddStrNam_ClsAddressStreetId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_ClsAddStrNam_LocalizationId");

                    b.HasIndex("Name3")
                        .HasName("IX_ClsAddStrNam_Name3");

                    b.ToTable("ClsAddressStreetName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ClsAddressStreetNumber", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ClsAddressStreetId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("EndCharacter");

                    b.Property<string>("EndCharacterEnd");

                    b.Property<int>("EndNumber");

                    b.Property<int>("EndNumberEnd");

                    b.Property<bool>("IsEven");

                    b.Property<bool>("IsValid");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<bool>("NonCls");

                    b.Property<Guid>("PostalCodeId");

                    b.Property<string>("StartCharacter");

                    b.Property<string>("StartCharacterEnd");

                    b.Property<int>("StartNumber");

                    b.Property<int>("StartNumberEnd");

                    b.Property<Guid>("UpdateIdentifier");

                    b.HasKey("Id");

                    b.HasIndex("ClsAddressStreetId")
                        .HasName("IX_ClsAddStrNum_ClsAddressStreetId");

                    b.HasIndex("Id")
                        .HasName("IX_ClsAddStrNum_Id");

                    b.HasIndex("PostalCodeId")
                        .HasName("IX_ClsAddStrNum_PostalCodeId");

                    b.ToTable("ClsAddressStreetNumber");
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
                        .HasName("IX_Coo_AddressId");

                    b.HasIndex("Id")
                        .HasName("IX_Coo_Id");

                    b.HasIndex("TypeId")
                        .HasName("IX_Coo_TypeId");

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
                        .HasName("IX_CooTyp_Id");

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
                        .HasName("IX_CooTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_CooTypNam_TypeId");

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
                        .HasName("IX_Cou_Id");

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
                        .HasName("IX_CouNam_CountryId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_CouNam_LocalizationId");

                    b.ToTable("CountryName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.DailyOpeningTime", b =>
                {
                    b.Property<Guid>("OpeningHourId");

                    b.Property<int>("DayFrom");

                    b.Property<int>("OrderNumber");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<int?>("DayTo");

                    b.Property<TimeSpan>("From");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<TimeSpan>("To");

                    b.HasKey("OpeningHourId", "DayFrom", "OrderNumber");

                    b.HasIndex("OpeningHourId", "DayFrom", "OrderNumber")
                        .HasName("IX_DaiOpeTim_OpeningHourId_DayFrom_OrderNumber");

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
                        .HasName("IX_DesTyp_Id");

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
                        .HasName("IX_DesTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_DesTypNam_TypeId");

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
                        .HasName("IX_DiaCod_CountryId");

                    b.HasIndex("Id")
                        .HasName("IX_DiaCod_Id");

                    b.ToTable("DialCode");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.DigitalAuthorization", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("IsValid");

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
                        .HasName("IX_DigAut_Id");

                    b.HasIndex("ParentId")
                        .HasName("IX_DigAut_ParentId");

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
                        .HasName("IX_DigAutNam_DigitalAuthorizationId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_DigAutNam_LocalizationId");

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
                        .HasName("IX_EleCha_Id");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasName("IX_EleCha_ServiceChannelVersionedId");

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
                        .HasName("IX_EleChaUrl_ElectronicChannelId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_EleChaUrl_LocalizationId");

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
                        .HasName("IX_Ema_Id");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_Ema_LocalizationId");

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
                        .HasName("IX_ExaMat_Id");

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
                        .HasName("IX_ExcHouStaTyp_Id");

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
                        .HasName("IX_ExcHouStaTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_ExcHouStaTypNam_TypeId");

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
                        .HasName("IX_ExtSou_Id");

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
                        .HasName("IX_ExtSubTyp_ExtraTypeId");

                    b.HasIndex("Id")
                        .HasName("IX_ExtSubTyp_Id");

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
                        .HasName("IX_ExtSubTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_ExtSubTypNam_TypeId");

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
                        .HasName("IX_ExtTyp_Id");

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
                        .HasName("IX_ExtTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_ExtTypNam_TypeId");

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
                        .HasName("IX_For_Id");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_For_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_For_TypeId");

                    b.ToTable("Form");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.FormState", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("EntityId");

                    b.Property<string>("EntityType");

                    b.Property<string>("FormName");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("State");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_ForSta_Id");

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
                        .HasName("IX_ForTyp_Id");

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
                        .HasName("IX_ForTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_ForTypNam_TypeId");

                    b.ToTable("FormTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionLanguageAvailability", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionVersionedId");

                    b.Property<Guid>("LanguageId");

                    b.Property<DateTime?>("ArchiveAt");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<DateTime?>("PublishAt");

                    b.Property<DateTime?>("Reviewed");

                    b.Property<string>("ReviewedBy");

                    b.Property<DateTime?>("SetForArchived");

                    b.Property<string>("SetForArchivedBy");

                    b.Property<Guid>("StatusId");

                    b.HasKey("StatutoryServiceGeneralDescriptionVersionedId", "LanguageId");

                    b.HasIndex("LanguageId")
                        .HasName("IX_GenDesLanAva_LanguageId");

                    b.HasIndex("StatusId")
                        .HasName("IX_GenDesLanAva_StatusId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasName("IX_GenDesLanAva_StatutoryServiceGeneralDescriptionVersionedId");

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

                    b.HasKey("StatutoryServiceGeneralDescriptionId", "ServiceChannelId");

                    b.HasIndex("ChargeTypeId")
                        .HasName("IX_GenDesSerCha_ChargeTypeId");

                    b.HasIndex("ServiceChannelId")
                        .HasName("IX_GenDesSerCha_ServiceChannelId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId")
                        .HasName("IX_GenDesSerCha_StatutoryServiceGeneralDescriptionId");

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

                    b.HasKey("TypeId", "LocalizationId", "ServiceChannelId", "StatutoryServiceGeneralDescriptionId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_GenDesSerChaDes_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_GenDesSerChaDes_TypeId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId", "ServiceChannelId")
                        .HasName("IX_GenDesSerChaDes_StatutoryServiceGeneralDescriptionId_ServiceChannelId");

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

                    b.HasKey("DigitalAuthorizationId", "StatutoryServiceGeneralDescriptionId", "ServiceChannelId");

                    b.HasIndex("DigitalAuthorizationId")
                        .HasName("IX_GenDesSerChaDigAut_DigitalAuthorizationId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId", "ServiceChannelId")
                        .HasName("IX_GenDesSerChaDigAut_StatutoryServiceGeneralDescriptionId_ServiceChannelId");

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

                    b.HasKey("Id");

                    b.HasIndex("ExtraSubTypeId")
                        .HasName("IX_GenDesSerChaExtTyp_ExtraSubTypeId");

                    b.HasIndex("Id")
                        .HasName("IX_GenDesSerChaExtTyp_Id");

                    b.HasIndex("ServiceChannelId")
                        .HasName("IX_GenDesSerChaExtTyp_ServiceChannelId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId")
                        .HasName("IX_GenDesSerChaExtTyp_StatutoryServiceGeneralDescriptionId");

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

                    b.HasKey("LocalizationId", "GeneralDescriptionServiceChannelExtraTypeId");

                    b.HasIndex("GeneralDescriptionServiceChannelExtraTypeId")
                        .HasName("IX_GenDesSerChaExtTypDes_GeneralDescriptionServiceChannelExtraTypeId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_GenDesSerChaExtTypDes_LocalizationId");

                    b.ToTable("GeneralDescriptionServiceChannelExtraTypeDescription");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionTranslationOrder", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionId");

                    b.Property<Guid>("TranslationOrderId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("StatutoryServiceGeneralDescriptionIdentifier");

                    b.HasKey("StatutoryServiceGeneralDescriptionId", "TranslationOrderId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId")
                        .HasName("IX_GenDesTraOrd_StatutoryServiceGeneralDescriptionId");

                    b.HasIndex("TranslationOrderId")
                        .HasName("IX_GenDesTraOrd_TranslationOrderId");

                    b.ToTable("GeneralDescriptionTranslationOrder");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionType", b =>
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
                        .HasName("IX_GenDesTyp_Id");

                    b.ToTable("GeneralDescriptionType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionTypeName", b =>
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
                        .HasName("IX_GenDesTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_GenDesTypNam_TypeId");

                    b.ToTable("GeneralDescriptionTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.IndustrialClass", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("IsValid");

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
                        .HasName("IX_IndCla_Id");

                    b.HasIndex("ParentId")
                        .HasName("IX_IndCla_ParentId");

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
                        .HasName("IX_IndClaNam_IndustrialClassId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_IndClaNam_LocalizationId");

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
                        .HasName("IX_Key_Id");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_Key_LocalizationId");

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

                    b.Property<bool>("IsForTranslation");

                    b.Property<bool>("IsValid");

                    b.Property<string>("LanguageStateCulture");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_Lan_Id");

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
                        .HasName("IX_LanNam_LanguageId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_LanNam_LocalizationId");

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
                        .HasName("IX_Law_Id");

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
                        .HasName("IX_LawNam_LawId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_LawNam_LocalizationId");

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
                        .HasName("IX_LawWebPag_LawId");

                    b.HasIndex("WebPageId")
                        .HasName("IX_LawWebPag_WebPageId");

                    b.ToTable("LawWebPage");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.LifeEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("IsValid");

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
                        .HasName("IX_LifEve_Id");

                    b.HasIndex("ParentId")
                        .HasName("IX_LifEve_ParentId");

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
                        .HasName("IX_LifEveNam_LifeEventId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_LifEveNam_LocalizationId");

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
                        .HasName("IX_Loc_Id");

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
                        .HasName("IX_Mun_Id");

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
                        .HasName("IX_MunNam_LocalizationId");

                    b.HasIndex("MunicipalityId")
                        .HasName("IX_MunNam_MunicipalityId");

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
                        .HasName("IX_NamTyp_Id");

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
                        .HasName("IX_NamTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_NamTypNam_TypeId");

                    b.ToTable("NameTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.NotificationServiceServiceChannel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ChannelId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("OrganizationId");

                    b.Property<Guid>("ServiceId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_NotSerSerCha_Id");

                    b.ToTable("NotificationServiceServiceChannel");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.NotificationServiceServiceChannelFilter", b =>
                {
                    b.Property<Guid>("NotificationServiceServiceChannelId");

                    b.Property<Guid>("UserId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("NotificationServiceServiceChannelId", "UserId");

                    b.HasIndex("NotificationServiceServiceChannelId", "UserId")
                        .HasName("IX_NotSerSerChaFil_NotificationServiceServiceChannelId_UserId");

                    b.ToTable("NotificationServiceServiceChannelFilter");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OntologyTerm", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("IsValid");

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
                        .HasName("IX_OntTer_Id");

                    b.HasIndex("ParentUri")
                        .HasName("IX_OntTer_ParentUri");

                    b.HasIndex("Uri")
                        .HasName("IX_OntTer_Uri");

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
                        .HasName("IX_OntTerDes_LocalizationId");

                    b.HasIndex("OntologyTermId")
                        .HasName("IX_OntTerDes_OntologyTermId");

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
                        .HasName("IX_OntTerExaMat_ExactMatchId");

                    b.HasIndex("OntologyTermId")
                        .HasName("IX_OntTerExaMat_OntologyTermId");

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
                        .HasName("IX_OntTerNam_LocalizationId");

                    b.HasIndex("OntologyTermId")
                        .HasName("IX_OntTerNam_OntologyTermId");

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
                        .HasName("IX_OntTerPar_ChildId");

                    b.HasIndex("ParentId")
                        .HasName("IX_OntTerPar_ParentId");

                    b.ToTable("OntologyTermParent");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Organization", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_Org_Id");

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

                    b.Property<int?>("OrderNumber");

                    b.HasKey("OrganizationVersionedId", "AddressId", "CharacterId");

                    b.HasIndex("AddressId")
                        .HasName("IX_OrgAdd_AddressId");

                    b.HasIndex("CharacterId")
                        .HasName("IX_OrgAdd_CharacterId");

                    b.HasIndex("OrganizationVersionedId")
                        .HasName("IX_OrgAdd_OrganizationVersionedId");

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
                        .HasName("IX_OrgAre_AreaId");

                    b.HasIndex("OrganizationVersionedId")
                        .HasName("IX_OrgAre_OrganizationVersionedId");

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
                        .HasName("IX_OrgAreMun_MunicipalityId");

                    b.HasIndex("OrganizationVersionedId")
                        .HasName("IX_OrgAreMun_OrganizationVersionedId");

                    b.ToTable("OrganizationAreaMunicipality");
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
                        .HasName("IX_OrgDes_LocalizationId");

                    b.HasIndex("OrganizationVersionedId")
                        .HasName("IX_OrgDes_OrganizationVersionedId");

                    b.HasIndex("TypeId")
                        .HasName("IX_OrgDes_TypeId");

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
                        .HasName("IX_OrgDisNamTyp_DisplayNameTypeId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_OrgDisNamTyp_LocalizationId");

                    b.HasIndex("OrganizationVersionedId")
                        .HasName("IX_OrgDisNamTyp_OrganizationVersionedId");

                    b.ToTable("OrganizationDisplayNameType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationEInvoicing", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("ElectronicInvoicingAddress")
                        .HasMaxLength(110);

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("OperatorCode")
                        .HasMaxLength(110);

                    b.Property<int?>("OrderNumber");

                    b.Property<Guid>("OrganizationVersionedId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_OrgEInv_Id");

                    b.HasIndex("OrganizationVersionedId")
                        .HasName("IX_OrgEInv_OrganizationVersionedId");

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
                        .HasName("IX_OrgEInvAddInf_LocalizationId");

                    b.HasIndex("OrganizationEInvoicingId")
                        .HasName("IX_OrgEInvAddInf_OrganizationEInvoicingId");

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

                    b.Property<int?>("OrderNumber");

                    b.HasKey("EmailId", "OrganizationVersionedId");

                    b.HasIndex("EmailId")
                        .HasName("IX_OrgEma_EmailId");

                    b.HasIndex("OrganizationVersionedId")
                        .HasName("IX_OrgEma_OrganizationVersionedId");

                    b.ToTable("OrganizationEmail");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationFilter", b =>
                {
                    b.Property<Guid>("FilterId");

                    b.Property<Guid>("OrganizationId");

                    b.HasKey("FilterId", "OrganizationId");

                    b.HasIndex("FilterId")
                        .HasName("IX_OrgFil_FilterId");

                    b.HasIndex("OrganizationId")
                        .HasName("IX_OrgFil_OrganizationId");

                    b.ToTable("OrganizationFilter");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationLanguageAvailability", b =>
                {
                    b.Property<Guid>("OrganizationVersionedId");

                    b.Property<Guid>("LanguageId");

                    b.Property<DateTime?>("ArchiveAt");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<DateTime?>("PublishAt");

                    b.Property<DateTime?>("Reviewed");

                    b.Property<string>("ReviewedBy");

                    b.Property<DateTime?>("SetForArchived");

                    b.Property<string>("SetForArchivedBy");

                    b.Property<Guid>("StatusId");

                    b.HasKey("OrganizationVersionedId", "LanguageId");

                    b.HasIndex("LanguageId")
                        .HasName("IX_OrgLanAva_LanguageId");

                    b.HasIndex("OrganizationVersionedId")
                        .HasName("IX_OrgLanAva_OrganizationVersionedId");

                    b.HasIndex("StatusId")
                        .HasName("IX_OrgLanAva_StatusId");

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
                        .HasName("IX_OrgNam_LocalizationId");

                    b.HasIndex("OrganizationVersionedId")
                        .HasName("IX_OrgNam_OrganizationVersionedId");

                    b.HasIndex("TypeId")
                        .HasName("IX_OrgNam_TypeId");

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

                    b.Property<int?>("OrderNumber");

                    b.HasKey("PhoneId", "OrganizationVersionedId");

                    b.HasIndex("OrganizationVersionedId")
                        .HasName("IX_OrgPho_OrganizationVersionedId");

                    b.HasIndex("PhoneId")
                        .HasName("IX_OrgPho_PhoneId");

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
                        .HasName("IX_OrgSer_OrganizationId");

                    b.HasIndex("ServiceVersionedId")
                        .HasName("IX_OrgSer_ServiceVersionedId");

                    b.ToTable("OrganizationService");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("IsValid");

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
                        .HasName("IX_OrgTyp_Id");

                    b.HasIndex("ParentId")
                        .HasName("IX_OrgTyp_ParentId");

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
                        .HasName("IX_OrgTypNam_LocalizationId");

                    b.HasIndex("OrganizationTypeId")
                        .HasName("IX_OrgTypNam_OrganizationTypeId");

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

                    b.Property<Guid?>("ResponsibleOrganizationRegionId");

                    b.Property<Guid?>("TypeId");

                    b.Property<Guid>("UnificRootId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.Property<Guid?>("VersioningId");

                    b.HasKey("Id");

                    b.HasIndex("AreaInformationTypeId")
                        .HasName("IX_OrgVer_AreaInformationTypeId");

                    b.HasIndex("BusinessId")
                        .HasName("IX_OrgVer_BusinessId");

                    b.HasIndex("Id")
                        .HasName("IX_OrgVer_Id");

                    b.HasIndex("MunicipalityId")
                        .HasName("IX_OrgVer_MunicipalityId");

                    b.HasIndex("Oid")
                        .HasName("IX_OrgVer_Oid");

                    b.HasIndex("ParentId")
                        .HasName("IX_OrgVer_ParentId");

                    b.HasIndex("PublishingStatusId")
                        .HasName("IX_OrgVer_PublishingStatusId");

                    b.HasIndex("ResponsibleOrganizationRegionId")
                        .HasName("IX_OrgVer_ResponsibleOrganizationRegionId");

                    b.HasIndex("TypeId")
                        .HasName("IX_OrgVer_TypeId");

                    b.HasIndex("UnificRootId")
                        .HasName("IX_OrgVer_UnificRootId");

                    b.HasIndex("VersioningId")
                        .HasName("IX_OrgVer_VersioningId");

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

                    b.Property<int?>("OrderNumber");

                    b.HasKey("OrganizationVersionedId", "WebPageId", "TypeId");

                    b.HasIndex("OrganizationVersionedId")
                        .HasName("IX_OrgWebPag_OrganizationVersionedId");

                    b.HasIndex("TypeId")
                        .HasName("IX_OrgWebPag_TypeId");

                    b.HasIndex("WebPageId")
                        .HasName("IX_OrgWebPag_WebPageId");

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
                        .HasName("IX_Pho_ChargeTypeId");

                    b.HasIndex("Id")
                        .HasName("IX_Pho_Id");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_Pho_LocalizationId");

                    b.HasIndex("PrefixNumberId")
                        .HasName("IX_Pho_PrefixNumberId");

                    b.HasIndex("TypeId")
                        .HasName("IX_Pho_TypeId");

                    b.ToTable("Phone");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PhoneExtraType", b =>
                {
                    b.Property<Guid>("PhoneId");

                    b.Property<Guid>("ExtraTypeId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("PhoneId", "ExtraTypeId");

                    b.HasIndex("ExtraTypeId")
                        .HasName("IX_PhoExtTyp_ExtraTypeId");

                    b.HasIndex("PhoneId")
                        .HasName("IX_PhoExtTyp_PhoneId");

                    b.ToTable("PhoneExtraType");
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
                        .HasName("IX_PhoNumTyp_Id");

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
                        .HasName("IX_PhoNumTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_PhoNumTypNam_TypeId");

                    b.ToTable("PhoneNumberTypeName");
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
                        .HasName("IX_PosOffBoxNam_AddressPostOfficeBoxId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_PosOffBoxNam_LocalizationId");

                    b.ToTable("PostOfficeBoxName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PostalCode", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Polygon>("Border");

                    b.Property<Point>("CenterCoordinate");

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
                        .HasName("IX_PosCod_Id");

                    b.HasIndex("MunicipalityId")
                        .HasName("IX_PosCod_MunicipalityId");

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
                        .HasName("IX_PosCodNam_LocalizationId");

                    b.HasIndex("PostalCodeId")
                        .HasName("IX_PosCodNam_PostalCodeId");

                    b.ToTable("PostalCodeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PrintableFormChannel", b =>
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
                        .HasName("IX_PriForCha_Id");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasName("IX_PriForCha_ServiceChannelVersionedId");

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
                        .HasName("IX_PriForChaIde_LocalizationId");

                    b.HasIndex("PrintableFormChannelId")
                        .HasName("IX_PriForChaIde_PrintableFormChannelId");

                    b.ToTable("PrintableFormChannelIdentifier");
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
                        .HasName("IX_PriForChaUrl_Id");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_PriForChaUrl_LocalizationId");

                    b.HasIndex("PrintableFormChannelId")
                        .HasName("IX_PriForChaUrl_PrintableFormChannelId");

                    b.HasIndex("TypeId")
                        .HasName("IX_PriForChaUrl_TypeId");

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
                        .HasName("IX_PriForChaUrlTyp_Id");

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
                        .HasName("IX_PriForChaUrlTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_PriForChaUrlTypNam_TypeId");

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
                        .HasName("IX_UseAccRig_AccessRightId_UserId");

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
                        .HasName("IX_ProTyp_Id");

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
                        .HasName("IX_ProTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_ProTypNam_TypeId");

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
                        .HasName("IX_PubStaTyp_Id");

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
                        .HasName("IX_PubStaTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_PubStaTypNam_TypeId");

                    b.ToTable("PublishingStatusTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.RestrictedType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("TypeName")
                        .IsRequired();

                    b.Property<Guid>("Value");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_ResTyp_Id");

                    b.ToTable("RestrictedType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.RestrictionFilter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("BlockOtherTypes");

                    b.Property<string>("ColumnName")
                        .IsRequired();

                    b.Property<string>("EntityType")
                        .IsRequired();

                    b.Property<string>("FilterName")
                        .IsRequired();

                    b.Property<int>("FilterType");

                    b.Property<Guid>("RestrictedTypeId");

                    b.HasKey("Id");

                    b.HasIndex("ColumnName")
                        .HasName("IX_ResFil_ColumnName");

                    b.HasIndex("EntityType")
                        .HasName("IX_ResFil_EntityType");

                    b.HasIndex("FilterName")
                        .IsUnique()
                        .HasName("IX_ResFil_FilterName");

                    b.HasIndex("Id")
                        .HasName("IX_ResFil_Id");

                    b.HasIndex("RestrictedTypeId")
                        .HasName("IX_ResFil_RestrictedTypeId");

                    b.ToTable("RestrictionFilter");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.SahaOrganizationInformation", b =>
                {
                    b.Property<Guid>("OrganizationId");

                    b.Property<Guid>("SahaId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<Guid>("SahaParentId");

                    b.HasKey("OrganizationId", "SahaId");

                    b.HasIndex("OrganizationId", "SahaId")
                        .HasName("IX_SahOrgInf_OrganizationId_SahaId");

                    b.ToTable("SahaOrganizationInformation");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Service", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_Ser_Id");

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
                        .HasName("IX_SerAre_AreaId");

                    b.HasIndex("ServiceVersionedId")
                        .HasName("IX_SerAre_ServiceVersionedId");

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
                        .HasName("IX_SerAreMun_MunicipalityId");

                    b.HasIndex("ServiceVersionedId")
                        .HasName("IX_SerAreMun_ServiceVersionedId");

                    b.ToTable("ServiceAreaMunicipality");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_SerCha_Id");

                    b.ToTable("ServiceChannel");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelAccessibilityClassification", b =>
                {
                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<Guid>("AccessibilityClassificationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceChannelVersionedId", "AccessibilityClassificationId");

                    b.HasIndex("AccessibilityClassificationId")
                        .HasName("IX_SerChaAccCla_AccessibilityClassificationId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasName("IX_SerChaAccCla_ServiceChannelVersionedId");

                    b.ToTable("ServiceChannelAccessibilityClassification");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelAddress", b =>
                {
                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<Guid>("AddressId");

                    b.Property<Guid>("CharacterId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceChannelVersionedId", "AddressId");

                    b.HasIndex("AddressId")
                        .HasName("IX_SerChaAdd_AddressId");

                    b.HasIndex("CharacterId")
                        .HasName("IX_SerChaAdd_CharacterId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasName("IX_SerChaAdd_ServiceChannelVersionedId");

                    b.ToTable("ServiceChannelAddress");
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
                        .HasName("IX_SerChaAre_AreaId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasName("IX_SerChaAre_ServiceChannelVersionedId");

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
                        .HasName("IX_SerChaAreMun_MunicipalityId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasName("IX_SerChaAreMun_ServiceChannelVersionedId");

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
                        .HasName("IX_SerChaAtt_AttachmentId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasName("IX_SerChaAtt_ServiceChannelVersionedId");

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
                        .HasName("IX_SerChaConTyp_Id");

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
                        .HasName("IX_SerChaConTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_SerChaConTypNam_TypeId");

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
                        .HasName("IX_SerChaDes_LocalizationId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasName("IX_SerChaDes_ServiceChannelVersionedId");

                    b.HasIndex("TypeId")
                        .HasName("IX_SerChaDes_TypeId");

                    b.ToTable("ServiceChannelDescription");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelDisplayNameType", b =>
                {
                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("DisplayNameTypeId");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceChannelVersionedId", "LocalizationId");

                    b.HasIndex("DisplayNameTypeId")
                        .HasName("IX_SerChaDisNamTyp_DisplayNameTypeId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_SerChaDisNamTyp_LocalizationId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasName("IX_SerChaDisNamTyp_ServiceChannelVersionedId");

                    b.ToTable("ServiceChannelDisplayNameType");
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

                    b.Property<int?>("OrderNumber");

                    b.HasKey("EmailId", "ServiceChannelVersionedId");

                    b.HasIndex("EmailId")
                        .HasName("IX_SerChaEma_EmailId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasName("IX_SerChaEma_ServiceChannelVersionedId");

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
                        .HasName("IX_SerChaKey_KeywordId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasName("IX_SerChaKey_ServiceChannelVersionedId");

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

                    b.Property<int?>("OrderNumber");

                    b.HasKey("ServiceChannelVersionedId", "LanguageId");

                    b.HasIndex("LanguageId")
                        .HasName("IX_SerChaLan_LanguageId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasName("IX_SerChaLan_ServiceChannelVersionedId");

                    b.ToTable("ServiceChannelLanguage");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelLanguageAvailability", b =>
                {
                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<Guid>("LanguageId");

                    b.Property<DateTime?>("ArchiveAt");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<DateTime?>("PublishAt");

                    b.Property<DateTime?>("Reviewed");

                    b.Property<string>("ReviewedBy");

                    b.Property<DateTime?>("SetForArchived");

                    b.Property<string>("SetForArchivedBy");

                    b.Property<Guid>("StatusId");

                    b.HasKey("ServiceChannelVersionedId", "LanguageId");

                    b.HasIndex("LanguageId")
                        .HasName("IX_SerChaLanAva_LanguageId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasName("IX_SerChaLanAva_ServiceChannelVersionedId");

                    b.HasIndex("StatusId")
                        .HasName("IX_SerChaLanAva_StatusId");

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
                        .HasName("IX_SerChaNam_LocalizationId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasName("IX_SerChaNam_ServiceChannelVersionedId");

                    b.HasIndex("TypeId")
                        .HasName("IX_SerChaNam_TypeId");

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
                        .HasName("IX_SerChaOntTer_OntologyTermId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasName("IX_SerChaOntTer_ServiceChannelVersionedId");

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

                    b.Property<int?>("OrderNumber");

                    b.HasKey("PhoneId", "ServiceChannelVersionedId");

                    b.HasIndex("PhoneId")
                        .HasName("IX_SerChaPho_PhoneId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasName("IX_SerChaPho_ServiceChannelVersionedId");

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
                        .HasName("IX_SerChaSerCla_ServiceChannelVersionedId");

                    b.HasIndex("ServiceClassId")
                        .HasName("IX_SerChaSerCla_ServiceClassId");

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
                        .HasName("IX_SerChaSerHou_ServiceChannelVersionedId");

                    b.HasIndex("ServiceHoursId")
                        .HasName("IX_SerChaSerHou_ServiceHoursId");

                    b.ToTable("ServiceChannelServiceHours");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelSocialHealthCenter", b =>
                {
                    b.Property<Guid>("ServiceChannelId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Oid")
                        .HasMaxLength(100);

                    b.HasKey("ServiceChannelId");

                    b.HasIndex("Oid")
                        .HasName("IX_SerChaSocHeaCen_Oid");

                    b.HasIndex("ServiceChannelId")
                        .HasName("IX_SerChaSocHeaCen_ServiceChannelId");

                    b.ToTable("ServiceChannelSocialHealthCenter");
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
                        .HasName("IX_SerChaTarGro_ServiceChannelVersionedId");

                    b.HasIndex("TargetGroupId")
                        .HasName("IX_SerChaTarGro_TargetGroupId");

                    b.ToTable("ServiceChannelTargetGroup");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelTranslationOrder", b =>
                {
                    b.Property<Guid>("ServiceChannelId");

                    b.Property<Guid>("TranslationOrderId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("ServiceChannelIdentifier");

                    b.HasKey("ServiceChannelId", "TranslationOrderId");

                    b.HasIndex("ServiceChannelId")
                        .HasName("IX_SerChaTraOrd_ServiceChannelId");

                    b.HasIndex("TranslationOrderId")
                        .HasName("IX_SerChaTraOrd_TranslationOrderId");

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
                        .HasName("IX_SerChaTyp_Id");

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
                        .HasName("IX_SerChaTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_SerChaTypNam_TypeId");

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

                    b.Property<Guid?>("OriginalId");

                    b.Property<Guid>("PublishingStatusId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("UnificRootId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.Property<Guid?>("VersioningId");

                    b.HasKey("Id");

                    b.HasIndex("AreaInformationTypeId")
                        .HasName("IX_SerChaVer_AreaInformationTypeId");

                    b.HasIndex("ConnectionTypeId")
                        .HasName("IX_SerChaVer_ConnectionTypeId");

                    b.HasIndex("Id")
                        .HasName("IX_SerChaVer_Id");

                    b.HasIndex("OrganizationId")
                        .HasName("IX_SerChaVer_OrganizationId");

                    b.HasIndex("OriginalId")
                        .HasName("IX_SerChaVer_OriginalId");

                    b.HasIndex("PublishingStatusId")
                        .HasName("IX_SerChaVer_PublishingStatusId");

                    b.HasIndex("TypeId")
                        .HasName("IX_SerChaVer_TypeId");

                    b.HasIndex("UnificRootId")
                        .HasName("IX_SerChaVer_UnificRootId");

                    b.HasIndex("VersioningId")
                        .HasName("IX_SerChaVer_VersioningId");

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

                    b.Property<int?>("OrderNumber");

                    b.Property<Guid>("TypeId");

                    b.HasKey("ServiceChannelVersionedId", "WebPageId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasName("IX_SerChaWebPag_ServiceChannelVersionedId");

                    b.HasIndex("TypeId")
                        .HasName("IX_SerChaWebPag_TypeId");

                    b.HasIndex("WebPageId")
                        .HasName("IX_SerChaWebPag_WebPageId");

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
                        .HasName("IX_ServCharType_Id");

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
                        .HasName("IX_ServCharTypeName_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_ServCharTypeName_TypeId");

                    b.ToTable("ServiceChargeTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceClass", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("IsValid");

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
                        .HasName("IX_SerCla_Id");

                    b.HasIndex("ParentId")
                        .HasName("IX_SerCla_ParentId");

                    b.HasIndex("ParentUri")
                        .HasName("IX_SerCla_ParentUri");

                    b.HasIndex("Uri")
                        .HasName("IX_SerCla_Uri");

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
                        .HasName("IX_SerClaDes_LocalizationId");

                    b.HasIndex("ServiceClassId")
                        .HasName("IX_SerClaDes_ServiceClassId");

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
                        .HasName("IX_SerClaNam_LocalizationId");

                    b.HasIndex("ServiceClassId")
                        .HasName("IX_SerClaNam_ServiceClassId");

                    b.ToTable("ServiceClassName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceCollection", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_SerCol_Id");

                    b.ToTable("ServiceCollection");
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
                        .HasName("IX_SerColDes_LocalizationId");

                    b.HasIndex("ServiceCollectionVersionedId")
                        .HasName("IX_SerColDes_ServiceCollectionVersionedId");

                    b.HasIndex("TypeId")
                        .HasName("IX_SerColDes_TypeId");

                    b.ToTable("ServiceCollectionDescription");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceCollectionLanguageAvailability", b =>
                {
                    b.Property<Guid>("ServiceCollectionVersionedId");

                    b.Property<Guid>("LanguageId");

                    b.Property<DateTime?>("ArchiveAt");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<DateTime?>("PublishAt");

                    b.Property<DateTime?>("Reviewed");

                    b.Property<string>("ReviewedBy");

                    b.Property<DateTime?>("SetForArchived");

                    b.Property<string>("SetForArchivedBy");

                    b.Property<Guid>("StatusId");

                    b.HasKey("ServiceCollectionVersionedId", "LanguageId");

                    b.HasIndex("LanguageId")
                        .HasName("IX_SerColLanAva_LanguageId");

                    b.HasIndex("ServiceCollectionVersionedId")
                        .HasName("IX_SerColLanAva_ServiceCollectionVersionedId");

                    b.HasIndex("StatusId")
                        .HasName("IX_SerColLanAva_StatusId");

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
                        .HasName("IX_SerColNam_LocalizationId");

                    b.HasIndex("ServiceCollectionVersionedId")
                        .HasName("IX_SerColNam_ServiceCollectionVersionedId");

                    b.HasIndex("TypeId")
                        .HasName("IX_SerColNam_TypeId");

                    b.ToTable("ServiceCollectionName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceCollectionService", b =>
                {
                    b.Property<Guid>("ServiceCollectionId");

                    b.Property<Guid>("ServiceId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("ServiceCollectionId", "ServiceId");

                    b.HasIndex("ServiceCollectionId")
                        .HasName("IX_SerColSer_ServiceCollectionId");

                    b.HasIndex("ServiceId")
                        .HasName("IX_SerColSer_ServiceId");

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

                    b.Property<Guid?>("OriginalId");

                    b.Property<Guid>("PublishingStatusId");

                    b.Property<Guid>("UnificRootId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.Property<Guid?>("VersioningId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_SerColVer_Id");

                    b.HasIndex("OrganizationId")
                        .HasName("IX_SerColVer_OrganizationId");

                    b.HasIndex("OriginalId")
                        .HasName("IX_SerColVer_OriginalId");

                    b.HasIndex("PublishingStatusId")
                        .HasName("IX_SerColVer_PublishingStatusId");

                    b.HasIndex("UnificRootId")
                        .HasName("IX_SerColVer_UnificRootId");

                    b.HasIndex("VersioningId")
                        .HasName("IX_SerColVer_VersioningId");

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
                        .HasName("IX_SerDes_LocalizationId");

                    b.HasIndex("ServiceVersionedId")
                        .HasName("IX_SerDes_ServiceVersionedId");

                    b.HasIndex("TypeId")
                        .HasName("IX_SerDes_TypeId");

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
                        .HasName("IX_SerEleComCha_Id");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_SerEleComCha_LocalizationId");

                    b.HasIndex("ServiceVersionedId")
                        .HasName("IX_SerEleComCha_ServiceVersionedId");

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
                        .HasName("IX_SerEleNotCha_Id");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_SerEleNotCha_LocalizationId");

                    b.HasIndex("ServiceVersionedId")
                        .HasName("IX_SerEleNotCha_ServiceVersionedId");

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
                        .HasName("IX_SerFunTyp_Id");

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
                        .HasName("IX_SerFunTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_SerFunTypNam_TypeId");

                    b.ToTable("ServiceFundingTypeName");
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
                        .HasName("IX_SerHouTyp_Id");

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
                        .HasName("IX_SerHouTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_SerHouTypNam_TypeId");

                    b.ToTable("ServiceHourTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceHours", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("IsClosed");

                    b.Property<bool>("IsNonStop");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<DateTime?>("OpeningHoursFrom");

                    b.Property<DateTime?>("OpeningHoursTo");

                    b.Property<int?>("OrderNumber");

                    b.Property<Guid>("ServiceHourTypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_SerHou_Id");

                    b.HasIndex("ServiceHourTypeId")
                        .HasName("IX_SerHou_ServiceHourTypeId");

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
                        .HasName("IX_SerHouAddInf_LocalizationId");

                    b.HasIndex("ServiceHoursId")
                        .HasName("IX_SerHouAddInf_ServiceHoursId");

                    b.ToTable("ServiceHoursAdditionalInformation");
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
                        .HasName("IX_SerIndCla_IndustrialClassId");

                    b.HasIndex("ServiceVersionedId")
                        .HasName("IX_SerIndCla_ServiceVersionedId");

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
                        .HasName("IX_SerKey_KeywordId");

                    b.HasIndex("ServiceVersionedId")
                        .HasName("IX_SerKey_ServiceVersionedId");

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

                    b.Property<int?>("OrderNumber");

                    b.HasKey("ServiceVersionedId", "LanguageId");

                    b.HasIndex("LanguageId")
                        .HasName("IX_SerLan_LanguageId");

                    b.HasIndex("ServiceVersionedId")
                        .HasName("IX_SerLan_ServiceVersionedId");

                    b.ToTable("ServiceLanguage");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceLanguageAvailability", b =>
                {
                    b.Property<Guid>("ServiceVersionedId");

                    b.Property<Guid>("LanguageId");

                    b.Property<DateTime?>("ArchiveAt");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<DateTime?>("PublishAt");

                    b.Property<DateTime?>("Reviewed");

                    b.Property<string>("ReviewedBy");

                    b.Property<DateTime?>("SetForArchived");

                    b.Property<string>("SetForArchivedBy");

                    b.Property<Guid>("StatusId");

                    b.HasKey("ServiceVersionedId", "LanguageId");

                    b.HasIndex("LanguageId")
                        .HasName("IX_SerLanAva_LanguageId");

                    b.HasIndex("ServiceVersionedId")
                        .HasName("IX_SerLanAva_ServiceVersionedId");

                    b.HasIndex("StatusId")
                        .HasName("IX_SerLanAva_StatusId");

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

                    b.Property<int?>("OrderNumber");

                    b.HasKey("ServiceVersionedId", "LawId");

                    b.HasIndex("LawId")
                        .HasName("IX_SerLaw_LawId");

                    b.HasIndex("ServiceVersionedId")
                        .HasName("IX_SerLaw_ServiceVersionedId");

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
                        .HasName("IX_SerLifEve_LifeEventId");

                    b.HasIndex("ServiceVersionedId")
                        .HasName("IX_SerLifEve_ServiceVersionedId");

                    b.ToTable("ServiceLifeEvent");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceName", b =>
                {
                    b.Property<Guid>("ServiceVersionedId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("Inherited");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.HasKey("ServiceVersionedId", "TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_SerNam_LocalizationId");

                    b.HasIndex("ServiceVersionedId")
                        .HasName("IX_SerNam_ServiceVersionedId");

                    b.HasIndex("TypeId")
                        .HasName("IX_SerNam_TypeId");

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
                        .HasName("IX_SerOntTer_OntologyTermId");

                    b.HasIndex("ServiceVersionedId")
                        .HasName("IX_SerOntTer_ServiceVersionedId");

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
                        .HasName("IX_SerPro_Id");

                    b.HasIndex("ProvisionTypeId")
                        .HasName("IX_SerPro_ProvisionTypeId");

                    b.HasIndex("ServiceVersionedId")
                        .HasName("IX_SerPro_ServiceVersionedId");

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
                        .HasName("IX_SerProAddInf_Id");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_SerProAddInf_LocalizationId");

                    b.HasIndex("ServiceProducerId")
                        .HasName("IX_SerProAddInf_ServiceProducerId");

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
                        .HasName("IX_SerProOrg_OrganizationId");

                    b.HasIndex("ServiceProducerId")
                        .HasName("IX_SerProOrg_ServiceProducerId");

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
                        .HasName("IX_SerReq_Id");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_SerReq_LocalizationId");

                    b.HasIndex("ServiceVersionedId")
                        .HasName("IX_SerReq_ServiceVersionedId");

                    b.ToTable("ServiceRequirement");
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
                        .HasName("IX_SerSerCha_ChargeTypeId");

                    b.HasIndex("ServiceChannelId")
                        .HasName("IX_SerSerCha_ServiceChannelId");

                    b.HasIndex("ServiceId")
                        .HasName("IX_SerSerCha_ServiceId");

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
                        .HasName("IX_SerSerChaAdd_AddressId");

                    b.HasIndex("CharacterId")
                        .HasName("IX_SerSerChaAdd_CharacterId");

                    b.HasIndex("ServiceId", "ServiceChannelId")
                        .HasName("IX_SerSerChaAdd_ServiceId_ServiceChannelId");

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
                        .HasName("IX_SerSerChaDes_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_SerSerChaDes_TypeId");

                    b.HasIndex("ServiceId", "ServiceChannelId")
                        .HasName("IX_SerSerChaDes_ServiceId_ServiceChannelId");

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
                        .HasName("IX_SerSerChaDigAut_DigitalAuthorizationId");

                    b.HasIndex("ServiceId", "ServiceChannelId")
                        .HasName("IX_SerSerChaDigAut_ServiceId_ServiceChannelId");

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
                        .HasName("IX_SerSerChaEma_EmailId");

                    b.HasIndex("ServiceId", "ServiceChannelId")
                        .HasName("IX_SerSerChaEma_ServiceId_ServiceChannelId");

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
                        .HasName("IX_SerSerChaExtTyp_ExtraSubTypeId");

                    b.HasIndex("Id")
                        .HasName("IX_SerSerChaExtTyp_Id");

                    b.HasIndex("ServiceId", "ServiceChannelId")
                        .HasName("IX_SerSerChaExtTyp_ServiceId_ServiceChannelId");

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
                        .HasName("IX_SerSerChaExtTypDes_LocalizationId");

                    b.HasIndex("ServiceServiceChannelExtraTypeId")
                        .HasName("IX_SerSerChaExtTypDes_ServiceServiceChannelExtraTypeId");

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
                        .HasName("IX_SerSerChaPho_PhoneId");

                    b.HasIndex("ServiceId", "ServiceChannelId")
                        .HasName("IX_SerSerChaPho_ServiceId_ServiceChannelId");

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
                        .HasName("IX_SerSerChaSerHou_ServiceHoursId");

                    b.HasIndex("ServiceId", "ServiceChannelId")
                        .HasName("IX_SerSerChaSerHou_ServiceId_ServiceChannelId");

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
                        .HasName("IX_SerSerChaWebPag_WebPageId");

                    b.HasIndex("ServiceId", "ServiceChannelId")
                        .HasName("IX_SerSerChaWebPag_ServiceId_ServiceChannelId");

                    b.ToTable("ServiceServiceChannelWebPage");
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
                        .HasName("IX_SerSerCla_ServiceClassId");

                    b.HasIndex("ServiceVersionedId")
                        .HasName("IX_SerSerCla_ServiceVersionedId");

                    b.ToTable("ServiceServiceClass");
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
                        .HasName("IX_SerTarGro_ServiceVersionedId");

                    b.HasIndex("TargetGroupId")
                        .HasName("IX_SerTarGro_TargetGroupId");

                    b.ToTable("ServiceTargetGroup");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceTranslationOrder", b =>
                {
                    b.Property<Guid>("ServiceId");

                    b.Property<Guid>("TranslationOrderId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("ServiceVersionedIdentifier");

                    b.HasKey("ServiceId", "TranslationOrderId");

                    b.HasIndex("ServiceId")
                        .HasName("IX_SerTraOrd_ServiceId");

                    b.HasIndex("TranslationOrderId")
                        .HasName("IX_SerTraOrd_TranslationOrderId");

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
                        .HasName("IX_SerTyp_Id");

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
                        .HasName("IX_SerTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_SerTypNam_TypeId");

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

                    b.Property<Guid?>("OriginalId");

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
                        .HasName("IX_SerVer_AreaInformationTypeId");

                    b.HasIndex("ChargeTypeId")
                        .HasName("IX_SerVer_ChargeTypeId");

                    b.HasIndex("FundingTypeId")
                        .HasName("IX_SerVer_FundingTypeId");

                    b.HasIndex("Id")
                        .HasName("IX_SerVer_Id");

                    b.HasIndex("OrganizationId")
                        .HasName("IX_SerVer_OrganizationId");

                    b.HasIndex("OriginalId")
                        .HasName("IX_SerVer_OriginalId");

                    b.HasIndex("PublishingStatusId")
                        .HasName("IX_SerVer_PublishingStatusId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId")
                        .HasName("IX_SerVer_StatutoryServiceGeneralDescriptionId");

                    b.HasIndex("TypeId")
                        .HasName("IX_SerVer_TypeId");

                    b.HasIndex("UnificRootId")
                        .HasName("IX_SerVer_UnificRootId");

                    b.HasIndex("VersioningId")
                        .HasName("IX_SerVer_VersioningId");

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

                    b.Property<int?>("OrderNumber");

                    b.HasKey("ServiceVersionedId", "WebPageId");

                    b.HasIndex("ServiceVersionedId")
                        .HasName("IX_SerWebPag_ServiceVersionedId");

                    b.HasIndex("WebPageId")
                        .HasName("IX_SerWebPag_WebPageId");

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
                        .HasName("IX_StaSerDes_LocalizationId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasName("IX_StaSerDes_StatutoryServiceGeneralDescriptionVersionedId");

                    b.HasIndex("TypeId")
                        .HasName("IX_StaSerDes_TypeId");

                    b.ToTable("StatutoryServiceDescription");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_StaSerGenDes_Id");

                    b.ToTable("StatutoryServiceGeneralDescription");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceGeneralDescriptionVersioned", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("ChargeTypeId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("GeneralDescriptionTypeId");

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
                        .HasName("IX_StaSerGenDesVer_ChargeTypeId");

                    b.HasIndex("GeneralDescriptionTypeId")
                        .HasName("IX_StaSerGenDesVer_GeneralDescriptionTypeId");

                    b.HasIndex("Id")
                        .HasName("IX_StaSerGenDesVer_Id");

                    b.HasIndex("PublishingStatusId")
                        .HasName("IX_StaSerGenDesVer_PublishingStatusId");

                    b.HasIndex("TypeId")
                        .HasName("IX_StaSerGenDesVer_TypeId");

                    b.HasIndex("UnificRootId")
                        .HasName("IX_StaSerGenDesVer_UnificRootId");

                    b.HasIndex("VersioningId")
                        .HasName("IX_StaSerGenDesVer_VersioningId");

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
                        .HasName("IX_StaSerIndCla_IndustrialClassId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasName("IX_StaSerIndCla_StatutoryServiceGeneralDescriptionVersionedId");

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
                        .HasName("IX_StaSerLan_LanguageId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasName("IX_StaSerLan_StatutoryServiceGeneralDescriptionVersionedId");

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

                    b.Property<int?>("OrderNumber");

                    b.HasKey("StatutoryServiceGeneralDescriptionVersionedId", "LawId");

                    b.HasIndex("LawId")
                        .HasName("IX_StaSerLaw_LawId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasName("IX_StaSerLaw_StatutoryServiceGeneralDescriptionVersionedId");

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
                        .HasName("IX_StaSerLifEve_LifeEventId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasName("IX_StaSerLifEve_StatutoryServiceGeneralDescriptionVersionedId");

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
                        .HasName("IX_StaSerNam_LocalizationId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasName("IX_StaSerNam_StatutoryServiceGeneralDescriptionVersionedId");

                    b.HasIndex("TypeId")
                        .HasName("IX_StaSerNam_TypeId");

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
                        .HasName("IX_StaSerOntTer_OntologyTermId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasName("IX_StaSerOntTer_StatutoryServiceGeneralDescriptionVersionedId");

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
                        .HasName("IX_StaSerReq_Id");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_StaSerReq_LocalizationId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasName("IX_StaSerReq_StatutoryServiceGeneralDescriptionVersionedId");

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
                        .HasName("IX_StaSerSerCla_ServiceClassId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasName("IX_StaSerSerCla_StatutoryServiceGeneralDescriptionVersionedId");

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
                        .HasName("IX_StaSerTarGro_StatutoryServiceGeneralDescriptionVersionedId");

                    b.HasIndex("TargetGroupId")
                        .HasName("IX_StaSerTarGro_TargetGroupId");

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
                        .HasName("IX_StrNam_AddressStreetId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_StrNam_LocalizationId");

                    b.ToTable("StreetName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TargetGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("IsValid");

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
                        .HasName("IX_TarGro_Id");

                    b.HasIndex("ParentId")
                        .HasName("IX_TarGro_ParentId");

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
                        .HasName("IX_TarGroNam_LocalizationId");

                    b.HasIndex("TargetGroupId")
                        .HasName("IX_TarGroNam_TargetGroupId");

                    b.ToTable("TargetGroupName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TasksConfiguration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Entity");

                    b.Property<string>("Interval");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("PublishingStatusId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_TasCon_Id");

                    b.HasIndex("PublishingStatusId")
                        .HasName("IX_TasCon_PublishingStatusId");

                    b.ToTable("TasksConfiguration");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TasksFilter", b =>
                {
                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("UserId");

                    b.Property<Guid>("EntityId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("EntityModified");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("TypeId", "UserId", "EntityId");

                    b.HasIndex("TypeId", "UserId", "EntityId")
                        .HasName("IX_TasFil_TypeId_UserId_EntityId");

                    b.ToTable("TasksFilter");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TrackingEntityVersioned", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("EntityType");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("OperationType");

                    b.Property<Guid>("OrganizationId");

                    b.Property<Guid>("UnificRootId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_TraEntVer_Id");

                    b.ToTable("TrackingEntityVersioned");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TrackingGeneralDescriptionServiceChannel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ChannelId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("GeneralDescriptionId");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("OperationType");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_TraGenDesSerCha_Id");

                    b.ToTable("TrackingGeneralDescriptionServiceChannel");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TrackingGeneralDescriptionVersioned", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("GenerealDescriptionId");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("OperationType");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_TraGenDesVer_Id");

                    b.ToTable("TrackingGeneralDescriptionVersioned");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TrackingServiceCollectionService", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("OperationType");

                    b.Property<Guid>("ServiceCollectionId");

                    b.Property<Guid>("ServiceId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_TraSerColSer_Id");

                    b.ToTable("TrackingServiceCollectionService");
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
                        .HasName("IX_TraSerSerCha_Id");

                    b.ToTable("TrackingServiceServiceChannel");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TrackingTranslationOrder", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("OperationType");

                    b.Property<Guid>("OrganizationId");

                    b.Property<Guid>("TranslationOrderId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_TraTraOrd_Id");

                    b.ToTable("TrackingTranslationOrder");
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
                        .HasName("IX_TraCom_Id");

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

                    b.Property<string>("OrganizationBusinessCode");

                    b.Property<Guid?>("OrganizationIdentifier");

                    b.Property<string>("OrganizationName");

                    b.Property<Guid?>("PreviousTranslationOrderId");

                    b.Property<string>("SenderEmail")
                        .HasMaxLength(100);

                    b.Property<string>("SenderName")
                        .HasMaxLength(100);

                    b.Property<string>("SourceEntityName");

                    b.Property<long>("SourceLanguageCharAmount");

                    b.Property<string>("SourceLanguageData");

                    b.Property<string>("SourceLanguageDataHash");

                    b.Property<Guid>("SourceLanguageId");

                    b.Property<string>("TargetLanguageData");

                    b.Property<string>("TargetLanguageDataHash");

                    b.Property<Guid>("TargetLanguageId");

                    b.Property<Guid>("TranslationCompanyId");

                    b.Property<string>("TranslationCompanyOrderIdentifier");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_TraOrd_Id");

                    b.HasIndex("PreviousTranslationOrderId")
                        .HasName("IX_TraOrd_PreviousTranslationOrderId");

                    b.HasIndex("SourceLanguageId")
                        .HasName("IX_TraOrd_SourceLanguageId");

                    b.HasIndex("TargetLanguageId")
                        .HasName("IX_TraOrd_TargetLanguageId");

                    b.HasIndex("TranslationCompanyId")
                        .HasName("IX_TraOrd_TranslationCompanyId");

                    b.ToTable("TranslationOrder");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TranslationOrderState", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Checked");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("InfoDetails");

                    b.Property<bool>("Last");

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
                        .HasName("IX_TraOrdSta_Id");

                    b.HasIndex("TranslationOrderId")
                        .HasName("IX_TraOrdSta_TranslationOrderId");

                    b.HasIndex("TranslationStateId")
                        .HasName("IX_TraOrdSta_TranslationStateId");

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
                        .HasName("IX_TraStaTyp_Id");

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
                        .HasName("IX_TraStaTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_TraStaTypNam_TypeId");

                    b.ToTable("TranslationStateTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.UserAccessRightsGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("AccessRightFlag");

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("UserRole");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_UseAccRigGro_Id");

                    b.ToTable("UserAccessRightsGroup");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.UserAccessRightsGroupName", b =>
                {
                    b.Property<Guid>("UserAccessRightsGroupId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("LastOperationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("UserAccessRightsGroupId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_UseAccRigGroNam_LocalizationId");

                    b.HasIndex("UserAccessRightsGroupId")
                        .HasName("IX_UseAccRigGroNam_UserAccessRightsGroupId");

                    b.ToTable("UserAccessRightsGroupName");
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
                        .HasName("IX_UseOrg_Id");

                    b.HasIndex("OrganizationId")
                        .HasName("IX_UseOrg_OrganizationId");

                    b.ToTable("UserOrganization");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Versioning", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("Ignored");

                    b.Property<string>("LastOperationId");

                    b.Property<string>("Meta");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid?>("PreviousVersionId");

                    b.Property<Guid?>("UnificRootId");

                    b.Property<int>("VersionMajor");

                    b.Property<int>("VersionMinor");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("IX_Ver_Id");

                    b.HasIndex("PreviousVersionId")
                        .HasName("IX_Ver_PreviousVersionId");

                    b.ToTable("Versioning");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.WcagLevelType", b =>
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
                        .HasName("IX_WcaLevTyp_Id");

                    b.ToTable("WcagLevelType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.WcagLevelTypeName", b =>
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
                        .HasName("IX_WcaLevTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_WcaLevTypNam_TypeId");

                    b.ToTable("WcagLevelTypeName");
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
                        .HasName("IX_WebPag_Id");

                    b.HasIndex("LocalizationId")
                        .HasName("IX_WebPag_LocalizationId");

                    b.ToTable("WebPage");
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
                        .HasName("IX_WebPagTyp_Id");

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
                        .HasName("IX_WebPagTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasName("IX_WebPagTypNam_TypeId");

                    b.ToTable("WebPageTypeName");
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
                        .HasName("IX_WebCha_Id");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasName("IX_WebCha_ServiceChannelVersionedId");

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
                        .HasName("IX_WebChaUrl_LocalizationId");

                    b.HasIndex("WebpageChannelId")
                        .HasName("IX_WebChaUrl_WebpageChannelId");

                    b.ToTable("WebpageChannelUrl");
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

            modelBuilder.Entity("PTV.Database.Model.Models.AccessRightsOperationsUI", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Organization", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AccessibilityClassification", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AccessibilityClassificationLevelType", "AccessibilityClassificationLevelType")
                        .WithMany()
                        .HasForeignKey("AccessibilityClassificationLevelTypeId")
                        .HasConstraintName("FK_AccCla_AccessibilityClassificationLevelType_AccessibilityClassificationLevelTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.WcagLevelType", "WcagLevelType")
                        .WithMany()
                        .HasForeignKey("WcagLevelTypeId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AccessibilityClassificationLevelTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .HasConstraintName("FK_AccClaLevTypNam_Language_LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.AccessibilityClassificationLevelType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .HasConstraintName("FK_AccClaLevTypNam_TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AccessibilityRegister", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Address", "Address")
                        .WithMany("AccessibilityRegisters")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "AddressLanguage")
                        .WithMany()
                        .HasForeignKey("AddressLanguageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("AccessibilityRegisters")
                        .HasForeignKey("ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AccessibilityRegisterEntrance", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AccessibilityRegister", "AccessibilityRegister")
                        .WithMany("Entrances")
                        .HasForeignKey("AccessibilityRegisterId")
                        .HasConstraintName("FK_AccRegEnt_AccessibilityRegister_AccessibilityRegisterId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Address", "Address")
                        .WithMany("AccessibilityRegisterEntrances")
                        .HasForeignKey("AddressId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AccessibilityRegisterEntranceName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AccessibilityRegisterEntrance", "AccessibilityRegisterEntrance")
                        .WithMany("Names")
                        .HasForeignKey("AccessibilityRegisterEntranceId")
                        .HasConstraintName("FK_AccRegEntNam_AccessibilityRegisterEntranceId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AccessibilityRegisterGroup", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AccessibilityRegisterEntrance", "AccessibilityRegisterEntrance")
                        .WithMany("SentenceGroups")
                        .HasForeignKey("AccessibilityRegisterEntranceId")
                        .HasConstraintName("FK_AccRegGro_AccessibilityRegisterEntrance_AccessibilityRegisterEntranceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AccessibilityRegisterGroupValue", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AccessibilityRegisterGroup", "AccessibilityRegisterGroup")
                        .WithMany("Values")
                        .HasForeignKey("AccessibilityRegisterGroupId")
                        .HasConstraintName("FK_AccRegGroVal_AccessibilityRegisterGroup_AccessibilityRegisterGroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AccessibilityRegisterSentence", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AccessibilityRegisterGroup", "SentenceGroup")
                        .WithMany("Sentences")
                        .HasForeignKey("SentenceGroupId")
                        .HasConstraintName("FK_AccRegSen_AccessibilityRegisterGroup_SentenceGroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AccessibilityRegisterSentenceValue", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AccessibilityRegisterSentence", "AccessibilityRegisterSentence")
                        .WithMany("Values")
                        .HasForeignKey("AccessibilityRegisterSentenceId")
                        .HasConstraintName("FK_AccRegSenVal_AccessibilityRegisterSentenceId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.AddressExtraType", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Address", "Address")
                        .WithMany("ExtraTypes")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ExtraType", "ExtraType")
                        .WithMany()
                        .HasForeignKey("ExtraTypeId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.AddressOther", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Address", "Address")
                        .WithMany("AddressOthers")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PostalCode", "PostalCode")
                        .WithMany()
                        .HasForeignKey("PostalCodeId");
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

            modelBuilder.Entity("PTV.Database.Model.Models.AddressReceiver", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Address", "Address")
                        .WithMany("Receivers")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);
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

            modelBuilder.Entity("PTV.Database.Model.Models.ClsAddressPoint", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Address", "Address")
                        .WithMany("ClsAddressPoints")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ClsAddressStreet", "AddressStreet")
                        .WithMany()
                        .HasForeignKey("AddressStreetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ClsAddressStreetNumber", "AddressStreetNumber")
                        .WithMany()
                        .HasForeignKey("AddressStreetNumberId");

                    b.HasOne("PTV.Database.Model.Models.Municipality", "Municipality")
                        .WithMany()
                        .HasForeignKey("MunicipalityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PostalCode", "PostalCode")
                        .WithMany()
                        .HasForeignKey("PostalCodeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ClsAddressStreet", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Municipality", "Municipality")
                        .WithMany("Streets")
                        .HasForeignKey("MunicipalityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ClsAddressStreetName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ClsAddressStreet", "ClsAddressStreet")
                        .WithMany("StreetNames")
                        .HasForeignKey("ClsAddressStreetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ClsAddressStreetNumber", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ClsAddressStreet", "ClsAddressStreet")
                        .WithMany("StreetNumbers")
                        .HasForeignKey("ClsAddressStreetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PostalCode", "PostalCode")
                        .WithMany("ClsAddressStreetNumbers")
                        .HasForeignKey("PostalCodeId")
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
                        .HasConstraintName("FK_DigAutNam_DigitalAuthorization_DigitalAuthorizationId")
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
                        .HasConstraintName("FK_EleCha_ServiceChannelVersioned_ServiceChannelVersionedId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionLanguageAvailability", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .HasConstraintName("FK_GenDesLanAva_StatusId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescriptionVersioned", "StatutoryServiceGeneralDescriptionVersioned")
                        .WithMany("LanguageAvailabilities")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasConstraintName("FK_GenDesLanAva_StatutoryServiceGeneralDescriptionVersioned_StatutoryServiceGeneralDescriptionVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionServiceChannel", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChargeType", "ChargeType")
                        .WithMany()
                        .HasForeignKey("ChargeTypeId")
                        .HasConstraintName("FK_GenDesSerCha_ServiceChargeType_ChargeTypeId");

                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("StatutoryServiceGeneralDescriptionServiceChannels")
                        .HasForeignKey("ServiceChannelId")
                        .HasConstraintName("FK_GenDesSerCha_ServiceChannel_ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", "StatutoryServiceGeneralDescription")
                        .WithMany("StatutoryServiceGeneralDescriptionServiceChannels")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId")
                        .HasConstraintName("FK_GenDesSerCha_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionServiceChannelDescription", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .HasConstraintName("FK_GenDesSerChaDes_Language_LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.DescriptionType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .HasConstraintName("FK_GenDesSerChaDes_TypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.GeneralDescriptionServiceChannel", "GeneralDescriptionServiceChannel")
                        .WithMany("GeneralDescriptionServiceChannelDescriptions")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId", "ServiceChannelId")
                        .HasConstraintName("FK_GenDesSerChaDes_StatutoryServiceGeneralDescriptionId_ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionServiceChannelDigitalAuthorization", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.DigitalAuthorization", "DigitalAuthorization")
                        .WithMany()
                        .HasForeignKey("DigitalAuthorizationId")
                        .HasConstraintName("FK_GenDesSerChaDigAut_DigitalAuthorizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.GeneralDescriptionServiceChannel", "GeneralDescriptionServiceChannel")
                        .WithMany("GeneralDescriptionServiceChannelDigitalAuthorizations")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId", "ServiceChannelId")
                        .HasConstraintName("FK_GenDesSerChaDigAut_StatutoryServiceGeneralDescriptionId_ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionServiceChannelExtraType", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ExtraSubType", "ExtraSubType")
                        .WithMany()
                        .HasForeignKey("ExtraSubTypeId")
                        .HasConstraintName("FK_GenDesSerChaExtTyp_ExtraSubType_ExtraSubTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("StatutoryServiceGeneralDescriptionServiceChannelExtraTypes")
                        .HasForeignKey("ServiceChannelId")
                        .HasConstraintName("FK_GenDesSerChaExtTyp_ServiceChannel_ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", "StatutoryServiceGeneralDescription")
                        .WithMany("StatutoryServiceGeneralDescriptionServiceChannelExtraTypes")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId")
                        .HasConstraintName("FK_GenDesSerChaExtTyp_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionServiceChannelExtraTypeDescription", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.GeneralDescriptionServiceChannelExtraType", "GeneralDescriptionServiceChannelExtraType")
                        .WithMany("GeneralDescriptionServiceChannelExtraTypeDescriptions")
                        .HasForeignKey("GeneralDescriptionServiceChannelExtraTypeId")
                        .HasConstraintName("FK_GenDesSerChaExtTypDes_GeneralDescriptionServiceChannelExtraTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .HasConstraintName("FK_GenDesSerChaExtTypDes_Language_LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionTranslationOrder", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", "StatutoryServiceGeneralDescription")
                        .WithMany("GeneralDescriptionTranslationOrders")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId")
                        .HasConstraintName("FK_GenDesTraOrd_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.TranslationOrder", "TranslationOrder")
                        .WithMany("GeneralDescriptionTranslationOrders")
                        .HasForeignKey("TranslationOrderId")
                        .HasConstraintName("FK_GenDesTraOrd_TranslationOrder_TranslationOrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.GeneralDescriptionType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
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

            modelBuilder.Entity("PTV.Database.Model.Models.NotificationServiceServiceChannelFilter", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.NotificationServiceServiceChannel", "NotificationServiceServiceChannel")
                        .WithMany("Filters")
                        .HasForeignKey("NotificationServiceServiceChannelId")
                        .HasConstraintName("FK_NotSerSerChaFil_NotificationServiceServiceChannelId")
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
                        .HasConstraintName("FK_OrgAdd_OrganizationVersioned_OrganizationVersionedId")
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
                        .HasConstraintName("FK_OrgAre_OrganizationVersioned_OrganizationVersionedId")
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
                        .HasConstraintName("FK_OrgAreMun_OrganizationVersioned_OrganizationVersionedId")
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
                        .HasConstraintName("FK_OrgDes_OrganizationVersioned_OrganizationVersionedId")
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
                        .HasConstraintName("FK_OrgDisNamTyp_OrganizationVersioned_OrganizationVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationEInvoicing", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.OrganizationVersioned", "OrganizationVersioned")
                        .WithMany("OrganizationEInvoicings")
                        .HasForeignKey("OrganizationVersionedId")
                        .HasConstraintName("FK_OrgEInv_OrganizationVersioned_OrganizationVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationEInvoicingAdditionalInformation", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .HasConstraintName("FK_OrgEInvAddInf_Language_LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.OrganizationEInvoicing", "OrganizationEInvoicing")
                        .WithMany("EInvoicingAdditionalInformations")
                        .HasForeignKey("OrganizationEInvoicingId")
                        .HasConstraintName("FK_OrgEInvAddInf_OrganizationEInvoicingId")
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
                        .HasConstraintName("FK_OrgEma_OrganizationVersioned_OrganizationVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationFilter", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.RestrictionFilter", "Filter")
                        .WithMany("OrganizationFilters")
                        .HasForeignKey("FilterId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Organization", "Organization")
                        .WithMany("OrganizationFilters")
                        .HasForeignKey("OrganizationId")
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
                        .HasConstraintName("FK_OrgLanAva_OrganizationVersioned_OrganizationVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .HasConstraintName("FK_OrgLanAva_PublishingStatusType_StatusId")
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
                        .HasConstraintName("FK_OrgNam_OrganizationVersioned_OrganizationVersionedId")
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
                        .HasConstraintName("FK_OrgPho_OrganizationVersioned_OrganizationVersionedId")
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
                        .HasConstraintName("FK_OrgVer_AreaInformationType_AreaInformationTypeId")
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
                        .HasConstraintName("FK_OrgVer_PublishingStatusType_PublishingStatusId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Organization", "ResponsibleOrganizationRegion")
                        .WithMany("RegionRelatedOrganizations")
                        .HasForeignKey("ResponsibleOrganizationRegionId")
                        .HasConstraintName("FK_OrgVer_Organization_ResponsibleOrganizationRegionId");

                    b.HasOne("PTV.Database.Model.Models.OrganizationType", "Type")
                        .WithMany("Organization")
                        .HasForeignKey("TypeId");

                    b.HasOne("PTV.Database.Model.Models.Organization", "UnificRoot")
                        .WithMany("Versions")
                        .HasForeignKey("UnificRootId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Versioning", "Versioning")
                        .WithMany("Organizations")
                        .HasForeignKey("VersioningId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationWebPage", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.OrganizationVersioned", "OrganizationVersioned")
                        .WithMany("OrganizationWebAddress")
                        .HasForeignKey("OrganizationVersionedId")
                        .HasConstraintName("FK_OrgWebPag_OrganizationVersioned_OrganizationVersionedId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.PhoneExtraType", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ExtraType", "ExtraType")
                        .WithMany()
                        .HasForeignKey("ExtraTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Phone", "Phone")
                        .WithMany("ExtraTypes")
                        .HasForeignKey("PhoneId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.PostOfficeBoxName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AddressPostOfficeBox", "AddressPostOfficeBox")
                        .WithMany("PostOfficeBoxNames")
                        .HasForeignKey("AddressPostOfficeBoxId")
                        .HasConstraintName("FK_PosOffBoxNam_AddressPostOfficeBox_AddressPostOfficeBoxId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PostalCode", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Municipality", "Municipality")
                        .WithMany("PostalCodes")
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

            modelBuilder.Entity("PTV.Database.Model.Models.PrintableFormChannel", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("PrintableFormChannels")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .HasConstraintName("FK_PriForCha_ServiceChannelVersioned_ServiceChannelVersionedId")
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
                        .HasConstraintName("FK_PriForChaIde_PrintableFormChannel_")
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
                        .HasConstraintName("FK_PriForChaUrl_PrintableFormChannel_PrintableFormChannelId")
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
                        .HasConstraintName("FK_PriForChaUrlTypNam_TypeId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.RestrictionFilter", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.RestrictedType", "RestrictedType")
                        .WithMany()
                        .HasForeignKey("RestrictedTypeId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelAccessibilityClassification", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.AccessibilityClassification", "AccessibilityClassification")
                        .WithMany("ServiceChannelAccessibilityClassifications")
                        .HasForeignKey("AccessibilityClassificationId")
                        .HasConstraintName("FK_SerChaAccCla_AccessibilityClassificationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("AccessibilityClassifications")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .HasConstraintName("FK_SerChaAccCla_ServiceChannelVersioned_ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelAddress", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.AddressCharacter", "Character")
                        .WithMany()
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("Addresses")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .HasConstraintName("FK_SerChaAdd_ServiceChannelVersioned_ServiceChannelVersionedId")
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
                        .HasConstraintName("FK_SerChaAre_ServiceChannelVersioned_ServiceChannelVersionedId")
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
                        .HasConstraintName("FK_SerChaAreMun_ServiceChannelVersioned_ServiceChannelVersionedId")
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
                        .HasConstraintName("FK_SerChaAtt_ServiceChannelVersioned_ServiceChannelVersionedId")
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
                        .HasConstraintName("FK_SerChaConTypNam_TypeId")
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
                        .HasConstraintName("FK_SerChaDes_ServiceChannelVersioned_ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.DescriptionType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelDisplayNameType", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.NameType", "DisplayNameType")
                        .WithMany()
                        .HasForeignKey("DisplayNameTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("DisplayNameTypes")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .HasConstraintName("FK_SerChaDisNamTyp_ServiceChannelVersioned_ServiceChannelVersionedId")
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
                        .HasConstraintName("FK_SerChaEma_ServiceChannelVersioned_ServiceChannelVersionedId")
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
                        .HasConstraintName("FK_SerChaKey_ServiceChannelVersioned_ServiceChannelVersionedId")
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
                        .HasConstraintName("FK_SerChaLan_ServiceChannelVersioned_ServiceChannelVersionedId")
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
                        .HasConstraintName("FK_SerChaLanAva_ServiceChannelVersioned_ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .HasConstraintName("FK_SerChaLanAva_PublishingStatusType_StatusId")
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
                        .HasConstraintName("FK_SerChaNam_ServiceChannelVersioned_ServiceChannelVersionedId")
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
                        .HasConstraintName("FK_SerChaOntTer_ServiceChannelVersioned_ServiceChannelVersionedId")
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
                        .HasConstraintName("FK_SerChaPho_ServiceChannelVersioned_ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelServiceClass", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("ServiceClasses")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .HasConstraintName("FK_SerChaSerCla_ServiceChannelVersioned_ServiceChannelVersionedId")
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
                        .HasConstraintName("FK_SerChaSerHou_ServiceChannelVersioned_ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceHours", "ServiceHours")
                        .WithMany("ServiceChannelServiceHours")
                        .HasForeignKey("ServiceHoursId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelSocialHealthCenter", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("SocialHealthCenters")
                        .HasForeignKey("ServiceChannelId")
                        .HasConstraintName("FK_SerChaSocHeaCen_ServiceChannel_ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelTargetGroup", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("TargetGroups")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .HasConstraintName("FK_SerChaTarGro_ServiceChannelVersioned_ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.TargetGroup", "TargetGroup")
                        .WithMany()
                        .HasForeignKey("TargetGroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelTranslationOrder", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("ServiceChannelTranslationOrders")
                        .HasForeignKey("ServiceChannelId")
                        .HasConstraintName("FK_SerChaTraOrd_ServiceChannel_ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.TranslationOrder", "TranslationOrder")
                        .WithMany("ServiceChannelTranslationOrders")
                        .HasForeignKey("TranslationOrderId")
                        .HasConstraintName("FK_SerChaTraOrd_TranslationOrder_TranslationOrderId")
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
                        .HasForeignKey("AreaInformationTypeId")
                        .HasConstraintName("FK_SerChaVer_AreaInformationType_AreaInformationTypeId");

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelConnectionType", "ConnectionType")
                        .WithMany()
                        .HasForeignKey("ConnectionTypeId")
                        .HasConstraintName("FK_SerChaVer_ServiceChannelConnectionType_ConnectionTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Organization", "Organization")
                        .WithMany("OrganizationServiceChannelsVersioned")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "Original")
                        .WithMany()
                        .HasForeignKey("OriginalId");

                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "PublishingStatus")
                        .WithMany()
                        .HasForeignKey("PublishingStatusId")
                        .HasConstraintName("FK_SerChaVer_PublishingStatusType_PublishingStatusId")
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
                        .WithMany("Channels")
                        .HasForeignKey("VersioningId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelWebPage", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("WebPages")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .HasConstraintName("FK_SerChaWebPag_ServiceChannelVersioned_ServiceChannelVersionedId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceCollectionDescription", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceCollectionVersioned", "ServiceCollectionVersioned")
                        .WithMany("ServiceCollectionDescriptions")
                        .HasForeignKey("ServiceCollectionVersionedId")
                        .HasConstraintName("FK_SerColDes_ServiceCollectionVersioned_ServiceCollectionVersionedId")
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
                        .HasConstraintName("FK_SerColLanAva_ServiceCollectionVersioned_ServiceCollectionVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .HasConstraintName("FK_SerColLanAva_PublishingStatusType_StatusId")
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
                        .HasConstraintName("FK_SerColNam_ServiceCollectionVersioned_ServiceCollectionVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.NameType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceCollectionService", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceCollection", "ServiceCollection")
                        .WithMany("ServiceCollectionServices")
                        .HasForeignKey("ServiceCollectionId")
                        .HasConstraintName("FK_SerColSer_ServiceCollection_ServiceCollectionId")
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

                    b.HasOne("PTV.Database.Model.Models.ServiceCollectionVersioned", "Original")
                        .WithMany()
                        .HasForeignKey("OriginalId")
                        .HasConstraintName("FK_SerColVer_ServiceCollectionVersioned_OriginalId");

                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "PublishingStatus")
                        .WithMany()
                        .HasForeignKey("PublishingStatusId")
                        .HasConstraintName("FK_SerColVer_PublishingStatusType_PublishingStatusId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceCollection", "UnificRoot")
                        .WithMany("Versions")
                        .HasForeignKey("UnificRootId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Versioning", "Versioning")
                        .WithMany("ServiceCollections")
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
                        .HasConstraintName("FK_SerEleComCha_Language_LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "ServiceVersioned")
                        .WithMany("ServiceElectronicCommunicationChannels")
                        .HasForeignKey("ServiceVersionedId")
                        .HasConstraintName("FK_SerEleComCha_ServiceVersioned_ServiceVersionedId")
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
                        .HasConstraintName("FK_SerEleNotCha_ServiceVersioned_ServiceVersionedId")
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
                        .HasConstraintName("FK_SerHouAddInf_ServiceHours_ServiceHoursId")
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
                        .HasConstraintName("FK_SerLanAva_ServiceVersioned_ServiceVersionedId")
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
                        .HasConstraintName("FK_SerProAddInf_ServiceProducer_ServiceProducerId")
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
                        .HasConstraintName("FK_SerProOrg_ServiceProducer_ServiceProducerId")
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
                        .WithMany("ServiceServiceChannelAddresses")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.AddressCharacter", "Character")
                        .WithMany()
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceServiceChannel", "ServiceServiceChannel")
                        .WithMany("ServiceServiceChannelAddresses")
                        .HasForeignKey("ServiceId", "ServiceChannelId")
                        .HasConstraintName("FK_SerSerChaAdd_ServiceServiceChannel_ServiceId_ServiceChannelId")
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
                        .HasConstraintName("FK_SerSerChaDes_ServiceServiceChannel_ServiceId_ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannelDigitalAuthorization", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.DigitalAuthorization", "DigitalAuthorization")
                        .WithMany("ServiceServiceChannelDigitalAuthorizations")
                        .HasForeignKey("DigitalAuthorizationId")
                        .HasConstraintName("FK_SerSerChaDigAut_DigitalAuthorizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceServiceChannel", "ServiceServiceChannel")
                        .WithMany("ServiceServiceChannelDigitalAuthorizations")
                        .HasForeignKey("ServiceId", "ServiceChannelId")
                        .HasConstraintName("FK_SerSerChaDigAut_ServiceId_ServiceChannelId")
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
                        .HasConstraintName("FK_SerSerChaEma_ServiceServiceChannel_ServiceId_ServiceChannelId")
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
                        .HasConstraintName("FK_SerSerChaExtTyp_ServiceServiceChannel_ServiceId_ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannelExtraTypeDescription", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .HasConstraintName("FK_SerSerChaExtTypDes_Language_LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceServiceChannelExtraType", "ServiceServiceChannelExtraType")
                        .WithMany("ServiceServiceChannelExtraTypeDescriptions")
                        .HasForeignKey("ServiceServiceChannelExtraTypeId")
                        .HasConstraintName("FK_SerSerChaExtTypDes_ServiceServiceChannelExtraTypeId")
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
                        .HasConstraintName("FK_SerSerChaPho_ServiceServiceChannel_ServiceId_ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceChannelServiceHours", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceHours", "ServiceHours")
                        .WithMany("ServiceServiceChannelServiceHours")
                        .HasForeignKey("ServiceHoursId")
                        .HasConstraintName("FK_SerSerChaSerHou_ServiceHours_ServiceHoursId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceServiceChannel", "ServiceServiceChannel")
                        .WithMany("ServiceServiceChannelServiceHours")
                        .HasForeignKey("ServiceId", "ServiceChannelId")
                        .HasConstraintName("FK_SerSerChaSerHou_ServiceServiceChannel_ServiceId_ServiceChannelId")
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
                        .HasConstraintName("FK_SerSerChaWebPag_ServiceServiceChannel_ServiceId_ServiceChannelId")
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
                    b.HasOne("PTV.Database.Model.Models.Service", "Service")
                        .WithMany("ServiceTranslationOrders")
                        .HasForeignKey("ServiceId")
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

                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "Original")
                        .WithMany()
                        .HasForeignKey("OriginalId");

                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "PublishingStatus")
                        .WithMany()
                        .HasForeignKey("PublishingStatusId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", "StatutoryServiceGeneralDescription")
                        .WithMany()
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId")
                        .HasConstraintName("FK_SerVer_StatutoryServiceGeneralDescription_StatutoryServiceGeneralDescriptionId");

                    b.HasOne("PTV.Database.Model.Models.ServiceType", "Type")
                        .WithMany("Service")
                        .HasForeignKey("TypeId");

                    b.HasOne("PTV.Database.Model.Models.Service", "UnificRoot")
                        .WithMany("Versions")
                        .HasForeignKey("UnificRootId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Versioning", "Versioning")
                        .WithMany("Services")
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
                        .HasConstraintName("FK_StaSerDes_StatutoryServiceGeneralDescriptionVersioned_StatutoryServiceGeneralDescriptionVersionedId")
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
                        .HasForeignKey("ChargeTypeId")
                        .HasConstraintName("FK_StaSerGenDesVer_ServiceChargeType_ChargeTypeId");

                    b.HasOne("PTV.Database.Model.Models.GeneralDescriptionType", "GeneralDescriptionType")
                        .WithMany("StatutoryServiceGeneralDescription")
                        .HasForeignKey("GeneralDescriptionTypeId")
                        .HasConstraintName("FK_StaSerGenDesVer_GeneralDescriptionType_GeneralDescriptionTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "PublishingStatus")
                        .WithMany()
                        .HasForeignKey("PublishingStatusId")
                        .HasConstraintName("FK_StaSerGenDesVer_PublishingStatusType_PublishingStatusId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .HasConstraintName("FK_StaSerGenDesVer_ServiceType_TypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", "UnificRoot")
                        .WithMany("Versions")
                        .HasForeignKey("UnificRootId")
                        .HasConstraintName("FK_StaSerGenDesVer_UnificRootId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Versioning", "Versioning")
                        .WithMany("GeneralDescriptions")
                        .HasForeignKey("VersioningId")
                        .HasConstraintName("FK_StaSerGenDesVer_Versioning_VersioningId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceIndustrialClass", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.IndustrialClass", "IndustrialClass")
                        .WithMany()
                        .HasForeignKey("IndustrialClassId")
                        .HasConstraintName("FK_StaSerIndCla_IndustrialClass_IndustrialClassId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescriptionVersioned", "StatutoryServiceGeneralDescriptionVersioned")
                        .WithMany("IndustrialClasses")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasConstraintName("FK_StaSerIndCla_StatutoryServiceGeneralDescriptionVersioned_StatutoryServiceGeneralDescriptionVersionedId")
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
                        .HasConstraintName("FK_StaSerLan_StatutoryServiceGeneralDescriptionVersioned_StatutoryServiceGeneralDescriptionVersionedId")
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
                        .HasConstraintName("FK_StaSerLaw_StatutoryServiceGeneralDescriptionVersioned_StatutoryServiceGeneralDescriptionVersionedId")
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
                        .HasConstraintName("FK_StaSerLifEve_StatutoryServiceGeneralDescriptionVersioned_StatutoryServiceGeneralDescriptionVersionedId")
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
                        .HasConstraintName("FK_StaSerNam_StatutoryServiceGeneralDescriptionVersioned_StatutoryServiceGeneralDescriptionVersionedId")
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
                        .HasConstraintName("FK_StaSerOntTer_StatutoryServiceGeneralDescriptionVersioned_StatutoryServiceGeneralDescriptionVersionedId")
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
                        .HasConstraintName("FK_StaSerReq_StatutoryServiceGeneralDescriptionVersioned_StatutoryServiceGeneralDescriptionVersionedId")
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
                        .HasConstraintName("FK_StaSerSerCla_StatutoryServiceGeneralDescriptionVersioned_StatutoryServiceGeneralDescriptionVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceTargetGroup", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescriptionVersioned", "StatutoryServiceGeneralDescriptionVersioned")
                        .WithMany("TargetGroups")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionVersionedId")
                        .HasConstraintName("FK_StaSerTarGro_StatutoryServiceGeneralDescriptionVersioned_StatutoryServiceGeneralDescriptionVersionedId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.TasksConfiguration", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "PublishingStatus")
                        .WithMany()
                        .HasForeignKey("PublishingStatusId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TasksFilter", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.TasksConfiguration", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.TranslationOrder", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.TranslationOrder", "PreviousTranslationOrder")
                        .WithMany()
                        .HasForeignKey("PreviousTranslationOrderId");

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

                    b.HasOne("PTV.Database.Model.Models.TranslationStateType", "TranslationState")
                        .WithMany("TranslationOrderStates")
                        .HasForeignKey("TranslationStateId")
                        .HasConstraintName("FK_TraOrdSta_TranslationStateType_TranslationStateId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.UserAccessRightsGroupName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.UserAccessRightsGroup", "UserAccessRightsGroup")
                        .WithMany("UserAccessRightsGroupNames")
                        .HasForeignKey("UserAccessRightsGroupId")
                        .HasConstraintName("FK_UseAccRigGroNam_UserAccessRightsGroup_UserAccessRightsGroupId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.WcagLevelTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.WcagLevelType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.WebPage", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.WebpageChannel", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChannelVersioned", "ServiceChannelVersioned")
                        .WithMany("WebpageChannels")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .HasConstraintName("FK_WebCha_ServiceChannelVersioned_ServiceChannelVersionedId")
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
#pragma warning restore 612, 618
        }
    }
}
