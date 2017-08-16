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
import { getTopTargetGroupsObjectArray } from 'Routes/FrontPageV2/selectors'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import { compose } from 'redux'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import InfoBox from 'appComponents/InfoBox'

const messages = defineMessages({
  title: {
    id: 'FrontPage.TargetGroups.Title',
    defaultMessage: 'Pääkohderyhmä'
  },
  tooltip: {
    id: 'FrontPage.TargetGroups.Tooltip',
    defaultMessage: 'Voit hakea tietylle kohderyhmälle tarkoitettujen palveluiden pohjakuvauksia. Valitse pudotusvalikosta pääkohderyhmä. Tarvittaessa tarkenna kohderyhmävalintaa valitsemalla alakohderyhmä.'
  }
})

const renderTargetGroups = ({
  input,
  targetGroups,
  intl: { formatMessage },
  inlineLabel,
  ...rest
}) => (
  <div>
    {!inlineLabel &&
      <div className='form-field tooltip-label'>
        <strong>
          {formatMessage(messages.title)}
          <InfoBox tip={formatMessage(messages.tooltip)} />
        </strong>
      </div>
    }
    <Select
      label={inlineLabel && formatMessage(messages.title)}
      {...input}
      onChange={(option) => {
        input.onChange(option ? option.value : null)
      }}
      options={targetGroups}
      size='full'
      clearable
      {...rest}
    />
  </div>
)
renderTargetGroups.propTypes = {
  input: PropTypes.object,
  targetGroups: PropTypes.array,
  intl: intlShape.isRequired,
  inlineLabel: PropTypes.bool
}

renderTargetGroups.defaultProps = {
  inlineLabel: false
}

export default compose(
  connect(
    state => ({
      targetGroups: getTopTargetGroupsObjectArray(state)
    })
  ),
  injectIntl,
  injectSelectPlaceholder
)(renderTargetGroups)
