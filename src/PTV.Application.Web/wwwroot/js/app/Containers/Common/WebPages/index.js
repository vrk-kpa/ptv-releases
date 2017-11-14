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
import { injectIntl } from 'react-intl'
import ImmutablePropTypes from 'react-immutable-proptypes'
import shortId from 'shortid'

// components
import WebPage from './WebPageContainer'
import { PTVAddItem } from '../../../Components'

// selectors
import * as CommonSelectors from '../Selectors'

export const WebPages = ({
    readOnly,
    language,
    translationMode,
    items,
    intl,
    messages,
    withName,
    withTypes,
    withOrder,
    withAdditionInfo,
    withChecker,
    onAddWebPage,
    onRemoveWebPage,
    shouldValidate,
    collapsible,
    splitContainer,
    customConfiguration,
    withAddLink,
    includeReset
}) => {
  const sharedProps = { readOnly, language, translationMode, splitContainer }
  const renderWebPage = (id, index, isNew) => {
    return (
      <WebPage {...sharedProps}
        key={id}
        index={index}
        isNew={isNew}
        id={id}
        withName={withName}
        withTypes={withTypes}
        withAdditionInfo={withAdditionInfo}
        customConfiguration={customConfiguration}
        withOrder={withOrder}
        withChecker={withChecker}
        count={items.size}
        onAddWebPage={onAddWebPage}
        onRemovePage={onRemoveWebPage}
        messages={messages}
        shouldValidate={shouldValidate}
            />
    )
  }

  const onAddButtonClick = (object, count) => {
    onAddWebPage(items.size === 0
        ? [
            { id: shortId.generate(), orderNumber: 1 }, { id: shortId.generate(), orderNumber: 2 }
        ]
        : [
            { id: shortId.generate(), orderNumber: ++count }
        ])
  }

  return (
    <PTVAddItem
      items={items}
      readOnly={readOnly && translationMode === 'none'}
      renderItemContent={renderWebPage}
      messages={{ 'label': intl.formatMessage(messages.title) }}
      onAddButtonClick={onAddButtonClick}
      onRemoveItemClick={onRemoveWebPage}
      collapsible={collapsible && translationMode === 'none'}
      multiple={(translationMode === 'none' || translationMode === 'edit') && withAddLink}
      includeReset={includeReset} />
  )
}

WebPages.propTypes = {
  items: ImmutablePropTypes.map.isRequired,
  withName: PropTypes.bool,
  withTypes: PropTypes.bool,
  withOrder: PropTypes.bool,
  withAdditionInfo: PropTypes.bool,
  withChecker: PropTypes.bool,
  withAddLink: PropTypes.bool,
  readOnly: PropTypes.bool,
  shouldValidate: PropTypes.bool,
  includeReset: PropTypes.bool,
  onAddWebPage: PropTypes.func.isRequired,
  onRemoveWebPage: PropTypes.func.isRequired
}

WebPages.defaultProps = {
  withAddLink: true
}

export default injectIntl(WebPages)
