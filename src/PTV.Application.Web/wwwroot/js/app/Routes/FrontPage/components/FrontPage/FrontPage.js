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
import { Tabs, Tab } from 'sema-ui-components'
import { SecurityCreateAny } from 'appComponents/Security'
import FrontPageButtonMenu from '../FrontPageButtonMenu/FrontPageButtonMenu'
import { defineMessages } from 'react-intl'
import { browserHistory } from 'react-router'
import styles from './styles.scss'

const messages = defineMessages({
  pageTitle: {
    id: 'FrontPage.Search.Title',
    defaultMessage: 'Palvelutietovaranto'
  },
  pageDescription: {
    id: 'FrontPage.Search.Description',
    defaultMessage: 'Palvelu-ja pohjakuvaukset, asiontikanavat, organisaatiokuvaukset ja rekisterikuvaukset'
  },
  navigationSearch: {
    id: 'FrontPage.Navigation.Search.Title',
    defaultMessage: 'Kuvaukset'
  },
  navigationRelations: {
    id: 'FrontPage.Navigation.Relations.Title',
    defaultMessage: 'Liitokset'
  }
})

const FrontPage = ({
  intl: { formatMessage },
  location: { pathname },
  children
}) => (
  <div className={styles.page}>
    <header className={styles.pageHeader}>
      <div className={styles.pageHeaderInner}>
        <h1>{formatMessage(messages.pageTitle)}</h1>
        <div>{formatMessage(messages.pageDescription)}</div>
      </div>
    </header>
    <div className={styles.pageNavigation}>
      <Tabs
        index={{ '/frontpage/search': 0, '/frontpage/relations': 1 }[pathname]}
        className='indented'
      >
        <Tab
          onClick={() => browserHistory.push('/frontpage/search')}
          label={formatMessage(messages.navigationSearch)}
        />
        {/*<Tab
          onClick={() => browserHistory.push('/frontpage/relations')}
          label={formatMessage(messages.navigationRelations)}
        />*/ }
      </Tabs>
      <div>
        <SecurityCreateAny isNotAccessibleComponent={null}>
          <FrontPageButtonMenu />
        </SecurityCreateAny>
      </div>
    </div>
    <div className={styles.pageContent}>
      {children}
    </div>
  </div>
)
FrontPage.propTypes = {
  intl: PropTypes.object,
  children: PropTypes.any,
  location: PropTypes.object
}

export default FrontPage
