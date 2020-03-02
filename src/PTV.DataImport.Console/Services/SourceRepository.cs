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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PTV.DataImport.Console.DataAccess;
using PTV.DataImport.Console.Models;

namespace PTV.DataImport.Console.Services
{
    /// <summary>
    /// Source repository implementation (palvelunäkymä).
    /// </summary>
    public class SourceRepository : ISourceRepository
    {
        private readonly SourceDbContext dbContext;
        private readonly ILogger<ISourceRepository> logger;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="dbContext">db context to be used to access the database</param>
        /// <param name="logger">logger where to write log entries</param>
        public SourceRepository(SourceDbContext dbContext, ILogger<ISourceRepository> logger)
        {
            dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            this.dbContext = dbContext;
            this.logger = logger;
        }

        #region Organization

        /// <summary>
        /// Get all organizations.
        /// </summary>
        /// <returns>list of organizations</returns>
        public List<SourceOrganizationEntity> GetOrganizations()
        {
            var orgs = new List<SourceOrganizationEntity>(10);

            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = CommandType.Text;
                command.CommandText = "select jsonobject from organization_entity";

                EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var entity = JsonHelper.Deserialize<SourceOrganizationEntity>(reader.GetString(0));

                            if (entity != null)
                            {
                                orgs.Add(entity);
                            }
                        }
                    }
                    else
                    {
                        logger.LogWarning("No organizations found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return orgs;
        }

        public List<JObject> GetOrganizationsJson()
        {
            var jsonObjects = new List<JObject>(15);

            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = CommandType.Text;
                command.CommandText = "select jsonobject from organization_entity";

                EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var jsonData = reader.GetValue(0) as string;

                            if (!string.IsNullOrWhiteSpace(jsonData))
                            {
                                jsonObjects.Add(JObject.Parse(jsonData));
                            }
                        }
                    }
                    else
                    {
                        logger.LogWarning("No organizations found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return jsonObjects;
        }

        /// <summary>
        /// Gets organization ids from source repository.
        /// </summary>
        /// <returns>int list of organization ids</returns>
        public List<int> GetOrganizationIds()
        {
            var orgIds = new List<int>(10);

            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = CommandType.Text;
                command.CommandText = "select (jsonobject->>'id')::bigint from organization_entity";

                EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            orgIds.Add(reader.GetInt32(0));
                        }
                    }
                    else
                    {
                        logger.LogWarning("No organizations found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return orgIds;
        }

        /// <summary>
        /// Get organization with id.
        /// </summary>
        /// <param name="organizationId">organization id</param>
        /// <returns>organization or null if the organization wasn't found with given organization id</returns>
        public SourceOrganizationEntity GetOrganization(int organizationId)
        {
            SourceOrganizationEntity entity = null;

            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = CommandType.Text;
                command.CommandText = "select jsonobject from organization_entity where id = @orgid";

                var param = command.CreateParameter();
                param.DbType = DbType.Int64;
                param.Direction = ParameterDirection.Input;
                param.ParameterName = "@orgid";
                param.Value = organizationId;

                command.Parameters.Add(param);

                EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            entity = JsonHelper.Deserialize<SourceOrganizationEntity>(reader.GetString(0));
                        }
                        else
                        {
                            logger.LogError($"Failed to read organization with id '{organizationId}' from result.");
                        }
                    }
                    else
                    {
                        logger.LogWarning($"No organizations found from source database with id: '{organizationId}'.");
                    }
                }

                command.Connection.Close();
            }

            return entity;
        }

        #endregion

        #region Phone entity

        /// <summary>
        /// Get all phone entities.
        /// </summary>
        /// <returns>list of phone entities</returns>
        public List<SourcePhoneEntity> GetPhoneEntities()
        {
            var entities = new List<SourcePhoneEntity>(10);

            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = CommandType.Text;
                command.CommandText = "select jsonobject from phone_entity";

                EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var entity = JsonHelper.Deserialize<SourcePhoneEntity>(reader.GetString(0));

                            if (entity != null)
                            {
                                entities.Add(entity);
                            }
                        }
                    }
                    else
                    {
                        logger.LogWarning("No phone entities found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return entities;
        }

        public List<JObject> GetPhoneEntitiesJson()
        {
            var entities = new List<JObject>(10);

            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = CommandType.Text;
                command.CommandText = "select jsonobject from phone_entity";

                EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var jsonData = reader.GetValue(0) as string;

                            if (!string.IsNullOrWhiteSpace(jsonData))
                            {
                                entities.Add(JObject.Parse(jsonData));
                            }
                        }
                    }
                    else
                    {
                        logger.LogWarning("No phone entities found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return entities;
        }

        #endregion

        #region Service entity

        /// <summary>
        /// Get all services.
        /// </summary>
        /// <returns>list of service entities</returns>
        public List<SourceServiceEntity> GetServices()
        {
            var entities = new List<SourceServiceEntity>(30);

            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = CommandType.Text;
                command.CommandText = "select jsonobject from service_entity";

                EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var entity = JsonHelper.Deserialize<SourceServiceEntity>(reader.GetString(0));

                            if (entity != null)
                            {
                                entities.Add(entity);
                            }
                        }
                    }
                    else
                    {
                        logger.LogWarning("No service entities found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return entities;
        }

        public List<JObject> GetServicesJson()
        {
            var entities = new List<JObject>(30);

            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = CommandType.Text;
                command.CommandText = "select jsonobject from service_entity";

                EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var jsonData = reader.GetValue(0) as string;

                            if (!string.IsNullOrWhiteSpace(jsonData))
                            {
                                entities.Add(JObject.Parse(jsonData));
                            }
                        }
                    }
                    else
                    {
                        logger.LogWarning("No service entities found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return entities;
        }

        #endregion

        #region Transaction form entity

        /// <summary>
        /// Get all transaction forms.
        /// </summary>
        /// <returns>list of transaction form entities</returns>
        public List<SourceTransactionFormEntity> GetTransactionForms()
        {
            var entities = new List<SourceTransactionFormEntity>(30);

            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = CommandType.Text;
                command.CommandText = "select jsonobject from transaction_form_entity";

                EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var entity = JsonHelper.Deserialize<SourceTransactionFormEntity>(reader.GetString(0));

                            if (entity != null)
                            {
                                entities.Add(entity);
                            }
                        }
                    }
                    else
                    {
                        logger.LogWarning("No transaction form entities found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return entities;
        }

        public List<JObject> GetTransactionFormsJson()
        {
            var entities = new List<JObject>(30);

            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = CommandType.Text;
                command.CommandText = "select jsonobject from transaction_form_entity";

                EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var jsonData = reader.GetValue(0) as string;

                            if (!string.IsNullOrWhiteSpace(jsonData))
                            {
                                entities.Add(JObject.Parse(jsonData));
                            }
                        }
                    }
                    else
                    {
                        logger.LogWarning("No transaction form entities found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return entities;
        }

        #endregion

        #region Office entity

        /// <summary>
        /// Get all offices.
        /// </summary>
        /// <returns>list of office entities</returns>
        public List<SourceOfficeEntity> GetOffices()
        {
            var entities = new List<SourceOfficeEntity>(50);

            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = CommandType.Text;
                command.CommandText = "select jsonobject from office_entity";

                EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var entity = JsonHelper.Deserialize<SourceOfficeEntity>(reader.GetString(0));

                            if (entity != null)
                            {
                                entities.Add(entity);
                            }
                        }
                    }
                    else
                    {
                        logger.LogWarning("No office entities found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return entities;
        }

        public List<JObject> GetOfficesJson()
        {
            var entities = new List<JObject>(50);

            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = CommandType.Text;
                command.CommandText = "select jsonobject from office_entity";

                EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var jsonData = reader.GetValue(0) as string;

                            if (!string.IsNullOrWhiteSpace(jsonData))
                            {
                                entities.Add(JObject.Parse(jsonData));
                            }
                        }
                    }
                    else
                    {
                        logger.LogWarning("No office entities found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return entities;
        }

        #endregion

        #region General descriptioon entity

        /// <summary>
        /// Get all general descriptions.
        /// </summary>
        /// <returns>list of general description entities</returns>
        public List<SourceGeneralDescription> GetGeneralDescriptions()
        {
            var entities = new List<SourceGeneralDescription>(20);

            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = CommandType.Text;
                command.CommandText = "select jsonobject from general_description_entity";

                EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var entity = JsonHelper.Deserialize<SourceGeneralDescription>(reader.GetString(0));

                            if (entity != null)
                            {
                                entities.Add(entity);
                            }
                        }
                    }
                    else
                    {
                        logger.LogWarning("No general description entities found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return entities;
        }

        public List<JObject> GetGeneralDescriptionsJson()
        {
            var entities = new List<JObject>(20);

            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = CommandType.Text;
                command.CommandText = "select jsonobject from general_description_entity";

                EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var jsonData = reader.GetValue(0) as string;

                            if (!string.IsNullOrWhiteSpace(jsonData))
                            {
                                entities.Add(JObject.Parse(jsonData));
                            }
                        }
                    }
                    else
                    {
                        logger.LogWarning("No general description entities found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return entities;
        }

        #endregion

        #region Electronic transaction service

        /// <summary>
        /// Get all electronic transaction services.
        /// </summary>
        /// <returns>list of electronic transaction service entities</returns>
        public List<SourceElectronicTransactionService> GetElectronicTransactionServices()
        {
            var entities = new List<SourceElectronicTransactionService>(20);

            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = CommandType.Text;
                command.CommandText = "select jsonobject from electronic_transaction_service_entity";

                EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var entity = JsonHelper.Deserialize<SourceElectronicTransactionService>(reader.GetString(0));

                            if (entity != null)
                            {
                                entities.Add(entity);
                            }
                        }
                    }
                    else
                    {
                        logger.LogWarning("No electronic transaction service entities found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return entities;
        }

        public List<JObject> GetElectronicTransactionServicesJson()
        {
            var entities = new List<JObject>(20);

            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = CommandType.Text;
                command.CommandText = "select jsonobject from electronic_transaction_service_entity";

                EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var jsonData = reader.GetValue(0) as string;

                            if (!string.IsNullOrWhiteSpace(jsonData))
                            {
                                entities.Add(JObject.Parse(jsonData));
                            }
                        }
                    }
                    else
                    {
                        logger.LogWarning("No electronic transaction service entities found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return entities;
        }

        #endregion

        #region Electronic information service

        /// <summary>
        /// Get all electronic information services.
        /// </summary>
        /// <returns>list of electronic information service entities</returns>
        public List<SourceElectronicInformationService> GetElectronicInformationServices()
        {
            var entities = new List<SourceElectronicInformationService>(20);

            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = CommandType.Text;
                command.CommandText = "select jsonobject from electronic_information_service_entity";

                EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var entity = JsonHelper.Deserialize<SourceElectronicInformationService>(reader.GetString(0));

                            if (entity != null)
                            {
                                entities.Add(entity);
                            }
                        }
                    }
                    else
                    {
                        logger.LogWarning("No electronic information service entities found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return entities;
        }

        public List<JObject> GetElectronicInformationServicesJson()
        {
            var entities = new List<JObject>(20);

            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = CommandType.Text;
                command.CommandText = "select jsonobject from electronic_information_service_entity";

                EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var jsonData = reader.GetValue(0) as string;

                            if (!string.IsNullOrWhiteSpace(jsonData))
                            {
                                entities.Add(JObject.Parse(jsonData));
                            }
                        }
                    }
                    else
                    {
                        logger.LogWarning("No electronic information service entities found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return entities;
        }

        #endregion

        #region Helper methods

        private static void EnsureConnectionOpen(DbCommand cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            if (cmd.Connection.State == ConnectionState.Open)
            {
                return;
            }

            if (cmd.Connection.State == ConnectionState.Closed)
            {
                cmd.Connection.Open();
                return;
            }

            throw new InvalidOperationException($"The connection object state {cmd.Connection.State.ToString()} is invalid. Connection can not be opened.");
        }

        #endregion
    }
}
