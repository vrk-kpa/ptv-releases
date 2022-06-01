import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { makeStyles } from '@mui/styles';
import { Chip, Text } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { GeneralDescriptionModel } from 'types/generalDescriptionTypes';
import { TextEditorView } from 'components/TextEditorView';
import { getGdValueOrDefault } from 'utils/gd';
import { RemoveGdModal } from './RemoveGdModal';

const useStyles = makeStyles(() => ({
  selectedGd: {
    marginTop: '10px',
  },
}));

type SelectedGdProps = {
  tabLanguage: Language;
  gd: GeneralDescriptionModel | null | undefined;
  remove: () => void;
};

export default function SelectedGd(props: SelectedGdProps): React.ReactElement {
  const { t } = useTranslation();
  const [isOpen, setIsOpen] = useState<boolean>(false);
  const classes = useStyles();

  function onRemoveChipClick() {
    setIsOpen(true);
  }

  function renderSelectedGd() {
    if (!props.gd) {
      return null;
    }

    return (
      <Chip removable onClick={onRemoveChipClick} actionLabel='Remove'>
        {getGdValueOrDefault(props.gd.languageVersions, props.tabLanguage, (lv) => lv.name, null)}
      </Chip>
    );
  }

  function onOk() {
    setIsOpen(false);
    props.remove();
  }

  function onCancel() {
    setIsOpen(false);
  }

  const instructions = props.gd?.languageVersions
    ? getGdValueOrDefault(props.gd.languageVersions, props.tabLanguage, (lv) => lv.userInstructions, null)
    : null;

  return (
    <div>
      {props.gd && (
        <div>
          <Text variant='bold'>{t('Ptv.Service.Form.GdSearch.SelectedGd.Title')}</Text>
          <div className={classes.selectedGd}>{renderSelectedGd()}</div>
          <TextEditorView id='foo' valueLabel={t('Ptv.Service.Form.GdSearch.SelectedGd.Instructions.Title')} value={instructions} />
          <RemoveGdModal gd={props.gd} visible={isOpen} onOk={onOk} onCancel={onCancel} />
        </div>
      )}
    </div>
  );
}
