import React, { FunctionComponent, useCallback, useMemo } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { RadioOption, RhfRadioButtonGroup } from 'fields';
import { Language, YesNoType } from 'types/enumTypes';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { OrganizationModel } from 'types/organizationTypes';
import { Message } from 'components/Message';
import { useFormMetaContext } from 'context/formMeta';
import { getFieldId } from 'utils/fieldIds';

interface SelfProducerSelectInterface {
  defaultSelfProducers: OrganizationModel[];
  tabLanguage: Language;
  control: Control<ServiceModel>;
  setSelfProducers: (organizations: OrganizationModel[]) => void;
  hasSelfProducers: boolean;
}

export const SelfProducerSelect: FunctionComponent<SelfProducerSelectInterface> = (props) => {
  const { t } = useTranslation();
  const { mode } = useFormMetaContext();
  const id = getFieldId(`${cService.hasSelfProducers}`, props.tabLanguage, undefined);

  const items = useMemo((): RadioOption[] => {
    return [
      {
        value: YesNoType.YES,
        text: t('Ptv.Common.Yes'),
      },
      {
        value: YesNoType.NO,
        text: t('Ptv.Common.No'),
      },
    ];
  }, [t]);

  function onChanged(value: boolean) {
    if (value) {
      props.setSelfProducers([...props.defaultSelfProducers]);
    } else {
      props.setSelfProducers([]);
    }
  }

  const toFieldValue = useCallback((str: string): boolean => {
    return str === YesNoType.YES ? true : false;
  }, []);

  const toRadioButtonValue = useCallback((value: boolean): string => {
    return value === true ? YesNoType.YES : YesNoType.NO;
  }, []);

  return (
    <Box>
      <RhfRadioButtonGroup<boolean>
        control={props.control}
        id={id}
        name={`${cService.hasSelfProducers}`}
        mode={mode}
        items={items}
        labelText={t('Ptv.Service.Form.Field.ServiceProducers.HasSelfProducers.Label')}
        toFieldValue={toFieldValue}
        toRadioButtonValue={toRadioButtonValue}
        onChanged={onChanged}
      />
      {!props.hasSelfProducers && <Message>{t('Ptv.Service.Form.Field.ServiceProducers.HasSelfProducers.Message.NotSelected')}</Message>}
    </Box>
  );
};
