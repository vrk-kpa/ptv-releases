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
import React, { Fragment } from 'react'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import withFormStates, {
  formStatesPropTypes
} from 'util/redux-form/HOC/withFormStates'
import {
  getIsAccessibilityRegisterSet,
  getAccessibilityRegisterSetAt,
  getIsAccessibilityRegisterValid
} from './selectors'
import styles from './styles.scss'

const messages = defineMessages({
  arNotFetched: {
    id: 'Util.ReduxForm.Fields.Accessibility.Instructions.arNotFetched',
    defaultMessage: 'Voit antaa palvelupaikan esteettömyystiedot erillistä esteettömyyssovellusta käyttäen. Kun lisäät palvelupaikalle ensimmäisen osoitteen ja tallennat tiedot, käyntiosoitteen yhteyteen tulee näkyviin linkki esteettömyyssovellukseen (Avaa esteettömyyssovellus). Tarkempia ohjeita esteettömyyssovelluksen käyttöön. Avaa ohje (the link that opens the instructions page) Esteettömyyssovellus on käytettävissä vain suomeksi.'
  },
  arFetched: {
    id: 'Util.ReduxForm.Fields.Accessibility.Instructions.arFetched',
    defaultMessage: 'Palvelupaikan esteettömyystiedot on haettu erillisestä esteettömyystietosovellukse sta, mikäli haluat muokata tietoja, tallenna palvelupaikka ja klikkaa esikatselussa nakyviin tulevaa linkkiä Avaa esteettömyyssovellus", joka avaa sovelluksen.'
  },
  downloadLinkTitle: {
    id: 'Util.ReduxForm.Fields.Accessibility.Instructions.downloadLink',
    defaultMessage: 'Avaa ohje'
  },
  downloadLinkHref: {
    id: 'Accessibility.Instructions.downloadLinkHref',
    defaultMessage: 'https://palveluhallinta.suomi.fi/storage/cms.files/xMOCHOAzsjtjvWcA.docx'
  }
})

const Instructions = ({
  intl: { formatMessage },
  arFetched,
  arValid,
  isReadOnly
}) => (
  <Fragment>
    {arFetched && arValid
      ? isReadOnly
        ? null
        : formatMessage(messages.arFetched)
      : (
        <div className={isReadOnly ? styles.readOnlyInstructions : undefined}>
          {formatMessage(messages.arNotFetched)}
          {' '}
          <a href={formatMessage(messages.downloadLinkHref)} target='_blank' rel='noopener noreferrer'>
            {formatMessage(messages.downloadLinkTitle)}
          </a>
        </div>
      )}
  </Fragment>
)
Instructions.propTypes = {
  ...formStatesPropTypes,
  intl: intlShape
}

export default compose(
  withFormStates,
  injectIntl,
  connect(state => ({
    arFetched: getIsAccessibilityRegisterSet(state),
    arValid: getIsAccessibilityRegisterValid(state),
    setAt: getAccessibilityRegisterSetAt(state)
  })),
)(Instructions)
