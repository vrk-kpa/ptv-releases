import React, { ReactNode } from 'react';
import { useTranslation } from 'react-i18next';
import { Grid, Tab, Tabs } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { Checkbox } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ServiceFormValues } from 'types/forms/serviceFormTypes';
import { FormDivider } from 'components/formLayout/FormDivider';
import { getEnabledLanguagesByPriority } from 'utils/service';
import { getKeyForLanguage } from 'utils/translations';

const useStyles = makeStyles(() => ({
  tabControlRoot: {
    display: 'flex',
    flexWrap: 'wrap',
    flex: 1,
    flexDirection: 'column',
  },
  tabHeaderList: {
    display: 'flex',
    flexDirection: 'row',
    flexWrap: 'wrap',
  },
}));

type TabsProps = {
  onTabChange: (language: Language) => void;
  selectedLanguage: Language;
  isComparing: boolean;
  toggleCompare?: () => void;
  getFormValues: () => ServiceFormValues;
  children: ReactNode;
};

export function LanguageTabs(props: TabsProps): React.ReactElement {
  const classes = useStyles();
  const { t } = useTranslation();

  const enabledLanguages = getEnabledLanguagesByPriority(props.getFormValues().languageVersions);

  function onChange(_event: React.SyntheticEvent, value: Language) {
    props.onTabChange(value);
  }

  return (
    <div className={classes.tabControlRoot}>
      <Grid container wrap='wrap' direction='row' alignItems='center' justifyContent='space-between'>
        <Grid item>
          <Tabs value={props.selectedLanguage} onChange={onChange}>
            {enabledLanguages.map((lang) => {
              return <Tab key={lang} label={t(getKeyForLanguage(lang))} value={lang} />;
            })}
          </Tabs>
        </Grid>
        {!!props.toggleCompare && (
          <Grid item>
            <Checkbox onClick={props.toggleCompare} checked={props.isComparing} disabled={enabledLanguages.length <= 1}>
              {t('Ptv.TabControl.ShowComparison')}
            </Checkbox>
          </Grid>
        )}
      </Grid>
      <FormDivider mt={0} />
      {props.children}
    </div>
  );
}
