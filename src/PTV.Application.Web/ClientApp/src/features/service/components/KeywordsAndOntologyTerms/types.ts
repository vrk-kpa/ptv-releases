import { Control } from 'react-hook-form';
import { OntologyTerm } from 'types/classificationItemsTypes';
import { ServiceModel } from 'types/forms/serviceFormTypes';

export type EditorViewMode = 'Edit' | 'Summary';
export type EditorTab = 'OntologyTerms' | 'Keywords';
export type KeywordStatus = 'OntologyTermsFound' | 'KeywordCreated' | 'KeywordAlreadyAdded';

export type KeywordsAndOntologyTermsProps = {
  namespace: string;
  control: Control<ServiceModel>;
  gdKeywords: string[];
  gdOntologyTerms: OntologyTerm[];
};
