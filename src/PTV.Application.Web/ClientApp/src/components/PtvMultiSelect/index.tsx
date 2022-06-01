import { keyframes } from '@mui/material';
import { styled } from '@mui/material/styles';
import { MultiSelect } from 'suomifi-ui-components';

const loadingKeyframe = keyframes`
   to {
     transform: rotate(360deg);
    }
`;

// Simple wrapper for suomifi components multiselect, that will display
// loading indicator when class 'loading' is applied to the component
export const PtvMultiSelect = styled(MultiSelect)(({ theme }) => ({
  '&.fi-multiselect.loading': {
    '& .fi-filter-input_input-element-container': {
      position: 'relative',
      '&:before': {
        content: "''",
        position: 'absolute',
        top: '50%',
        right: '14px',
        width: '20px',
        height: '20px',
        marginTop: '-10px',
        borderStyle: 'solid',
        borderColor: `${theme.suomifi.colors.blackBase} transparent transparent transparent !important`,
        animation: `${loadingKeyframe} 1.5s cubic-bezier(0.4, 0, 0.4, 1) infinite`,
        borderWidth: '2px',
        borderRadius: '50%',
        pointerEvents: 'none',
      },
    },
  },
}));
