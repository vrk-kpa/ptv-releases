import SmallTable from './SmallTable';
import Table from './Table';

export type CellType = {
  value: string;
  customComponent?: React.ReactElement | null | undefined;
  className?: string | undefined;
  asHeader?: boolean;
};

export type RowType = {
  id: string;
  cells: CellType[];
  className?: string | undefined;
};

export { Table, SmallTable };
