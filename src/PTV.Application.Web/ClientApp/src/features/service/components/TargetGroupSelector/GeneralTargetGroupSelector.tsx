import React from 'react';
import { makeStyles } from '@mui/styles';
import { ViewValueList } from 'fields';
import { HintText } from 'suomifi-ui-components';
import { TargetGroup } from 'types/targetGroupTypes';
import { useFormMetaContext } from 'context/formMeta';
import { Fieldset } from 'fields/Fieldset/Fieldset';
import { Legend } from 'fields/Legend/Legend';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { translateToLang } from 'utils/translations';
import TargetGroupCheckbox from './TargetGroupCheckbox';

const useStyles = makeStyles(() => ({
  header: {
    marginTop: '20px',
    marginBottom: '10px',
  },
  instructions: {
    '&.fi-hint-text': {
      marginBottom: '10px',
    },
  },
}));

type GeneralTargetGroupSelectorProps = {
  title: string;
  hint?: string;
  targetGroups: TargetGroup[];
  selected: string[];
  select: (codes: string[]) => void;
  unselect: (codes: string[]) => void;
  id: string;
};

export function GeneralTargetGroupSelector(props: GeneralTargetGroupSelectorProps): React.ReactElement {
  const uiLang = useGetUiLanguage();
  const classes = useStyles();
  const { mode } = useFormMetaContext();
  function renderCheckboxes() {
    return props.targetGroups.map((tg) => {
      return (
        <TargetGroupCheckbox
          id={props.id}
          key={tg.code}
          targetGroup={tg}
          select={props.select}
          unselect={props.unselect}
          selected={props.selected}
        />
      );
    });
  }

  if (mode === 'view') {
    const values = props.targetGroups.filter((x) => props.selected.includes(x.code)).map((tg) => translateToLang(uiLang, tg.names) ?? '');
    return <ViewValueList id={props.id} labelText={props.title} values={values} />;
  }

  const checkBoxes = renderCheckboxes();

  return (
    <Fieldset className={classes.header} id={props.id}>
      <Legend>{props.title}</Legend>
      {props.hint && <HintText className={classes.instructions}>{props.hint}</HintText>}
      {checkBoxes}
    </Fieldset>
  );
}
