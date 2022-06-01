import React from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Paragraph, Text } from 'suomifi-ui-components';
import { PublishingStatus } from 'types/enumTypes';
import { CopyTextButton } from 'components/Buttons';
import { VisualHeading } from 'components/VisualHeading';

type IdInfoProps = {
  unificRootId: string;
  id: string;
  entityStatus: PublishingStatus;
  version: string;
};

const useStyles = makeStyles((theme) => ({
  copyButton: {
    display: 'block',
    marginTop: '5px',
  },
  idInfoContent: {
    '& .fi-paragraph': {
      marginBottom: '5px',
    },
    '& .fi-text--body': {
      display: 'block',
      marginTop: '5px',
    },
  },
  visualHeading: {},
}));

export default function IdInfo(props: IdInfoProps): React.ReactElement {
  const { t } = useTranslation();
  const copyLabel = t('Ptv.Form.Header.IdInformation.Copy');
  const { unificRootId, id, entityStatus, version } = props;
  const classes = useStyles();

  return (
    <span className={classes.idInfoContent}>
      <VisualHeading variant='h5'>{t('Ptv.Form.Header.IdInformation.UnificRootId')}</VisualHeading>
      <Paragraph>
        <Text>{unificRootId}</Text>
        <CopyTextButton label={copyLabel} text={unificRootId} asLink className={classes.copyButton} />
      </Paragraph>

      <VisualHeading variant='h5'>{t('Ptv.Form.Header.IdInformation.Id')}</VisualHeading>
      <Paragraph>
        <Text>{id}</Text>
        <CopyTextButton label={copyLabel} text={id} asLink className={classes.copyButton} />
      </Paragraph>

      <VisualHeading variant='h5'>{t('Ptv.Form.Header.IdInformation.Version')}</VisualHeading>
      <Paragraph>
        <Text>{`${entityStatus[0].toUpperCase()} ${version}`}</Text>
      </Paragraph>
    </span>
  );
}
