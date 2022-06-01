import React from 'react';
import { CircularProgress } from '@mui/material';

export default function LoadingIndicator(props: { size?: string | null | undefined }): React.ReactElement {
  // TODO Replace with PTV style progress indicator
  return <CircularProgress size={props.size ? props.size : 40} />;
}
