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
import React, { PropTypes } from 'react'
import { browserHistory } from 'react-router'
import { connect } from 'react-redux'
import { camelCase } from 'lodash'
// Components
import EntityVersion from 'appComponents/EntityVersion'
import LanguageBar from 'appComponents/LanguageBar'
import cx from 'classnames'
import { setRowLanguage } from 'reducers/frontPage'
import { onRecordSelect, setLanguageTo } from 'Routes/FrontPageV2/actions'
import { getFrontPageRowLanguageId,
  getFrontPageRowFirstAvailableLanguageId,
  getFrontPageRowLanguage,
  getFrontPageRowFirstAvailableLanguageCode
} from 'Routes/FrontPageV2/selectors'
import { updateUI } from 'Utils/redux-ui/action-reducer'

const rowSetup = {
  service: {
    service: {
      keyToState: 'service',
      path: '/service/manageService'
    }
  },
  channel: {
    eChannel: {
      keyToState: 'eChannel',
      path: '/channels/manage/eChannel'
    },
    webPage: {
      keyToState: 'webPage',
      path: '/channels/manage/webPage'
    },
    printableForm: {
      keyToState: 'printableForm',
      path: '/channels/manage/printableForm'
    },
    phone: {
      keyToState: 'phone',
      path: '/channels/manage/phone'
    },
    serviceLocation: {
      keyToState: 'serviceLocation',
      path: '/channels/manage/serviceLocation'
    }
  },
  organization: {
    organization: {
      keyToState: 'organization',
      path: '/manage/manage/organizations'
    }
  },
  generalDescription: {
    generalDescription: {
      keyToState: 'generalDescription',
      path: '/manage/manage/generalDescription'
    }
  }
}

const NameCell = ({
    name = '',
    version,
    publishingStatusId = '',
    firstLanguageId,
    firstLanguageCode,
    languageId,
    languageCode,
    componentClass,
    onMouseEnter,
    languagesAvailabilities,
    main,
    sub,
    updateUI,
    id,
    onRecordSelect,
    setRowLanguage,
    setLanguageTo
}) => {
  const handleOnClick = () => {
    setLanguageTo(rowSetup[main][sub].keyToState, typeof name === 'string' ? firstLanguageId : languageId, typeof name === 'string' ? firstLanguageCode : languageCode)
    onRecordSelect(id, rowSetup[main][sub].keyToState)
    browserHistory.push(rowSetup[main][sub].path)
  }
  const handleOnLanguageClick = (data) => {
    setLanguageTo(rowSetup[main][sub].keyToState, data.languageId, data.languageCode)
    onRecordSelect(id, rowSetup[main][sub].keyToState)
    browserHistory.push(rowSetup[main][sub].path)
  }
  const handleOnMouseEnter = (data) => {
    onMouseEnter && onMouseEnter(data)
    setRowLanguage(id, data.languageId, data.languageCode)
  }
  return (
    <div className={cx(componentClass, 'cell', 'cell-name')}>
      <div className='name' onClick={handleOnClick}>{typeof name === 'string' ? name : name && name[languageCode] }</div>
      <LanguageBar onMouseEnter={handleOnMouseEnter}
        onClick={handleOnLanguageClick} languages={languagesAvailabilities} />
      <EntityVersion version={version} publishingStatusId={publishingStatusId} />
    </div>
  )
}

NameCell.propTypes = {
  publishingStatusId: PropTypes.string.isRequired,
  name: PropTypes.any, // currently is returned as string for old impl, in new as object (fi,sv...)
  version: PropTypes.object,
  intl: PropTypes.object,
  onMouseEnter: PropTypes.func,
  updateUI: PropTypes.func,
  componentClass: PropTypes.string,
  languageId: PropTypes.string,
  firstLanguageId: PropTypes.string,
  languageCode: PropTypes.string,
  firstLanguageCode: PropTypes.string,
  languagesAvailabilities: PropTypes.array.isRequired,
  main: PropTypes.string,
  sub: PropTypes.string,
  id: PropTypes.string,
  onRecordSelect: PropTypes.func.isRequired,
  setRowLanguage: PropTypes.func.isRequired,
  setLanguageTo: PropTypes.func.isRequired
}

export default connect((state, ownProps) => ({
  firstLanguageId: getFrontPageRowFirstAvailableLanguageId(state, ownProps),
  firstLanguageCode: getFrontPageRowFirstAvailableLanguageCode(state, ownProps),
  languageId: getFrontPageRowLanguageId(state, ownProps),
  languageCode: getFrontPageRowLanguage(state, ownProps),
  main: camelCase(ownProps.mainEntityType),
  sub: camelCase(ownProps.subEntityType)
}
),
 { onRecordSelect, setLanguageTo, setRowLanguage, updateUI })(NameCell)
