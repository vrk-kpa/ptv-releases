import React from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Heading, Link } from 'suomifi-ui-components';
import { ConnectionHistoryType } from 'types/api/connectionHistoryType';
import { SimpleLanguageVersionType } from 'types/api/simpleLanguageVersionType';
import { Language, MainEntityType } from 'types/enumTypes';
import { ShowMoreButton } from 'components/Buttons';
import { LastModifiedCell } from 'components/Cells';
import LoadingIndicator from 'components/LoadingIndicator';
import { CellType, RowType, Table } from 'components/Table';
import { useGetConnectionHistory } from 'hooks/queries/useGetConnectionHistory';
import { getOrderedLanguageVersionKeys } from 'utils/languages';
import { getKeyForLanguage } from 'utils/translations';

type ConnectionsHistoryProps = {
  id: string;
  selectedLanguage: Language;
  entityType: MainEntityType;
  isVisible: boolean;
};

const useStyles = makeStyles(() => ({
  link: {
    fontSize: '16px !important',
  },
}));

export default function ConnectionsHistory(props: ConnectionsHistoryProps): React.ReactElement {
  const historyQuery = useGetConnectionHistory(props.entityType, props.id, { enabled: props.isVisible });
  const { t } = useTranslation();
  const classes = useStyles();

  const title = t('Ptv.Form.Header.ConnectionHistory.Title');

  const headers = [
    t('Ptv.Form.Header.ConnectionHistory.Name'),
    t('Ptv.Form.Header.ConnectionHistory.Languages'),
    t('Ptv.Form.Header.ConnectionHistory.Operation'),
    t('Ptv.Form.Header.ConnectionHistory.Edited'),
  ];

  const getLink = (row: ConnectionHistoryType): string => {
    switch (row.entityType) {
      case 'Channel':
        return `${window.location.origin}/channels/${row.subEntityType}/${row.id}`;
      default:
        return `${window.location.origin}/${row.entityType}/${row.id}`;
    }
  };

  const getName = (row: ConnectionHistoryType): CellType => {
    const languages = getOrderedLanguageVersionKeys<SimpleLanguageVersionType | undefined>(row.languageVersions);
    const name = row.languageVersions[props.selectedLanguage]?.name ?? row.languageVersions[languages[0]]?.name ?? '';
    return {
      value: name,
      customComponent: (
        <Link className={classes.link} href={getLink(row)}>
          {name}
        </Link>
      ),
    };
  };

  const getLanguageVersions = (row: ConnectionHistoryType): CellType => {
    const languages = getOrderedLanguageVersionKeys(row.languageVersions);
    return {
      value: languages.map((ln) => t(getKeyForLanguage(ln))).join(', '),
    };
  };

  const getDescription = (row: ConnectionHistoryType) => {
    const translationKey = `Ptv.Entity.ConnectionOperations.${row.operationType}`;
    return { value: t(translationKey) };
  };

  const getEdited = (row: ConnectionHistoryType): CellType => ({
    value: row.editedAt,
    customComponent: <LastModifiedCell {...row} />,
  });

  const getRows = (): RowType[] =>
    (historyQuery.data?.pages ?? [])
      .map((page) => page.data)
      .flat()
      .map((row, index) => ({
        id: row.operationId ?? index,
        cells: [getName(row), getLanguageVersions(row), getDescription(row), getEdited(row)],
      })) ?? [];

  const showMoreButton = historyQuery.hasNextPage ? (
    <ShowMoreButton fetchNextPage={historyQuery.fetchNextPage} isLoading={historyQuery.isFetchingNextPage} />
  ) : undefined;

  return (
    <div>
      <Heading variant='h4' as='h3'>
        {title}
      </Heading>
      {historyQuery.isLoading ? (
        <LoadingIndicator />
      ) : (
        <Table ariaLabel={title} headers={headers} rows={getRows()} appendix={showMoreButton} />
      )}
    </div>
  );
}
