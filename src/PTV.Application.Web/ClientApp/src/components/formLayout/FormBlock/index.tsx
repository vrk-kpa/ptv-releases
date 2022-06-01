import React, { FunctionComponent } from 'react';
import { Box, BoxProps } from '@mui/material';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';
import { useFormMetaContext } from 'context/formMeta';

const useStyles = makeStyles(() => ({
  compareModeOff: {
    maxWidth: '700px',
  },
  compareModeOn: {
    maxWidth: 'none',
  },
}));

export const FormBlock: FunctionComponent<BoxProps> = (props) => {
  const { compareLanguageCode } = useFormMetaContext();
  const classes = useStyles();

  const formBlockClass = clsx(compareLanguageCode ? classes.compareModeOn : classes.compareModeOff);

  return (
    <Box className={formBlockClass} mt={2} {...props}>
      {props.children}
    </Box>
  );
};
