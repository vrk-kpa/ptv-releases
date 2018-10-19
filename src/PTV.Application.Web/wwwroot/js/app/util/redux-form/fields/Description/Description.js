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
import { RenderTextEditor, RenderTextEditorDisplay } from 'util/redux-form/renders'
import asLocalizable from 'util/redux-form/HOC/asLocalizable'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'
import withValidation from 'util/redux-form/HOC/withValidation'

import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { Field } from 'redux-form/immutable'
import { compose } from 'redux'
import { isDraftEditorSizeExceeded } from 'util/redux-form/validators'
import CommonMessages from 'util/redux-form/messages'
import withTranslationLock from 'util/redux-form/HOC/withTranslationLock'

const messages = defineMessages({
  tooltip: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Description.Tooltip',
    defaultMessage: 'Kuvaa verkkoasiointikanava mahdollisimman selkeästi ja asiakkaan kannalta ymmärrettävästi. Kerro, mitä verkkoasiointi pitää sisällään ja miten se toimii. Käytä selkeää yleiskieltä, vältä hallinnollisia termejä. Kuvaa vain verkkoasiointikanavaa, älä siihen kytkettyä palvelua tai palvelua järjestävää organisaatiota tai sen tehtäviä!'
  },
  placeholder: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Description.Placeholder',
    defaultMessage: 'Kirjoita selkeä ja ymmärrettävä kuvausteksti.'
  }
})

const Description = ({
  intl: { formatMessage },
  ...rest
}) => (
  <Field
    name='description'
    component={RenderTextEditor}
    label={`${formatMessage(CommonMessages.description)}`}
    tooltip={formatMessage(messages.tooltip)}
    placeholder={formatMessage(messages.placeholder)}
    {...rest}
  />
)
Description.propTypes = {
  intl: intlShape,
  validate: PropTypes.func
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderTextEditorDisplay }),
  withTranslationLock({ fieldType: 'textEditor' }),
  asDisableable,
  asLocalizable,
  withValidation({
    label: CommonMessages.description,
    validate: isDraftEditorSizeExceeded
  }),
)(Description)
