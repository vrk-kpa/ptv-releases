import { Language } from 'types/enumTypes';
import { cC, cDaily, cHour, cTimeRange } from 'types/forms/connectionFormTypes';

const FieldIdPrefix = 'connection-form';

export function getFormLvFieldName(fieldName: string, language: Language, compareLanguage: Language | undefined): string {
  const lang = compareLanguage || language;
  return `${cC.languageVersions}.${lang}.${fieldName}`;
}

export function getStdHourLvFieldName(hourIndex: number, lang: Language, fieldName: string): string {
  return `${cC.standardOpeningHours}.${hourIndex}.languageVersions.${lang}.${fieldName}`;
}

export function getStdHourFieldName(hourIndex: number, fieldName: string): string {
  return `${cC.standardOpeningHours}.${hourIndex}.${fieldName}`;
}

function getWeekdayPath(hourIndex: number, dayIndex: number): string {
  return `${cC.standardOpeningHours}.${hourIndex}.${cHour.dailyOpeningTimes}.${dayIndex}`;
}

export function getWeekdayFieldName(hourIndex: number, dayIndex: number, fieldName: string): string {
  const weekday = getWeekdayPath(hourIndex, dayIndex);
  return `${weekday}.${fieldName}`;
}

export function getTimeRangeFromFieldName(hourIndex: number, dayIndex: number, timesIndex: number): string {
  const times = getWeekdayFieldName(hourIndex, dayIndex, cDaily.times);
  return `${times}.${timesIndex}.${cTimeRange.from}`;
}

export function getTimeRangeToFieldName(hourIndex: number, dayIndex: number, timesIndex: number): string {
  const times = getWeekdayFieldName(hourIndex, dayIndex, cDaily.times);
  return `${times}.${timesIndex}.${cTimeRange.to}`;
}

export function getSpecialHourLvFieldName(hourIndex: number, lang: Language, fieldName: string): string {
  return `${cC.specialOpeningHours}.${hourIndex}.languageVersions.${lang}.${fieldName}`;
}

export function getExceptionalHourLvFieldName(hourIndex: number, lang: Language, fieldName: string): string {
  return `${cC.exceptionalOpeningHours}.${hourIndex}.languageVersions.${lang}.${fieldName}`;
}

export function getExceptionalHourFieldName(hourIndex: number, fieldName: string): string {
  return `${cC.exceptionalOpeningHours}.${hourIndex}.${fieldName}`;
}

export function toFieldId(str: string): string {
  return `${FieldIdPrefix}.${str}`;
}
