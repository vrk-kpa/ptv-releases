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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace PTV.Database.DataAccess.Tests.Translators
{
    public class TestDbSet<T> : DbSet<T>, IQueryable<T> where T : class
    {
        protected readonly List<T> encapsulated;

        public TestDbSet(List<T> encapsulated)
        {
            if (encapsulated == null)
            {
                throw new Exception("DbSetActiveFiltered: encapsulated dbset is null");
            }
            this.encapsulated = encapsulated;

        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return encapsulated.AsQueryable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual Expression Expression
        {
            get
            {
                return encapsulated.AsQueryable().Expression;
            }
        }

        public override EntityEntry<T> Add(T entity)
        {
            encapsulated.Add(entity);
            return null;
        }

        public override ValueTask<EntityEntry<T>> AddAsync(T entity, CancellationToken cancellationToken = new CancellationToken())
        {
            encapsulated.Add(entity);
            return new ValueTask<EntityEntry<T>>();
        }

        public override EntityEntry<T> Remove(T entity)
        {
            encapsulated.Remove(entity);
            return null;
        }
        public IQueryProvider Provider
        {
            get
            {
                return (encapsulated.AsQueryable()).Provider;
            }
        }


        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return encapsulated.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            return encapsulated.Equals(obj);
        }

        public override int GetHashCode()
        {
            return encapsulated.GetHashCode();
        }
    }
}
