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
import { compose } from 'redux'
import { connect } from 'react-redux'
// Components
import LanguageBar from 'appComponents/LanguageBar'
import { setContentLanguage, setSelectedEntity } from 'reducers/selections'
import { browserHistory } from 'react-router'
import { camelCase } from 'lodash'

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

const LanguageBarCell = ({
  languagesAvailabilities,
  main,
  sub,
  id,
  setSelectedEntity,
  setContentLanguage,
  clickable,
  ...rest
}) => {
  const handleOnLanguageClick = ({ languageId, languageCode }) => {
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
    <LanguageBar values={languagesAvailabilities} {...rest} onClick={clickable ? handleOnLanguageClick : () => {}} />
  )
}

LanguageBarCell.propTypes = {
  languagesAvailabilities: PropTypes.array,
  main: PropTypes.string,
  sub: PropTypes.string,
  id: PropTypes.string,
  setSelectedEntity: PropTypes.func.isRequired,
  setContentLanguage: PropTypes.func.isRequired,
  clickable: PropTypes.bool
}

LanguageBarCell.defaultProps = {
  clickable: false
}

export default compose(connect(
  (state, ownProps) => ({
    main: camelCase(ownProps.mainEntityType),
    sub: camelCase(ownProps.subEntityType)
  }), {
    setSelectedEntity,
    setContentLanguage
    /* setRowLanguage,
    updateUI */
  }
))(LanguageBarCell)
