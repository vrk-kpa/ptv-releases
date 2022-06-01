import React from 'react';
import { Control, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { ExternalLink, Heading, Paragraph } from 'suomifi-ui-components';
import { ServiceApiModel } from 'types/api/serviceApiModel';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { Message } from 'components/Message';
import { useGetQualityIssues } from 'context/qualityAgent/useGetQualityIssues';
import { getOrderedLanguageVersionKeys } from 'utils/languages';
import TranslationsModal from './TranslationsModal';
import TranslationsStatus from './TranslationsStatus';

type TranslationsProps = {
  canUpdate: boolean;
  control: Control<ServiceModel>;
  translationOrderSuccess: (data: ServiceApiModel) => void;
};

export default function Translations(props: TranslationsProps): React.ReactElement {
  const { t } = useTranslation();
  const lastTranslations = useWatch({ control: props.control, name: `${cService.lastTranslations}` });
  const qualityIssues = useGetQualityIssues();

  const languageVersions = useWatch({ control: props.control, name: `${cService.languageVersions}`, exact: true });
  const serviceId = useWatch({ control: props.control, name: `${cService.id}` });

  const languages = getOrderedLanguageVersionKeys(languageVersions);
  const translatableLanguages = languages.filter((ln) => languageVersions[ln]?.translationAvailability?.canBeTranslated);

  const canTranslate = !!serviceId && props.canUpdate && translatableLanguages.length > 0;

  const translationsWithStatus = languages
    .filter((ln) => {
      const translationAvailability = languageVersions[ln]?.translationAvailability;
      return translationAvailability?.isInTranslation || translationAvailability?.isTranslationDelivered;
    })
    .map((ln) => languageVersions[ln]);

  return (
    <div>
      <Heading variant='h4' as='h3'>
        {t('Ptv.Form.Header.Translate.Title')}
      </Heading>
      <Paragraph>{t('Ptv.Form.Header.Translate.Description')}</Paragraph>
      {!canTranslate && (
        <Message>
          {`${t('Ptv.Form.Header.Translate.Forbidden')} `}
          <ExternalLink labelNewWindow={t('Ptv.Link.Label.NewWindow')} href={t('Ptv.Form.Header.Translate.Forbidden.Link.Url')}>
            {t('Ptv.Form.Header.Translate.Forbidden.Link.Text')}
          </ExternalLink>
        </Message>
      )}
      {qualityIssues?.length > 0 && <Message>{t('Ptv.Form.Header.Translate.QualityIssues')}</Message>}

      <TranslationsModal
        canTranslate={canTranslate}
        serviceId={serviceId}
        languageVersions={languageVersions}
        lastTranslations={lastTranslations}
        translationOrderSuccess={props.translationOrderSuccess}
      />

      {translationsWithStatus.length > 0 && <TranslationsStatus lastTranslations={lastTranslations} control={props.control} />}
    </div>
  );
}
