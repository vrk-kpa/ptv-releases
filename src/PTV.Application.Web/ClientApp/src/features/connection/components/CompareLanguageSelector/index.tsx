import React, { useContext } from 'react';
import { useTranslation } from 'react-i18next';
import { Box } from '@mui/material';
import { Dropdown, HintText, Label } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { DispatchContext, useFormMetaContext } from 'context/formMeta';
import { useConnectionContext } from 'features/connection/context';
import { LanguageList } from './LanguageList';

const NoValue = 'NoValue'; // Dropdown does not accept undefined/null

export function CompareLanguageSelector(): React.ReactElement | null {
  const { t } = useTranslation();
  const ctx = useConnectionContext();
  const metaCtx = useFormMetaContext();
  const dispatch = useContext(DispatchContext);

  const selectedLanguage = metaCtx.selectedLanguageCode;
  const selectedValue: string = metaCtx.compareLanguageCode || NoValue;
  const languages = (Object.keys(ctx.connection.languageVersions) as Language[]).filter((x) => x !== selectedLanguage);

  // Do not display comparison if there are no language versions to compare
  if (languages.length < 1) {
    return null;
  }

  function onChange(value: string) {
    if (value === NoValue) {
      dispatch({ type: 'SelectCompareLanguage', payload: undefined });
    } else {
      dispatch({ type: 'SelectCompareLanguage', payload: value as Language });
    }
  }

  return (
    <>
      <Box>
        <Label className='custom-label' id='compare-language-selector-title'>
          {t('Ptv.ConnectionDetails.LanguageComparison.Selector.Label')}
        </Label>
        <HintText>{t('Ptv.ConnectionDetails.LanguageComparison.Selector.Hint')}</HintText>
      </Box>
      <Dropdown
        id={`summary.${selectedLanguage}.lang-selector`}
        labelText={t('Ptv.ConnectionDetails.LanguageComparison.Selector.Label')}
        labelMode={'hidden'}
        value={selectedValue}
        aria-labelledby='compare-language-selector-title'
        onChange={onChange}
        visualPlaceholder={t('Ptv.Form.CompareLanguageSelection.Placeholder')}
      >
        <LanguageList noValueKey={NoValue} languages={languages} />
      </Dropdown>
    </>
  );
}
