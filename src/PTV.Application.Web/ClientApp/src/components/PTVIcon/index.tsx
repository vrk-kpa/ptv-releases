import React from 'react';
import { makeStyles } from '@mui/styles';
import 'draft-js/dist/Draft.css';
import sprite from 'images/svgdefs.svg';

const useStyles = makeStyles(() => ({
  iconWrap: {
    display: 'inline-block',
    verticalAlign: 'text-top',
    cursor: 'pointer',
  },
}));

type PTVIconProps = {
  iconClass: string;
  iconHeight: number;
  iconWidth: number;
  role?: string;
  onClick?: () => void;
};

export default function PTVIcon(props: PTVIconProps): React.ReactElement {
  const classes = useStyles();
  return (
    <span role={props.role} onClick={props.onClick} className={classes.iconWrap}>
      <svg height={props.iconHeight} width={props.iconWidth}>
        <use href={sprite + '#' + props.iconClass} />
      </svg>
    </span>
  );
}
