import { UseFormSetValue } from 'react-hook-form';
import { Modifier, RawDraftContentState, SelectionState, convertToRaw } from 'draft-js';
import { escapeRegExp, isEmpty } from 'lodash';
import { GuidedCorrectionItem, QualityResult } from 'types/qualityAgentResponses';
import { getContentState } from 'utils/draftjs';
import { matchingTokenRegExp } from 'utils/qualityAssurance';
import { setFormValue } from 'utils/rhf';

const issuesForGuidedCorrection = (qualityResult: QualityResult[]): GuidedCorrectionItem[] => {
  const items: GuidedCorrectionItem[] = [];
  qualityResult.forEach((result) => {
    if (result.result.info?.matches != null) {
      result.result.info?.matches.forEach((match) => {
        if (match && match.suggestions && match.suggestions.length > 0) {
          items.push({
            rule: result.rule,
            ruleId: result.ruleId,
            issueData: result.processed.Data,
            issueExplanataion: result.explanation,
            matchTitle: match.title,
            matchToken: match.token,
            matchExplanation: match.explanation,
            matchSuggestions: match.suggestions,
            matchStart: match.start,
            matchLength: match.length,
            fieldId: result.fieldId,
            matchSentence: findSentence(result.processed.Data, match.start, match.start + match.length),
          });
        }
      });
    }
  });
  return items;
};

const findSentence = (data: string, start: number, end: number): string => {
  const word = escapeRegExp(data.substring(start, end).trim());
  const trimmedData = data.replace(/(\r\n|\n|\r)/gm, '');
  const regExp = new RegExp(`([^.]+?${word}[^.]+.)`, 'g');
  const matches = trimmedData.match(regExp);
  matches?.forEach((match) => {
    const searchRes = trimmedData.search(escapeRegExp(match.trim()));
    if (start >= searchRes && end <= searchRes + match.length) {
      return match;
    }
  });
  return matches != null && matches.length > 0 ? matches[0].trim() : data;
};

const handleRhfTextFieldChange = (
  value: string,
  item: GuidedCorrectionItem,
  currentValue: string,
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  setValue: UseFormSetValue<any>,
  fieldName: string
): void => {
  currentValue = `${currentValue.substring(0, item.matchStart)}${value}${currentValue.substring(
    item.matchStart + item.matchLength,
    currentValue.length
  )}`;
  setFormValue(setValue, fieldName, currentValue, { shouldValidate: true, shouldDirty: true });
};

const handleRhfRichTextFieldChange = (
  value: string,
  item: GuidedCorrectionItem,
  currentValue: string | null | undefined,
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  setValue: UseFormSetValue<any>,
  fieldName: string
): void => {
  if (currentValue == null || isEmpty(currentValue)) return;
  const currentContentState = getContentState(JSON.parse(currentValue) as RawDraftContentState);

  const blockMap = currentContentState.getBlockMap();

  const blockToReplace = blockMap.find((contentBlock) => contentBlock?.getText().includes(item.matchToken, item.matchStart) || false);

  if (blockToReplace) {
    const blockKey = blockToReplace.getKey();
    // Match the matchToken only as a separate word.
    // Separate words have either unicode white space \p{Z} or \p{P} punctuation between them,
    // or they are at the beginning or the of the string.
    const exactMatchToken = matchingTokenRegExp(item.matchToken);
    const contentBlockText = blockToReplace.getText();
    const matchStartIndex = contentBlockText.search(exactMatchToken);

    if (matchStartIndex >= 0) {
      const startsWithTheWord = contentBlockText.search(`^${escapeRegExp(item.matchToken)}`) === 0;
      const wordStartIndex = startsWithTheWord ? matchStartIndex : matchStartIndex + 1;
      const replacedBlock = SelectionState.createEmpty(blockKey).merge({
        anchorOffset: wordStartIndex,
        focusOffset: wordStartIndex + item.matchToken.length,
      });
      const content = Modifier.replaceText(currentContentState, replacedBlock, value);
      setFormValue(setValue, fieldName, JSON.stringify(convertToRaw(content)), {
        shouldDirty: true,
        shouldValidate: true,
      });
    }
  }
};

export { issuesForGuidedCorrection, handleRhfTextFieldChange, handleRhfRichTextFieldChange };
