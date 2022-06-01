import React from 'react';
import { makeStyles } from '@mui/styles';
import { Checkbox } from 'suomifi-ui-components';
import { TargetGroup } from 'types/targetGroupTypes';
import useTranslateLocalizedText from 'hooks/useTranslateLocalizedText';

const useStyles = makeStyles(() => ({
  checkBox: {
    marginBottom: '10px',
  },
}));

type TargetGroupCheckboxProps = {
  targetGroup: TargetGroup;
  selected: string[];
  select: (codes: string[]) => void;
  unselect: (codes: string[]) => void;
  id: string;
};

export default function TargetGroupCheckbox(props: TargetGroupCheckboxProps): React.ReactElement {
  const { targetGroup, id } = props;
  const translate = useTranslateLocalizedText();
  const selected = props.selected;

  function isSelected(targetGroupCode: string) {
    return selected.includes(targetGroupCode);
  }

  function toggleCheckbox(targetGroupCode: string) {
    if (selected.includes(targetGroupCode)) {
      props.unselect([targetGroupCode]);
    } else {
      props.select([targetGroupCode]);
    }
  }

  const classes = useStyles();

  return (
    <div className={classes.checkBox}>
      <Checkbox
        id={`${id}.${targetGroup.code}`}
        checked={isSelected(targetGroup.code)}
        key={targetGroup.code}
        onClick={() => toggleCheckbox(targetGroup.code)}
      >
        {translate(targetGroup.names)}
      </Checkbox>
    </div>
  );
}
