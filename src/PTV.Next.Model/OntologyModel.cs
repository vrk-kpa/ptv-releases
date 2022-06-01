using System.Collections.Generic;

namespace PTV.Next.Model
{
    public class OntologyModel : CodeBaseModel
    {
        public List<OntologyModel> Parents { get; set; }
        public List<OntologyModel> Children { get; set; }
        public List<string> ExactMatches { get; set; }
    }
}