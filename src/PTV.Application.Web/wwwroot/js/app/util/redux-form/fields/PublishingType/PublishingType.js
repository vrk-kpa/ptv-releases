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
import { connect } from 'react-redux'
import { RenderRadioButtonGroup, RenderRadioButtonGroupDisplay } from 'util/redux-form/renders'
import { Field, change } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { timingTypes, formTypesEnum } from 'enums'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'

const messages = defineMessages({
  publishNow: {
    id: 'PublishingType.Options.PublishNow',
    defaultMessage: 'Julkaise heti'
  },
  publishTimed: {
    id: 'PublishingType.Options.PublishTimed',
    defaultMessage: 'Julkaise ajastetusti'
  },
  title: {
    id: 'PublishingType.Publish.Title',
    defaultMessage: 'Julkaisutapa'
  },
  tooltip: {
    id: 'PublishingType.Publish.Tooltip',
    defaultMessage: 'Julkaisutapa tooltip'
  }
})

let types = {
  [timingTypes.NOW]: messages.publishNow,
  [timingTypes.TIMED]: messages.publishTimed
}

const PublishingType = ({
  intl: { formatMessage },
  change,
  ...rest
}) => {
  const handleOnChange = (_, newValue) => {
    if (newValue === timingTypes.NOW) {
      change(formTypesEnum.MASSTOOLFORM, 'publishAt', null)
    }
  }
  const options = Object.keys(types).map((value) => ({ value, label: formatMessage(types[value]) }))
  return (
    <Field
      name='timingType'
      component={RenderRadioButtonGroup}
      options={options}
      defaultValue={timingTypes.NOW}
      label={formatMessage(messages.title)}
      tooltip={formatMessage(messages.tooltip)}
      onChange={handleOnChange}
      {...rest}
    />
  )
}
PublishingType.propTypes = {
  intl: intlShape.isRequired,
  change: PropTypes.func
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderRadioButtonGroupDisplay }),
  asDisableable,
  connect(null, {
    change
  })
)(PublishingType)
