import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { styled } from '@mui/material/styles';
import { Checkbox } from 'suomifi-ui-components';
import { getKeys } from 'utils';
import { Language } from 'types/enumTypes';
import { LanguageVersionType, LanguageVersionWithName } from 'types/languageVersionTypes';
import { Fieldset } from 'fields/Fieldset/Fieldset';
import { Legend } from 'fields/Legend/Legend';
import { LanguagePriorities } from 'utils/languages';
import { getKeyForLanguage } from 'utils/translations';
import { TextInputWithLangLabel } from './TextInputWithLangLabel';

type SelectedLanguageVersionWithName = {
  selected: boolean;
  name: string;
};

type AddLanguageVersionContentProps = {
  className?: string;
  hasGeneralDescription: boolean;
  onChange: (languageVersions: LanguageVersionWithName[]) => void;
  getExistingLanguages: () => LanguageVersionWithName[];
};

export const AddLanguageVersionContent = styled((props: AddLanguageVersionContentProps): React.ReactElement | null => {
  const [selectedLanguageVersions, setSelectedLanguageVersions] = useState<LanguageVersionType<SelectedLanguageVersionWithName>>({});
  const { t } = useTranslation();

  const existingLanguages = props.getExistingLanguages();

  const handleOnChange = (newState: LanguageVersionType<SelectedLanguageVersionWithName>) => {
    const selectedLanguageVersions = getKeys(newState)
      .filter((key) => newState[key]?.selected === true)
      .map((lang) => {
        const name = newState[lang]?.name || '';
        return { language: lang, name: name };
      });
    props.onChange(selectedLanguageVersions);
  };

  const toggleCheckBox = (language: Language) => {
    const newState = { ...selectedLanguageVersions };
    const languageVersion = newState[language];
    if (!!languageVersion) {
      languageVersion.selected = !languageVersion.selected;
    } else {
      newState[language] = { selected: true, name: '' };
    }
    setSelectedLanguageVersions(newState);
    handleOnChange(newState);
  };

  const onNameChange = (name: string | number | undefined, language: Language) => {
    const newState = { ...selectedLanguageVersions };
    const languageVersion = newState[language];
    if (!!languageVersion && name !== undefined) {
      languageVersion.name = name.toString();
      setSelectedLanguageVersions(newState);
      handleOnChange(newState);
    }
  };

  const languageDisabled = (language: Language) => {
    return existingLanguages.some((ln) => ln.language === language);
  };

  const nameInputVisible = (language: Language) => {
    return existingLanguages.some((ln) => ln.language === language) || !!selectedLanguageVersions[language]?.selected;
  };

  const valueOrDefaultValue = (language: Language) => {
    const existingLang = existingLanguages.find((ln) => ln.language === language);
    if (!!existingLang) {
      return { value: existingLang.name || '' };
    }
    return { defaultValue: selectedLanguageVersions[language]?.name || '' };
  };

  const hintKey = !!props.hasGeneralDescription ? 'Ptv.Service.Form.Field.Name.GdSelected.Hint' : 'Ptv.Service.Form.Field.Name.Hint';

  return (
    <Fieldset className={props.className}>
      <Legend className='legend'>{t('Ptv.Service.Form.Empty.Modal.LanguagePicker')}</Legend>
      {LanguagePriorities.map((language) => {
        const disabled = languageDisabled(language);
        return (
          <div key={language} className='checkbox' id={`languageVersionCheckbox.${language}`}>
            <Checkbox
              id={`languageVersion.${language}`}
              disabled={disabled}
              {...(disabled ? { checked: true } : {})}
              onClick={() => toggleCheckBox(language)}
            >
              {t(getKeyForLanguage(language))}
            </Checkbox>
            {!!nameInputVisible(language) && (
              <TextInputWithLangLabel
                language={language}
                disabled={disabled}
                labelText={t('Ptv.Service.Form.Field.Name.Label')}
                visualPlaceholder={t('Ptv.Service.Form.Field.Name.Placeholder')}
                hintText={t(hintKey)}
                onChange={(value) => onNameChange(value, language)}
                {...valueOrDefaultValue(language)}
              />
            )}
          </div>
        );
      })}
    </Fieldset>
  );
})(() => ({
  marginTop: '15px',
  '& .checkbox': {
    marginTop: '10px',
  },
  '& .legend': {
    marginBottom: 0,
  },
}));

AddLanguageVersionContent.displayName = 'AddLanguageVersionContent';
