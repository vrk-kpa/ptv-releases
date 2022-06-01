import React from 'react';
import { useTranslation } from 'react-i18next';
import { Block, Text } from 'suomifi-ui-components';
import { LinkModel } from 'types/link';
import { NoValueLabel } from 'components/NoValueLabel/NoValueLabel';
import { useFormMetaContext } from 'context/formMeta';
import { ServiceLawView } from './ServiceLawView';

interface IServiceLawsGdView {
  id: string;
  name: string;
  concise?: boolean;
  value: LinkModel[];
}

export function ServiceLawsGdView(props: IServiceLawsGdView): React.ReactElement {
  const { mode } = useFormMetaContext();
  const { t } = useTranslation();

  if (props.value.length === 0) {
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
      {props.value.map((item, index) => {
        return <ServiceLawView key={index} id={props.id} value={item} index={index} />;
      })}
    </Block>
  );
}
