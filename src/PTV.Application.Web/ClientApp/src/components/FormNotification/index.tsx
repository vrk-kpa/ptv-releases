import React from 'react';
import { makeStyles } from '@mui/styles';
import { Paragraph } from 'suomifi-ui-components';

enum FormNotificationTypes {
  'Warning',
  'Info',
}

type FormNotificationProps = {
  notificationType: FormNotificationTypes;
  text: string;
};

const useStyles = makeStyles((theme) => ({
  notification: {
    padding: '20px',
    borderLeft: '4px solid',
  },
  notificationWarning: {
    backgroundColor: theme.colors.notificationWarning,
    borderColor: 'rgb(232, 103, 23)',
  },
  notificationInfo: {
    backgroundColor: theme.colors.notificationInfo,
    borderColor: 'rgb(26, 153, 199)',
  },
}));

function FormNotification(props: FormNotificationProps): React.ReactElement {
  const classes = useStyles();
  return (
    <div
      className={`${classes.notification} ${
        props.notificationType === FormNotificationTypes.Info ? classes.notificationInfo : classes.notificationWarning
      }`}
    >
      {/* TODO: Warning icon */}
      <Paragraph>{props.text}</Paragraph>
    </div>
  );
}

export { FormNotification, FormNotificationTypes };
