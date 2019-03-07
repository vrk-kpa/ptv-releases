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
