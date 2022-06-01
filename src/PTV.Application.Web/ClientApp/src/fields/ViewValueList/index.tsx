import React, { FunctionComponent } from 'react';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';
import { NoValueLabel } from 'fields';
import { Block, Text } from 'suomifi-ui-components';
import { VisualHeading } from 'components/VisualHeading';

interface IViewValueList {
  labelText?: string;
  values: string[];
  hideInViewMode?: boolean;
  id: string;
}

const useStyles = makeStyles(() => ({
  wrap: {
    '&.custom': {
      marginTop: '20px',
    },
  },
  label: {
    '&.custom': {
      marginBottom: '3px',
    },
  },
  list: {
    margin: '0',
    padding: '0 0 0 20px',
  },
  noValue: {
    '&.custom': {
      marginTop: '20px',
    },
  },
}));

export const ViewValueList: FunctionComponent<IViewValueList> = ({ labelText, values, hideInViewMode, id }) => {
  const classes = useStyles();

  if (hideInViewMode) {
    return null;
  }

  if (values.length === 0) {
    return (
      <Block id={id} className={clsx(classes.noValue, 'custom')}>
        {labelText && (
          <VisualHeading variant='h5' className={clsx(classes.label, 'custom')}>
            {labelText}
          </VisualHeading>
        )}
        <NoValueLabel />
      </Block>
    );
  }

  return (
    <Block id={id} className={clsx(classes.wrap, 'custom')}>
      {labelText && (
        <VisualHeading variant='h5' className={clsx(classes.label, 'custom')}>
          {labelText}
        </VisualHeading>
      )}
      <ul className={classes.list}>
        {values.map((value, index) => (
          <li key={index}>
            <Text variant='body' smallScreen>
              {value}
            </Text>
          </li>
        ))}
      </ul>
    </Block>
  );
};
