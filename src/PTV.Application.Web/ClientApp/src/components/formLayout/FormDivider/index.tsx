import React, { FunctionComponent } from 'react';
import { Box, BoxProps, Divider } from '@mui/material';

export const FormDivider: FunctionComponent<BoxProps> = (props) => {
  return (
    <Box {...props}>
      <Divider />
    </Box>
  );
};
