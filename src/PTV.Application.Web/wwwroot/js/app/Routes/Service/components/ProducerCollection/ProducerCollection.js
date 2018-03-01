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
import {
  Producer,
  ProducerDisplay
} from 'util/redux-form/sections'
import { compose } from 'redux'
import {
  asCollection,
  asContainer,
  asGroup,
  withFormStates
} from 'util/redux-form/HOC'
import withErrorDisplay from 'util/redux-form/HOC/withErrorDisplay'
import { defineMessages, injectIntl, FormattedMessage } from 'react-intl'
import { Label } from 'sema-ui-components'
import styles from './styles.scss'
import cx from 'classnames'
import CommonMessages from 'util/redux-form/messages'

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
    tooltip: <FormattedMessage {...messages.serviceProducersTooltip} />
  }),
  asCollection({
    name: 'serviceProducer',
    simple: true,
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />
  })
)(props => <Producer {...props} />)

export const ProducerCollectionDisplay = compose(
  // asGroup({}),
  withFormStates,
  injectIntl,
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
    <ProducerCollectionDisplayCollection {...rest} />
  </div>
})

export const ProducerCollectionDisplayCollection = compose(
  asCollection({
    name: 'serviceProducer',
    shouldHideControls: true
  }),
)(props =>
  (<div>
    <ProducerDisplay {...props} />
  </div>))
