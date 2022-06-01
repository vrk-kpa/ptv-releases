import React from 'react';
import { useTranslation } from 'react-i18next';
import { Table, TableBody, TableCell, TableContainer, TableRow } from '@mui/material';
import { createStyles, makeStyles, withStyles } from '@mui/styles';
import { Text } from 'suomifi-ui-components';
import { ServiceChannelConnection } from 'types/api/serviceChannelModel';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import useTranslateLocalizedText from 'hooks/useTranslateLocalizedText';
import { getServiceChannelConnectionValue } from 'utils/serviceChannel';

const StyledTableCell = withStyles((theme) =>
  createStyles({
    head: {
      backgroundColor: 'rgb(255, 255, 255)',
    },
    body: {
      paddingLeft: '15px',
      paddingRight: '2px',
      paddingTop: '6px',
      paddingBottom: '8px',
      margin: '0px',
    },
  })
)(TableCell);

const StyledTableRow = withStyles((theme) =>
  createStyles({
    root: {
      '&:nth-of-type(even)': {
        backgroundColor: 'rgb(247, 247, 248)',
      },
    },
  })
)(TableRow);

const useStyles = makeStyles({
  tableContainer: {
    border: '1px solid rgb(200, 205, 208)',
  },
});

type ConnectedServicesTableProps = {
  connections: ServiceChannelConnection[];
};

export function ConnectedServicesTable(props: ConnectedServicesTableProps): React.ReactElement {
  const classes = useStyles();
  const { t } = useTranslation();
  const rows = props.connections;
  const translate = useTranslateLocalizedText();
  const lang = useGetUiLanguage();

  // TODO: replace with the custom <Table> component?
  return (
    <TableContainer className={classes.tableContainer}>
      <Table size='small' aria-label={t('Ptv.Service.Form.ServiceChannelSearch.SearchResult.ConnectedChannels.Table.Title')}>
        <TableBody>
          {rows.map((row) => (
            <StyledTableRow key={row.serviceId}>
              <StyledTableCell component='th' scope='row'>
                <Text smallScreen={true} variant='bold'>
                  {getServiceChannelConnectionValue(row.languageVersions, lang, (lv) => lv.serviceName, '')}
                </Text>
              </StyledTableCell>
              <StyledTableCell>
                <Text>{translate(row.serviceOrganization.texts)}</Text>
              </StyledTableCell>
            </StyledTableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
}
