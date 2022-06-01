import React from 'react';
import { useTranslation } from 'react-i18next';
import { BaseIconKeys, Button } from 'suomifi-ui-components';
import { EditorViewMode } from 'features/service/components/KeywordsAndOntologyTerms/types';

type ViewModeSwitcherProps = {
  viewMode: EditorViewMode;
  selectedCount: number;
  onClick: () => void;
};

export function ViewModeSwitcher(props: ViewModeSwitcherProps): React.ReactElement {
  const { t } = useTranslation();

  const title =
    props.viewMode === 'Edit'
      ? t(`Ptv.Service.Form.Field.OntologyTerms.Select.GoToSummary.Label`, { count: props.selectedCount })
      : t('Ptv.Service.Form.Field.OntologyTerms.Summary.GoToSelect.Label');
  const icon: BaseIconKeys = props.viewMode === 'Edit' ? 'arrowRight' : 'arrowLeft';

  return (
    <Button icon={icon} variant='secondaryNoBorder' onClick={props.onClick}>
      {title}
    </Button>
  );
}
