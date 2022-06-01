import { Control, useWatch } from 'react-hook-form';
import { Language } from 'types/enumTypes';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { LanguagePriorities } from 'utils/languages';

type EnabledLanguages = {
  language: Language;
  isEnabled: boolean;
};

export const useGetEnabledLanguagesByPriority = (control: Control<ServiceModel>): Language[] => {
  const fiIsEnabled = useWatch({ control: control, name: `${cService.languageVersions}.fi.isEnabled` });
  const svIsEnabled = useWatch({ control: control, name: `${cService.languageVersions}.sv.isEnabled` });
  const enIsEnabled = useWatch({ control: control, name: `${cService.languageVersions}.en.isEnabled` });
  const seIsEnabled = useWatch({ control: control, name: `${cService.languageVersions}.se.isEnabled` });
  const smnIsEnabled = useWatch({ control: control, name: `${cService.languageVersions}.smn.isEnabled` });
  const smsIsEnabled = useWatch({ control: control, name: `${cService.languageVersions}.sms.isEnabled` });

  const langs: EnabledLanguages[] = [
    { language: 'fi', isEnabled: fiIsEnabled },
    { language: 'sv', isEnabled: svIsEnabled },
    { language: 'en', isEnabled: enIsEnabled },
    { language: 'se', isEnabled: seIsEnabled },
    { language: 'smn', isEnabled: smnIsEnabled },
    { language: 'sms', isEnabled: smsIsEnabled },
  ];

  const enabledLanguages = langs.filter((lang) => lang.isEnabled).map((lang) => lang.language);

  return LanguagePriorities.filter((x) => enabledLanguages.includes(x));
};
