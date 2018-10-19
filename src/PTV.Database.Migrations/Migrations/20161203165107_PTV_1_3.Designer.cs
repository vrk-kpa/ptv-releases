using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PTV.Database.DataAccess.ApplicationDbContext;

namespace PTV.Database.Migrations.Migrations
{
    [DbContext(typeof(PtvDbContext))]
    [Migration("20161203165107_PTV_1_3")]
    partial class PTV_1_3
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("Npgsql:PostgresExtension:uuid-ossp", "'uuid-ossp', '', ''")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("PTV.Database.Model.Models.Address", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CoordinateState");

                    b.Property<Guid>("CountryId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<double?>("Latitude");

                    b.Property<double?>("Longtitude");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid?>("MunicipalityId");

                    b.Property<string>("PostOfficeBox");

                    b.Property<Guid>("PostalCodeId");

                    b.Property<string>("StreetNumber");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.HasIndex("Id");

                    b.HasIndex("MunicipalityId");

                    b.HasIndex("PostalCodeId");

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

                    b.Property<string>("Text");

                    b.HasKey("AddressId", "LocalizationId");

                    b.HasIndex("AddressId");

                    b.HasIndex("LocalizationId");

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

                    b.HasIndex("Id");

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

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("TypeId");

                    b.ToTable("AddressTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Attachment", b =>
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

                    b.Property<Guid?>("TypeId");

                    b.Property<string>("Url");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("TypeId");

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

                    b.HasIndex("Id");

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

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("TypeId");

                    b.ToTable("AttachmentTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Business", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("Business");
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

                    b.HasIndex("Id");

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

                    b.HasIndex("CountryId");

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

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

                    b.HasIndex("OpeningHourId", "DayFrom", "IsExtra");

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

                    b.HasIndex("Id");

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

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("TypeId");

                    b.ToTable("DescriptionTypeName");
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

                    b.Property<Guid>("ServiceChannelId");

                    b.Property<int?>("SignatureQuantity");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("ServiceChannelId");

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

                    b.HasIndex("ElectronicChannelId");

                    b.HasIndex("LocalizationId");

                    b.ToTable("ElectronicChannelUrl");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Email", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Value");

                    b.Property<bool>("Visible");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.ToTable("Email");
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

                    b.HasIndex("Id");

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

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("TypeId");

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

                    b.HasIndex("Id");

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

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("TypeId");

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

                    b.HasIndex("Id");

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

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("TypeId");

                    b.ToTable("FormTypeName");
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

                    b.HasIndex("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("IndustrialClass");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.IndustrialClassName", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid?>("IndustrialClassId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("IndustrialClassId");

                    b.HasIndex("LocalizationId");

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

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

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

                    b.HasKey("Id");

                    b.HasIndex("Id");

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

                    b.HasIndex("Id");

                    b.HasIndex("LanguageId");

                    b.HasIndex("LocalizationId");

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

                    b.HasIndex("Id");

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

                    b.Property<string>("Name");

                    b.HasKey("LawId", "LocalizationId");

                    b.HasIndex("LawId");

                    b.HasIndex("LocalizationId");

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

                    b.HasIndex("LawId");

                    b.HasIndex("WebPageId");

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

                    b.HasIndex("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("LifeEvent");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.LifeEventName", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid?>("LifeEventId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("LifeEventId");

                    b.HasIndex("LocalizationId");

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

                    b.HasIndex("Id");

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

                    b.Property<string>("Name");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("Municipality");
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

                    b.HasIndex("Id");

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

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("TypeId");

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

                    b.HasIndex("Id");

                    b.ToTable("OntologyTerm");
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

                    b.Property<Guid?>("OntologyTermId");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("OntologyTermId");

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

                    b.HasIndex("ChildId");

                    b.HasIndex("ParentId");

                    b.ToTable("OntologyTermParent");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Organization", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("BusinessId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("DisplayNameTypeId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid?>("MunicipalityId");

                    b.Property<string>("Oid");

                    b.Property<Guid?>("ParentId");

                    b.Property<Guid?>("PublishingStatusId");

                    b.Property<Guid>("TypeId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("Id");

                    b.HasIndex("BusinessId");

                    b.HasIndex("DisplayNameTypeId");

                    b.HasIndex("Id");

                    b.HasIndex("MunicipalityId");

                    b.HasIndex("Oid");

                    b.HasIndex("ParentId");

                    b.HasIndex("PublishingStatusId");

                    b.HasIndex("TypeId");

                    b.ToTable("Organization");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationAddress", b =>
                {
                    b.Property<Guid>("OrganizationId");

                    b.Property<Guid>("AddressId");

                    b.Property<Guid>("TypeId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("OrganizationId", "AddressId", "TypeId");

                    b.HasIndex("AddressId");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("TypeId");

                    b.ToTable("OrganizationAddress");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationDescription", b =>
                {
                    b.Property<Guid>("OrganizationId");

                    b.Property<Guid>("TypeId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<bool>("Visible");

                    b.HasKey("OrganizationId", "TypeId");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("TypeId");

                    b.ToTable("OrganizationDescription");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationEmail", b =>
                {
                    b.Property<Guid>("EmailId");

                    b.Property<Guid>("OrganizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("EmailId", "OrganizationId");

                    b.HasIndex("EmailId");

                    b.HasIndex("OrganizationId");

                    b.ToTable("OrganizationEmail");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationName", b =>
                {
                    b.Property<Guid>("OrganizationId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<bool>("Visible");

                    b.HasKey("OrganizationId", "TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("TypeId");

                    b.ToTable("OrganizationName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationPhone", b =>
                {
                    b.Property<Guid>("PhoneId");

                    b.Property<Guid>("OrganizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("PhoneId", "OrganizationId");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("PhoneId");

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

                    b.Property<Guid>("ServiceId");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("ProvisionTypeId");

                    b.HasIndex("RoleTypeId");

                    b.HasIndex("ServiceId");

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

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("OrganizationServiceId");

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

                    b.HasIndex("OrganizationServiceId");

                    b.HasIndex("TypeId");

                    b.HasIndex("WebPageId");

                    b.ToTable("OrganizationServiceWebPage");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationType", b =>
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

                    b.HasIndex("Id");

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

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("TypeId");

                    b.ToTable("OrganizationTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationWebPage", b =>
                {
                    b.Property<Guid>("OrganizationId");

                    b.Property<Guid>("WebPageId");

                    b.Property<Guid>("TypeId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("OrganizationId", "WebPageId", "TypeId");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("TypeId");

                    b.HasIndex("WebPageId");

                    b.ToTable("OrganizationWebPage");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Phone", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AdditionalInformation");

                    b.Property<string>("ChargeDescription");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Number");

                    b.Property<string>("PrefixNumber");

                    b.Property<Guid>("ServiceChargeTypeId");

                    b.Property<Guid>("TypeId");

                    b.Property<bool>("Visible");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("ServiceChargeTypeId");

                    b.HasIndex("TypeId");

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

                    b.HasIndex("Id");

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

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("TypeId");

                    b.ToTable("PhoneNumberTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PostalCode", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid?>("MunicipalityId");

                    b.Property<string>("PostOffice");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("MunicipalityId");

                    b.ToTable("PostalCode");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.PrintableFormChannel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<Guid?>("DeliveryAddressId");

                    b.Property<string>("FormIdentifier");

                    b.Property<string>("FormReceiver");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("ServiceChannelId");

                    b.HasKey("Id");

                    b.HasIndex("DeliveryAddressId");

                    b.HasIndex("Id");

                    b.HasIndex("ServiceChannelId");

                    b.ToTable("PrintableFormChannel");
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

                    b.Property<string>("Url");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("PrintableFormChannelId");

                    b.HasIndex("TypeId");

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

                    b.HasIndex("Id");

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

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("TypeId");

                    b.ToTable("PrintableFormChannelUrlTypeName");
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

                    b.HasIndex("Id");

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

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("TypeId");

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

                    b.HasIndex("Id");

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

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("TypeId");

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

                    b.HasIndex("Id");

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

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("TypeId");

                    b.ToTable("RoleTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.Service", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<bool>("ElectronicCommunication");

                    b.Property<bool>("ElectronicNotification");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid?>("PublishingStatusId");

                    b.Property<Guid?>("ServiceChargeTypeId");

                    b.Property<Guid?>("ServiceCoverageTypeId");

                    b.Property<Guid?>("StatutoryServiceGeneralDescriptionId");

                    b.Property<Guid>("TypeId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.Property<Guid?>("VersioningId");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("PublishingStatusId");

                    b.HasIndex("ServiceChargeTypeId");

                    b.HasIndex("ServiceCoverageTypeId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId");

                    b.HasIndex("TypeId");

                    b.HasIndex("VersioningId");

                    b.ToTable("Service");
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

                    b.HasIndex("Id");

                    b.HasIndex("ParentId");

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

                    b.Property<Guid?>("ServiceClassId");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("ServiceClassId");

                    b.ToTable("ServiceClassName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceCoverageType", b =>
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

                    b.HasIndex("Id");

                    b.ToTable("ServiceCoverageType");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceCoverageTypeName", b =>
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

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("TypeId");

                    b.ToTable("ServiceCoverageTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceDescription", b =>
                {
                    b.Property<Guid>("ServiceId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<bool>("Visible");

                    b.HasKey("ServiceId", "TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("ServiceId");

                    b.HasIndex("TypeId");

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

                    b.Property<Guid>("ServiceId");

                    b.Property<bool>("Visible");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("ServiceId");

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

                    b.Property<Guid>("ServiceId");

                    b.Property<bool>("Visible");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("ServiceId");

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

                    b.Property<string>("Text");

                    b.HasKey("ServiceChannelServiceHoursId", "LocalizationId");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("ServiceChannelServiceHoursId");

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

                    b.HasIndex("Id");

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

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("TypeId");

                    b.ToTable("ServiceHourTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Charge");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("OrganizationId");

                    b.Property<Guid?>("PublishingStatusId");

                    b.Property<Guid>("TypeId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("PublishingStatusId");

                    b.HasIndex("TypeId");

                    b.ToTable("ServiceChannel");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelAttachment", b =>
                {
                    b.Property<Guid>("ServiceChannelId");

                    b.Property<Guid>("AttachmentId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceChannelId", "AttachmentId");

                    b.HasIndex("AttachmentId");

                    b.HasIndex("ServiceChannelId");

                    b.ToTable("ServiceChannelAttachment");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelDescription", b =>
                {
                    b.Property<Guid>("ServiceChannelId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<bool>("Visible");

                    b.HasKey("ServiceChannelId", "TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("ServiceChannelId");

                    b.HasIndex("TypeId");

                    b.ToTable("ServiceChannelDescription");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelEmail", b =>
                {
                    b.Property<Guid>("EmailId");

                    b.Property<Guid>("ServiceChannelId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("EmailId", "ServiceChannelId");

                    b.HasIndex("EmailId");

                    b.HasIndex("ServiceChannelId");

                    b.ToTable("ServiceChannelEmail");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelKeyword", b =>
                {
                    b.Property<Guid>("ServiceChannelId");

                    b.Property<Guid>("KeywordId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceChannelId", "KeywordId");

                    b.HasIndex("KeywordId");

                    b.HasIndex("ServiceChannelId");

                    b.ToTable("ServiceChannelKeyword");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelLanguage", b =>
                {
                    b.Property<Guid>("ServiceChannelId");

                    b.Property<Guid>("LanguageId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceChannelId", "LanguageId");

                    b.HasIndex("LanguageId");

                    b.HasIndex("ServiceChannelId");

                    b.ToTable("ServiceChannelLanguage");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelName", b =>
                {
                    b.Property<Guid>("ServiceChannelId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<bool>("Visible");

                    b.HasKey("ServiceChannelId", "TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("ServiceChannelId");

                    b.HasIndex("TypeId");

                    b.ToTable("ServiceChannelName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelOntologyTerm", b =>
                {
                    b.Property<Guid>("ServiceChannelId");

                    b.Property<Guid>("OntologyTermId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceChannelId", "OntologyTermId");

                    b.HasIndex("OntologyTermId");

                    b.HasIndex("ServiceChannelId");

                    b.ToTable("ServiceChannelOntologyTerm");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelPhone", b =>
                {
                    b.Property<Guid>("PhoneId");

                    b.Property<Guid>("ServiceChannelId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("PhoneId", "ServiceChannelId");

                    b.HasIndex("PhoneId");

                    b.HasIndex("ServiceChannelId");

                    b.ToTable("ServiceChannelPhone");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelServiceClass", b =>
                {
                    b.Property<Guid>("ServiceChannelId");

                    b.Property<Guid>("ServiceClassId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceChannelId", "ServiceClassId");

                    b.HasIndex("ServiceChannelId");

                    b.HasIndex("ServiceClassId");

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

                    b.Property<Guid>("ServiceChannelId");

                    b.Property<Guid>("ServiceHourTypeId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("ServiceChannelId");

                    b.HasIndex("ServiceHourTypeId");

                    b.ToTable("ServiceChannelServiceHours");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelTargetGroup", b =>
                {
                    b.Property<Guid>("ServiceChannelId");

                    b.Property<Guid>("TargetGroupId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceChannelId", "TargetGroupId");

                    b.HasIndex("ServiceChannelId");

                    b.HasIndex("TargetGroupId");

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

                    b.HasIndex("Id");

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

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("TypeId");

                    b.ToTable("ServiceChannelTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelWebPage", b =>
                {
                    b.Property<Guid>("ServiceChannelId");

                    b.Property<Guid>("WebPageId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("TypeId");

                    b.HasKey("ServiceChannelId", "WebPageId");

                    b.HasIndex("ServiceChannelId");

                    b.HasIndex("TypeId");

                    b.HasIndex("WebPageId");

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

                    b.HasIndex("Id");

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

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("TypeId");

                    b.ToTable("ServiceChargeTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceIndustrialClass", b =>
                {
                    b.Property<Guid>("ServiceId");

                    b.Property<Guid>("IndustrialClassId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceId", "IndustrialClassId");

                    b.HasIndex("IndustrialClassId");

                    b.HasIndex("ServiceId");

                    b.ToTable("ServiceIndustrialClass");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceKeyword", b =>
                {
                    b.Property<Guid>("ServiceId");

                    b.Property<Guid>("KeywordId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceId", "KeywordId");

                    b.HasIndex("KeywordId");

                    b.HasIndex("ServiceId");

                    b.ToTable("ServiceKeyword");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceLanguage", b =>
                {
                    b.Property<Guid>("ServiceId");

                    b.Property<Guid>("LanguageId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceId", "LanguageId");

                    b.HasIndex("LanguageId");

                    b.HasIndex("ServiceId");

                    b.ToTable("ServiceLanguage");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceLifeEvent", b =>
                {
                    b.Property<Guid>("ServiceId");

                    b.Property<Guid>("LifeEventId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceId", "LifeEventId");

                    b.HasIndex("LifeEventId");

                    b.HasIndex("ServiceId");

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

                    b.Property<bool>("ServiceAreaRestricted");

                    b.Property<Guid>("ServiceChannelId");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("ServiceChannelId");

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

                    b.HasIndex("AddressId");

                    b.HasIndex("ServiceLocationChannelId");

                    b.HasIndex("TypeId");

                    b.ToTable("ServiceLocationChannelAddress");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceLocationChannelServiceArea", b =>
                {
                    b.Property<Guid>("ServiceLocationChannelId");

                    b.Property<Guid>("MunicipalityId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceLocationChannelId", "MunicipalityId");

                    b.HasIndex("MunicipalityId");

                    b.HasIndex("ServiceLocationChannelId");

                    b.ToTable("ServiceLocationChannelServiceArea");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceMunicipality", b =>
                {
                    b.Property<Guid>("ServiceId");

                    b.Property<Guid>("MunicipalityId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceId", "MunicipalityId");

                    b.HasIndex("MunicipalityId");

                    b.HasIndex("ServiceId");

                    b.ToTable("ServiceMunicipality");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceName", b =>
                {
                    b.Property<Guid>("ServiceId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.Property<bool>("Visible");

                    b.HasKey("ServiceId", "TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("ServiceId");

                    b.HasIndex("TypeId");

                    b.ToTable("ServiceName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceOntologyTerm", b =>
                {
                    b.Property<Guid>("ServiceId");

                    b.Property<Guid>("OntologyTermId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceId", "OntologyTermId");

                    b.HasIndex("OntologyTermId");

                    b.HasIndex("ServiceId");

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

                    b.Property<Guid>("ServiceId");

                    b.Property<bool>("Visible");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("ServiceId");

                    b.ToTable("ServiceRequirement");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceClass", b =>
                {
                    b.Property<Guid>("ServiceId");

                    b.Property<Guid>("ServiceClassId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceId", "ServiceClassId");

                    b.HasIndex("ServiceClassId");

                    b.HasIndex("ServiceId");

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

                    b.HasIndex("ServiceChannelId");

                    b.HasIndex("ServiceChargeTypeId");

                    b.HasIndex("ServiceId");

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

                    b.Property<string>("Description");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("TypeId", "LocalizationId", "ServiceChannelId", "ServiceId");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("TypeId");

                    b.HasIndex("ServiceId", "ServiceChannelId");

                    b.ToTable("ServiceServiceChannelDescription");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceTargetGroup", b =>
                {
                    b.Property<Guid>("ServiceId");

                    b.Property<Guid>("TargetGroupId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("ServiceId", "TargetGroupId");

                    b.HasIndex("ServiceId");

                    b.HasIndex("TargetGroupId");

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

                    b.HasIndex("Id");

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

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("TypeId");

                    b.ToTable("ServiceTypeName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceWebPage", b =>
                {
                    b.Property<Guid>("ServiceId");

                    b.Property<Guid>("WebPageId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<Guid>("TypeId");

                    b.HasKey("ServiceId", "WebPageId");

                    b.HasIndex("ServiceId");

                    b.HasIndex("TypeId");

                    b.HasIndex("WebPageId");

                    b.ToTable("ServiceWebPage");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceDescription", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Description");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("StatutoryServiceGeneralDescriptionId", "TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId");

                    b.HasIndex("TypeId");

                    b.ToTable("StatutoryServiceDescription");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("ChargeTypeId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("ReferenceCode");

                    b.Property<Guid>("TypeId");

                    b.HasKey("Id");

                    b.HasIndex("ChargeTypeId");

                    b.HasIndex("Id");

                    b.HasIndex("TypeId");

                    b.ToTable("StatutoryServiceGeneralDescription");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceIndustrialClass", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionId");

                    b.Property<Guid>("IndustrialClassId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("StatutoryServiceGeneralDescriptionId", "IndustrialClassId");

                    b.HasIndex("IndustrialClassId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId");

                    b.ToTable("StatutoryServiceIndustrialClass");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceLanguage", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionId");

                    b.Property<Guid>("LanguageId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("StatutoryServiceGeneralDescriptionId", "LanguageId");

                    b.HasIndex("LanguageId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId");

                    b.ToTable("StatutoryServiceLanguage");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceLaw", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionId");

                    b.Property<Guid>("LawId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("StatutoryServiceGeneralDescriptionId", "LawId");

                    b.HasIndex("LawId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId");

                    b.ToTable("StatutoryServiceLaw");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceLifeEvent", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionId");

                    b.Property<Guid>("LifeEventId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("StatutoryServiceGeneralDescriptionId", "LifeEventId");

                    b.HasIndex("LifeEventId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId");

                    b.ToTable("StatutoryServiceLifeEvent");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceName", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionId");

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("LocalizationId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.Property<string>("Name");

                    b.HasKey("StatutoryServiceGeneralDescriptionId", "TypeId", "LocalizationId");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId");

                    b.HasIndex("TypeId");

                    b.ToTable("StatutoryServiceName");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceOntologyTerm", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionId");

                    b.Property<Guid>("OntologyTermId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("StatutoryServiceGeneralDescriptionId", "OntologyTermId");

                    b.HasIndex("OntologyTermId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId");

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

                    b.Property<string>("Requirement")
                        .HasMaxLength(2500);

                    b.Property<Guid>("StatutoryServiceGeneralDescriptionId");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId");

                    b.ToTable("StatutoryServiceRequirement");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceServiceClass", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionId");

                    b.Property<Guid>("ServiceClassId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("StatutoryServiceGeneralDescriptionId", "ServiceClassId");

                    b.HasIndex("ServiceClassId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId");

                    b.ToTable("StatutoryServiceServiceClass");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceTargetGroup", b =>
                {
                    b.Property<Guid>("StatutoryServiceGeneralDescriptionId");

                    b.Property<Guid>("TargetGroupId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("ModifiedBy");

                    b.HasKey("StatutoryServiceGeneralDescriptionId", "TargetGroupId");

                    b.HasIndex("StatutoryServiceGeneralDescriptionId");

                    b.HasIndex("TargetGroupId");

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

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

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

                    b.HasIndex("Id");

                    b.HasIndex("ParentId");

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

                    b.Property<Guid?>("TargetGroupId");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("TargetGroupId");

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

                    b.Property<string>("RelationId");

                    b.Property<Guid>("UserId");

                    b.Property<string>("UserName");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("Id");

                    b.HasIndex("Id");

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

                    b.HasIndex("Id");

                    b.HasIndex("PreviousVersionId");

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

                    b.Property<string>("Name");

                    b.Property<int?>("OrderNumber");

                    b.Property<string>("Url");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

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

                    b.Property<Guid>("ServiceChannelId");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("ServiceChannelId");

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

                    b.HasIndex("LocalizationId");

                    b.HasIndex("WebpageChannelId");

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

                    b.HasIndex("Id");

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

                    b.HasIndex("Id");

                    b.HasIndex("LocalizationId");

                    b.HasIndex("TypeId");

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

            modelBuilder.Entity("PTV.Database.Model.Models.ElectronicChannel", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("ElectronicChannels")
                        .HasForeignKey("ServiceChannelId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.IndustrialClass", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.IndustrialClass", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.IndustrialClassName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.IndustrialClass")
                        .WithMany("Names")
                        .HasForeignKey("IndustrialClassId");

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
                        .WithMany("LanguageNames")
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
                    b.HasOne("PTV.Database.Model.Models.LifeEvent")
                        .WithMany("Names")
                        .HasForeignKey("LifeEventId");

                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.OntologyTermName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.OntologyTerm")
                        .WithMany("Names")
                        .HasForeignKey("OntologyTermId");
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

            modelBuilder.Entity("PTV.Database.Model.Models.Organization", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Business", "Business")
                        .WithMany()
                        .HasForeignKey("BusinessId");

                    b.HasOne("PTV.Database.Model.Models.NameType", "DisplayNameType")
                        .WithMany()
                        .HasForeignKey("DisplayNameTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Municipality", "Municipality")
                        .WithMany("Organizations")
                        .HasForeignKey("MunicipalityId");

                    b.HasOne("PTV.Database.Model.Models.Organization", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "PublishingStatus")
                        .WithMany()
                        .HasForeignKey("PublishingStatusId");

                    b.HasOne("PTV.Database.Model.Models.OrganizationType", "Type")
                        .WithMany("Organization")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationAddress", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Address", "Address")
                        .WithMany("OrganizationAddresses")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Organization", "Organization")
                        .WithMany("OrganizationAddresses")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.AddressType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationDescription", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Organization", "Organization")
                        .WithMany("OrganizationDescriptions")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.DescriptionType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationEmail", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Email", "Email")
                        .WithMany()
                        .HasForeignKey("EmailId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Organization", "Organization")
                        .WithMany("OrganizationEmails")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Organization", "Organization")
                        .WithMany("OrganizationNames")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.NameType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationPhone", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Organization", "Organization")
                        .WithMany("OrganizationPhones")
                        .HasForeignKey("OrganizationId")
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

                    b.HasOne("PTV.Database.Model.Models.Service", "Service")
                        .WithMany("OrganizationServices")
                        .HasForeignKey("ServiceId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.OrganizationType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.OrganizationWebPage", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Organization", "Organization")
                        .WithMany("OrganizationWebAddress")
                        .HasForeignKey("OrganizationId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.PrintableFormChannel", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Address", "DeliveryAddress")
                        .WithMany()
                        .HasForeignKey("DeliveryAddressId");

                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("PrintableFormChannels")
                        .HasForeignKey("ServiceChannelId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.Service", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "PublishingStatus")
                        .WithMany()
                        .HasForeignKey("PublishingStatusId");

                    b.HasOne("PTV.Database.Model.Models.ServiceChargeType", "ServiceChargeType")
                        .WithMany()
                        .HasForeignKey("ServiceChargeTypeId");

                    b.HasOne("PTV.Database.Model.Models.ServiceCoverageType", "ServiceCoverageType")
                        .WithMany()
                        .HasForeignKey("ServiceCoverageTypeId");

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", "StatutoryServiceGeneralDescription")
                        .WithMany()
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId");

                    b.HasOne("PTV.Database.Model.Models.ServiceType", "Type")
                        .WithMany("Service")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Versioning", "Versioning")
                        .WithMany()
                        .HasForeignKey("VersioningId");
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

                    b.HasOne("PTV.Database.Model.Models.ServiceClass")
                        .WithMany("Names")
                        .HasForeignKey("ServiceClassId");
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceCoverageTypeName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceCoverageType", "Type")
                        .WithMany("Names")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceDescription", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Service", "Service")
                        .WithMany("ServiceDescriptions")
                        .HasForeignKey("ServiceId")
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

                    b.HasOne("PTV.Database.Model.Models.Service", "Service")
                        .WithMany("ServiceElectronicCommunicationChannels")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceElectronicNotificationChannel", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Service", "Service")
                        .WithMany("ServiceElectronicNotificationChannels")
                        .HasForeignKey("ServiceId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannel", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Organization", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.PublishingStatusType", "PublishingStatus")
                        .WithMany()
                        .HasForeignKey("PublishingStatusId");

                    b.HasOne("PTV.Database.Model.Models.ServiceChannelType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelAttachment", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Attachment", "Attachment")
                        .WithMany("ServiceChannelAttachments")
                        .HasForeignKey("AttachmentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("Attachments")
                        .HasForeignKey("ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelDescription", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("ServiceChannelDescriptions")
                        .HasForeignKey("ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.DescriptionType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelEmail", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Email", "Email")
                        .WithMany()
                        .HasForeignKey("EmailId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("Emails")
                        .HasForeignKey("ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelKeyword", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Keyword", "Keyword")
                        .WithMany()
                        .HasForeignKey("KeywordId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("Keywords")
                        .HasForeignKey("ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelLanguage", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("Languages")
                        .HasForeignKey("ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("ServiceChannelNames")
                        .HasForeignKey("ServiceChannelId")
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

                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("OntologyTerms")
                        .HasForeignKey("ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelPhone", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Phone", "Phone")
                        .WithMany("ServiceChannelPhones")
                        .HasForeignKey("PhoneId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("Phones")
                        .HasForeignKey("ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelServiceClass", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("ServiceClasses")
                        .HasForeignKey("ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceClass", "ServiceClass")
                        .WithMany()
                        .HasForeignKey("ServiceClassId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelServiceHours", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("ServiceHours")
                        .HasForeignKey("ServiceChannelId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceHourType", "ServiceHourType")
                        .WithMany()
                        .HasForeignKey("ServiceHourTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelTargetGroup", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("TargetGroups")
                        .HasForeignKey("ServiceChannelId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceChannelWebPage", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("WebPages")
                        .HasForeignKey("ServiceChannelId")
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

                    b.HasOne("PTV.Database.Model.Models.Service", "Service")
                        .WithMany("ServiceIndustrialClasses")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceKeyword", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Keyword", "Keyword")
                        .WithMany("ServiceKeywords")
                        .HasForeignKey("KeywordId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Service", "Service")
                        .WithMany("ServiceKeywords")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceLanguage", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Service", "Service")
                        .WithMany("ServiceLanguages")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceLifeEvent", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.LifeEvent", "LifeEvent")
                        .WithMany("ServiceLifeEvents")
                        .HasForeignKey("LifeEventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Service", "Service")
                        .WithMany("ServiceLifeEvents")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceLocationChannel", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("ServiceLocationChannels")
                        .HasForeignKey("ServiceChannelId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceLocationChannelServiceArea", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Municipality", "Municipality")
                        .WithMany()
                        .HasForeignKey("MunicipalityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.ServiceLocationChannel", "ServiceLocationChannel")
                        .WithMany("ServiceAreas")
                        .HasForeignKey("ServiceLocationChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceMunicipality", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Municipality", "Municipality")
                        .WithMany("ServiceMunicipalities")
                        .HasForeignKey("MunicipalityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Service", "Service")
                        .WithMany("ServiceMunicipalities")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Service", "Service")
                        .WithMany("ServiceNames")
                        .HasForeignKey("ServiceId")
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

                    b.HasOne("PTV.Database.Model.Models.Service", "Service")
                        .WithMany("ServiceOntologyTerms")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceRequirement", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Service", "Service")
                        .WithMany("ServiceRequirements")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceServiceClass", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceClass", "ServiceClass")
                        .WithMany("ServiceServiceClasses")
                        .HasForeignKey("ServiceClassId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.Service", "Service")
                        .WithMany("ServiceServiceClasses")
                        .HasForeignKey("ServiceId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceTargetGroup", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Service", "Service")
                        .WithMany("ServiceTargetGroups")
                        .HasForeignKey("ServiceId")
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

            modelBuilder.Entity("PTV.Database.Model.Models.ServiceWebPage", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Service", "Service")
                        .WithMany("ServiceWebpages")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.WebPageType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
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

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", "StatutoryServiceGeneralDescription")
                        .WithMany("Descriptions")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.DescriptionType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceChargeType", "ChargeType")
                        .WithMany()
                        .HasForeignKey("ChargeTypeId");

                    b.HasOne("PTV.Database.Model.Models.ServiceType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceIndustrialClass", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.IndustrialClass", "IndustrialClass")
                        .WithMany()
                        .HasForeignKey("IndustrialClassId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", "StatutoryServiceGeneralDescription")
                        .WithMany("IndustrialClasses")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceLanguage", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", "StatutoryServiceGeneralDescription")
                        .WithMany("Languages")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceLaw", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Law", "Law")
                        .WithMany()
                        .HasForeignKey("LawId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", "StatutoryServiceGeneralDescription")
                        .WithMany("StatutoryServiceLaws")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceLifeEvent", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.LifeEvent", "LifeEvent")
                        .WithMany()
                        .HasForeignKey("LifeEventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", "StatutoryServiceGeneralDescription")
                        .WithMany("LifeEvents")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceName", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", "StatutoryServiceGeneralDescription")
                        .WithMany("Names")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId")
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

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", "StatutoryServiceGeneralDescription")
                        .WithMany("OntologyTerms")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceRequirement", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.Language", "Localization")
                        .WithMany()
                        .HasForeignKey("LocalizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", "StatutoryServiceGeneralDescription")
                        .WithMany("StatutoryServiceRequirements")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceServiceClass", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.ServiceClass", "ServiceClass")
                        .WithMany()
                        .HasForeignKey("ServiceClassId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", "StatutoryServiceGeneralDescription")
                        .WithMany("ServiceClasses")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PTV.Database.Model.Models.StatutoryServiceTargetGroup", b =>
                {
                    b.HasOne("PTV.Database.Model.Models.StatutoryServiceGeneralDescription", "StatutoryServiceGeneralDescription")
                        .WithMany("TargetGroups")
                        .HasForeignKey("StatutoryServiceGeneralDescriptionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PTV.Database.Model.Models.TargetGroup", "TargetGroup")
                        .WithMany()
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

                    b.HasOne("PTV.Database.Model.Models.TargetGroup")
                        .WithMany("Names")
                        .HasForeignKey("TargetGroupId");
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
                    b.HasOne("PTV.Database.Model.Models.ServiceChannel", "ServiceChannel")
                        .WithMany("WebpageChannels")
                        .HasForeignKey("ServiceChannelId")
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
