import React, { FunctionComponent } from 'react';
import { Control, UseFormTrigger, useFieldArray } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Box } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { Block, Button, Paragraph, Text } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ServiceModel, cLv, cService } from 'types/forms/serviceFormTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { Link, LinkModel } from 'types/link';
import { Message } from 'components/Message';
import { getGdValueOrDefault } from 'utils/gd';
import { ServiceLaw } from './ServiceLaw';
import { ServiceLawsGdView } from './ServiceLawsGdView';

const useStyles = makeStyles((theme) => ({
  fieldArray: {
    border: `1px solid ${theme.colors.archived}`,
    marginBottom: '20px',
  },
  message: {
    marginBottom: '20px',
  },
}));

interface ServiceLawsEditInterface {
  name: string;
  id: string;
  language: Language;
  gd: GeneralDescriptionModel | null | undefined;
  control: Control<ServiceModel>;
  trigger: UseFormTrigger<ServiceModel>;
}

export const ServiceLawsEdit: FunctionComponent<ServiceLawsEditInterface> = ({ name, id, language, gd, control, trigger }) => {
  const { t } = useTranslation();
  const classes = useStyles();

  const { fields, append, remove } = useFieldArray({
    name: `${cService.languageVersions}.${language}.${cLv.laws}`,
    control: control,
  });

  function getGdLaws(): LinkModel[] | undefined {
    if (!gd) {
      return [];
    }

    return getGdValueOrDefault(gd.languageVersions, language, (x) => x.laws, undefined);
  }

  function removeLaw(index: number) {
    remove(index);
    trigger(`${cService.languageVersions}.${language}.${cLv.laws}`);
  }

  function addLaw() {
    const index = fields.length;
    append(Link());
    trigger(`${cService.languageVersions}.${language}.${cLv.laws}.${index}`);
  }

  const gdLaws = getGdLaws() || [];

  return (
    <Box mb={2}>
      <Box mb={2}>
        <Paragraph>{t('Ptv.Service.Form.Field.Laws.Description')}</Paragraph>
      </Box>
      {gd && (
        <Message type='generalDescription' className={classes.message}>
          <Text variant='bold' smallScreen>
            {t('Ptv.Service.Form.FromGD.Preview.Label')}
          </Text>
          <Box mt={2}>
            <ServiceLawsGdView name={name} id={`from-gd-languageVersions[${language}].laws`} value={gdLaws} concise />
          </Box>
        </Message>
      )}
      <Box>
        {fields.map((item, index) => {
          return (
            <div key={item.id}>
              <div className={classes.fieldArray}>
                <Block padding='m'>
                  <ServiceLaw mode='edit' name={name} id={id} index={index} onRemove={() => removeLaw(index)} control={control} />
                </Block>
              </div>
            </div>
          );
        })}
        <Button id={`add.law.${language}`} type='button' onClick={addLaw}>
          {t('Ptv.Service.Form.Field.Laws.AddNew.Label')}
        </Button>
      </Box>
    </Box>
  );
};
