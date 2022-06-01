import React, { useContext, useState } from 'react';
import { Control, UseFormSetValue, UseFormTrigger } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Tab, Tabs } from '@mui/material';
import Grid from '@mui/material/Grid';
import { makeStyles } from '@mui/styles';
import { Checkbox } from 'suomifi-ui-components';
import * as enumTypes from 'types/enumTypes';
import { ConnectionFormModel, LanguageVersionExpanderTypes, StateLanguageVersionExpander } from 'types/forms/connectionFormTypes';
import { TabPanel } from 'components/TabPanel';
import { FormDivider } from 'components/formLayout/FormDivider';
import { DispatchContext, selectLanguage, switchCompareMode, useFormMetaContext } from 'context/formMeta';
import { LanguagePriorities, languagesSort } from 'utils/languages';
import { getKeyForLanguage } from 'utils/translations';
import { ConnectionForm } from 'features/connection/components/ConnectionForm';

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
  control: Control<ConnectionFormModel>;
  setValue: UseFormSetValue<ConnectionFormModel>;
  getFormValues: () => ConnectionFormModel;
  trigger: UseFormTrigger<ConnectionFormModel>;
};

export function LanguageTabs(props: TabsProps): React.ReactElement {
  const classes = useStyles();
  const { t } = useTranslation();
  const { selectedLanguageCode, displayComparison } = useFormMetaContext();
  const [expanderOpen, setExpanderOpen] = useState<StateLanguageVersionExpander>({
    BasicInfo: false,
    ContactInformation: false,
    ServiceHours: false,
    Authorization: false,
  });
  const enabledLanguages = Object.keys(props.getFormValues().languageVersions)
    .map((lang) => lang as enumTypes.Language)
    .sort(languagesSort);

  const formMetaDispatch = useContext(DispatchContext);

  function onCompareModeChanged() {
    switchCompareMode(formMetaDispatch, !displayComparison);
  }

  function onChange(_event: React.SyntheticEvent, value: enumTypes.Language) {
    switchCompareMode(formMetaDispatch, false);
    selectLanguage(formMetaDispatch, value);
  }

  function toggleExpander(expander: LanguageVersionExpanderTypes) {
    const newStates = { ...expanderOpen };
    newStates[expander] = !newStates[expander];
    setExpanderOpen(newStates);
  }

  return (
    <div className={classes.tabControlRoot}>
      <Grid container wrap='wrap' direction='row' alignItems='center' justifyContent='space-between'>
        <Grid item>
          <Tabs value={selectedLanguageCode} onChange={onChange}>
            {enabledLanguages.map((lang) => {
              return <Tab key={lang} label={t(getKeyForLanguage(lang))} value={lang} />;
            })}
          </Tabs>
        </Grid>
        <Grid item>
          <Checkbox onClick={onCompareModeChanged} checked={displayComparison} disabled={enabledLanguages.length <= 1}>
            {t('Ptv.TabControl.ShowComparison')}
          </Checkbox>
        </Grid>
      </Grid>
      <FormDivider mt={0} />
      {LanguagePriorities.map((lang) => {
        return (
          <TabPanel<enumTypes.Language> key={lang} tab={lang} selectedTab={selectedLanguageCode}>
            <ConnectionForm
              isComparing={displayComparison}
              control={props.control}
              enabledLanguages={enabledLanguages}
              setValue={props.setValue}
              getFormValues={props.getFormValues}
              trigger={props.trigger}
              expanderStates={expanderOpen}
              toggleExpander={toggleExpander}
            />
          </TabPanel>
        );
      })}
    </div>
  );
}
