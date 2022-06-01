import React from 'react';
import { Control, useFieldArray } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Block, Text } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { ServiceModel, cLv, cService } from 'types/forms/serviceFormTypes';
import { NoValueLabel } from 'components/NoValueLabel/NoValueLabel';
import { useFormMetaContext } from 'context/formMeta';
import { ServiceLawView } from './ServiceLawView';

interface IServiceLawsView {
  id: string;
  name: string;
  concise?: boolean;
  language: Language;
  control: Control<ServiceModel>;
}

export function ServiceLawsView(props: IServiceLawsView): React.ReactElement {
  const { mode } = useFormMetaContext();
  const { t } = useTranslation();

  const { fields } = useFieldArray({
    name: `${cService.languageVersions}.${props.language}.${cLv.laws}`,
    control: props.control,
  });

  if (fields.length === 0) {
    if (mode === 'view') {
      return <NoValueLabel />;
    } else {
      return (
        <Text variant='body' smallScreen>
          {t('Ptv.Service.Form.FromGD.EmptyMessage')}
        </Text>
      );
    }
  }

  return (
    <Block>
      {fields.map((item, index) => {
        return <ServiceLawView key={item.id} id={props.id} value={item} index={index} />;
      })}
    </Block>
  );
}
