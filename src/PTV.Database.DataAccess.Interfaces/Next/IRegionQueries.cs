using System;
using System.Collections.Generic;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Interfaces.Next
{
    public interface IRegionQueries
    {
        DateTime GetLastUpdate();
        List<AreaModel> GetProvinces();
        List<AreaModel> GetHospitalRegions();
        List<AreaModel> GetBusinessRegions();
        List<MunicipalityModel> GetMunicipalities();
    }
}