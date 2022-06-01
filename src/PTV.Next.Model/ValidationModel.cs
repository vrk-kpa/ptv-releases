using System.Collections.Generic;
using PTV.Framework;

namespace PTV.Next.Model
{
    public class ValidationModel<T>
    {
        public Dictionary<LanguageEnum, List<ValidationMessage>> ValidatedFields { get; set; }
        public T Entity { get; set; }
    }
}