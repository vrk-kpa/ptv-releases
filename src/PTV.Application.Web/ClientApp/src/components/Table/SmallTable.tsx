import React from 'react';
import { Table, TableBody, TableCell, TableContainer, TableRow } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { CellType, RowType } from '.';

export type TableProps = {
  ariaLabel: string;
  rows: RowType[];
};

const useCellStyles = makeStyles(() => ({
  body: {
    fontSize: '16px !important',
  },
  head: {
    fontWeight: 'bold !important',
    fontSize: '16px !important',
  },
}));

const useRowStyles = makeStyles(() => ({
  root: {
    '&:nth-of-type(odd)': {
      backgroundColor: 'rgb(247, 247, 248)',
    },
  },
}));

const useTableStyles = makeStyles((theme) => ({
  tableContainer: {
    border: '1px solid rgb(200, 205, 208)',
    margin: '10px 0 20px 0',
  },
}));

export default function SmallTable(props: TableProps): React.ReactElement {
  const cellClasses = useCellStyles();
  const rowClasses = useRowStyles();
  const tableClasses = useTableStyles();

  const renderCells = (cells: CellType[]) => {
    return cells.map((cell, index) => {
      const children = cell.customComponent ?? cell.value;

      if (cell.asHeader) {
        return (
          <TableCell classes={cellClasses} className={cell.className} key={index} component='th' scope='row' variant='head'>
            {children}
          </TableCell>
        );
      }

      return (
        <TableCell classes={cellClasses} className={cell.className} key={index}>
          {children}
        </TableCell>
      );
    });
  };

  const renderRows = () => {
    return props.rows.map((row) => (
      <TableRow classes={rowClasses} className={row.className} key={row.id}>
        {renderCells(row.cells)}
      </TableRow>
    ));
  };

  return (
    <TableContainer className={tableClasses.tableContainer}>
      <Table aria-label={props.ariaLabel} size='small'>
        <TableBody>{renderRows()}</TableBody>
      </Table>
    </TableContainer>
  );
}
