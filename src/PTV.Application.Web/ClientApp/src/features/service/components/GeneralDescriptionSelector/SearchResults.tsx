import React from 'react';
import { useTranslation } from 'react-i18next';
import { Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from '@mui/material';
import { createStyles, makeStyles, withStyles } from '@mui/styles';
import { RadioButton, Text } from 'suomifi-ui-components';
import LoadingIndicator from 'components/LoadingIndicator';
import { GdSearchItem } from 'hooks/queries/useSearchGeneralDescriptions';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { getKeyForGdType, getKeyForServiceType, getTextByLangPriority } from 'utils/translations';

const StyledTableCell = withStyles((theme) =>
  createStyles({
    head: {
      backgroundColor: 'rgb(255, 255, 255)',
      borderBottom: '2px solid rgb(42, 110, 187)',
      paddingLeft: '20px',
    },
    body: {
      paddingLeft: '20px',
      maxWidth: '350px',
    },
  })
)(TableCell);

const StyledTableRow = withStyles((theme) =>
  createStyles({
    root: {
      '&:nth-of-type(odd)': {
        backgroundColor: 'rgb(247, 247, 248)',
      },
    },
  })
)(TableRow);

const useStyles = makeStyles({
  tableContainer: {
    maxHeight: 400,
    borderLeft: '1px solid rgb(200, 205, 208)',
    borderRight: '1px solid rgb(200, 205, 208)',
    borderBottom: '1px solid rgb(200, 205, 208)',
  },
  loadingIndicator: {
    marginTop: '20px',
    display: 'flex',
    justifyContent: 'center',
  },
});

type SearchResultsProps = {
  isLoading: boolean;
  items: GdSearchItem[];
  selected: GdSearchItem | null | undefined;
  onSelect: (gdSearchItem: GdSearchItem) => void;
};

export default function SearchResults(props: SearchResultsProps): React.ReactElement | null {
  const classes = useStyles();
  const rows = props.items;
  const uiLang = useGetUiLanguage();
  const { t } = useTranslation();

  if (props.isLoading) {
    return (
      <div className={classes.loadingIndicator}>
        <LoadingIndicator />
      </div>
    );
  }

  if (props.items.length === 0) {
    return null;
  }

  function onChange(event: React.ChangeEvent<HTMLInputElement>) {
    const item = rows.find((x) => x.id === event.target.value);
    if (item) {
      props.onSelect(item);
    }
  }

  // TODO: replace with the custom <Table> component?
  return (
    <TableContainer className={classes.tableContainer}>
      <Table stickyHeader aria-label={t('Ptv.Service.Form.GdSearch.SearchResult.Table.Name')}>
        <TableHead>
          <TableRow>
            <StyledTableCell>
              <Text variant='bold'>{t('Ptv.Service.Form.GdSearch.SearchResult.Column.Name.Title')}</Text>
            </StyledTableCell>
            <StyledTableCell>
              <Text variant='bold'>{t('Ptv.Service.Form.GdSearch.SearchResult.Column.ServiceType.Title')}</Text>
            </StyledTableCell>
            <StyledTableCell>
              <Text variant='bold'>{t('Ptv.Service.Form.GdSearch.SearchResult.Column.AreaType.Title')}</Text>
            </StyledTableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {rows.map((row) => (
            <StyledTableRow key={row.id}>
              <StyledTableCell component='th' scope='row'>
                <RadioButton onChange={onChange} value={row.id} checked={row.id === props.selected?.id}>
                  {getTextByLangPriority(uiLang, row.names, row.id)}
                </RadioButton>
              </StyledTableCell>
              <StyledTableCell>
                <Text>{t(getKeyForServiceType(row.serviceType))}</Text>
              </StyledTableCell>
              <StyledTableCell>
                <Text>{t(getKeyForGdType(row.generalDescriptionType))}</Text>
              </StyledTableCell>
            </StyledTableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
}
