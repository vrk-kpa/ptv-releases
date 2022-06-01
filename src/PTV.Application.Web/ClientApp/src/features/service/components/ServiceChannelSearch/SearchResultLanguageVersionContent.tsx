import React from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Paragraph, Text } from 'suomifi-ui-components';
import { ConnectionApiModel } from 'types/api/connectionApiModel';
import { ServiceChannel } from 'types/api/serviceChannelModel';
import { DigitalAuthorizationModel } from 'types/digitalAuthTypes';
import { Language } from 'types/enumTypes';
import { LanguageStatus } from 'components/LanguageStatus';
import { OpeningHoursView } from 'components/OpeningHoursView';
import { useAppContextOrThrow } from 'context/useAppContextOrThrow';
import { getPlainText, parseRawDescription } from 'utils/draftjs';
import { hasItems } from 'utils/languageVersions';
import { AddressView } from './AddressView';
import { FaxNumbersView } from './FaxNumbersView';
import { PhoneNumbersView } from './PhoneNumbersView';
import { channelConnectionContainsGeneralData, channelLanguageVersionContainsData } from './utils';

const useStyles = makeStyles(() => ({
  loadingIndicator: {
    display: 'flex',
    flex: 1,
    justifyContent: 'center',
    alignItems: 'flex-start',
  },
  languageVersions: {
    marginTop: '10px',
  },
  languageVersion: {
    paddingLeft: '15px',
    borderLeftColor: '#a5abb0',
    borderLeft: '4px solid',
  },
  block: {
    marginTop: '20px',
  },
  innerBlock: {
    marginTop: '10px',
  },
  services: {
    marginTop: '10px',
  },
}));

type SearchResultLanguageVersionContentProps = {
  serviceChannel: ServiceChannel;
  channelConnection?: ConnectionApiModel;
};

function SearchResultLanguageVersionContent(props: SearchResultLanguageVersionContentProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();
  const appContext = useAppContextOrThrow();
  const digitalAuthorizations = appContext.staticData.digitalAuthorizations;

  const { serviceChannel, channelConnection } = props;
  const result: React.ReactElement[] = [];
  const languages = Object.keys(serviceChannel.languageVersions) as Language[];
  const openingHours = channelConnection?.openingHours;
  const someOpeningHoursDefined = openingHours && Object.values(openingHours).some((value) => value.length > 0);

  function getDigitalAuthorizatioName(
    model: DigitalAuthorizationModel[],
    authorization: string,
    lang: Language
  ): string | null | undefined {
    const auth = model.find((x) => x.id === authorization);
    if (auth) {
      return auth.names[lang];
    }
    const result = model.map((x) => getDigitalAuthorizatioName(x.children, authorization, lang)).find((x) => x !== null && x !== undefined);
    return result;
  }

  for (const lang of languages) {
    const serviceChannelLanguageVersion = serviceChannel.languageVersions[lang];
    const channelLanguageVersion = channelConnection?.languageVersions[lang];

    const showChannelDetails =
      channelLanguageVersion &&
      channelConnection &&
      (channelLanguageVersionContainsData(channelLanguageVersion) ||
        channelConnectionContainsGeneralData(channelConnection, lang) ||
        someOpeningHoursDefined);

    if (serviceChannelLanguageVersion) {
      result.push(
        <div key={lang}>
          <LanguageStatus
            status={serviceChannelLanguageVersion.status}
            language={serviceChannelLanguageVersion.language}
            isScheduled={!!serviceChannelLanguageVersion.scheduledPublish}
            className={classes.block}
          />
          {showChannelDetails && (
            <div className={classes.languageVersion}>
              {channelConnection && (
                <div className={classes.innerBlock}>
                  <Paragraph>
                    <Text variant='lead'>{t('Ptv.ConnectionDetails.Expanders.Title')}</Text>
                  </Paragraph>
                  {channelLanguageVersion?.description && (
                    <div className={classes.block}>
                      <Paragraph>
                        <Text variant='bold'>{t('Ptv.ConnectionDetails.BasicInformation.Description.Label')}</Text>
                      </Paragraph>
                    </div>
                  )}
                  <Paragraph>{getPlainText(parseRawDescription(channelLanguageVersion?.description))}</Paragraph>
                  {someOpeningHoursDefined && (
                    <div className={classes.block}>
                      <Paragraph>
                        <Text variant='bold'>{t('Ptv.ConnectionDetails.ServiceHours.Title')}</Text>
                      </Paragraph>
                      <OpeningHoursView openingHours={openingHours} language={lang} />
                    </div>
                  )}
                  {hasItems(channelConnection.emails, lang) && (
                    <div className={classes.block}>
                      <Text variant='bold'>{t('Ptv.ConnectionDetails.EmailAddress.Label')}</Text>
                      {channelConnection.emails[lang]?.map((email, _index) => {
                        return <Paragraph key={email.id}>{email.value}</Paragraph>;
                      })}
                    </div>
                  )}
                  {hasItems(channelConnection.phoneNumbers, lang) && (
                    <PhoneNumbersView phoneNumbers={channelConnection.phoneNumbers} lang={lang} />
                  )}
                  {channelConnection.addresses.length > 0 && <AddressView addresses={channelConnection.addresses} lang={lang} />}
                  {hasItems(channelConnection.webPages, lang) && (
                    <div className={classes.block}>
                      <Text variant='bold'>{t('Ptv.ConnectionDetails.WebPages.Title')}</Text>
                      {channelConnection.webPages[lang]?.map((webPage, _index) => {
                        return (
                          <div key={webPage.id}>
                            <Paragraph>{webPage.name}</Paragraph>
                            <Paragraph>{webPage.url}</Paragraph>
                            <Paragraph>{webPage.additionalInformation}</Paragraph>
                          </div>
                        );
                      })}
                    </div>
                  )}
                  {hasItems(channelConnection.faxNumbers, lang) && <FaxNumbersView faxNumbers={channelConnection.faxNumbers} lang={lang} />}
                  {channelConnection.digitalAuthorizations.length > 0 && (
                    <div className={classes.block}>
                      <Text variant='bold'>{t('Ptv.ConnectionDetails.Authorizations.Title')}</Text>
                      {channelConnection.digitalAuthorizations.map((authorization, _index) => {
                        return (
                          <Paragraph key={authorization}>{`- ${getDigitalAuthorizatioName(
                            digitalAuthorizations,
                            authorization,
                            lang
                          )}`}</Paragraph>
                        );
                      })}
                    </div>
                  )}
                </div>
              )}
            </div>
          )}
        </div>
      );
    }
  }

  return <div>{result.map((res) => res)}</div>;
}

export { SearchResultLanguageVersionContent };
