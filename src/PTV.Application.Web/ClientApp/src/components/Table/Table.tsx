import React from 'react';
import { Table as MuiTable, TableBody, TableCell, TableContainer, TableHead, TableRow } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { CellType, RowType } from '.';

const useCellStyles = makeStyles(() => ({
  head: {
    backgroundColor: 'rgb(255, 255, 255)',
    borderBottom: '2px solid rgb(42, 110, 187)',
    padding: '15px 0 15px 20px',
    fontSize: '16px',
    fontWeight: 'bold',
  },
  body: {
    padding: '10px 0 10px 20px',
    maxWidth: '350px',
    fontSize: '16px',
  },
}));

const useRowStyles = makeStyles(() => ({
  root: {
    '&:nth-of-type(odd)': {
      backgroundColor: 'rgb(247, 247, 248)',
    },
  },
}));

const useStyles = makeStyles({
  tableContainer: {
    maxHeight: 400,
    border: '1px solid rgb(200, 205, 208)',
    marginTop: '20px',
  },
  loadingIndicator: {
    marginTop: '20px',
    display: 'flex',
    justifyContent: 'center',
  },
  columnHeader: {
    whiteSpace: 'nowrap',
  },
});

export type TableProps = {
  ariaLabel: string;
  headers: string[];
  rows: RowType[];
  appendix?: React.ReactElement | undefined;
};

export default function Table(props: TableProps): React.ReactElement {
  const classes = useStyles();
  const rowClasses = useRowStyles();
  const cellClasses = useCellStyles();

  const renderCell = (cell: CellType) => {
    if (cell.customComponent) {
      return (
        <TableCell key={cell.value} classes={cellClasses} className={cell.className}>
          {cell.customComponent}
        </TableCell>
      );
    }

    return (
      <TableCell key={cell.value} classes={cellClasses} className={cell.className}>
        {cell.value}
      </TableCell>
    );
  };

  const renderRow = (row: RowType) => {
    return (
      <TableRow key={row.id} classes={rowClasses}>
        {row.cells.map((cell) => renderCell(cell))}
      </TableRow>
    );
  };

  return (
    <TableContainer className={classes.tableContainer}>
      <MuiTable stickyHeader aria-label={props.ariaLabel}>
        <TableHead>
          <TableRow>
            {props.headers.map((header) => (
              <TableCell key={header} classes={cellClasses}>
                <span className={classes.columnHeader}>{header}</span>
              </TableCell>
            ))}
          </TableRow>
        </TableHead>
        <TableBody>{props.rows.map((row) => renderRow(row))}</TableBody>
      </MuiTable>
      {props.appendix}
    </TableContainer>
  );
}
