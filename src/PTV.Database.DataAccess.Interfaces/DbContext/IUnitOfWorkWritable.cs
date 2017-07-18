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
namespace PTV.Database.DataAccess.Interfaces.DbContext
{
    /// <summary>
    /// Unit of Work is scope of database operations. Writable type provides Save method which allows to make changes in database
    /// </summary>
    public interface IUnitOfWorkWritable : IUnitOfWork
    {
        /// <summary>
        /// Push changes done in context into database
        /// </summary>
        void Save(SaveMode saveMode = SaveMode.Normal, object parentEntity = null, string userName = null);

        /// <summary>
        /// Get user name for auditing, takes <see cref="userName"/> if specified, or take it from user identity.
        /// If <see cref="SaveMode.AllowAnonymous"/> is set and user is not found then defaul app user name is set. (Import)
        /// </summary>
        /// <param name="saveMode"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        string GetUserNameForAuditing(SaveMode saveMode = SaveMode.Normal, string userName = null);

        void MarkPropertyModified(object entity, string property, bool modified);
    }

    public enum SaveMode
    {
        Normal,
        AllowAnonymous,
        NonTrackedDataMigration
    }
}
