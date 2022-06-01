import React, { FunctionComponent, useContext, useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { Box } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { Dropdown, DropdownItem, HintText, Label } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { DispatchContext, changeCompareLanguage, useFormMetaContext } from 'context/formMeta';
import { getKeyForLanguage } from 'utils/translations';

const useStyles = makeStyles(() => ({
  comparisonBox: {
    backgroundColor: 'rgb(247, 247, 248)',
    border: '1px solid rgb(200, 205, 208)',
    '& .fi-hint-text': {
      marginBottom: '10px',
    },
  },
}));

type CompareLanguageSelectionProps = {
  enabledLanguages: Language[];
};

export const CompareLanguageSelection: FunctionComponent<CompareLanguageSelectionProps> = (props: CompareLanguageSelectionProps) => {
  const { selectedLanguageCode, compareLanguageCode } = useFormMetaContext();
  const dispatch = useContext(DispatchContext);
  const classes = useStyles();
  const { t } = useTranslation();

  const compareLanguages = props.enabledLanguages.filter((lv) => lv !== selectedLanguageCode);

  const items = useMemo(() => {
    return compareLanguages.map((lang) => {
      const languageKey = getKeyForLanguage(lang);
      return (
        <DropdownItem key={lang} value={lang}>
          {t(`${languageKey}`)}
        </DropdownItem>
      );
    });
  }, [compareLanguages, t]);

  if (compareLanguages.length < 1) {
    return null;
  }

  const handleCompareLanguageChange = (value: Language) => {
    changeCompareLanguage(dispatch, value);
  };

  return (
    <Box mt={2} mb={3} p={2} className={classes.comparisonBox}>
      <Label id='compare-language-selection-title'>{t('Ptv.Form.CompareLanguageSelection.Title')}</Label>
      <HintText>{t('Ptv.Form.CompareLanguageSelection.Description')}</HintText>
      <Dropdown
        labelMode='hidden'
        visualPlaceholder={t('Ptv.Form.CompareLanguageSelection.Placeholder')}
        aria-labelledby='compare-language-selection-title'
        labelText=''
        value={compareLanguageCode}
        onChange={handleCompareLanguageChange}
        id='compareLanguageSelect'
      >
        {items}
      </Dropdown>
    </Box>
  );
};
