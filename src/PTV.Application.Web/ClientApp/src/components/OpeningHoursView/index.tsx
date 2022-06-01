import React from 'react';
import { OpeningHours } from 'types/api/connectionApiModel';
import { Language } from 'types/enumTypes';
import { ExceptionalHoursView } from './ExceptionalHoursView';
import { HolidayHoursView } from './HolidayHoursView';
import { SpecialHoursView } from './SpecialHoursView';
import { StandardHoursView } from './StandardHoursView';

type OpeningHoursViewProps = {
  openingHours: OpeningHours;
  language: Language;
};

function OpeningHoursView(props: OpeningHoursViewProps): React.ReactElement {
  const { openingHours } = props;
  return (
    <div>
      {openingHours.standardHours && StandardHoursView(openingHours.standardHours, props.language)}
      {openingHours.specialHours && SpecialHoursView(openingHours.specialHours, props.language)}
      {openingHours.exceptionalHours && ExceptionalHoursView(openingHours.exceptionalHours, props.language)}
      {openingHours.holidayHours && HolidayHoursView(openingHours.holidayHours)}
    </div>
  );
}

export { OpeningHoursView };
