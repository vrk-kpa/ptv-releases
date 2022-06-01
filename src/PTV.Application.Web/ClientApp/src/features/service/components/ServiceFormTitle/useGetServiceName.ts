import { Control, useWatch } from 'react-hook-form';
import { ServiceModel, cLv, cService } from 'types/forms/serviceFormTypes';
import { RequiredLanguageVersionType } from 'types/languageVersionTypes';
import { useFormMetaContext } from 'context/formMeta';

export function useGetServiceName(control: Control<ServiceModel>): string {
  const { selectedLanguageCode } = useFormMetaContext();

  // Normally we would use single useWatch like this:
  // useWatch({ control: control, name: `${cService.languageVersions}.${selectedLanguageCode}.${cLv.name}` })
  // That does not work when user switches between tabs. Here is what happens when there are FI and EN tabs
  // 1. FI tab is selected
  // 2. User switches to EN tab
  // 3. useFormMetaContext returns selectedLanguageCode correctly as 'en'
  // 4. useWatch returns the value of languageVersions.fi.name

  const name: RequiredLanguageVersionType<string> = {
    fi: useWatch({ control: control, name: `${cService.languageVersions}.fi.${cLv.name}` }),
    sv: useWatch({ control: control, name: `${cService.languageVersions}.sv.${cLv.name}` }),
    en: useWatch({ control: control, name: `${cService.languageVersions}.en.${cLv.name}` }),
    se: useWatch({ control: control, name: `${cService.languageVersions}.se.${cLv.name}` }),
    smn: useWatch({ control: control, name: `${cService.languageVersions}.smn.${cLv.name}` }),
    sms: useWatch({ control: control, name: `${cService.languageVersions}.sms.${cLv.name}` }),
  };

  return name[selectedLanguageCode];
}
