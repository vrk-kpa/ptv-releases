import { Country, Municipality, Region } from './areaTypes';
import { ClassificationItem } from './classificationItemsTypes';
import { DigitalAuthorizationModel } from './digitalAuthTypes';
import { DialCode, EnumItemType } from './enumItemType';
import { AppEnvironment, Language } from './enumTypes';
import { LocalizedText } from './miscellaneousTypes';
import { TargetGroup } from './targetGroupTypes';

export type AppSettings = {
  uiApiUrl: string;
  environmentType: AppEnvironment;
  isPAHALoginEnabled: boolean;
  isFakeAuthenticationEnabled: boolean;
  versionPrefix: string | null;
  version: string | null;
  releaseNumber: string | null;
  pahaRedirectUrl: string | null;
  pahaReturnUrl: string | null;
};

export type LanguageItem = EnumItemType & {
  code: Language;
  id: string;
  isForData: boolean;
  isForTranslation: boolean;
  names: LocalizedText;
};

// Returned from the new /api/next endpoints
export type StaticData = {
  targetGroups: TargetGroup[];
  serviceClasses: ClassificationItem[];
  ontologyTerms: ClassificationItem[];
  lifeEvents: ClassificationItem[];
  industrialClasses: ClassificationItem[];
  municipalities: Municipality[];
  businessRegions: Region[];
  hospitalRegions: Region[];
  provinces: Region[];
  languages: LanguageItem[];
  dialCodes: DialCode[];
  countries: Country[];
  digitalAuthorizations: DigitalAuthorizationModel[];
};
