import { Language, ValidationErrorType } from 'types/enumTypes';
import { IServiceContext } from 'features/service/ServiceContextProvider';

export const getMissingRequiredInfo = (ctx: IServiceContext, language: Language): string[] => {
  const result = ctx.validationResult?.validatedFields[language]
    ?.filter((item) => (item.errorType as ValidationErrorType) === 'mandatoryField')
    .map((item) => item.fieldName)
    .reduce((unique, item) => (unique.includes(item) ? unique : [...unique, item]), [] as string[]);
  return result ?? [];
};

export const getMissingOrganizationLanguages = (ctx: IServiceContext): Language[] => {
  if (!ctx.validationResult?.validatedFields) {
    return [];
  }

  return (Object.keys(ctx.validationResult.validatedFields) as Language[]).filter((key) =>
    ctx.validationResult?.validatedFields[key]?.some((item) => {
      const errorType = item.errorType as ValidationErrorType;
      return errorType === 'publishedMandatoryField' || errorType === 'publishedOrganizationLanguageMandatoryField';
    })
  );
};

export const getOtherLanguagesWithErrors = (ctx: IServiceContext, language: Language): Language[] => {
  if (!ctx.validationResult?.validatedFields) {
    return [];
  }

  return (Object.keys(ctx.validationResult.validatedFields) as Language[]).filter(
    (key) => key !== language && !!ctx.validationResult?.validatedFields[key]
  );
};

export const getLanguagesWithErrors = (ctx: IServiceContext, language: Language): Language[] => {
  if (!ctx.validationResult?.validatedFields) {
    return [];
  }

  return (Object.keys(ctx.validationResult.validatedFields) as Language[]).filter((key) => !!ctx.validationResult?.validatedFields[key]);
};
