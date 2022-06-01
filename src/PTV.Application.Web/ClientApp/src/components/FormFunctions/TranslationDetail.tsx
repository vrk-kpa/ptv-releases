import React, { useState } from 'react';
import { Trans, useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Button, Heading, Link, Modal, ModalContent, ModalFooter, ModalTitle, Paragraph, Text } from 'suomifi-ui-components';
import { TranslationDetailApiType } from 'types/api/translationApiTypes';
import { PublishingStatus } from 'types/enumTypes';
import { CopyTextButton } from 'components/Buttons';
import OpenDetailsCell from 'components/Cells/OpenDetailsCell';
import LoadingIndicator from 'components/LoadingIndicator';
import { VisualHeading } from 'components/VisualHeading';
import { FormDivider } from 'components/formLayout/FormDivider';
import { useGetTranslationOrder } from 'hooks/translationOrders/useGetTranslationOrder';
import { toDateAndTime } from 'utils/date';
import { getKeyForLanguage, getKeysForStatusType } from 'utils/translations';

type TranslationDetailProps = {
  orderId: string;
  languageStatus: PublishingStatus;
};

const useStyles = makeStyles(() => ({
  modalContent: {
    '& .fi-text--body': {
      marginBottom: '5px',
      display: 'block',
    },
  },
  copyButton: {
    '&.fi-button': {
      marginTop: '20px',
    },
  },
}));

export default function TranslationDetail(props: TranslationDetailProps): React.ReactElement {
  const { t } = useTranslation();
  const [isOpen, setIsOpen] = useState<boolean>(false);
  const { data, isLoading } = useGetTranslationOrder(props.orderId, { enabled: isOpen });

  const classes = useStyles();

  const handleOpen = () => setIsOpen(true);

  const handleClose = () => setIsOpen(false);

  const renderModalContent = (loadedData: TranslationDetailApiType) => {
    const { sourceLanguage, targetLanguage, subscriberEmail, orderedAt, additionalInformation, orderId, orderNumber } = loadedData;

    return (
      <ModalContent className={classes.modalContent}>
        <ModalTitle>{t('Ptv.TranslationDetail.Title')}</ModalTitle>

        <VisualHeading variant='h4'>{t('Ptv.TranslationDetail.Languages')}</VisualHeading>
        <Text>{`${t(getKeyForLanguage(sourceLanguage))} > ${t(getKeyForLanguage(targetLanguage))}`}</Text>

        <VisualHeading variant='h4'>{t('Ptv.TranslationDetail.Status')}</VisualHeading>
        <Text>{t(getKeysForStatusType(props.languageStatus))}</Text>

        <VisualHeading variant='h4'>{t('Ptv.TranslationDetail.Subscriber')}</VisualHeading>
        <Text>{subscriberEmail}</Text>

        <VisualHeading variant='h4'>{t('Ptv.TranslationDetail.OrderedAt')}</VisualHeading>
        <Text>{toDateAndTime(orderedAt)}</Text>

        {!!additionalInformation && (
          <>
            <VisualHeading variant='h4'>{t('Ptv.TranslationDetail.AdditionalInformation')}</VisualHeading>
            <Text>{additionalInformation}</Text>
          </>
        )}
        <FormDivider my={3} />
        <Heading variant='h3'>{t('Ptv.TranslationDetail.Title2')}</Heading>

        <VisualHeading variant='h4'>{t('Ptv.TranslationDetail.OrderNumber')}</VisualHeading>
        <Text>{orderNumber}</Text>

        <VisualHeading variant='h4'>{t('Ptv.TranslationDetail.OrderId')}</VisualHeading>
        <Text>{orderId}</Text>

        <CopyTextButton className={classes.copyButton} text={orderId} label={t('Ptv.TranslationDetail.Copy')} />
        <FormDivider my={3} />
        <Paragraph>
          <Text>
            <Trans i18nKey='Ptv.TranslationDetail.Paragraph1'>
              If you would like to contact the translation agency regarding the order, please include the order identification information
              in the contact. Copy the information and paste it into an e-mail (<Link href='mailto:ptv@lingsoft.fi'>ptv@lingsoft.fi</Link>).
            </Trans>
          </Text>
          <Text>{t('Ptv.TranslationDetail.Paragraph2')}</Text>
        </Paragraph>
      </ModalContent>
    );
  };

  return (
    <div>
      <OpenDetailsCell onClick={handleOpen} label={t('Ptv.Form.Header.Translate.Status.Details')} />
      <Modal appElementId='root' visible={isOpen} onEscKeyDown={handleClose}>
        {!data || isLoading ? <LoadingIndicator /> : renderModalContent(data)}
        <ModalFooter>
          <Button onClick={handleClose}>{t('Ptv.Action.Cancel.Label')}</Button>
        </ModalFooter>
      </Modal>
    </div>
  );
}
