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
import asContainer from 'util/redux-form/HOC/asContainer'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'

const messages = defineMessages({
  title: {
    id: 'Util.ReduxForm.Fields.Accessibility.Explanation.Title',
    defaultMessage: 'Käyntiosoitteen esteettömmystiedot'
  },
  explanation: {
    id: 'Util.ReduxForm.Fields.Accessibility.Explanation.Text',
    defaultMessage: 'Voit antaa palvelupaikan esteettömyystiedot erillistä esteettömyyssovellusta käyttäen. Kun lisäät palvelupaikalle ensimmäisen osoitteen ja tallennat tiedot, käyntiosoitteen yhteyteen tulee näkyviin linkki esteettömyyssovellukseen (Avaa esteettömyyssovellus). Tarkempia ohjeita esteettömyyssovelluksen käyttöön. Avaa ohje (the link that opens the instructions page) Esteettömyyssovellus on käytettävissä vain suomeksi.'
  }
})

const Explanation = ({
  intl: { formatMessage }
}) => (
  <div>
    {formatMessage(messages.explanation)}
  </div>
)
Explanation.propTypes = {
  intl: intlShape
}

export default compose(
  injectIntl,
  asContainer({
    title: messages.title
  })
)(Explanation)
