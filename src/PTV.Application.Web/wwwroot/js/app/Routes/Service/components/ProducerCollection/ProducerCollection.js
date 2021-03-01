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
import {
  Producer,
  ProducerDisplay
} from 'util/redux-form/sections'
import { compose } from 'redux'
import { withProps } from 'recompose'
import asGroup from 'util/redux-form/HOC/asGroup'
import asCollection from 'util/redux-form/HOC/asCollection'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import withErrorDisplay from 'util/redux-form/HOC/withErrorDisplay'
import { defineMessages, injectIntl, FormattedMessage } from 'util/react-intl'
import { Label } from 'sema-ui-components'
import styles from './styles.scss'
import cx from 'classnames'
import CommonMessages from 'util/redux-form/messages'
import ProducerTitle from './ProducerTitle'

const messages = defineMessages({
  serviceProducersTooltip:{
    id : 'Containers.Services.AddService.Step3.ServiceProducer.Header.Tooltip',
    defaultMessage : 'Palvelun toteutustapa ja tuottaja'
  },
  titleDisplay: {
    id : 'ReduxForm.Sections.Producer.ProducerPreview.Title',
    defaultMessage: 'ESIKATSELU'
  },
  subTitleDisplay: {
    id : 'ReduxForm.Sections.Producer.ProducerPreview.SubTitle',
    defaultMessage: 'Palvelun tuottaa'
  },
  addBtnTitle: {
    id : 'ReduxForm.Sections.Producer.AddButton.Title',
    defaultMessage: '+ Uusi tuottajatieto'
  }
})

export const ProducerCollection = compose(
  withErrorDisplay('serviceProducers'),
  asGroup({
    title: CommonMessages.serviceProducers,
    tooltip: <FormattedMessage {...messages.serviceProducersTooltip} />,
    required: true
  }),
  withProps(props => ({
    comparable: true
  })),
  asCollection({
    name: 'serviceProducer',
    stacked: true,
    simple: true,
    dragAndDrop: true,
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />,
    addNewBtnTitle: <FormattedMessage {...messages.addBtnTitle} />,
    Title: ProducerTitle
  })
)(Producer)

export const ProducerCollectionDisplay = compose(
  injectIntl,
  // asGroup({}),
  withFormStates,
)(
  ({ intl: { formatMessage }, isCompareMode, isReadOnly, ...rest }) => {
    const previewClass = cx(
      styles.preview,
      {
        [styles.compare]: isCompareMode,
        [styles.isReadOnly]: isReadOnly
      }
    )
    return <div className={previewClass}>
      <div className={styles.previewBox} />
      {!isReadOnly && <div className={styles.title}><Label labelText={formatMessage(messages.titleDisplay)} /></div>}
      <div className={styles.subTitle}><Label labelText={formatMessage(messages.subTitleDisplay)} /></div>
      <ProducerCollectionDisplayCollection isReadOnly {...rest} />
    </div>
  })

export const ProducerCollectionDisplayCollection = compose(
  injectIntl,
  asCollection({
    name: 'serviceProducer',
    shouldHideControls: true
  }),
)(props =>
  (<div>
    <ProducerDisplay {...props} />
  </div>))
