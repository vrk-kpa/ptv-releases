import { MultiSelectData } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { LocalizedText } from 'types/miscellaneousTypes';
import { OrganizationModel } from 'types/organizationTypes';
import { getTextByLangPriority } from './translations';

export interface MultiSelectItem extends MultiSelectData {
  organization: OrganizationModel;
}

const LanguagePriority = new Map<Language, Language>([
  ['fi', 'sv'],
  ['sv', 'fi'],
  ['en', 'fi'],
]);

export const organizationModelToMultiSelectItem = (organization: OrganizationModel, lang: Language): MultiSelectItem => {
  const secondaryLanguage = LanguagePriority.get(lang) ?? 'fi';
  const text = toText(organization.texts, lang, secondaryLanguage, organization.name);
  const chipText = getTextByLangPriority(lang, organization.texts) ?? organization.name;

  return {
    uniqueItemId: organization.id,
    organization: organization,
    labelText: text,
    chipText: chipText,
  };
};

function toText(text: LocalizedText, primary: Language, secondary: Language, fallBack: string): string {
  // We want to display e.g. "Company finnish name (Company swedish name)" to the end user
  // but we don't want to repeat the name if english and finnish name are the same

  const primaryText = text[primary];
  const secondaryText = text[secondary];

  if (primaryText === secondaryText) return primaryText ?? fallBack;
  if (primaryText && !secondaryText) return primaryText;
  if (secondaryText && !primaryText) return secondaryText;
  return `${primaryText} (${secondaryText})`;
}
