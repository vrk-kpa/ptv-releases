import React from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { Text } from 'suomifi-ui-components';

type SearchResultTitleProps = {
  value: string;
  suggested: boolean;
};

const useStyles = makeStyles((theme) => ({
  right: {
    marginLeft: 'auto',
  },
  chip: {
    border: '1px solid rgb(155, 92, 178)',
    borderRadius: '24px',
    backgroundColor: 'rgb(155, 92, 178)',
    padding: '1px',
    paddingLeft: '10px',
    paddingRight: '10px',
    fontSize: '16px',
    color: 'rgb(255, 255, 255)',
  },
}));

export function SearchResultTitle(props: SearchResultTitleProps): React.ReactElement {
  const { t } = useTranslation();
  const classes = useStyles();
  if (!props.suggested) {
    return <Text>{props.value}</Text>;
  }

  return (
    <Grid container direction='row' alignItems='center' justifyContent='space-between'>
      <Grid item>
        <Text>{props.value}</Text>
      </Grid>
      <Grid item className={classes.right}>
        <span className={classes.chip}>{t('Ptv.Service.Form.ServiceChannelSearch.SearchResult.SuggestedChannel.Text')}</span>
      </Grid>
    </Grid>
  );
}
