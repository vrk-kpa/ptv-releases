import React, { useMemo } from 'react';
import { Grid } from '@mui/material';
import { makeStyles } from '@mui/styles';
import { Checkbox } from 'suomifi-ui-components';
import { DigitalAuthorizationModel } from 'types/digitalAuthTypes';
import { Fieldset } from 'fields/Fieldset/Fieldset';
import { Legend } from 'fields/Legend/Legend';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import useTranslateLocalizedText from 'hooks/useTranslateLocalizedText';
import { localeCompareTexts } from 'utils/translations';
import { FormBlock } from 'features/connection/components/FormLayout';

const useStyles = makeStyles(() => ({
  checkBox: {
    marginBottom: '10px',
  },
}));

type AuthorizationGroupProps = {
  group: DigitalAuthorizationModel;
  selected: string[];
  toggle: (id: string) => void;
};

export function AuthorizationGroup(props: AuthorizationGroupProps): React.ReactElement {
  const translate = useTranslateLocalizedText();
  const classes = useStyles();
  const lang = useGetUiLanguage();
  const { selected, toggle } = props;

  const checkboxes = useMemo(() => {
    const children = props.group.children.slice().sort((left, right) => localeCompareTexts(left.names, right.names, lang));
    return children.map((item: DigitalAuthorizationModel) => (
      <div key={item.id} className={classes.checkBox}>
        <Checkbox onClick={() => toggle(item.id)} key={item.id} checked={selected.includes(item.id)}>
          {translate(item.names, item.id)}
        </Checkbox>
      </div>
    ));
  }, [props.group, selected, translate, classes.checkBox, lang, toggle]);

  return (
    <Grid container direction='column'>
      <Fieldset>
        <Grid item>
          <Legend>{translate(props.group.names, props.group.id)}</Legend>
        </Grid>
        <Grid item>
          <FormBlock marginTop='20px'>{checkboxes}</FormBlock>
        </Grid>
      </Fieldset>
    </Grid>
  );
}
