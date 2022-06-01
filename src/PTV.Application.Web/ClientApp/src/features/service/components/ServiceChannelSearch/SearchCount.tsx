import React from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { Checkbox, Text } from 'suomifi-ui-components';

type SearchCountProps = {
  count: number;
  showSuggestedChannels: boolean;
  showLatestFirst: boolean;
  serviceHasGeneralDescription: boolean;
  onShowSuggestedChannels: (value: boolean) => void;
  onShowLatestFirst: (value: boolean) => void;
};

const useStyles = makeStyles(() => ({
  count: {
    marginLeft: '10px',
    marginRight: 'auto',
  },
  suggestedChannels: {
    marginRight: '20px',
    marginLeft: 'auto',
  },
  latestFirst: {
    '& .fi-checkbox': {
      marginLeft: '20px',
      marginRight: '20px',
    },
  },
}));

export default function SearchCount(props: SearchCountProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();
  return (
    <Grid container alignItems='center'>
      <Grid item className={classes.count}>
        <Text variant='bold' smallScreen={true}>
          {props.count + ' ' + t('Ptv.Service.Form.ServiceChannelSearch.SearchCount.Text')}
        </Text>
      </Grid>
      {props.serviceHasGeneralDescription && (
        <Grid item className={classes.suggestedChannels}>
          <Checkbox onClick={(e) => props.onShowSuggestedChannels(e.checkboxState)} checked={props.showSuggestedChannels}>
            {t('Ptv.Service.Form.ServiceChannelSearch.ShowSuggested.Label')}
          </Checkbox>
        </Grid>
      )}
      <Grid item className={classes.latestFirst}>
        <Checkbox onClick={(e) => props.onShowLatestFirst(e.checkboxState)} checked={props.showLatestFirst}>
          {t('Ptv.Service.Form.ServiceChannelSearch.LatestFirst.Label')}
        </Checkbox>
      </Grid>
    </Grid>
  );
}
