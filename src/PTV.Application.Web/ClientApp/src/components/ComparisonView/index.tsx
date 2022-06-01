import React from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { styled } from '@mui/material/styles';
import { StaticChip } from 'suomifi-ui-components';
import { useFormMetaContext } from 'context/formMeta';
import { getKeyForLanguage } from 'utils/translations';

const StyledStaticChip = styled(StaticChip)(({ theme }) => ({
  '&.lang-chip.lang-chip': {
    background: theme.suomifi.colors.depthSecondaryDark1,
    color: theme.suomifi.colors.blackBase,
    fontSize: '14px',
    position: 'relative',
    top: '28px',
    right: '20px',
  },
}));

export type ComparisonViewProps = {
  left: React.ReactElement;
  right: React.ReactElement | null | undefined;
};

export function ComparisonView(props: ComparisonViewProps): React.ReactElement {
  const { t } = useTranslation();
  const meta = useFormMetaContext();

  if (!meta.compareLanguageCode || !props.right) {
    return props.left;
  }

  return (
    <Grid container spacing={2}>
      <Grid item xs={12} md={6}>
        <Grid container justifyContent='flex-end'>
          <Grid item>
            <StyledStaticChip className='lang-chip'>{t(getKeyForLanguage(meta.selectedLanguageCode))}</StyledStaticChip>
          </Grid>
        </Grid>
        {props.left}
      </Grid>
      <Grid item xs={12} md={6}>
        <Grid container justifyContent='flex-end'>
          <Grid item>
            <StyledStaticChip className='lang-chip'>{t(getKeyForLanguage(meta.compareLanguageCode))}</StyledStaticChip>
          </Grid>
        </Grid>
        {props.right}
      </Grid>
    </Grid>
  );
}
