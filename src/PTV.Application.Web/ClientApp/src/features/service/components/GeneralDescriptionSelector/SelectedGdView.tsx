import React, { FunctionComponent } from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Text } from 'suomifi-ui-components';
import * as enumTypes from 'types/enumTypes';
import { GeneralDescriptionModel, cGeneralDescription } from 'types/generalDescriptionTypes';
import { Message } from 'components/Message';
import { TextEditorView } from 'components/TextEditorView';
import { useFormMetaContext } from 'context/formMeta';
import { getGdValueOrDefault } from 'utils/gd';

const useStyles = makeStyles(() => ({
  root: {
    paddingTop: '20px',
    marginTop: '20px',
  },
  label: {
    marginRight: '5px',
  },
}));

interface SelectedGdViewInterface {
  selectedGeneralDescription: GeneralDescriptionModel | null | undefined;
}

export const SelectedGdView: FunctionComponent<SelectedGdViewInterface> = ({ selectedGeneralDescription }) => {
  const { t } = useTranslation();
  const classes = useStyles();
  const ctx = useFormMetaContext();

  function getName(gd: GeneralDescriptionModel, language: enumTypes.Language): React.ReactElement {
    const value = gd.languageVersions[language];
    if (value?.name) {
      return (
        <Text smallScreen variant='bold'>
          {value.name}
        </Text>
      );
    }

    return (
      <Text variant='bold' smallScreen>
        {t('Ptv.Service.Form.AttachedGD.NotAvailableInSelectedLanguage.Label')}
      </Text>
    );
  }

  function getUseOfGeneralDescription(gd: GeneralDescriptionModel, language: enumTypes.Language) {
    if (!gd) {
      return null;
    }

    const gdTypeAdditionalInformation = gd.languageVersions
      ? getGdValueOrDefault(gd.languageVersions, language, (lv) => lv.generalDescriptionTypeAdditionalInformation, null)
      : null;

    return (
      <TextEditorView
        id={`languageVersions[${language}].${cGeneralDescription.generalDescriptionTypeAdditionalInformation}`}
        value={gdTypeAdditionalInformation}
      />
    );
  }

  if (!selectedGeneralDescription) {
    return null;
  }

  return (
    <Message className={classes.root}>
      <span className={classes.label}>{t('Ptv.Service.Form.AttachedGD.Label')}</span>
      {getName(selectedGeneralDescription, ctx.selectedLanguageCode)}
      {getUseOfGeneralDescription(selectedGeneralDescription, ctx.selectedLanguageCode)}
    </Message>
  );
};
