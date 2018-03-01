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
import { compose } from 'redux'
import { injectIntl, intlShape, defineMessages } from 'react-intl'
import {
  ArrowDown,
  ArrowUp,
  ComposedCell,
  ValueCell,
  EmailCell,
  DateTimeCell
} from 'appComponents'
import {
  OrderStateCell
} from 'appComponents/TranslationOrderDialog/components'
import RenderAdditionalInformationDetail from './RenderAdditionalInformationDetail'
import { Button } from 'sema-ui-components'
import styles from './styles.scss'
import cx from 'classnames'
import moment from 'moment'

const messages = defineMessages({
  additionalInformationButton: {
    id: 'Containers.TranslationOrderDialog.Row.AdditionalInformation.Button',
    defaultMessage: 'Lisätiedot'
  },
  estimationText: {
    id: 'Containers.TranslationOrderDialog.Row.EstimationText',
    defaultMessage: 'Arvioitu valmistuminen'
  }
})

const RenderOrderTableRow = ({ intl: { formatMessage }, inputData, isOpen, onOpen, rowId }) => {
  const tableBodyRowWrapClass = cx(
    styles.tableBodyRowWrap,
    {
      [styles.activeRow]: isOpen
    }
  )
  const detailButtonClass = cx(
    styles.toggleButton,
    {
      [styles.expanded]: isOpen,
      [styles.collapsed]: !isOpen
    }
  )

  const translationOrder = inputData.has('translationOrder') && inputData.get('translationOrder')
  const deliverAt = translationOrder && inputData.get('deliverAt')
  return (
    <div className={tableBodyRowWrapClass}>
      <div className={styles.tableBodyRow}>
        <DateTimeCell
          flexBasis={'b120'}
          value={inputData.get('sentAt')} />
        <ComposedCell flexBasis={'b160'} flexGrow
          main={<OrderStateCell id={inputData.get('translationStateTypeId')} />}
          sub={deliverAt && <span>
            {formatMessage(messages.estimationText)} {moment(deliverAt).format('DD.MM.YYYY')}
          </span>} />
        <ComposedCell flexBasis={'b120'}>
          <ValueCell value={translationOrder && translationOrder.get('orderIdentifier')} />
        </ComposedCell>
        <ComposedCell flexBasis={'b80'}>
          <ValueCell componentClass={styles.languages} value={translationOrder.get('sourceLanguageCode') + ' > ' +
            translationOrder.get('targetLanguageCode')} />
        </ComposedCell>
        <ComposedCell
          flexBasis={'b180'}
          main={<ValueCell value={translationOrder && translationOrder.get('senderName')} />}
          sub={<EmailCell email={translationOrder && translationOrder.get('senderEmail')} />}
        />
        <ComposedCell
          flexBasis={'b180'}
          main={<ValueCell value={translationOrder && translationOrder.get('translationCompanyName')} />}
          sub={<EmailCell email={translationOrder && translationOrder.get('translationCompanyEmail')} />}
        />
        <ComposedCell flexBasis={'b160'}>
          <div className={detailButtonClass}>
            <Button
              small
              secondary={!isOpen}
              onClick={onOpen}
            >
              <div>{formatMessage(messages.additionalInformationButton)}</div>
              {isOpen ? <ArrowUp /> : <ArrowDown />}
            </Button>
          </div>
        </ComposedCell>
      </div>
      {isOpen &&
        <RenderAdditionalInformationDetail
          inputData={translationOrder}
          stateTypeId={inputData.get('lastOrder') && inputData.get('translationStateTypeId')}
        />
      }
    </div>
  )
}

RenderOrderTableRow.propTypes = {
  intl: intlShape,
  onOpen: PropTypes.func,
  isOpen: PropTypes.bool,
  rowId: PropTypes.string,
  inputData: PropTypes.object
}

export default compose(
  injectIntl
)(RenderOrderTableRow)
