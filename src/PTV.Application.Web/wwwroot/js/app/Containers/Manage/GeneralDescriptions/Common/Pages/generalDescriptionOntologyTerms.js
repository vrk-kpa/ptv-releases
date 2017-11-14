/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
import React, { PropTypes, Component } from 'react'
import { connect } from 'react-redux'
import { injectIntl } from 'react-intl'

// components
import { OntologyTermTree, getDefaultTranslationText } from '../../../../Common/FintoTree'
import { PTVButton, PTVPreloader, PTVLabel } from '../../../../../Components'
import PTVTreeList from '../../../../../Components/PTVTreeViewComponent/PTVTreeList'
import PTVTreeView from '../../../../../Components/PTVTreeViewComponent/PTVTreeView'
import NotificationContainer from '../../../../Common/NotificationContainer'

// actions
import * as generealDescriptionActions from '../../GeneralDescriptions/Actions'
import { resetMessages } from '../../../../Common/Actions'
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'

// selectors
import * as GeneralDescriptionSelectors from '../../GeneralDescriptions/Selectors'
import * as CommonServiceSelectors from '../../../../Services/Common/Selectors'
import * as CommonSelectors from '../../../../Common/Selectors'
// Validators
import * as PTVValidatorTypes from '../../../../../Components/PTVValidators'

const GeneralDescriptionOntologyTerms = ({
    messages,
    readOnly,
    language,
    translationMode,
    actions,
    intl,
    keyToState,
    annotationIsFetching,
    entityId,
    tooltipData, 
    tooltipIsFetching, 
    selectedCount }) => {
  const onListChange = (input) => (value, isAdd) => {
    actions.onListChange(input, value, isAdd)
  }
  const onListRemove = (input) => (value) => {
    actions.onListChange(input, value, false)
  }
  const validatorsOntologyTerms = [{ ...PTVValidatorTypes.IS_REQUIRED, errorMessage: messages.errorMessageIsRequired }]
  
  const onLinkClick = () => {
    actions.resetMessages([keyToState, 'annotation'])
    actions.getAnnotations(language)
  }

  const renderAnnotationButton = () => {
    let env = getEnvironmentType()
    const alertStyle = {
      borderRadius: '2px',
      fontSize: '15px',
      letterSpacing: '0px',
      lineHeight: '20px',
      padding: '10px',
      color: '#C13832',
      background: '#F1F1F1',
      marginBottom: '10px'
    }
    return (
            env === '_prod'
                ? <div>
                  <div style={alertStyle}>
                    {intl.formatMessage(messages.annotationToolAlert)}
                  </div>
                  <PTVButton onClick={() => { }} disabled>
                    {intl.formatMessage(messages.annotationButton)}
                  </PTVButton>
                </div>
                : <div>
                  <NotificationContainer
                    keyToState={keyToState}
                    notificationKey='annotation'
                    />
                  {annotationIsFetching
                        ? <PTVPreloader small />
                        : <PTVButton onClick={onLinkClick}>
                          {intl.formatMessage(messages.annotationButton)}
                        </PTVButton>}
                </div>
    )
  }

  const renderAnnotationLabel = () => {
    return (<div>
      <PTVLabel>{intl.formatMessage(messages.annotationTitle)}</PTVLabel>
    </div>)
  }

  const onTagTooltipOpen = (id) => {
    actions.getAnnotationHierarchy(id, entityId)
  }

  const renderToolTipNode = (node, isRootLevel) => {
    return (
      <div>
        <PTVLabel labelClass={node.get('isLeaf') ? 'strong' : ''}>
          {node.get('name')}
        </PTVLabel>
      </div>
    )
  }

  const renderTooltipContent = () => {
    return (
      <div className='tooltip-tree'>
        {intl.formatMessage(messages.annotationTagTooltip)}
        {tooltipIsFetching
                    ? <PTVPreloader small />
                    : tooltipData
                        ? <PTVTreeView
                          tree={tooltipData}
                          getNode={CommonServiceSelectors.getAnnotationOntologyTerm}
                          getNodeIsCollapsed={() => { return false }}
                          getNodeIsFetching={() => { return false }}
                          renderNode={renderToolTipNode}
                        /> : null}
      </div>
    )
  }

  return (
    <div className='form-group'>
      <OntologyTermTree
        treeViewInnerClass='bordered inner clearfix'
        treeViewClass='col-md-6 ontology-term-tree'
        resultsClass='col-md-6'
        labelTooltip={intl.formatMessage(messages.tooltip)}
        label={intl.formatMessage(messages.title)}
        onNodeSelect={onListChange('ontologyTerms')}
        onNodeRemove={onListRemove('ontologyTerms')}
        treeTargetHeader={intl.formatMessage(messages.targetListHeader, { count: selectedCount })}
        order={70}
        readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}
        isSelectedSelector={GeneralDescriptionSelectors.getIsSelectedOntologyTerms}
        getSelectedSelector={GeneralDescriptionSelectors.getSelectedWithoutAnnotationOntologyTerms}
        getValidationSelector={GeneralDescriptionSelectors.getSelectedWithAnnotationOntologyTerms}
        language={language}
        keyToState={keyToState}
        renderTreeLabel={renderAnnotationLabel}
        renderTreeButton={renderAnnotationButton}
        treeLinkLabel={intl.formatMessage(messages.treeLink)}
        minHeight
        tagType='vertical'
        tagColor='user-modified'
        validatedField={messages.title}
        validators={validatorsOntologyTerms}
            >
        <div>
          <PTVTreeList
            getDefaultTranslationText={getDefaultTranslationText(intl)}
            onNodeRemove={onListRemove('annotationOntologyTerms')}
            listSelector={GeneralDescriptionSelectors.getAnnotationOntologyTerms}
            readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}
            getItemSelector={CommonServiceSelectors.getOntologyTerm}
            language={language}
            tagPostfix={intl.formatMessage(messages.annotationTagPostfix)}
            tagType='vertical'
            tagTooltipContent={renderTooltipContent}
            tagTooltipCallback={onTagTooltipOpen} />
        </div>
      </OntologyTermTree>
    </div>
  )
}

function mapStateToProps (state, ownProps) {
  const entityId = CommonSelectors.getPageEntityId(state, ownProps)
  const tagProps = {
    keyToState: ['AnnotationOntologyTerm', entityId],
    language: ownProps.language
  }

  const tooltipData = CommonSelectors.getSearchIds(state, tagProps)
  const allOntologies = GeneralDescriptionSelectors.getSelectedWithAnnotationOntologyTerms(state, tagProps)

  return {
    annotationIsFetching: CommonSelectors.getAnnotationIsFetching(state, ownProps),
    tooltipIsFetching: CommonSelectors.getSearchIsFetching(state, tagProps),
    selectedCount: allOntologies.size + '',
    entityId,
    tooltipData
  }
}

const actions = [
  generealDescriptionActions,
  { resetMessages }
]

export default injectIntl(connect(mapStateToProps, mapDispatchToProps(actions))(GeneralDescriptionOntologyTerms))

