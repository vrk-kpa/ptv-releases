import React from 'react';
import { toDateAndTime } from 'utils/date';

type LastModifiedCellProps = {
  editor: string;
  editedAt: string;
};

export default function LastModifiedCell(props: LastModifiedCellProps): React.ReactElement {
  const { editor, editedAt } = props;

  return (
    <div>
      <div>{toDateAndTime(editedAt)}</div>
      <div>{editor}</div>
    </div>
  );
}
