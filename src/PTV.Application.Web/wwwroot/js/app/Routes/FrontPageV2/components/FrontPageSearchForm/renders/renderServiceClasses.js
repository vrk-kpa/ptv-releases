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
import React, { PropTypes } from 'react'
import { Select } from 'sema-ui-components'
import { connect } from 'react-redux'
import { getServiceClassesJS } from 'Routes/FrontPageV2/selectors'
import { compose } from 'redux'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import { localizeList } from 'appComponents/Localize'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import InfoBox from 'appComponents/InfoBox'

const messages = defineMessages({
  serviceClassComboTitle: {
    id: 'FrontPage.ServiceClassCombo.Title',
    defaultMessage: 'Palveluluokka'
  },
  serviceClassComboTooltip: {
    id: 'FrontPage.ServiceClassCombo.Tooltip',
    defaultMessage: 'Voit hakea palveluita myös palveluluokan mukaan. Palveluluokka on on aihetunniste, jonka avulla palvelut voidaan ryhmitellä ja löytää. Palvelu voi kuulua useaan eri luokkaan. Valitse pudotusvalikosta haluamasi palveluluokka.'
  }
})

const renderServiceClasses = ({
  input,
  serviceClasses,
  intl: { formatMessage },
  inlineLabel,
  ...rest
}) => (
  <div>
    {!inlineLabel &&
      <div className='form-field tooltip-label'>
        <strong>
          {formatMessage(messages.serviceClassComboTitle)}
          <InfoBox tip={formatMessage(messages.serviceClassComboTooltip)} />
        </strong>
      </div>
    }
    <Select
      label={inlineLabel && formatMessage(messages.serviceClassComboTitle)}
      {...input}
      onChange={(option) => {
        input.onChange(option ? option.value : null)
      }}
      options={serviceClasses}
      size='full'
      clearable
      {...rest}
      onBlur={() => {}} // onBlurResetsInput doesn't work react-select #751
      onBlurResetsInput={false}
    />
  </div>
)

renderServiceClasses.propTypes = {
  input: PropTypes.object.isRequired,
  serviceClasses: PropTypes.array.isRequired,
  intl: intlShape.isRequired,
  inlineLabel: PropTypes.bool
}

renderServiceClasses.defaultProps = {
  inlineLabel: false
}

export default compose(
  injectIntl,
  connect(
    state => ({
      serviceClasses: getServiceClassesJS(state)
    })
  ),
  localizeList({
    input:'serviceClasses',
    idAttribute: 'value',
    nameAttribute: 'label',
    isSorted: true
  }),
  injectSelectPlaceholder
)(renderServiceClasses)
