import React from 'react';
import { useTranslation } from 'react-i18next';
import { Heading, Paragraph, Text } from 'suomifi-ui-components';
import { ConnectableChannel } from 'types/api/serviceChannelModel';
import { OrganizationModel } from 'types/organizationTypes';
import { VisualHeading } from 'components/VisualHeading';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { useTranslateWithUiLanguage } from 'hooks/useTranslateWithUiLanguage';
import { getConnectableChannelValue } from 'utils/serviceChannel';
import { FormBlock } from 'features/connection/components/FormLayout';
import { toFieldId } from 'features/connection/utils/fieldid';

type ConnectionSummaryProps = {
  serviceOrganization: OrganizationModel | null | undefined;
  serviceName: string;
  channel: ConnectableChannel;
};

export function ConnectionSummary(props: ConnectionSummaryProps): React.ReactElement {
  const { t } = useTranslation();
  const lang = useGetUiLanguage();
  const uitranslate = useTranslateWithUiLanguage();

  const channelOrg = uitranslate(props.channel.organization?.texts, props.channel.organization?.name || '???');
  const channelName = getConnectableChannelValue(props.channel.languageVersions, lang, (x) => x.name, '');
  const serviceOrg = uitranslate(props.serviceOrganization?.texts, props.serviceOrganization?.name || '???');
  const idPart = `summary.${lang}`;

  return (
    <div>
      <FormBlock marginTop='20px'>
        <Paragraph>{t('Ptv.ConnectionDetails.Description.Text')}</Paragraph>
      </FormBlock>

      <FormBlock marginTop='20px'>
        <Heading variant='h3'>{t('Ptv.ConnectionDetails.ConnectionHeading.Title')}</Heading>
      </FormBlock>

      <FormBlock marginTop='10px'>
        <VisualHeading variant='h5'>{t('Ptv.ConnectionDetails.ServiceHeading.Title')}</VisualHeading>
        <FormBlock marginTop='5px'>
          <Text id={toFieldId(`${idPart}.service`)}>{`${props.serviceName} - ${serviceOrg}`}</Text>
        </FormBlock>
      </FormBlock>

      <FormBlock marginTop='20px'>
        <VisualHeading variant='h5'>{t('Ptv.ConnectionDetails.ChannelHeading.Title')}</VisualHeading>
        <FormBlock marginTop='5px'>
          <Text id={toFieldId(`${idPart}.service-channel`)}>{`${channelName} - ${channelOrg}`}</Text>
        </FormBlock>
      </FormBlock>
    </div>
  );
}
