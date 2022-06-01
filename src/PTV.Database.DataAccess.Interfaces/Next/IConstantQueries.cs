using System.Collections.Generic;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Interfaces.Next
{
    public interface IConstantQueries
    {
        /// <summary>
        /// A dictionary of dictionaries. To get a particular value, you need to provide the following indexes:
        /// dictionary[typeName][typeValue][language]
        /// E.g.:
        /// dictionary["publishingStatus"]["Published"]["fi"]
        /// </summary>
        /// <returns></returns>
        Dictionary<string, Dictionary<string, Dictionary<LanguageEnum, string>>> GetTexts();
    }
}