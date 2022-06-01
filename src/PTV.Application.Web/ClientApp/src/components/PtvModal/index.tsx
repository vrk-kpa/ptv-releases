import React from 'react';
import { makeStyles } from '@mui/styles';
import { Modal, ModalProps } from 'suomifi-ui-components';

const useStyles = makeStyles((theme) => ({
  ptvModal: {
    '&.fi-modal': {
      [theme.breakpoints.down('md')]: {
        width: `${theme.breakpoints.values['md']}px !important`,
      },
      [theme.breakpoints.only('lg')]: {
        width: `${(theme.breakpoints.values['lg'] / 12) * 9}px !important`,
      },
      [theme.breakpoints.only('xl')]: {
        width: `${(theme.breakpoints.values['xl'] / 12) * 7}px !important`,
      },
    },
  },
}));

type PtvModalProps = ModalProps & {
  children?: React.ReactNode | null | undefined;
};

export default function PtvModal(props: PtvModalProps): React.ReactElement {
  const classes = useStyles();
  return (
    <Modal
      className={classes.ptvModal}
      appElementId={props.appElementId}
      scrollable={props.scrollable !== undefined ? props.scrollable : true}
      visible={props.visible}
      onEscKeyDown={props.onEscKeyDown}
    >
      {props.children}
    </Modal>
  );
}
