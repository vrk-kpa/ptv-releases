import React from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { RhfTextEditorView } from 'fields';
import { Block } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel, cGeneralDescription } from 'types/generalDescriptionTypes';
import { Message } from 'components/Message';
import { useFormMetaContext } from 'context/formMeta';

const useStyles = makeStyles(() => ({
  message: {
    margin: '10px 0',
  },
}));

type GdDescriptionProps = {
  gd: GeneralDescriptionModel | null | undefined;
  fieldName: string;
  language: Language;
  control: Control<ServiceModel>;
};

export function GdDescription(props: GdDescriptionProps): React.ReactElement | null {
  const { t } = useTranslation();
  const classes = useStyles();
  const { mode } = useFormMetaContext();
  const name = `${cService.generalDescription}.${cGeneralDescription.languageVersions}.${props.language}.${props.fieldName}`;

  if (!props.gd) return null;

  return (
    <Block>
      {mode === 'view' && (
        <RhfTextEditorView control={props.control} name={name} id={name} labelText={t('Ptv.Service.Form.FromGD.Label')} />
      )}
      {mode === 'edit' && (
        <Message type='generalDescription' className={classes.message}>
          <RhfTextEditorView control={props.control} name={name} id={name} labelText={t('Ptv.Service.Form.FromGD.Label')} />
        </Message>
      )}
    </Block>
  );
}
