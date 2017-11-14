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
import { connect } from 'react-redux'
import { getTopTargetGroupsObjectArray } from 'Routes/FrontPage/routes/Search/selectors'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import { compose } from 'redux'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import { Field } from 'redux-form/immutable'
import { localizeList } from 'appComponents/Localize'
import { RenderSelect, RenderSelectDisplay } from 'util/redux-form/renders'
import { translateValidation } from 'util/redux-form/validators'
import { asDisableable, asComparable } from 'util/redux-form/HOC'

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

const TargetGroup = ({
  intl: { formatMessage },
  targetGroups,
  validate,
  ...rest
}) => (
  <Field
    name='targetGroup'
    label={formatMessage(messages.title)}
    tooltip={formatMessage(messages.tooltip)}
    component={RenderSelect}
    options={targetGroups}
    //validate={translateValidation(validate, formatMessage, messages.title)}
    {...rest}
  />
)
TargetGroup.propTypes = {
  intl: intlShape.isRequired,
  validate: PropTypes.func,
  targetGroups: PropTypes.array
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderSelectDisplay }),
  asDisableable,
  connect(
    state => ({
      targetGroups: getTopTargetGroupsObjectArray(state)
    })
  ),
  localizeList({
    input:'serviceClasses',
    idAttribute: 'value',
    nameAttribute: 'label',
    isSorted: true
  }),
  injectSelectPlaceholder()
)(TargetGroup)
