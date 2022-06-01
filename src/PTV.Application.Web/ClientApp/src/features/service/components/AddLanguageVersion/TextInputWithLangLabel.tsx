import React from 'react';
import { useTranslation } from 'react-i18next';
import { styled } from '@mui/material/styles';
import clsx from 'clsx';
import { StaticChip, TextInput, TextInputProps } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { getKeyForLanguage } from 'utils/translations';

export type TextInputWithLangLabelProps = TextInputProps & {
  language: Language;
  inputClassName?: string;
};

export const TextInputWithLangLabel = styled(
  ({ language, className, inputClassName, ...rest }: TextInputWithLangLabelProps): React.ReactElement => {
    const { t } = useTranslation();

    return (
      <div className={className}>
        <StaticChip className='lang-chip'>{t(getKeyForLanguage(language))}</StaticChip>
        <TextInput id={`input-${language}`} className={clsx(inputClassName, 'fit-content')} {...rest} />
      </div>
    );
  }
)(({ theme }) => ({
  position: 'relative',
  width: 'fit-content',
  minWidth: '290px',
  maxWidth: '100%',
  margin: '10px 0 20px 25px',
  '& .fi-text-input.fit-content': {
    width: 'fit-content',
    '& .fi-label-text': {
      paddingRight: '120px',
    },
  },
  '& .lang-chip.lang-chip': {
    background: theme.suomifi.colors.depthSecondaryDark1,
    color: theme.suomifi.colors.blackBase,
    fontSize: '14px',
    position: 'absolute',
    top: '0px',
    right: '0px',
  },
}));

TextInputWithLangLabel.displayName = 'TextInputWithLangLabel';
