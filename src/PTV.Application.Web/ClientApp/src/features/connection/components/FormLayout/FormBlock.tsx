import React from 'react';
import { Box, BoxProps } from '@mui/material';

export function FormBlock(props: BoxProps): React.ReactElement {
  return <Box {...props}>{props.children}</Box>;
}
