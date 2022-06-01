import { OntologyTermsResponse } from 'types/annotationToolTypes';
import { ClassificationItem, OntologyTerm, OntologyTermTreeType } from 'types/classificationItemsTypes';
import { Language } from 'types/enumTypes';
import { MaxOntologyTerms, getOntologyTermCount } from 'features/service/validation/ontologyTerms';

export function getFirstTerm(termTree: OntologyTermTreeType[] | undefined): OntologyTermTreeType | null {
  if (!termTree) return null;
  if (termTree.length === 0) return null;
  return termTree[0];
}

export function getSearchResultCount(term: OntologyTermTreeType | null): number {
  if (!term) return 0;

  // +1 because the number of shown search results to the user is
  // the actual term (+1) + children + parents
  return term.children.length + term.parents.length + 1;
}

export function toSortedOntologyTerms(lang: Language, items: ClassificationItem[] | OntologyTermsResponse[]): OntologyTerm[] {
  return sortOntologyTerms(
    lang,
    items.map((item) => toOntologyTerm(item))
  );
}

export function sortOntologyTerms(lang: Language, items: OntologyTerm[]): OntologyTerm[] {
  return items.sort((a, b): number => {
    const nameA = a.names[lang] || '';
    const nameB = b.names[lang] || '';
    return nameA.localeCompare(nameB, lang);
  });
}

export function toOntologyTerm(source: ClassificationItem | OntologyTermsResponse): OntologyTerm {
  return {
    id: source.id,
    names: source.names,
    code: source.code,
  };
}

export function addOrRemoveOntologyTerm(ontologyTerm: OntologyTerm, terms: OntologyTerm[]): OntologyTerm[] {
  if (terms.some((x) => x.id === ontologyTerm.id)) {
    return terms.filter((x) => x.id !== ontologyTerm.id);
  }

  return [...terms, ontologyTerm];
}

export function sortKeywords(lang: Language, items: string[]): string[] {
  return items.sort((a, b): number => {
    return a.localeCompare(b, lang);
  });
}

export function isKeywordIncluded(keyword: string, keywords: string[]): boolean {
  const str = toComparableKeyword(keyword);
  return keywords.some((x) => toComparableKeyword(x) === str);
}

export function addOrRemoveKeyword(keyword: string, keywords: string[]): string[] {
  const str = toComparableKeyword(keyword);

  if (isKeywordIncluded(keyword, keywords)) {
    return keywords.filter((x) => toComparableKeyword(x) !== str);
  }

  return [...keywords, keyword.trim()];
}

function toComparableKeyword(keyword: string): string {
  // User cannot add keywords that only differ in number of spaces/capitalization
  // In order to see if keyword has already been added, remove all extra
  // white space and trim. If user has entered "foo bar" she cannot write
  // " foo   BAR "
  return keyword.replace(/\s+/g, ' ').trim().toLowerCase();
}

export function hasOntologyTermLimitBeenReached(gdOntologyTerms: OntologyTerm[], serviceOntologyTerms: OntologyTerm[]): boolean {
  const count = getOntologyTermCount(gdOntologyTerms, serviceOntologyTerms);
  return count >= MaxOntologyTerms;
}

export function hasTooManyTermsBeenSelected(gdOntologyTerms: OntologyTerm[], serviceOntologyTerms: OntologyTerm[]): boolean {
  const count = getOntologyTermCount(gdOntologyTerms, serviceOntologyTerms);
  return count > MaxOntologyTerms;
}

export function isOntologyTermDisabled(
  ontologyTerm: OntologyTerm,
  gdOntologyTerms: OntologyTerm[],
  selectedOntologyTerms: OntologyTerm[]
): boolean {
  if (gdOntologyTerms.some((x) => x.id === ontologyTerm.id)) return true;

  // If user has selected max number of terms disable selecting new terms
  // but allow to unselect already selected terms
  if (hasOntologyTermLimitBeenReached(gdOntologyTerms, selectedOntologyTerms)) {
    return !selectedOntologyTerms.some((x) => x.id === ontologyTerm.id);
  }

  return false;
}
