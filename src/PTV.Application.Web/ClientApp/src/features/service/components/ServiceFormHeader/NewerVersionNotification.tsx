import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, Notification, Paragraph } from 'suomifi-ui-components';
import { OtherVersionType } from 'types/forms/otherVersionType';
import { toDate } from 'utils/date';

type NewerVersionNotificationProps = {
  className?: string;
  otherModifiedVersion: OtherVersionType | null | undefined;
};

export function NewerVersionNotification(props: NewerVersionNotificationProps): React.ReactElement {
  const { t } = useTranslation();
  const [isVisible, setIsVisible] = useState(true);

  if (!isVisible || !props.otherModifiedVersion) {
    return <></>;
  }

  const linkToNewVersion = `/service/${props.otherModifiedVersion.id}`;
  const dateTime = toDate(`${props.otherModifiedVersion.modified}`);
  const notificationHeader = 'Ptv.Service.Form.Header.NewVersionNotification.HeadingText';
  const notificationText = 'Ptv.Service.Form.Header.NewVersionNotification.Text';
  const linkText = 'Ptv.Service.Form.Header.NewVersionNotification.LinkText';

  return (
    <Notification
      className={props.className}
      status='error'
      closeText={t('Ptv.Service.Form.Header.NewVersionNotification.Close')}
      headingVariant='h2'
      headingText={t(notificationHeader, { date: dateTime })}
      onCloseButtonClick={() => {
        setIsVisible(false);
      }}
    >
      <Paragraph>{t(notificationText)}</Paragraph>
      <Paragraph>
        <Link href={linkToNewVersion}>{t(linkText)}</Link>
      </Paragraph>
    </Notification>
  );
}
