import React from 'react';
import { makeStyles } from '@mui/styles';
import { Icon, Link } from 'suomifi-ui-components';
import { LinkButton } from 'components/Buttons';

type OpenDetailsCellProps = {
  label: string;
} & ({ link: string; onClick?: never } | { link?: never; onClick: () => void });

const useStyles = makeStyles((theme) => ({
  linkButton: {
    fontSize: '16px',
  },
  linkIcon: {
    color: theme.colors.link,
    marginRight: '5px',
  },
}));

export default function OpenDetailsCell(props: OpenDetailsCellProps): React.ReactElement {
  const classes = useStyles();

  return (
    <div>
      <Icon icon='infoFilled' className={classes.linkIcon} />
      {props.link && <Link href={props.link}>{props.label}</Link>}
      {props.onClick && <LinkButton onClick={props.onClick} label={props.label} className={classes.linkButton} />}
    </div>
  );
}
