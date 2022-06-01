import React, { ReactElement, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useDebounce } from 'react-use';
import { Grid, TextField } from '@mui/material';
import { styled } from '@mui/material/styles';
import i18n from 'i18';
import { DateTime } from 'luxon';
import { HintText, Label, StatusText } from 'suomifi-ui-components';
import { Language } from 'types/enumTypes';
import { StyledDiv } from './index';

const DateStatusErrorText = styled(StatusText)(({ theme }) => ({
  '&.fi-status-text': {
    marginTop: theme.suomifi.spacing.s,
  },
}));

type ScheduleLangVersionProps = {
  language: Language;
  labelText: string;
  hintText: string;
  id: string;
  disabled: boolean;
  scheduled: boolean;
  scheduledDate: DateTime;
  minScheduleDate: DateTime;
  maxScheduleDate: DateTime;
  setDate: (language: Language, date: string) => void;
  isValidDate: boolean;
  isValidDateCombination: boolean;
};

export function ScheduleLangVersion({
  language,
  labelText,
  hintText,
  id,
  disabled,
  scheduled,
  scheduledDate,
  maxScheduleDate,
  minScheduleDate,
  setDate,
  isValidDate,
  isValidDateCombination,
}: ScheduleLangVersionProps): ReactElement {
  const { t } = useTranslation();
  const [value, setValue] = useState<string>(scheduledDate.toISODate());

  const [, cancel] = useDebounce(() => setDate(language, value), 400, [value]);

  return (
    <StyledDiv>
      {scheduled && (
        <Grid container>
          <Grid item>
            <Label htmlFor={`${id}.timestamp`}>{labelText}</Label>
          </Grid>
          <Grid item>
            <HintText id={`${id}.timestamp-hint-text`}>{hintText}</HintText>
          </Grid>
          <Grid item>
            <TextField
              error={!isValidDate || !isValidDateCombination}
              disabled={disabled}
              id={`${id}.timestamp`}
              aria-labelledby={`${id}.timestamp-hint-text`}
              type='date'
              value={value}
              onChange={(evt) => setValue(evt.target.value)}
              onBlur={() => {
                cancel();
                setDate(language, value);
              }}
              InputLabelProps={{
                shrink: true,
              }}
              inputProps={{ max: maxScheduleDate.toISODate(), min: minScheduleDate.toISODate() }}
            />
            {(!isValidDate || !isValidDateCombination) && (
              <DateStatusErrorText status='error'>
                {!isValidDate &&
                  t('Ptv.PublishingDialog.InvalidDate', {
                    startDate: minScheduleDate.toLocaleString({}, { locale: i18n.language }),
                    endDate: maxScheduleDate.toLocaleString({}, { locale: i18n.language }),
                  })}
                {!isValidDate && !isValidDateCombination && <br />}
                {!isValidDateCombination && t('Ptv.PublishingDialog.InvalidSchedule')}
              </DateStatusErrorText>
            )}
          </Grid>
        </Grid>
      )}
    </StyledDiv>
  );
}
