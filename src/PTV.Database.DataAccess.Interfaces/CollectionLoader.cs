using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Framework;

namespace PTV.Database.DataAccess.Interfaces
{
    public interface IDeepClauseResults
    {
        bool NullDetected { get; set; }
        bool Result { get; set; }
        DeepClauseComparisonType ActionType { get; set; }
    }
    

    public class DeepClauseInitier<TEntity>
    {
        public Dictionary<TEntity, IDeepClauseResults> CallChecks(IEnumerable<TEntity> entities, Action<DefinitionDeepClause<TEntity>> deepConditions)
        {
            return entities.ToDictionary(i => i, i =>
            {
                IDeepClauseResults holder = new DeepClauseResults();
                deepConditions(new DefinitionDeepClause<TEntity>(holder, new List<TEntity>() { i}));
                return holder;
            });
        }

        private class DeepClauseResults : IDeepClauseResults
        {
            public bool NullDetected { get; set; }
            public bool Result { get; set; }
            public DeepClauseComparisonType ActionType { get; set; }

            internal DeepClauseResults()
            {
                Result = true;
                NullDetected = false;
                ActionType = DeepClauseComparisonType.None;
            }
        }
    }


    public class DefinitionDeepClause<TEntity> : IDefinitionDeepClause<TEntity>
    {
        private readonly IEnumerable<TEntity> entities;
        private readonly IDeepClauseResults parent;

        public DefinitionDeepClause(IDeepClauseResults parent, IEnumerable<TEntity> entities)
        {
            this.entities = entities ?? new List<TEntity>();
            this.parent = parent;
        }

        public DefinitionDeepClause(IDeepClauseResults parent, TEntity entity) : this(parent, new List<TEntity>() { entity })
        {}

        public DefinitionDeepClause<TLast> Check<TLast>(Func<TEntity, TLast> selector)
        {
            var selectedValue = entities.Select(selector).ToList();
            parent.NullDetected |= selectedValue.Any(i => i == null);
            return new DefinitionDeepClause<TLast>(parent, selectedValue.Where(i => i != null));
        }

        public DefinitionDeepClause<TLast> Check<TLast>(Func<TEntity, ICollection<TLast>> selector)
        {
            var selectedValue = entities.SelectMany(selector).ToList();
            parent.NullDetected |= selectedValue.Any(i => i == null);
            return new DefinitionDeepClause<TLast>(parent, selectedValue.Where(i => i != null));
        }

        public DefinitionDeepClause<TLast> Check<TLast>(Func<TEntity, ICollection<TLast>> selector, Func<TLast, bool> where)
        {
            if (where == null) { where = i => true; }
            var selectedValue = entities.SelectMany(selector).Where(where).ToList();
            parent.NullDetected |= selectedValue.Any(i => i == null);
            return new DefinitionDeepClause<TLast>(parent, selectedValue.Where(i => i != null));
        }

        public DefinitionDeepClause<TEntity> Is(TEntity value)
        {
            parent.ActionType = DeepClauseComparisonType.Is;
            parent.Result &= entities.Where(i => i != null).Any(j => j.Equals(value));
            return this;
        }

        public DefinitionDeepClause<TEntity> Not(TEntity value)
        {
            parent.ActionType = DeepClauseComparisonType.Not;
            parent.Result &= entities.IsNullOrEmpty() || entities.Where(i => i != null).Any(j => !j.Equals(value));
            return this;
        }

        public DefinitionDeepClause<TEntity> Any()
        {
            parent.ActionType = DeepClauseComparisonType.Any;
            parent.Result &= entities.Any(i => i != null);
            return this;
        }

        public DefinitionDeepClause<TEntity> Nothing()
        {
            parent.ActionType = DeepClauseComparisonType.Nothing;
            parent.Result &= entities.IsNullOrEmpty();
            return this;
        }
    }

    public enum DeepClauseComparisonType
    {
        None,
        Is,
        Not,
        Any,
        Nothing
    }
}