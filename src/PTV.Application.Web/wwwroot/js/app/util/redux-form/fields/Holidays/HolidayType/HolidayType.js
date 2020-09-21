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
import PropTypes from 'prop-types'
import { RenderRadioButtonGroup } from 'util/redux-form/renders'
import { Field, change } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import { connect } from 'react-redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { fromJS } from 'immutable'
import withPath from 'util/redux-form/HOC/withPath'

const messages = defineMessages({
  close: {
    id: 'Util.ReduxForm.OpeningHour.Holiday.Close',
    defaultMessage: 'Suljettu koko päivän'
  },
  open: {
    id: 'Containers.Channels.Common.OpeningHours.ValidityType.OpenMessage',
    defaultMessage: 'Avoinna'
  }
})
let types = {
  close: messages.close,
  open: messages.open
}

const defaultTime = { from: 28800000, to: 57600000 }

const HolidayType = ({
  intl: { formatMessage },
  setDayIntervals,
  formName,
  hasInterval,
  path,
  ...rest
}) => {
  const options = Object.keys(types).map((value) => ({ value, label: formatMessage(types[value]) }))
  const handleOnChange = (_, newValue) => {
    if (newValue === 'open' && !hasInterval) {
      const intervalInputName = rest.name.replace('type', 'intervals')
      console.log(newValue)
      setDayIntervals(
        formName,
        `${path + '.' + intervalInputName}`, fromJS([defaultTime]))
    }
  }
  return (
    <Field
      name='type'
      component={RenderRadioButtonGroup}
      options={options}
      defaultValue={'close'}
      onChange={handleOnChange}
      small
      {...rest}
    />
  )
}
HolidayType.propTypes = {
  intl: intlShape.isRequired,
  setDayIntervals: PropTypes.func,
  formName: PropTypes.string
}

export default compose(
  injectIntl,
  injectFormName,
  withPath,
  connect(null, {
    setDayIntervals: change
  })
)(HolidayType)
