import React from 'react';
import { useTranslation } from 'react-i18next';
import { Grid, styled } from '@mui/material';
import { Icon, Text } from 'suomifi-ui-components';

type KeywordCreatedNotificationProps = {
  keyword: string;
};

const StyledIcon = styled(Icon)(() => ({
  '&.fi-icon.custom-icon': {
    width: '60px',
    height: '60px',
  },
}));

const ItemContainer = styled('div')({
  marginTop: '5px',
});

export function KeywordCreatedNotification(props: KeywordCreatedNotificationProps): React.ReactElement {
  const { t } = useTranslation();

  return (
    <Grid container justifyContent='center' alignItems='center' direction='column'>
      <Grid item>
        <StyledIcon icon='checkCircleFilled' fill='#09a581' className='custom-icon' />
      </Grid>
      <Grid item>
        <ItemContainer>
          <Text variant='bold'>{t('Ptv.Service.Form.Field.FreeKeywords.Message.KeywordAdded', { keyword: props.keyword })}</Text>
        </ItemContainer>
      </Grid>
    </Grid>
  );
}
