/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import { compose } from 'redux'
import HeaderFormatter from 'appComponents/HeaderFormatter'
import CellHeaders from 'appComponents/Cells/CellHeaders'
import PublishCheckbox from './PublishCheckbox'
import { injectIntl, defineMessages } from 'util/react-intl'
import { Button } from 'sema-ui-components'
import Language from 'appComponents/LanguageBar/Language'
import { connect } from 'react-redux'
import { getName, getStatus } from './selectors'
import styles from './styles.scss'
import { ErrorsForUnapproved } from './Errors'

const Name = compose(
  connect((state, ownProps) => ({
    name: getName(state, ownProps)
  }))
)(({ name }) => <div className={styles.textOverflow}>{name}</div>)

const LanguageIcon = compose(
  connect((state, ownProps) => ({
    statusId: getStatus(state, ownProps)
  }))
)(Language)

export const getApprovedColumnDefinition = () => {
  return [
    {
      property: 'action',
      header: {
        formatters: [props => <HeaderFormatter label={CellHeaders.publishableTitle} />]
      },
      cell: {
        formatters: [
          (name, { rowData, rowIndex }) => {
            return (
              <PublishCheckbox
                {...rowData}
                rowIndex={rowData.index}
              />
            )
          }
        ]
      }
    }, {
      property: 'name',
      header: {
        formatters: [props => <HeaderFormatter label={CellHeaders.name} />]
      },
      cell: {
        formatters: [(name, { rowData }) => {
          return <div className={styles.contentColumn}>
            <LanguageIcon
              componentClass={styles.languageIcon}
              id={rowData.id}
              type={rowData.meta.type}
              languageId={rowData.languageId}
            />
            <Name id={rowData.id} type={rowData.meta.type} language={rowData.language} />
          </div>
        }]
      }
    }
  ]
}

const messages = defineMessages({
  goBackToReview: {
    id: 'AppComponents.MassPublishTable.GoBackToReview.Title',
    defaultMessage: 'Siirry hyväksymään'
  }
})

const LabelFormatter = compose(
  injectIntl
)(({ intl: { formatMessage }, label }) => <span>{formatMessage(label)}</span>)

const goBackToReview = (step, props) => () => {
  props.moveToStep(step, props)
  window.scrollTo(0, 0)
  props.closeDialog()
}

export const getNotApprovedColumnDefinition = (props) => {
  return [
    {
      property: 'name',
      header: {
        formatters: [props => <HeaderFormatter label={CellHeaders.name} />]
      },
      cell: {
        formatters: [(name, { rowData }) => {
          return <div className={styles.contentColumn}>
            <LanguageIcon
              componentClass={styles.languageIcon}
              id={rowData.id}
              type={rowData.meta.type}
              languageId={rowData.languageId}
            />
            <Name id={rowData.id} type={rowData.meta.type} language={rowData.language} />
          </div>
        }]
      }
    }, {
      property: 'action',
      cell: {
        formatters: [
          (name, { rowData, rowIndex }) => {
            return (
              <span>
                <Button
                  link
                  onClick={goBackToReview(rowData.step, props)}
                  children={<LabelFormatter label={messages.goBackToReview} />}
                />
                {rowData.shouldPublish && <ErrorsForUnapproved {...rowData} /> || null}
              </span>
            )
          }
        ]
      }
    }
  ]
}
