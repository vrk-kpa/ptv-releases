import { compose } from 'redux'
import FrontPage from 'Routes/FrontPage/components/FrontPage'
import { injectIntl } from 'react-intl'

export default compose(
  injectIntl
)(FrontPage)
