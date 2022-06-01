import { Language } from 'types/enumTypes';
import { TargetGroup } from 'types/targetGroupTypes';
import { localeCompareTexts } from 'utils/translations';

export function getSelectedForGroup(allSelected: string[], groupCode: string): string[] {
  return allSelected.filter((code) => code.startsWith(groupCode));
}

export function getTargetGroups(all: TargetGroup[], groupCode: string, lang: Language, includeParent: boolean): TargetGroup[] {
  if (includeParent) {
    return all.filter(({ code }) => code.startsWith(groupCode)).sort((left, right) => sortTargetGroups(left, right, lang));
  }
  return all
    .filter(({ code, parentId }) => {
      return code.startsWith(groupCode) && parentId != null;
    })
    .sort((left, right) => sortTargetGroups(left, right, lang));
}

// Put parent group first and after that sort alphabetically by locale
function sortTargetGroups(left: TargetGroup, right: TargetGroup, lang: Language): number {
  if (right.parentId == null) {
    return Infinity;
  }
  if (left.parentId == null) {
    return -Infinity;
  }
  return localeCompareTexts(left.names, right.names, lang);
}
