import React from 'react';
import { Control, useWatch } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import Box from '@mui/material/Box';
import { Heading, Paragraph } from 'suomifi-ui-components';
import { ServiceModel, cService } from 'types/forms/serviceFormTypes';
import { LastTranslationType } from 'types/forms/translationTypes';
import { CellWithStatus } from 'components/Cells';
import { CellType, RowType, SmallTable } from 'components/Table';
import { displayDateTime, toLocalDateTime } from 'utils/date';
import { getKeyForLanguage } from 'utils/translations';
import TranslationDetail from './TranslationDetail';

type TranslationsStatusProps = {
  lastTranslations: LastTranslationType[];
  control: Control<ServiceModel>;
};

export default function TranslationsStatus(props: TranslationsStatusProps): React.ReactElement {
  const { t } = useTranslation();
  const languageVersions = useWatch({ control: props.control, name: `${cService.languageVersions}` });

  const getCells = (lastTranslation: LastTranslationType): CellType[] => {
    const source = t(getKeyForLanguage(lastTranslation.sourceLanguage));
    const target = t(getKeyForLanguage(lastTranslation.targetLanguage));
    const status = languageVersions[lastTranslation.targetLanguage]?.status ?? 'Draft';
    const statusText = t(`Ptv.Translations.State.${lastTranslation.state}`, {
      created: displayDateTime(toLocalDateTime(lastTranslation.orderedAt)),
      delivered: lastTranslation.estimatedDelivery ? displayDateTime(toLocalDateTime(lastTranslation.estimatedDelivery)) : null,
    });

    return [
      {
        value: `${source} > ${target}`,
        asHeader: true,
      },
      {
        value: '',
        customComponent: <CellWithStatus status={status} value={statusText} />,
      },
      {
        value: '',
        customComponent: <TranslationDetail orderId={lastTranslation.translationId} languageStatus={status} />,
      },
    ];
  };

  const getRows = (): RowType[] =>
    props.lastTranslations.map((lt) => ({
      cells: getCells(lt),
      id: lt.translationId,
    }));

  return (
    <div>
      <Box mt={2} mb={1}>
        <Heading variant='h4' as='h3'>
          {t('Ptv.Form.Header.Translate.Status.Title')}
        </Heading>
      </Box>
      <Paragraph>{t('Ptv.Form.Header.Translate.Status.Description')}</Paragraph>
      <Box mt={2}>
        <SmallTable ariaLabel={t('Ptv.Form.Header.Translate.Status.Title')} rows={getRows()} />
      </Box>
    </div>
  );
}
