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
