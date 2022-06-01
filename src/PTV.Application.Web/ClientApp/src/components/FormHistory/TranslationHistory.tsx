import React from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Heading, Paragraph } from 'suomifi-ui-components';
import { TranslationHistoryApiType } from 'types/api/translationApiTypes';
import { Language, MainEntityType } from 'types/enumTypes';
import { LastModifiedCell } from 'components/Cells';
import LoadingIndicator from 'components/LoadingIndicator';
import { CellType, RowType, Table } from 'components/Table';
import { useGetTranslationHistory } from 'hooks/queries/useGetTranslationHistory';
import { toDateAndTime } from 'utils/date';

type TranslationHistoryProps = {
  id: string;
  selectedLanguage: Language;
  entityType: MainEntityType;
  isVisible: boolean;
};

const useStyles = makeStyles(() => ({
  root: {
    '& p.noTopMargin': {
      marginTop: '18px',
    },
  },
}));

export default function TranslationHistory(props: TranslationHistoryProps): React.ReactElement {
  const historyQuery = useGetTranslationHistory(props.entityType, props.id, props.selectedLanguage, { enabled: props.isVisible });
  const { t } = useTranslation();
  const classes = useStyles();

  const title = t('Ptv.Form.Header.TranslationHistory.Title');

  const headers = [
    t('Ptv.Form.Header.TranslationHistory.Event'),
    t('Ptv.Form.Header.TranslationHistory.Translation'),
    t('Ptv.Form.Header.TranslationHistory.OrderNumber'),
    t('Ptv.Form.Header.TranslationHistory.Ordered'),
  ];

  const getEvent = (row: TranslationHistoryApiType): CellType => ({
    value: t(`Ptv.Translations.State.${row.state}`, {
      created: toDateAndTime(row.orderedAt),
      delivered: row.estimatedDelivery ? toDateAndTime(row.estimatedDelivery) : null,
    }),
  });

  const getTranslation = (row: TranslationHistoryApiType): CellType => ({
    value: `${row.sourceLanguage} > ${row.targetLanguage}`,
  });

  const getOrderNumber = (row: TranslationHistoryApiType): CellType => ({
    value: row.orderNumber.toString(),
  });

  const getOrdered = (row: TranslationHistoryApiType): CellType => ({
    value: row.orderedAt,
    customComponent: <LastModifiedCell editor={row.subscriberEmail} editedAt={row.orderedAt} />,
  });

  const getRows = (): RowType[] =>
    historyQuery.data?.map((row, index) => ({
      id: row.orderId ?? index,
      cells: [getEvent(row), getTranslation(row), getOrderNumber(row), getOrdered(row)],
    })) ?? [];

  return (
    <div className={classes.root}>
      <Heading variant='h4' as='h3'>
        {title}
      </Heading>
      <Paragraph className='noTopMargin'>{t('Ptv.Form.Header.TranslationHistory.Description')}</Paragraph>
      {historyQuery.isLoading ? <LoadingIndicator /> : <Table ariaLabel={title} headers={headers} rows={getRows()} />}
    </div>
  );
}
