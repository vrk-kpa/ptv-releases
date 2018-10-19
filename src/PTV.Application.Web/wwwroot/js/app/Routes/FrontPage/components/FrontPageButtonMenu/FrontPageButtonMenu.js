import React from 'react'
import { SecurityCreate } from 'appComponents/Security'
import { Link } from 'react-router-dom'
import { ButtonMenu } from 'sema-ui-components'
import { compose } from 'redux'
import { intlShape, injectIntl } from 'util/react-intl'
import styles from './styles.scss'
import { entityTypesMessages } from 'util/redux-form/messages'

const FrontPageButtonMenu = ({
  intl: { formatMessage }
}) => {
  return (
    <ButtonMenu
      className='createNewEntity'
      buttonLabel={formatMessage(entityTypesMessages.addNewContentTitle)}
      align='right'
      elements={[
        {
          header: null,
          content: (
            <div className={styles.buttonList}>
              <SecurityCreate domain='services' >
                <ul>
                  <li>
                    <Link to='/service'>
                      {formatMessage(entityTypesMessages.services)}
                    </Link>
                  </li>
                </ul>
              </SecurityCreate>
              <SecurityCreate domain='channels' >
                <ul>
                  <li>
                    <div>{formatMessage(entityTypesMessages.channels)}</div>
                  </li>
                  <li>
                    <Link to='/channels/electronic'>
                      {formatMessage(entityTypesMessages.eChannelLinkTitle)}
                    </Link>
                  </li>
                  <li>
                    <Link to='/channels/webPage'>
                      {formatMessage(entityTypesMessages.webPageLinkTitle)}
                    </Link>
                  </li>
                  <li>
                    <Link to='/channels/printableForm'>
                      {formatMessage(entityTypesMessages.printableFormLinkTitle)}
                    </Link>
                  </li>
                  <li>
                    <Link to='/channels/phone'>
                      {formatMessage(entityTypesMessages.phoneLinkTitle)}
                    </Link>
                  </li>
                  <li>
                    <Link to='/channels/serviceLocation'>
                      {formatMessage(entityTypesMessages.serviceLocationLinkTitle)}
                    </Link>
                  </li>
                </ul>
              </SecurityCreate>
              <SecurityCreate domain='generalDescriptions' >
                <ul>
                  <li>
                    <Link to='/generalDescription'>
                      {formatMessage(entityTypesMessages.generalDescriptions)}
                    </Link>
                  </li>
                </ul>
              </SecurityCreate>
              <SecurityCreate domain='organizations' >
                <ul>
                  <li>
                    <Link to='/organization'>
                      {formatMessage(entityTypesMessages.organizations)}
                    </Link>
                  </li>
                </ul>
              </SecurityCreate>
              <SecurityCreate domain='organizations' >
                <ul>
                  <li>
                    <Link to='/serviceCollection'>
                      {formatMessage(entityTypesMessages.serviceCollections)}
                    </Link>
                  </li>
                </ul>
              </SecurityCreate>
            </div>
          )
        }
      ]}
    />
  )
}
FrontPageButtonMenu.propTypes = {
  intl: intlShape
}

export default compose(
  injectIntl,
)(FrontPageButtonMenu)
