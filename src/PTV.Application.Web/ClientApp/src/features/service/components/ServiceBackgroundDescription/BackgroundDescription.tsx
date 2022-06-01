import React from 'react';
import { useTranslation } from 'react-i18next';
import { Box } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { Block, Heading, Paragraph } from 'suomifi-ui-components';
import { Message } from 'components/Message';
import { TextEditorView } from 'components/TextEditorView';
import { useFormMetaContext } from 'context/formMeta';

const useStyles = makeStyles(() => ({
  message: {
    margin: '10px 0',
  },
}));

type BackgroundDescriptionProps = {
  name: string;
  id: string;
  value: string | null;
};

export function BackgroundDescription(props: BackgroundDescriptionProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();
  const { mode } = useFormMetaContext();

  function renderTextEditorView() {
    return <TextEditorView id={props.id} value={props.value} valueLabel={t('Ptv.Service.Form.FromGD.Label')} />;
  }

  return (
    <Block>
      <Box mb={2}>
        <Heading variant='h4'>{t('Ptv.Service.Form.FromGD.BackgroundDescription.Title.Text')}</Heading>
      </Box>
      <Box mb={2}>
        <Paragraph>{t('Ptv.Service.Form.FromGD.BackgroundDescription.Title.Description')}</Paragraph>
      </Box>
      {mode === 'view' && renderTextEditorView()}
      {mode === 'edit' && (
        <Message type='generalDescription' className={classes.message}>
          {renderTextEditorView()}
        </Message>
      )}
    </Block>
  );
}
