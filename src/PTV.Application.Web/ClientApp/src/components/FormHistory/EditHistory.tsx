import React from 'react';
import { Trans, useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Heading, Link } from 'suomifi-ui-components';
import { EntityHistoryType } from 'types/api/entityHistoryType';
import { Language, MainEntityType } from 'types/enumTypes';
import { ShowMoreButton } from 'components/Buttons';
import { LastModifiedCell } from 'components/Cells';
import LoadingIndicator from 'components/LoadingIndicator';
import { CellType, RowType, Table } from 'components/Table';
import { useGetEditHistory } from 'hooks/queries/useGetEditHistory';
import { getOrderedLanguageVersionKeys } from 'utils/languages';
import { getKeyForLanguage } from 'utils/translations';

type EditHistoryProps = {
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

export default function EditHistory(props: EditHistoryProps): React.ReactElement {
  const historyQuery = useGetEditHistory(props.entityType, props.id, { enabled: props.isVisible });
  const { t } = useTranslation();
  const classes = useStyles();

  const title = t('Ptv.Form.Header.EditHistory.Title');

  const headers = [
    t('Ptv.Form.Header.EditHistory.Header.Event'),
    t('Ptv.Form.Header.EditHistory.Header.Languages'),
    t('Ptv.Form.Header.EditHistory.Header.Edited'),
  ];

  const getDescriptionComponent = (translationKey: string, row: EntityHistoryType): React.ReactElement => (
    <Trans
      i18nKey={translationKey}
      values={{
        version: row.version,
        templateOrganization: row.copyInfo?.templateOrganizationNames[props.selectedLanguage],
        nextVersion: row.nextVersion,
        sourceLanguage: row.sourceLanguage,
        targetLanguages: row.targetLanguages.join(', '),
      }}
      components={[
        <Link key={row.id} href={`/${props.entityType}/${row.id}`} className={classes.link}>
          sth
        </Link>,
      ]}
    />
  );

  const getSimpleDescription = (translationKey: string, row: EntityHistoryType): string =>
    t(translationKey, {
      version: row.version,
      templateOrganization: row.copyInfo?.templateOrganizationNames[props.selectedLanguage],
      nextVersion: row.nextVersion,
      sourceLanguage: row.sourceLanguage,
      targetLanguages: row.targetLanguages.join(', '),
    });

  const getDescription = (row: EntityHistoryType): CellType => {
    const translationKey = `Ptv.Entity.HistoryAction.${row.historyAction}`;
    return row.showLink
      ? {
          value: row.historyAction,
          customComponent: getDescriptionComponent(translationKey, row),
        }
      : {
          value: getSimpleDescription(translationKey, row),
        };
  };

  const getLanguageVersions = (row: EntityHistoryType): CellType => {
    const languages = getOrderedLanguageVersionKeys(row.languageVersions);
    return {
      value: languages.map((ln) => t(getKeyForLanguage(ln))).join(', '),
    };
  };

  const getEdited = (row: EntityHistoryType): CellType => ({
    value: row.editedAt,
    customComponent: <LastModifiedCell {...row} />,
  });

  const getRows = (): RowType[] =>
    (historyQuery.data?.pages ?? [])
      .map((page) => page.data)
      .flat()
      .map((row, index) => ({
        id: row.operationId ?? index,
        cells: [getDescription(row), getLanguageVersions(row), getEdited(row)],
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
