namespace PTV.Next.Model
{
    public class OrganizationSearchModel
    {
        public string SearchValue { get; set; } = "";
        public bool SearchAll { get; set; }
        public bool SearchOnlyDraftAndPublished { get; set; }
    }
}
