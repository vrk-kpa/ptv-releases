import React from 'react';
import { useTranslation } from 'react-i18next';
import { DropdownItem } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { getKeyForLanguage } from 'utils/translations';

type LanguageListProps = {
  noValueKey: string;
  languages: Language[];
};

export function LanguageList(props: LanguageListProps): React.ReactElement {
  const { t } = useTranslation();
  return (
    <>
      <DropdownItem key={props.noValueKey} value={props.noValueKey}>
        {t('Ptv.ConnectionDetails.LanguageComparison.Selector.NoLanguage.Label')}
      </DropdownItem>
      {props.languages.map((lang) => {
        return (
          <DropdownItem key={lang} value={lang}>
            {t(getKeyForLanguage(lang))}
          </DropdownItem>
        );
      })}
    </>
  );
}
