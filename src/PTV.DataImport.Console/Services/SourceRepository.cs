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

using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PTV.DataImport.ConsoleApp.DataAccess;
using PTV.DataImport.ConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PTV.DataImport.ConsoleApp.Services
{
    /// <summary>
    /// Source repository implementation (palvelunäkymä).
    /// </summary>
    public class SourceRepository : ISourceRepository
    {
        private SourceDbContext _dbContext;
        private ILogger<ISourceRepository> _logger;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="dbContext">db context to be used to access the database</param>
        /// <param name="logger">logger where to write log entries</param>
        public SourceRepository(SourceDbContext dbContext, ILogger<ISourceRepository> logger)
        {
            dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _dbContext = dbContext;
            _logger = logger;
        }

        #region Organization

        /// <summary>
        /// Get all organizations.
        /// </summary>
        /// <returns>list of organizations</returns>
        public List<SourceOrganizationEntity> GetOrganizations()
        {
            List<SourceOrganizationEntity> orgs = new List<SourceOrganizationEntity>(10);

            using (var command = this._dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "select jsonobject from organization_entity";

                SourceRepository.EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            SourceOrganizationEntity entity = JsonHelper.Deserialize<SourceOrganizationEntity>(reader.GetString(0) as string);

                            if (entity != null)
                            {
                                orgs.Add(entity);
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No organizations found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return orgs;
        }

        public List<JObject> GetOrganizationsJson()
        {
            List<JObject> jsonObjects = new List<JObject>(15);

            using (var command = this._dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "select jsonobject from organization_entity";

                SourceRepository.EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string jsondata = reader.GetValue(0) as string;

                            if (!string.IsNullOrWhiteSpace(jsondata))
                            {
                                jsonObjects.Add(JObject.Parse(jsondata));
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No organizations found from source database.");
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
            List<int> orgIds = new List<int>(10);

            using (var command = this._dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "select (jsonobject->>'id')::bigint from organization_entity";

                SourceRepository.EnsureConnectionOpen(command);

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
                        _logger.LogWarning("No organizations found from source database.");
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

            using (var command = this._dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "select jsonobject from organization_entity where id = @orgid";

                var param = command.CreateParameter();
                param.DbType = System.Data.DbType.Int64;
                param.Direction = System.Data.ParameterDirection.Input;
                param.ParameterName = "@orgid";
                param.Value = organizationId;

                command.Parameters.Add(param);

                SourceRepository.EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            entity = JsonHelper.Deserialize<SourceOrganizationEntity>(reader.GetString(0) as string);
                        }
                        else
                        {
                            _logger.LogError($"Failed to read organization with id '{organizationId}' from result.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"No organizations found from source database with id: '{organizationId}'.");
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
            List<SourcePhoneEntity> entities = new List<SourcePhoneEntity>(10);

            using (var command = this._dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "select jsonobject from phone_entity";

                SourceRepository.EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            SourcePhoneEntity entity = JsonHelper.Deserialize<SourcePhoneEntity>(reader.GetString(0) as string);

                            if (entity != null)
                            {
                                entities.Add(entity);
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No phone entities found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return entities;
        }

        public List<JObject> GetPhoneEntitiesJson()
        {
            List<JObject> entities = new List<JObject>(10);

            using (var command = this._dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "select jsonobject from phone_entity";

                SourceRepository.EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string jsondata = reader.GetValue(0) as string;

                            if (!string.IsNullOrWhiteSpace(jsondata))
                            {
                                entities.Add(JObject.Parse(jsondata));
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No phone entities found from source database.");
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
            List<SourceServiceEntity> entities = new List<SourceServiceEntity>(30);

            using (var command = this._dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "select jsonobject from service_entity";

                SourceRepository.EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            SourceServiceEntity entity = JsonHelper.Deserialize<SourceServiceEntity>(reader.GetString(0) as string);

                            if (entity != null)
                            {
                                entities.Add(entity);
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No service entities found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return entities;
        }

        public List<JObject> GetServicesJson()
        {
            List<JObject> entities = new List<JObject>(30);

            using (var command = this._dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "select jsonobject from service_entity";

                SourceRepository.EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string jsondata = reader.GetValue(0) as string;

                            if (!string.IsNullOrWhiteSpace(jsondata))
                            {
                                entities.Add(JObject.Parse(jsondata));
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No service entities found from source database.");
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
            List<SourceTransactionFormEntity> entities = new List<SourceTransactionFormEntity>(30);

            using (var command = this._dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "select jsonobject from transaction_form_entity";

                SourceRepository.EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            SourceTransactionFormEntity entity = JsonHelper.Deserialize<SourceTransactionFormEntity>(reader.GetString(0) as string);

                            if (entity != null)
                            {
                                entities.Add(entity);
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No transaction form entities found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return entities;
        }

        public List<JObject> GetTransactionFormsJson()
        {
            List<JObject> entities = new List<JObject>(30);

            using (var command = this._dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "select jsonobject from transaction_form_entity";

                SourceRepository.EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string jsondata = reader.GetValue(0) as string;

                            if (!string.IsNullOrWhiteSpace(jsondata))
                            {
                                entities.Add(JObject.Parse(jsondata));
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No transaction form entities found from source database.");
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
            List<SourceOfficeEntity> entities = new List<SourceOfficeEntity>(50);

            using (var command = this._dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "select jsonobject from office_entity";

                SourceRepository.EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            SourceOfficeEntity entity = JsonHelper.Deserialize<SourceOfficeEntity>(reader.GetString(0) as string);

                            if (entity != null)
                            {
                                entities.Add(entity);
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No office entities found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return entities;
        }

        public List<JObject> GetOfficesJson()
        {
            List<JObject> entities = new List<JObject>(50);

            using (var command = this._dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "select jsonobject from office_entity";

                SourceRepository.EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string jsondata = reader.GetValue(0) as string;

                            if (!string.IsNullOrWhiteSpace(jsondata))
                            {
                                entities.Add(JObject.Parse(jsondata));
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No office entities found from source database.");
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
            List<SourceGeneralDescription> entities = new List<SourceGeneralDescription>(20);

            using (var command = this._dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "select jsonobject from general_description_entity";

                SourceRepository.EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            SourceGeneralDescription entity = JsonHelper.Deserialize<SourceGeneralDescription>(reader.GetString(0) as string);

                            if (entity != null)
                            {
                                entities.Add(entity);
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No general description entities found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return entities;
        }

        public List<JObject> GetGeneralDescriptionsJson()
        {
            List<JObject> entities = new List<JObject>(20);

            using (var command = this._dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "select jsonobject from general_description_entity";

                SourceRepository.EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string jsondata = reader.GetValue(0) as string;

                            if (!string.IsNullOrWhiteSpace(jsondata))
                            {
                                entities.Add(JObject.Parse(jsondata));
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No general description entities found from source database.");
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
            List<SourceElectronicTransactionService> entities = new List<SourceElectronicTransactionService>(20);

            using (var command = this._dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "select jsonobject from electronic_transaction_service_entity";

                SourceRepository.EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            SourceElectronicTransactionService entity = JsonHelper.Deserialize<SourceElectronicTransactionService>(reader.GetString(0) as string);

                            if (entity != null)
                            {
                                entities.Add(entity);
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No electronic transaction service entities found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return entities;
        }

        public List<JObject> GetElectronicTransactionServicesJson()
        {
            List<JObject> entities = new List<JObject>(20);

            using (var command = this._dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "select jsonobject from electronic_transaction_service_entity";

                SourceRepository.EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string jsondata = reader.GetValue(0) as string;

                            if (!string.IsNullOrWhiteSpace(jsondata))
                            {
                                entities.Add(JObject.Parse(jsondata));
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No electronic transaction service entities found from source database.");
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
            List<SourceElectronicInformationService> entities = new List<SourceElectronicInformationService>(20);

            using (var command = this._dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "select jsonobject from electronic_information_service_entity";

                SourceRepository.EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            SourceElectronicInformationService entity = JsonHelper.Deserialize<SourceElectronicInformationService>(reader.GetString(0) as string);

                            if (entity != null)
                            {
                                entities.Add(entity);
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No electronic information service entities found from source database.");
                    }
                }

                command.Connection.Close();
            }

            return entities;
        }

        public List<JObject> GetElectronicInformationServicesJson()
        {
            List<JObject> entities = new List<JObject>(20);

            using (var command = this._dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 15;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = "select jsonobject from electronic_information_service_entity";

                SourceRepository.EnsureConnectionOpen(command);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string jsondata = reader.GetValue(0) as string;

                            if (!string.IsNullOrWhiteSpace(jsondata))
                            {
                                entities.Add(JObject.Parse(jsondata));
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No electronic information service entities found from source database.");
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

            if (cmd.Connection.State == System.Data.ConnectionState.Open)
            {
                return;
            }

            if (cmd.Connection.State == System.Data.ConnectionState.Closed)
            {
                cmd.Connection.Open();
                return;
            }

            throw new InvalidOperationException($"The connection object state {cmd.Connection.State.ToString()} is invalid. Connection can not be opened.");
        }

        #endregion
    }
}
