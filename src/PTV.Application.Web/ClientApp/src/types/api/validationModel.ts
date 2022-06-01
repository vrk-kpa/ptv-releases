import { LanguageVersionType } from 'types/languageVersionTypes';

type ValidationPathModel = {
  name: string;
  id: string;
};

export type ValidationMessageModel = {
  key: string;
  fieldName: string;
  errorType: string;
  validationPaths: ValidationPathModel[];
};

export type ValidationModel<T> = {
  entity: T;
  validatedFields: LanguageVersionType<ValidationMessageModel[]>;
};
