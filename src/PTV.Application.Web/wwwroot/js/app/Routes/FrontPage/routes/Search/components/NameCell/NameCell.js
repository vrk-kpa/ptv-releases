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
import { browserHistory } from 'react-router'
import { connect } from 'react-redux'
import { camelCase } from 'lodash'
import EntityVersion from 'appComponents/EntityVersion'
import { Popup } from 'appComponents'
import cx from 'classnames'
import { setRowLanguage } from 'reducers/frontPage'
import { updateUI } from 'util/redux-ui/action-reducer'
import {
  getFrontPageRowLanguageId,
  getFrontPageRowLanguage
} from 'Routes/FrontPage/routes/Search/selectors'
import { setContentLanguage, setSelectedEntity } from 'reducers/selections'
import styles from './styles.scss'

const rowSetup = {
  service: {
    service: {
      keyToState: 'service',
      path: '/service',
      type: 'services'
    }
  },
  channel: {
    eChannel: {
      keyToState: 'eChannel',
      path: '/channels/electronic',
      type: 'channels'
    },
    webPage: {
      keyToState: 'webPage',
      path: '/channels/webPage',
      type: 'channels'
    },
    printableForm: {
      keyToState: 'printableForm',
      path: '/channels/printableForm',
      type: 'channels'
    },
    phone: {
      keyToState: 'phone',
      path: '/channels/phone',
      type: 'channels'
    },
    serviceLocation: {
      keyToState: 'serviceLocation',
      path: '/channels/serviceLocation',
      type: 'channels'
    }
  },
  organization: {
    organization: {
      keyToState: 'organization',
      path: '/organization',
      type: 'organizations'
    }
  },
  generalDescription: {
    generalDescription: {
      keyToState: 'generalDescription',
      path: '/generalDescription',
      type: 'generalDescriptions'
    }
  }
}

const getName = (name, languageCode) =>
  name && (
    name[languageCode] ||
    name[Object.keys(name)[0]]
  )

const NameCell = ({
    name = '',
    version,
    publishingStatusId = '',
    languageId,
    languageCode,
    componentClass,
    onMouseEnter,
    languagesAvailabilities,
    main,
    sub,
    updateUI,
    id,
    setContentLanguage,
    setSelectedEntity,
    setRowLanguage
}) => {
  const handleOnClick = () => {
    setContentLanguage({
      id: languageId,
      code: languageCode
    })
    setSelectedEntity({
      id,
      type: rowSetup[main][sub].type
    })
    browserHistory.push(rowSetup[main][sub].path + '/' + id)
  }

  return (
    <Popup
      trigger={
        <div className={cx(componentClass, 'cell', 'cell-name')}>
          <div className={styles.nameCell}
            onClick={handleOnClick}>{typeof name === 'string' ? name : getName(name, languageCode) }</div>
          {/* Version should not be visibile "it confuses the users"
            <EntityVersion version={version} publishingStatusId={publishingStatusId} /> */}
        </div>
      }
      position='top left'
      on='hover'
      iconClose={false}
      content={getName(name, languageCode)}
    />
  )
}

NameCell.propTypes = {
  publishingStatusId: PropTypes.string.isRequired,
  name: PropTypes.any, // currently is returned as string for old impl, in new as object (fi,sv...)
  version: PropTypes.object,
  onMouseEnter: PropTypes.func,
  updateUI: PropTypes.func,
  componentClass: PropTypes.string,
  languageId: PropTypes.string,
  languageCode: PropTypes.string,
  setContentLanguage: PropTypes.func,
  setSelectedEntity: PropTypes.func,
  languagesAvailabilities: PropTypes.array.isRequired,
  main: PropTypes.string,
  sub: PropTypes.string,
  id: PropTypes.string,
  setRowLanguage: PropTypes.func.isRequired
}

export default connect(
  (state, ownProps) => ({
    languageId: getFrontPageRowLanguageId(state, ownProps),
    languageCode: getFrontPageRowLanguage(state, ownProps),
    main: camelCase(ownProps.mainEntityType),
    sub: camelCase(ownProps.subEntityType)
  }), {
    setSelectedEntity,
    setContentLanguage,
    setRowLanguage,
    updateUI
  }
)(NameCell)
