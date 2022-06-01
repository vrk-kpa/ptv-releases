import React, { useEffect, useState } from 'react';
import { Control } from 'react-hook-form';
import { useTranslation } from 'react-i18next';
import { Tab, Tabs } from '@mui/material';
import Box from '@mui/material/Box';
import { styled } from '@mui/material/styles';
import { Button, InlineAlert, Modal, ModalContent, ModalFooter, ModalTitle, Paragraph } from 'suomifi-ui-components';
import { OntologyTerm } from 'types/classificationItemsTypes';
import { ServiceModel } from 'types/forms/serviceFormTypes';
import { useGetUiLanguage } from 'hooks/useGetUiLanguage';
import { EditorTab, EditorViewMode } from 'features/service/components/KeywordsAndOntologyTerms/types';
import {
  addOrRemoveKeyword,
  addOrRemoveOntologyTerm,
  hasOntologyTermLimitBeenReached,
  hasTooManyTermsBeenSelected,
  isKeywordIncluded,
  isOntologyTermDisabled,
  sortKeywords,
  sortOntologyTerms,
} from 'features/service/components/KeywordsAndOntologyTerms/utils';
import { KeywordTab } from './Tabs/KeywordTab';
import { OntologyTab } from './Tabs/OntologyTab';
import { Summary } from './Tabs/Summary';
import { ViewModeSwitcher } from './ViewModeSwitcher';

type KeywordsAndOntologyTermsModalProps = {
  control: Control<ServiceModel>;
  isOpen: boolean;
  serviceOntologyTerms: OntologyTerm[];
  gdOntologyTerms: OntologyTerm[];
  serviceKeywords: string[];
  gdKeywords: string[];
  closeDiscardChanges: () => void;
  closeSaveChanges: (chosenOntologyTerms: OntologyTerm[], chosenKeywords: string[]) => void;
};

const Action = styled('div')({
  display: 'inline-block',
  marginRight: '15px',
});

const AreaContainer = styled('div')({
  minHeight: '600px',
});

export function Editor(props: KeywordsAndOntologyTermsModalProps): React.ReactElement {
  const uiLang = useGetUiLanguage();
  const { t } = useTranslation();
  const [selectedTab, setSelectedTab] = useState<EditorTab>('OntologyTerms');
  const [viewMode, setViewMode] = useState<EditorViewMode>('Edit');
  const [selectedOntologyTerms, setSelectedOntologyTerms] = useState<OntologyTerm[]>([]);
  const [selectedKeywords, setSelectedKeywords] = useState<string[]>([]);

  useEffect(() => {
    setSelectedOntologyTerms(props.serviceOntologyTerms);
  }, [props.serviceOntologyTerms]);

  useEffect(() => {
    setSelectedKeywords(props.serviceKeywords);
  }, [props.serviceKeywords]);

  function toggleOntologyTerm(ontologyTerm: OntologyTerm) {
    setSelectedOntologyTerms(sortOntologyTerms(uiLang, addOrRemoveOntologyTerm(ontologyTerm, selectedOntologyTerms)));
  }

  function toggleKeyword(keyword: string) {
    setSelectedKeywords(sortKeywords(uiLang, addOrRemoveKeyword(keyword, selectedKeywords)));
  }

  function isKeywordSelected(keyword: string): boolean {
    return isKeywordIncluded(keyword, selectedKeywords);
  }

  function isOntologyTermChecked(ontologyTerm: OntologyTerm): boolean {
    if (selectedOntologyTerms.some((x) => x.id === ontologyTerm.id)) return true;
    if (props.gdOntologyTerms.some((x) => x.id === ontologyTerm.id)) return true;
    return false;
  }

  function isTermDisabled(ontologyTerm: OntologyTerm): boolean {
    return isOntologyTermDisabled(ontologyTerm, props.gdOntologyTerms, selectedOntologyTerms);
  }

  function onTabChange(event: React.SyntheticEvent, value: EditorTab) {
    setSelectedTab(value);
  }

  function switchViewMode() {
    setViewMode(viewMode === 'Edit' ? 'Summary' : 'Edit');
  }

  function onCloseAndSaveChanges() {
    props.closeSaveChanges(selectedOntologyTerms, selectedKeywords);
  }

  const count = selectedOntologyTerms.length + selectedKeywords.length + props.gdOntologyTerms.length + props.gdKeywords.length;
  const showLimitNotification = hasOntologyTermLimitBeenReached(props.gdOntologyTerms, selectedOntologyTerms);
  const tooManyTermsSelected = hasTooManyTermsBeenSelected(props.gdOntologyTerms, selectedOntologyTerms);

  return (
    <Modal scrollable={true} visible={props.isOpen} appElementId={'root'}>
      <ModalContent>
        <ModalTitle>{t(`Ptv.Service.Form.Field.OntologyTerms.Select.Label`)}</ModalTitle>
        <AreaContainer>
          <Box hidden={viewMode !== 'Edit'}>
            <Paragraph>{t('Ptv.Service.Form.Field.OntologyTerms.Select.Description')}</Paragraph>
            <Tabs value={viewMode === 'Edit' ? selectedTab : false} onChange={onTabChange}>
              <Tab value='OntologyTerms' key='OntologyTerms' label={t('Ptv.Service.Form.Field.OntologyTerms.Tabs.OntologyTerms.Label')} />
              <Tab value='Keywords' key='Keywords' label={t('Ptv.Service.Form.Field.OntologyTerms.Tabs.Keywords.Label')} />
            </Tabs>
            <OntologyTab
              isVisible={selectedTab === 'OntologyTerms'}
              toggleOntologyTerm={toggleOntologyTerm}
              isOntologyTermChecked={isOntologyTermChecked}
              isOntologyTermDisabled={isTermDisabled}
            />
            <KeywordTab isVisible={selectedTab === 'Keywords'} toggleKeyword={toggleKeyword} isKeywordSelected={isKeywordSelected} />
          </Box>
          <Box hidden={viewMode !== 'Summary'}>
            <Summary
              ontologyTerms={selectedOntologyTerms}
              gdOntologyTerms={props.gdOntologyTerms}
              keywords={selectedKeywords}
              gdKeywords={props.gdKeywords}
              toggleOntologyTerm={toggleOntologyTerm}
              toggleKeyword={toggleKeyword}
            />
          </Box>
        </AreaContainer>
      </ModalContent>
      <ModalFooter>
        <ViewModeSwitcher viewMode={viewMode} selectedCount={count} onClick={switchViewMode} />
        {showLimitNotification && (
          <InlineAlert status='neutral'>{t(`Ptv.Service.Form.Field.OntologyTerms.Message.LimitReached`)}</InlineAlert>
        )}
        <Box>
          <Action>
            <Button key='confirm' disabled={tooManyTermsSelected} onClick={onCloseAndSaveChanges}>
              {t('Ptv.Action.ConfirmSelection.Label')}
            </Button>
          </Action>
          <Button variant='secondary' onClick={props.closeDiscardChanges}>
            {t('Ptv.Action.Cancel.Label')}
          </Button>
        </Box>
      </ModalFooter>
    </Modal>
  );
}
