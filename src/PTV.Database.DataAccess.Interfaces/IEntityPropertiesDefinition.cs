using System;
using System.Collections.Generic;

namespace PTV.Database.DataAccess.Interfaces
{
//   public interface IEntityPropertiesDefinition
//        {
//            List<ForeignKeyNavigationPair> NavigationsMap { get; }
//            List<PrimaryKeyDefinition> PrimaryKeysDefinition { get; }
//            List<CollectionNavigationDefinition> CollectionNavigationMap { get; }
//        }

    /// <summary>
    /// Holder class for relations between navigation and foreign key
    /// </summary>
    public class ForeignKeyNavigationPair
    {
        public List<NameTypePairDefinition> ForeignKeys { get; set; }
        public NameTypePairDefinition Navigation { get; set; }
    }

    public class CollectionNavigationDefinition
    {
        public NameTypePairDefinition Navigation { get; set; }
        public NameTypePairDefinition DeclaringEntity { get; set; }
        public NameTypePairDefinition PrincipalEntity { get; set; }
        public List<ForeignKeyPairDefinition> ForeignKeys { get; set; }
    }

    public class ForeignKeyPairDefinition
    {
        public NameTypePairDefinition DeclaringEntityColumn { get; set; }
        public NameTypePairDefinition PrincipalEntityColumn { get; set; }
    }

    public class EntityPropertiesDefinition //: IEntityPropertiesDefinition
    {
        public Type EntityType { get; set; }
        public List<ForeignKeyNavigationPair> NavigationsMap { get; set; }
        public List<PrimaryKeyDefinition> PrimaryKeysDefinition { get; set; }
        public List<CollectionNavigationDefinition> CollectionNavigationMap { get; set; }
    }

    public class PrimaryKeyDefinition : NameTypePairDefinition
    {
        public bool IsPrimaryAndForeign;
    }

    public class NameTypePairDefinition
    {
        public string Name;
        public Type Type;
    }
}