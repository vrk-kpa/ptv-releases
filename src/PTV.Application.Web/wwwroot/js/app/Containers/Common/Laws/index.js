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
import React from 'react'
import PropTypes from 'prop-types'
import { injectIntl } from 'react-intl'
import ImmutablePropTypes from 'react-immutable-proptypes'
import shortId from 'shortid'
// components
import Law from './LawContainer'
import { PTVAddItem } from '../../../Components'
// messages
import { connectGeneralDescriptionMessages } from '../../Services/Service/Messages'

export const Laws = ({
  readOnly,
  language,
  translationMode,
  items,
  intl,
  messages,
  withName,
  onAddLaw,
  onRemoveLaw,
  shouldValidate,
  collapsible,
  generalDescriptionName,
  isHidden = false
}) => {
  const sharedProps = { readOnly, language, translationMode }
  const renderLaw = (id, index, isNew) => {
    return (
      <Law {...sharedProps}
        key={id}
        isNew={isNew}
        id={id}
        withName={withName}
        count={items.size}
        onAddLaw={onAddLaw}
        onRemoveLaw={onRemoveLaw}
        messages={messages}
        shouldValidate={shouldValidate}
      />
    )
  }

  const onAddButtonClick = (object, count) => {
    onAddLaw(items.size === 0
      ? [
          { id: shortId.generate(), orderNumber: 1 },
          { id: shortId.generate(), orderNumber: 2 }
        ]
      : [
          { id: shortId.generate(), orderNumber: ++count }
        ]
    )
  }
  const labelPostfix = generalDescriptionName
    ? intl.formatMessage(connectGeneralDescriptionMessages.optionConnectedDescriptionTitle) +
      ': ' +
      generalDescriptionName
    : ''

  const body = (
    <PTVAddItem
      items={items}
      readOnly={readOnly && translationMode === 'none'}
      renderItemContent={renderLaw}
      messages={{"label": intl.formatMessage(messages.title) +" "+labelPostfix}}
      onAddButtonClick={onAddButtonClick}
      onRemoveItemClick={onRemoveLaw}
      collapsible={collapsible && translationMode === 'none'}
      multiple={translationMode === 'none'}
    />
  )
  return !isHidden && body
}

Laws.propTypes = {
  items: ImmutablePropTypes.list.isRequired,
  withName: PropTypes.bool,
  readOnly: PropTypes.bool,
  shouldValidate: PropTypes.bool,
  onAddLaw: PropTypes.func.isRequired,
  onRemoveLaw: PropTypes.func.isRequired,
  isHidden: PropTypes.bool
}

export default injectIntl(Laws)
