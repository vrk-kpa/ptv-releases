import { makeStyles } from '@mui/styles';

export const useQualityStyles = makeStyles((theme) => ({
  textBorder: {
    '& input, textarea': {
      // Sorry for the !importants, but I cannot see any other way around
      // suomi-fi's strict designs.
      borderWidth: '2px !important',
      borderStyle: 'solid !important',
      borderColor: `${theme.suomifi.colors.warningBase} !important`,
    },
  },
  qualityList: {
    fontSize: '16px',
    color: theme.suomifi.colors.warningBase,
  },
  subList: {
    fontSize: '14px',
  },
  reset: {
    color: theme.suomifi.colors.blackBase,
  },
  message: {
    marginBottom: '20px',
  },
  divider: {
    marginBottom: '20px !important',
    marginTop: '20px !important',
  },
  expander: {
    marginTop: '20px',
    marginBottom: '20px',
  },
  warningBanner: {
    backgroundColor: '#fff6e0',
    borderLeft: 'solid 6px #e86717',
    fontSize: '16px',
    padding: '16px',
    margin: '10px 0px',
    display: 'flex',
    flexWrap: 'wrap',
    alignItems: 'center',
  },
  warningIcon: {
    marginRight: '16px',
  },
  extension: {
    marginTop: '8px',
  },
}));
