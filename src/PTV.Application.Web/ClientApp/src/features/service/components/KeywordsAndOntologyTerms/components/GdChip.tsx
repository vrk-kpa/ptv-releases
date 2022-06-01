import { styled } from '@mui/material/styles';
import { StaticChip } from 'suomifi-ui-components';

export const GdChip = styled(StaticChip)(({ theme }) => ({
  '&.fi-chip.custom': {
    backgroundColor: theme.suomifi.colors.accentTertiary,
  },
}));
