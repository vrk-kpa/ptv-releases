import React, { FunctionComponent } from 'react';
import { Box, BoxProps } from '@mui/material';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';

const useStyles = makeStyles(() => ({
  fieldArea: {
    position: 'relative',
    border: '1px solid rgb(201, 205, 207)',
    backgroundColor: '#ffffff',
    marginTop: '20px',
  },
}));

export const FormFieldArea: FunctionComponent<BoxProps> = (props) => {
  const classes = useStyles();
  return (
    <Box className={clsx(classes.fieldArea, props.className)}>
      <Box mx='85px' pt='40px' pb='40px'>
        {props.children}
      </Box>
    </Box>
  );
};
