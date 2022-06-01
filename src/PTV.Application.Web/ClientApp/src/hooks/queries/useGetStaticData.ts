import { getValueOrEmpty } from 'utils';
import { Country, Municipality, Region } from 'types/areaTypes';
import { ClassificationItem } from 'types/classificationItemsTypes';
import { DigitalAuthorizationModel } from 'types/digitalAuthTypes';
import { DialCode } from 'types/enumItemType';
import { HttpError } from 'types/miscellaneousTypes';
import { LanguageItem, StaticData } from 'types/settingTypes';
import { TargetGroup } from 'types/targetGroupTypes';
import { useCachedQuery } from './useCachedQuery';

type StaticDataResult = {
  isLoading: boolean;
  error: HttpError | null;
  data: StaticData;
};

export default function useGetStaticData(): StaticDataResult {
  const targetGroupQuery = useCachedQuery<TargetGroup[]>({ path: 'next/targetgroup/all' });
  const serviceClassesQuery = useCachedQuery<ClassificationItem[]>({ path: 'next/serviceclass/all' });
  const lifeEventsQuery = useCachedQuery<ClassificationItem[]>({ path: 'next/lifeevent/all' });
  const industrialClassesQuery = useCachedQuery<ClassificationItem[]>({ path: 'next/industrialclass/all' });
  const municipalitiesQuery = useCachedQuery<Municipality[]>({ path: 'next/region/municipalities' });
  const businessRegionsQuery = useCachedQuery<Region[]>({ path: 'next/region/businessregions' });
  const hospitalRegionsQuery = useCachedQuery<Region[]>({ path: 'next/region/hospitalregions' });
  const provincesQuery = useCachedQuery<Region[]>({ path: 'next/region/provinces' });
  const languagesQuery = useCachedQuery<LanguageItem[]>({ path: 'next/language/all' });
  const dialCodeQuery = useCachedQuery<DialCode[]>({ path: 'next/dialcode/all' });
  const countryQuery = useCachedQuery<Country[]>({ path: 'next/country/all' });
  const digitalAuthQuery = useCachedQuery<DigitalAuthorizationModel[]>({ path: 'next/digitalauthorization/all' });

  const isLoading =
    targetGroupQuery.isLoading ||
    serviceClassesQuery.isLoading ||
    lifeEventsQuery.isLoading ||
    industrialClassesQuery.isLoading ||
    municipalitiesQuery.isLoading ||
    businessRegionsQuery.isLoading ||
    hospitalRegionsQuery.isLoading ||
    provincesQuery.isLoading ||
    languagesQuery.isLoading ||
    dialCodeQuery.isLoading ||
    countryQuery.isLoading ||
    digitalAuthQuery.isLoading;
  const error =
    targetGroupQuery.error ||
    serviceClassesQuery.error ||
    lifeEventsQuery.error ||
    industrialClassesQuery.error ||
    municipalitiesQuery.error ||
    businessRegionsQuery.error ||
    hospitalRegionsQuery.error ||
    provincesQuery.error ||
    languagesQuery.error ||
    dialCodeQuery.error ||
    countryQuery.error ||
    digitalAuthQuery.error;

  return {
    isLoading: isLoading,
    error: error,
    data: {
      targetGroups: getValueOrEmpty(targetGroupQuery.data),
      ontologyTerms: [],
      serviceClasses: getValueOrEmpty(serviceClassesQuery.data),
      lifeEvents: getValueOrEmpty(lifeEventsQuery.data),
      industrialClasses: getValueOrEmpty(industrialClassesQuery.data),
      municipalities: getValueOrEmpty(municipalitiesQuery.data),
      businessRegions: getValueOrEmpty(businessRegionsQuery.data),
      hospitalRegions: getValueOrEmpty(hospitalRegionsQuery.data),
      provinces: getValueOrEmpty(provincesQuery.data),
      languages: getValueOrEmpty(languagesQuery.data),
      dialCodes: getValueOrEmpty(dialCodeQuery.data),
      countries: getValueOrEmpty(countryQuery.data),
      digitalAuthorizations: getValueOrEmpty(digitalAuthQuery.data),
    },
  };
}
