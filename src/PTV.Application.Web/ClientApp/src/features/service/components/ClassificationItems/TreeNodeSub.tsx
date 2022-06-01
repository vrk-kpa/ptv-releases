import React, { FunctionComponent } from 'react';
import { makeStyles } from '@mui/styles';
import { Block } from 'suomifi-ui-components';
import { ClassificationItem } from 'types/classificationItemsTypes';
import { Fieldset } from 'fields/Fieldset/Fieldset';
import { Legend } from 'fields/Legend/Legend';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { translateToLang } from 'utils/translations';
import { NodeLabel } from './NodeLabel';
import { TreeNodeList } from './TreeNodeList';

const useStyles = makeStyles(() => ({
  root: {
    '& fieldset': {
      marginTop: '10px',
    },
  },
}));

interface TreeNodeSubInterface {
  item: ClassificationItem;
}

export const TreeNodeSub: FunctionComponent<TreeNodeSubInterface> = ({ item }) => {
  const classes = useStyles();
  const { children } = item;
  const uiLang = useGetUiLanguage();

  const label = translateToLang(uiLang, item.names) ?? '';

  return (
    <Block className={classes.root}>
      <Fieldset>
        <Legend id={item.id}>
          <NodeLabel label={label} />
        </Legend>
        <TreeNodeList items={children} />
      </Fieldset>
    </Block>
  );
};
