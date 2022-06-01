import { Control } from 'react-hook-form';
import { Language } from 'types/enumTypes';
import { ServiceModel } from 'types/forms/serviceFormTypes';

export type AnnotationQueryProps = {
  control: Control<ServiceModel>;
  language: Language;
};
