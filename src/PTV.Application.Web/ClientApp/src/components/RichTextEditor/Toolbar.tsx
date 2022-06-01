import React from 'react';
import { useTranslation } from 'react-i18next';
import Grid from '@mui/material/Grid';
import { makeStyles } from '@mui/styles';
import clsx from 'clsx';
import { EditorState } from 'draft-js';
import 'draft-js/dist/Draft.css';
import { Dropdown, DropdownItem } from 'suomifi-ui-components';
import PTVIcon from 'components/PTVIcon';
import Counter from './Counter';
import './styles.css';

const useStyles = makeStyles((theme) => ({
  iconContainer: {
    display: 'flex',
    paddingTop: '10px',
    paddingLeft: '15px',
    paddingRight: '15px',
    paddingBottom: '2px',
    borderRight: '1px solid',
    borderRightColor: theme.suomifi.values.colors.depthDark3.hsl,
  },
  iconSelected: {
    backgroundColor: '#F6F6F7',
  },
  counter: {
    '& span': {
      marginRight: '8px',
    },
  },
}));

type ToolbarProps = {
  style: string;
  editorState: EditorState;
  maxCharacters: number;
  onStyleChange: (style: string) => void;
};

export const Unstyled = 'unstyled';
export const Heading = 'header-three';
export const UnorderedListItem = 'unordered-list-item';
export const OrderedListItem = 'ordered-list-item';

export function Toolbar(props: ToolbarProps): React.ReactElement {
  const { i18n, t } = useTranslation();

  function onStyleChange(value: string) {
    props.onStyleChange(value);
  }

  const dropDownSelectedValue = props.style === Heading ? props.style : Unstyled;

  const classes = useStyles();

  const bulletIconClassName = clsx(classes.iconContainer, {
    [classes.iconSelected]: props.style === UnorderedListItem,
  });

  const numberIconClassName = clsx(classes.iconContainer, {
    [classes.iconSelected]: props.style === OrderedListItem,
  });

  return (
    <Grid item container alignItems='center'>
      <Grid item>
        <Dropdown
          labelMode='hidden'
          labelText={t('Ptv.Component.RichTextEditor.Toolbar.Dropdown.Label.Text')}
          value={dropDownSelectedValue}
          onChange={onStyleChange}
          key={i18n.language} /* this line is a hack to force Dropdown component re-render on language change */
        >
          <DropdownItem value={Heading}>{t('Ptv.Component.RichTextEditor.Styles.Title.Text')}</DropdownItem>
          <DropdownItem value={Unstyled}>{t('Ptv.Component.RichTextEditor.Styles.Body.Text')}</DropdownItem>
        </Dropdown>
      </Grid>
      <Grid item>
        <div className={bulletIconClassName}>
          <PTVIcon
            role='button'
            onClick={() => onStyleChange(UnorderedListItem)}
            iconClass='icon-list-bullet'
            iconHeight={20}
            iconWidth={20}
          />
        </div>
      </Grid>
      <Grid item>
        <div className={numberIconClassName}>
          <PTVIcon
            role='button'
            onClick={() => onStyleChange(OrderedListItem)}
            iconClass='icon-list-number'
            iconHeight={20}
            iconWidth={20}
          />
        </div>
      </Grid>
      <Grid item style={{ flex: 1 }} />
      <Grid item className={classes.counter}>
        <Counter max={props.maxCharacters} editorState={props.editorState} />
      </Grid>
    </Grid>
  );
}
