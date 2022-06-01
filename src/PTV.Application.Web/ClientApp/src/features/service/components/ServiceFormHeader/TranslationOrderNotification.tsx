import React, { useContext } from 'react';
import { useTranslation } from 'react-i18next';
import { DateTime } from 'luxon';
import { Notification, Paragraph } from 'suomifi-ui-components';
import { LastTranslationType } from 'types/forms/translationTypes';
import { DispatchContext } from 'context/formMeta';
import { SetTranslationNotificationVisibility } from 'context/formMeta/actions';
import { useGetNotificationStatuses } from 'context/formMeta/useGetNotificationStatuses';
import { displayDate, toOptionalDateTime } from 'utils/date';

type TranslationOrderNotificationProps = {
  className?: string;
  hasTranslationOrder: boolean;
  lastTranslations: LastTranslationType[];
};

export function TranslationOrderNotification(props: TranslationOrderNotificationProps): React.ReactElement {
  const { t } = useTranslation();
  const formMetaDispatch = useContext(DispatchContext);
  const notificationStatuses = useGetNotificationStatuses();

  const furthestTranslationDeliveryDate =
    props.lastTranslations.filter((x) => x.estimatedDelivery != null).length > 0
      ? props.lastTranslations
          .map((x) => toOptionalDateTime(x.estimatedDelivery))
          .reduce((previousValue, currentValue): DateTime | null => {
            if (!previousValue && !currentValue) {
              return null;
            }
            if (!!previousValue && !!currentValue) {
              return previousValue > currentValue ? previousValue : currentValue;
            }
            return previousValue != null ? previousValue : currentValue;
          })
      : null;

  return (
    <>
      {props.hasTranslationOrder && notificationStatuses.translationVisible && (
        <Notification
          closeText={t('Ptv.Common.Close')}
          headingText={t('Ptv.Service.Form.TranslationOrder.Notification.Heading')}
          onCloseButtonClick={() => {
            SetTranslationNotificationVisibility(formMetaDispatch, false);
          }}
        >
          <Paragraph>{t('Ptv.Service.Form.TranslationOrder.Notification.Text')}</Paragraph>
          {furthestTranslationDeliveryDate != null && (
            <Paragraph>
              {t('Ptv.Service.Form.TranslationOrder.Notification.Estimate', {
                estimate: displayDate(furthestTranslationDeliveryDate),
              })}
            </Paragraph>
          )}
        </Notification>
      )}
    </>
  );
}
