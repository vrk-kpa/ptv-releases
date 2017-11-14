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
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import { RenderUrlChecker, RenderTextFieldDisplay } from 'util/redux-form/renders'
import { Field } from 'redux-form/immutable'
import { compose } from 'redux'
import { isUrl } from 'util/redux-form/validators'
import { asLocalizable, asDisableable, withValidation, asComparable } from 'util/redux-form/HOC'
import CommonMessages from 'util/redux-form/messages'

const messages = defineMessages({
  tooltip:  {
    id: 'Containers.Channels.AddElectronicChannel.Step1.AttachmentUrl.Tooltip',
    defaultMessage: 'Anna mahdollisimman tarkka verkko-osoite, josta dokumentti tai verkkosivu avautuu. Kopioi ja liitä verkko-osoite ja tarkista verkko-osoitteen toimivuus Testaa osoite -painikkeesta.'
  },
  placeholder: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.AttachmentUrl.Placeholder',
    defaultMessage: 'Kopioi ja liitä tarkka verkko-osoite.'
  },
  buttonText: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.AttachmentUrl.Button.Title',
    defaultMessage: 'Testaa osoite'
  },
  checkerInfo:  {
    id: 'Containers.Channels.AddElectronicChannel.Step1.AttachmentUrl.Icon.Tooltip',
    defaultMessage: 'Verkko-osoitetta ei löytynyt, tarkista sen muoto.'
  }
})

const UrlChecker = ({
  intl: { formatMessage },
  // validate = [
  //   isRequired,
  //   isUrl
  // ],
  tooltip,
  ...rest
}) => (
  <Field
    name='urlAddress'
    component={RenderUrlChecker}
    label={formatMessage(CommonMessages.urlAddress)}
    tooltip={tooltip || formatMessage(messages.tooltip)}
    placeholder={formatMessage(messages.placeholder)}
    buttonText={formatMessage(messages.buttonText)}
    {...rest}
  />
)
UrlChecker.propTypes = {
  intl: intlShape,
  validate: PropTypes.oneOfType([
    PropTypes.func,
    PropTypes.arrayOf(PropTypes.func)
  ]),
  tooltip: PropTypes.string
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderTextFieldDisplay }),
  asDisableable,
  asLocalizable,
  withValidation({
    label: CommonMessages.urlAddress,
    validate: isUrl
  })
)(UrlChecker)
