import React, { ReactElement, ReactNode } from 'react';
import { makeStyles } from '@mui/styles';

type HighlightProps = {
  children: ReactNode;
};

const useStyles = makeStyles((theme) => ({
  highlight: {
    backgroundColor: theme.colors.errorHighlight,
    textDecoration: 'underline',
    textDecorationStyle: 'wavy',
    textDecorationColor: theme.colors.errorUnderline,
  },
}));

export default function Highlight(props: HighlightProps): ReactElement {
  const classes = useStyles();
  return <span className={classes.highlight}>{props.children}</span>;
}
