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
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.Model.Interfaces;

namespace PTV.Database.DataAccess.Services
{
    /// <summary>
    /// Manager handling versions of entity which uses IVersioned interface
    /// </summary>
    internal interface IVersioningManager
    {
        /// <summary>
        /// Check latest version of entity increase it
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be updated</typeparam>
        /// <param name="unitOfWork">Unit Of Work</param>
        /// <param name="entity">Entity which will be updated</param>
        /// <param name="versionType">Major or Minor version will be increased</param>
        void CreateNewVersion<TEntityType>(IUnitOfWork unitOfWork, TEntityType entity, EVersionType versionType = EVersionType.Minor) where TEntityType : class, IVersioned;

        /// <summary>
        /// Get and return all available versions of entity
        /// </summary>
        /// <typeparam name="TEntityType"></typeparam>
        /// <param name="unitOfWork">Unit Of Work must be provided for correct functionality</param>
        /// <param name="entity">Entity which versions will be retrieved</param>
        /// <returns>List of available versions of specified entity</returns>
        List<VersionInfo> GetAllVersions<TEntityType>(IUnitOfWork unitOfWork, TEntityType entity) where TEntityType : class, IVersioned;

        /// <summary>
        /// Publish specified entity, check latest version and create new version with published state
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which will be promoted to published state</typeparam>
        /// <param name="unitOfWork">Unit Of Work</param>
        /// <param name="entity">Entity which will be promoted to published state</param>
        void PublishVersion<TEntityType>(IUnitOfWork unitOfWork, TEntityType entity) where TEntityType : class, IVersioned;
    }
}