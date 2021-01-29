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

namespace PTV.Database.Migrations.Migrations.Release_1_5
{
    [DbContext(typeof(PtvDbContext))]
    [Migration("20170428124050_ServiceServiceChannelChange")]
    partial class ServiceServiceChannelChange
    {
        public void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("Npgsql:PostgresExtension:uuid-ossp", "'uuid-ossp', '', ''")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("PTV.Database.Model.Models.Address", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CountryId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid?>("MunicipalityId");

                    b.Property<Guid>("PostalCodeId");

                    b.Property<string>("StreetNumber")
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.HasIndex("CountryId")
                        .HasAnnotation("Npgsql:Name", "IX_Add_CountryId");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_Add_Id");

                    b.HasIndex("MunicipalityId")
                        .HasAnnotation("Npgsql:Name", "IX_Add_MunicipalityId");

                    b.HasIndex("PostalCodeId")
                        .HasAnnotation("Npgsql:Name", "IX_Add_PostalCodeId");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.AddressAdditionalInformation", b =>
                {
                    b.Property<Guid>("AddressId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

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

            modelBuilder.Entity("PTV.Database.Model.Models.AddressType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_AddTypNam_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_AddTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_AddTypNam_TypeId");

                    b.ToTable("AddressTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Area", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AreaTypeId");

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_AreInfTypNam_Id");

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

                    b.Property<Guid>("Id");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_AreTypNam_Id");

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

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_AttTypNam_Id");

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

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_Bus_Id");

                    b.ToTable("Business");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Coordinate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AddressId");

                    b.Property<string>("CoordinateState");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<double>("Latitude");

                    b.Property<double>("Longtitude");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_CooTypNam_Id");

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

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_Cou_Id");

                    b.ToTable("Country");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.CountryName", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CountryId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("CountryId")
                        .HasAnnotation("Npgsql:Name", "IX_CouNam_CountryId");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_CouNam_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_CouNam_LocalizationId");

                    b.ToTable("CountryName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.DailyOpeningTime", b =>
                {
                    b.Property<Guid>("OpeningHourId");

                    b.Property<int>("DayFrom");

                    b.Property<bool>("IsExtra");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<int?>("DayTo");

                    b.Property<TimeSpan>("From");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<TimeSpan>("To");

                    b.HasKey("OpeningHourId", "DayFrom", "IsExtra");

                    b.HasIndex("OpeningHourId", "DayFrom", "IsExtra")
                        .HasAnnotation("Npgsql:Name", "IX_DaiOpeTim_OpeningHourId_DayFrom_IsExtra");

                    b.ToTable("DailyOpeningTime");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.DescriptionType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_DesTypNam_Id");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("DigitalAuthorizationId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("DigitalAuthorizationId")
                        .HasAnnotation("Npgsql:Name", "IX_DigAutNam_DigitalAuthorizationId");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_DigAutNam_Id");

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

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_ExcHouStaTypNam_Id");

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

            modelBuilder.Entity("PTV.Database.Model.Models.Form", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

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

            modelBuilder.Entity("PTV.Database.Model.Models.FormType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_ForTypNam_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_ForTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_ForTypNam_TypeId");

                    b.ToTable("FormTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.GeneralDescriptionLanguageAvailability", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionVersionedId");

                    b.Property<Guid>("LanguageId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

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

            modelBuilder.Entity("PTV.Database.Model.Models.IndustrialClass", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Label");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("IndustrialClassId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_IndClaNam_Id");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LanguageId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_LanNam_Id");

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

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

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

                    b.Property<Guid>("Id");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LifeEventId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_LifEveNam_Id");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_NamTypNam_Id");

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

            modelBuilder.Entity("PTV.Database.Model.Models.OntologyTermExactMatch", b =>
                {
                    b.Property<Guid>("OntologyTermId");

                    b.Property<Guid>("ExactMatchId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("OntologyTermId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_OntTerNam_Id");

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

                    b.Property<Guid>("TypeId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("OrganizationVersionedId", "AddressId", "TypeId");

                    b.HasIndex("AddressId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgAdd_AddressId");

                    b.HasIndex("OrganizationVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgAdd_OrganizationVersionedId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgAdd_TypeId");

                    b.ToTable("OrganizationAddress");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationArea", b =>
                {
                    b.Property<Guid>("OrganizationVersionedId");

                    b.Property<Guid>("AreaId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

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

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("OrganizationVersionedId", "MunicipalityId");

                    b.HasIndex("MunicipalityId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgAreMun_MunicipalityId");

                    b.HasIndex("OrganizationVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgAreMun_OrganizationVersionedId");

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

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationEmail", b =>
                {
                    b.Property<Guid>("EmailId");

                    b.Property<Guid>("OrganizationVersionedId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid?>("OrganizationId");

                    b.Property<Guid?>("ProvisionTypeId");

                    b.Property<Guid>("RoleTypeId");

                    b.Property<Guid>("ServiceVersionedId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_OrgSer_Id");

                    b.HasIndex("OrganizationId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgSer_OrganizationId");

                    b.HasIndex("ProvisionTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgSer_ProvisionTypeId");

                    b.HasIndex("RoleTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgSer_RoleTypeId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgSer_ServiceVersionedId");

                    b.ToTable("OrganizationService");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationServiceAdditionalInformation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("OrganizationServiceId");

                    b.Property<string>("Text")
                        .HasMaxLength(150);

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_OrgSerAddInf_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgSerAddInf_LocalizationId");

                    b.HasIndex("OrganizationServiceId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgSerAddInf_OrganizationServiceId");

                    b.ToTable("OrganizationServiceAdditionalInformation");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationServiceWebPage", b =>
                {
                    b.Property<Guid>("OrganizationServiceId");

                    b.Property<Guid>("WebPageId");

                    b.Property<Guid>("TypeId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("OrganizationServiceId", "WebPageId", "TypeId");

                    b.HasIndex("OrganizationServiceId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgSerWebPag_OrganizationServiceId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgSerWebPag_TypeId");

                    b.HasIndex("WebPageId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgSerWebPag_WebPageId");

                    b.ToTable("OrganizationServiceWebPage");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Label");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("OrganizationTypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_OrgTypNam_Id");

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

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid?>("MunicipalityId");

                    b.Property<string>("Oid")
                        .HasMaxLength(100);

                    b.Property<Guid?>("ParentId");

                    b.Property<Guid>("PublishingStatusId");

                    b.Property<Guid>("TypeId");

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

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Number")
                        .HasMaxLength(20);

                    b.Property<Guid?>("PrefixNumberId");

                    b.Property<Guid>("ServiceChargeTypeId");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_Pho_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_Pho_LocalizationId");

                    b.HasIndex("PrefixNumberId")
                        .HasAnnotation("Npgsql:Name", "IX_Pho_PrefixNumberId");

                    b.HasIndex("ServiceChargeTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_Pho_ServiceChargeTypeId");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_PhoNumTypNam_Id");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AddressId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Text")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("AddressId")
                        .HasAnnotation("Npgsql:Name", "IX_PosOffBoxNam_AddressId");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_PosOffBoxNam_Id");

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

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_PriForChaUrlTypNam_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_PriForChaUrlTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_PriForChaUrlTypNam_TypeId");

                    b.ToTable("PrintableFormChannelUrlTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Privileges.AccessRightName", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_AccRigNam_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_AccRigNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_AccRigNam_TypeId");

                    b.ToTable("AccessRightName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Privileges.AccessRightType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_AccRigTyp_Id");

                    b.ToTable("AccessRightType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Privileges.GeneralDescriptionBlockedAccessRight", b =>
                {
                    b.Property<Guid>("AccessBlockedId");

                    b.Property<Guid>("EntityId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("AccessBlockedId", "EntityId");

                    b.HasIndex("AccessBlockedId")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesBloAccRig_AccessBlockedId");

                    b.HasIndex("EntityId")
                        .HasAnnotation("Npgsql:Name", "IX_GenDesBloAccRig_EntityId");

                    b.ToTable("GeneralDescriptionBlockedAccessRight");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Privileges.ChannelBlockedAccessRight", b =>
                {
                    b.Property<Guid>("AccessBlockedId");

                    b.Property<Guid>("EntityId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

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

            modelBuilder.Entity("PTV.Database.Model.Models.Privileges.OrganizationBlockedAccessRight", b =>
                {
                    b.Property<Guid>("AccessBlockedId");

                    b.Property<Guid>("EntityId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("AccessBlockedId", "EntityId");

                    b.HasIndex("AccessBlockedId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgBloAccRig_AccessBlockedId");

                    b.HasIndex("EntityId")
                        .HasAnnotation("Npgsql:Name", "IX_OrgBloAccRig_EntityId");

                    b.ToTable("OrganizationBlockedAccessRight");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Privileges.ServiceBlockedAccessRight", b =>
                {
                    b.Property<Guid>("AccessBlockedId");

                    b.Property<Guid>("EntityId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("AccessBlockedId", "EntityId");

                    b.HasIndex("AccessBlockedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerBloAccRig_AccessBlockedId");

                    b.HasIndex("EntityId")
                        .HasAnnotation("Npgsql:Name", "IX_SerBloAccRig_EntityId");

                    b.ToTable("ServiceBlockedAccessRight");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Privileges.UserAccessRight", b =>
                {
                    b.Property<Guid>("AccessRightId");

                    b.Property<Guid>("UserId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_ProTypNam_Id");

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

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_PubStaTyp_Id");

                    b.ToTable("PublishingStatusType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PublishingStatusTypeName", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_PubStaTypNam_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_PubStaTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_PubStaTypNam_TypeId");

                    b.ToTable("PublishingStatusTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.RoleType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<int?>("OrderNumber");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_RolTyp_Id");

                    b.ToTable("RoleType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.RoleTypeName", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_RolTypNam_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_RolTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_RolTypNam_TypeId");

                    b.ToTable("RoleTypeName");
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

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceVersionedId", "MunicipalityId");

                    b.HasIndex("MunicipalityId")
                        .HasAnnotation("Npgsql:Name", "IX_SerAreMun_MunicipalityId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerAreMun_ServiceVersionedId");

                    b.ToTable("ServiceAreaMunicipality");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceClass", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Label");

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

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceClassName", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("ServiceClassId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerClaNam_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerClaNam_LocalizationId");

                    b.HasIndex("ServiceClassId")
                        .HasAnnotation("Npgsql:Name", "IX_SerClaNam_ServiceClassId");

                    b.ToTable("ServiceClassName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceDescription", b =>
                {
                    b.Property<Guid>("ServiceVersionedId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

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

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceHoursAdditionalInformation", b =>
                {
                    b.Property<Guid>("ServiceChannelServiceHoursId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Text")
                        .HasMaxLength(100);

                    b.HasKey("ServiceChannelServiceHoursId", "LocalizationId");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerHouAddInf_LocalizationId");

                    b.HasIndex("ServiceChannelServiceHoursId")
                        .HasAnnotation("Npgsql:Name", "IX_SerHouAddInf_ServiceChannelServiceHoursId");

                    b.ToTable("ServiceHoursAdditionalInformation");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceHourType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerHouTypNam_Id");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaConTypNam_Id");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("IsClosed");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<DateTime?>("OpeningHoursFrom");

                    b.Property<DateTime?>("OpeningHoursTo");

                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<Guid>("ServiceHourTypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaSerHou_Id");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaSerHou_ServiceChannelVersionedId");

                    b.HasIndex("ServiceHourTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaSerHou_ServiceHourTypeId");

                    b.ToTable("ServiceChannelServiceHours");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelTargetGroup", b =>
                {
                    b.Property<Guid>("ServiceChannelVersionedId");

                    b.Property<Guid>("TargetGroupId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceChannelVersionedId", "TargetGroupId");

                    b.HasIndex("ServiceChannelVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaTarGro_ServiceChannelVersionedId");

                    b.HasIndex("TargetGroupId")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaTarGro_TargetGroupId");

                    b.ToTable("ServiceChannelTargetGroup");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerChaTypNam_Id");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_ServCharTypeName_Id");

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

                    b.Property<Guid>("TypeId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceLocationChannelId", "AddressId", "TypeId");

                    b.HasIndex("AddressId")
                        .HasAnnotation("Npgsql:Name", "IX_SerLocChaAdd_AddressId");

                    b.HasIndex("ServiceLocationChannelId")
                        .HasAnnotation("Npgsql:Name", "IX_SerLocChaAdd_ServiceLocationChannelId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerLocChaAdd_TypeId");

                    b.ToTable("ServiceLocationChannelAddress");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceName", b =>
                {
                    b.Property<Guid>("ServiceVersionedId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

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

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceVersionedId", "OntologyTermId");

                    b.HasIndex("OntologyTermId")
                        .HasAnnotation("Npgsql:Name", "IX_SerOntTer_OntologyTermId");

                    b.HasIndex("ServiceVersionedId")
                        .HasAnnotation("Npgsql:Name", "IX_SerOntTer_ServiceVersionedId");

                    b.ToTable("ServiceOntologyTerm");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceRequirement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

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

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid?>("ServiceChargeTypeId");

                    b.HasKey("ServiceId", "ServiceChannelId");

                    b.HasIndex("ServiceChannelId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerCha_ServiceChannelId");

                    b.HasIndex("ServiceChargeTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerCha_ServiceChargeTypeId");

                    b.HasIndex("ServiceId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerCha_ServiceId");

                    b.ToTable("ServiceServiceChannel");
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

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("DigitalAuthorizationId", "ServiceId", "ServiceChannelId");

                    b.HasIndex("DigitalAuthorizationId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaDigAut_DigitalAuthorizationId");

                    b.HasIndex("ServiceId", "ServiceChannelId")
                        .HasAnnotation("Npgsql:Name", "IX_SerSerChaDigAut_ServiceId_ServiceChannelId");

                    b.ToTable("ServiceServiceChannelDigitalAuthorization");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceTargetGroup", b =>
                {
                    b.Property<Guid>("ServiceVersionedId");

                    b.Property<Guid>("TargetGroupId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

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

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerTypNam_Id");

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

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("ElectronicCommunication");

                    b.Property<bool>("ElectronicNotification");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("PublishingStatusId");

                    b.Property<Guid?>("ServiceChargeTypeId");

                    b.Property<Guid?>("StatutoryServiceGeneralDescriptionId");

                    b.Property<Guid?>("TypeId");

                    b.Property<Guid>("UnificRootId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.Property<Guid?>("VersioningId");

                    b.HasKey("Id");

                    b.HasIndex("AreaInformationTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerVer_AreaInformationTypeId");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_SerVer_Id");

                    b.HasIndex("PublishingStatusId")
                        .HasAnnotation("Npgsql:Name", "IX_SerVer_PublishingStatusId");

                    b.HasIndex("ServiceChargeTypeId")
                        .HasAnnotation("Npgsql:Name", "IX_SerVer_ServiceChargeTypeId");

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

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceDescription", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionVersionedId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AddressId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Text")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("AddressId")
                        .HasAnnotation("Npgsql:Name", "IX_StrNam_AddressId");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_StrNam_Id");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TargetGroupId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_TarGroNam_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_TarGroNam_LocalizationId");

                    b.HasIndex("TargetGroupId")
                        .HasAnnotation("Npgsql:Name", "IX_TarGroNam_TargetGroupId");

                    b.ToTable("TargetGroupName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.UserOrganization", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid?>("OrganizationId");

                    b.Property<string>("RelationId");

                    b.Property<Guid>("UserId");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_UseOrg_Id");

                    b.ToTable("UserOrganization");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Versioning", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

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
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasAnnotation("Npgsql:Name", "IX_WebPagTypNam_Id");

                    b.HasIndex("LocalizationId")
                        .HasAnnotation("Npgsql:Name", "IX_WebPagTypNam_LocalizationId");

                    b.HasIndex("TypeId")
                        .HasAnnotation("Npgsql:Name", "IX_WebPagTypNam_TypeId");

                    b.ToTable("WebPageTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Address", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Country", "Country")
                        .WithMany("Addresses")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Municipality", "Municipality")
                        .WithMany()
                        .HasForeignKey("MunicipalityId");

                    b.HasOne("PTV.Database.Model.Models.PostalCode", "PostalCode")
                        .WithMany("Addresses")
                        .HasForeignKey("PostalCodeId")
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
                    b.HasOne("PTV.Database.Model.Models.ServiceChannelServiceHours", "OpeningHour")
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
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescriptionVersioned", "StatutoryServiceGeneralDescriptionVersioned")
                        .WithMany("LanguageAvailabilities")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionVersionedId")
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

                    b.HasOne("PTV.Database.Model.Models.OrganizationVersioned", "OrganizationVersioned")
                        .WithMany("OrganizationAddresses")
                        .HasForeignKey("OrganizationVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.AddressType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
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
                        .HasForeignKey("OrganizationId");

                    b.HasOne("PTV.Database.Model.Models.ProvisionType", "ProvisionType")
                        .WithMany()
                        .HasForeignKey("ProvisionTypeId");

                    b.HasOne("PTV.Database.Model.Models.RoleType", "RoleType")
                        .WithMany()
                        .HasForeignKey("RoleTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceVersioned", "ServiceVersioned")
                        .WithMany("OrganizationServices")
                        .HasForeignKey("ServiceVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationServiceAdditionalInformation", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.OrganizationService", "OrganizationService")
                        .WithMany("AdditionalInformations")
                        .HasForeignKey("OrganizationServiceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationServiceWebPage", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.OrganizationService", "OrganizationService")
                        .WithMany("WebPages")
                        .HasForeignKey("OrganizationServiceId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.WebPageType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.WebPage", "WebPage")
                        .WithMany("OrganizationServiceWebPages")
                        .HasForeignKey("WebPageId")
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
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);

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
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.DialCode", "PrefixNumber")
                        .WithMany()
                        .HasForeignKey("PrefixNumberId");

                    b.HasOne("PTV.Database.Model.Models.ServiceChargeType", "ServiceChargeType")
                        .WithMany()
                        .HasForeignKey("ServiceChargeTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

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
                    b.HasOne("PTV.Database.Model.Models.Address", "Address")
                        .WithMany("PostOfficeBoxNames")
                        .HasForeignKey("AddressId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.Privileges.AccessRightName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Privileges.AccessRightType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Privileges.GeneralDescriptionBlockedAccessRight", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Privileges.AccessRightType", "AccessBlocked")
                        .WithMany()
                        .HasForeignKey("AccessBlockedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", "Entity")
                        .WithMany()
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Privileges.ChannelBlockedAccessRight", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Privileges.AccessRightType", "AccessBlocked")
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

            modelBuilder.Entity("PTV.Database.Model.Models.Privileges.OrganizationBlockedAccessRight", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Privileges.AccessRightType", "AccessBlocked")
                        .WithMany()
                        .HasForeignKey("AccessBlockedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Organization", "Entity")
                        .WithMany("BlockedAccessRights")
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Privileges.ServiceBlockedAccessRight", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Privileges.AccessRightType", "AccessBlocked")
                        .WithMany()
                        .HasForeignKey("AccessBlockedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Service", "Entity")
                        .WithMany("BlockedAccessRights")
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Privileges.UserAccessRight", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Privileges.AccessRightType", "AccessRight")
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

            modelBuilder.Entity("PTV.Database.Model.Models.RoleTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.RoleType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceClass", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceClass", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");
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

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceHoursAdditionalInformation", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelServiceHours", "ServiceChannelServiceHours")
                        .WithMany("AdditionalInformations")
                        .HasForeignKey("ServiceChannelServiceHoursId")
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
                        .WithMany("ServiceHours")
                        .HasForeignKey("ServiceChannelVersionedId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceHourType", "ServiceHourType")
                        .WithMany()
                        .HasForeignKey("ServiceHourTypeId")
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

                    b.HasOne("PTV.Database.Model.Models.ServiceLocationChannel", "ServiceLocationChannel")
                        .WithMany("Addresses")
                        .HasForeignKey("ServiceLocationChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.AddressType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
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
                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("ServiceServiceChannels")
                        .HasForeignKey("ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChargeType", "ServiceChargeType")
                        .WithMany()
                        .HasForeignKey("ServiceChargeTypeId");

                    b.HasOne("PTV.Database.Model.Models.Service", "Service")
                        .WithMany("ServiceServiceChannels")
                        .HasForeignKey("ServiceId")
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

                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "PublishingStatus")
                        .WithMany()
                        .HasForeignKey("PublishingStatusId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChargeType", "ServiceChargeType")
                        .WithMany()
                        .HasForeignKey("ServiceChargeTypeId");

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
                    b.HasOne("PTV.Database.Model.Models.Address", "Address")
                        .WithMany("StreetNames")
                        .HasForeignKey("AddressId")
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
