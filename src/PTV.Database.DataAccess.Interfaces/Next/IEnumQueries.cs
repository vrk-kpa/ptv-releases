using System;
using System.Collections.Generic;
using PTV.Framework.Extensions;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Interfaces.Next
{
    public interface IEnumQueries
    {
        List<OntologyModel> GetOntologyAll();
        IEnumerable<OntologyModel> GetOntologyById(IEnumerable<Guid> ids);
        List<OntologyModel> SearchOntology(string expression, string languageCode);
        List<OntologyModel> SearchByName(string name, string languageCode);
        DateTime CheckOntology();
        (AnnotationStates State, List<OntologyModel> Ontologies) GetAnnotations(ServiceInfo serviceInfo);
        
        List<IndustrialClassModel> GetIndustrialClassAll();
        DateTime CheckIndustrialClass();
        
        List<DigitalAuthorizationModel> GetDigitalAuthorizationAll();
        DateTime CheckDigitalAuthorization();
        
        List<LifeEventModel> GetLifeEventAll();
        DateTime CheckLifeEvent();
        
        List<ServiceClassModel> GetServiceClassAll();
        DateTime CheckServiceClass();
        
        List<TargetGroupModel> GetTargetGroupAll();
        DateTime CheckTargetGroup();
        List<HolidayModel> GetHolidayAll();
        DateTime CheckHoliday();
        List<ServiceNumberModel> GetServiceNumberAll();
        DateTime CheckServiceNumber();
        DateTime CheckDialCode();
        List<DialCodeModel> GetDialCodeAll();
        DateTime CheckLanguage();
        List<LanguageModel> GetLanguageAll();

        List<PostalCodeModel> GetAllPostalCodes();
        DateTime CheckPostalCode();
        List<CountryModel> GetAllCountries();
        DateTime CheckCountries();
    }
}