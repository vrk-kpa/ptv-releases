import React from 'react';
import { Control, UseFormTrigger, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { RhfTextarea } from 'fields';
import { Language } from 'types/enumTypes';
import { ConnectionFormModel, cC, cPhoneNumberLv } from 'types/forms/connectionFormTypes';
import { FormBlock } from 'features/connection/components/FormLayout';
import { toFieldId } from 'features/connection/utils/fieldid';
import { ChargeTypeSelector } from './ChargeTypeSelector';
import { DialCodeTypeSelector } from './DialCodeTypeSelector';
import { PhoneNumberDialCode } from './PhoneNumberDialCode';
import { PhoneNumberInput } from './PhoneNumberInput';
import { PhoneNumberPreview } from './PhoneNumberPreview';

const useStyles = makeStyles((theme) => ({
  formBlock: {
    marginTop: '20px',
  },
}));

type PhoneNumberProps = {
  phoneNumberIndex: number;
  control: Control<ConnectionFormModel>;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  trigger: UseFormTrigger<any>;
  language: Language;
};

export function PhoneNumber(props: PhoneNumberProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();
  const lang = props.language;

  const dialCodeType = useWatch({
    name: `${cC.phoneNumbers}.${lang}.${props.phoneNumberIndex}.${cPhoneNumberLv.dialCodeType}`,
    control: props.control,
  });

  function getFieldName(lang: Language, fieldName: string): string {
    return `${cC.phoneNumbers}.${lang}.${props.phoneNumberIndex}.${fieldName}`;
  }

  function validate() {
    props.trigger(`${cC.phoneNumbers}.${lang}.${props.phoneNumberIndex}`);
  }

  const infoFieldName = getFieldName(lang, cPhoneNumberLv.additionalInformation);
  const chargeInfoFieldName = getFieldName(lang, cPhoneNumberLv.chargeDescription);

  return (
    <Grid container direction='column'>
      <Grid item>
        <FormBlock>
          <DialCodeTypeSelector control={props.control} phoneNumberIndex={props.phoneNumberIndex} onChanged={validate} language={lang} />
        </FormBlock>
      </Grid>
      {dialCodeType === 'Normal' && (
        <Grid item>
          <FormBlock className={classes.formBlock}>
            <PhoneNumberDialCode control={props.control} phoneNumberIndex={props.phoneNumberIndex} onChanged={validate} language={lang} />
          </FormBlock>
        </Grid>
      )}
      <Grid item>
        <FormBlock className={classes.formBlock}>
          <PhoneNumberInput control={props.control} phoneNumberIndex={props.phoneNumberIndex} onChanged={validate} language={lang} />
        </FormBlock>
      </Grid>
      <Grid item>
        <FormBlock className={classes.formBlock}>
          <PhoneNumberPreview control={props.control} phoneNumberIndex={props.phoneNumberIndex} language={lang} />
        </FormBlock>
      </Grid>
      <Grid item>
        <FormBlock className={classes.formBlock}>
          <RhfTextarea
            mode='edit'
            control={props.control}
            name={infoFieldName}
            id={toFieldId(infoFieldName)}
            labelText={t('Ptv.ConnectionDetails.PhoneNumber.Info.Label')}
            visualPlaceholder={t('Ptv.ConnectionDetails.PhoneNumber.Info.Placeholder')}
            hintText={t('Ptv.ConnectionDetails.PhoneNumber.Info.Hint')}
            optionalText={t('Ptv.Common.Optional')}
          />
        </FormBlock>
      </Grid>
      <Grid item>
        <FormBlock className={classes.formBlock}>
          <ChargeTypeSelector control={props.control} phoneNumberIndex={props.phoneNumberIndex} language={lang} />
        </FormBlock>
      </Grid>
      <Grid item>
        <FormBlock className={classes.formBlock}>
          <RhfTextarea
            mode='edit'
            control={props.control}
            name={chargeInfoFieldName}
            id={toFieldId(chargeInfoFieldName)}
            labelText={t('Ptv.ConnectionDetails.PhoneNumber.ChargeDescription.Label')}
            visualPlaceholder={t('Ptv.ConnectionDetails.PhoneNumber.ChargeDescription.Placeholder')}
            hintText={t('Ptv.ConnectionDetails.PhoneNumber.ChargeDescription.Hint')}
            optionalText={t('Ptv.Common.Optional')}
          />
        </FormBlock>
      </Grid>
    </Grid>
  );
}
