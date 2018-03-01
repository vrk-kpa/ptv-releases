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
import { AreaInformationType } from 'util/redux-form/fields'
import { asGroup, asSection, injectFormName, withFormStates } from 'util/redux-form/HOC'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { AreaTrees } from 'util/redux-form/sections'
import { defineMessages, FormattedMessage, intlShape, injectIntl } from 'react-intl'
import { getIsTreeActive } from './selectors'
import { Label } from 'sema-ui-components'
import { formTypesEnum } from 'enums'
import cx from 'classnames'
import styles from './styles.scss'

export const areaInformationMessages = defineMessages({
  areaInformationTitle: {
    id: 'Containers.Services.AddService.Step1.AreaInformation.Title',
    defaultMessage: 'Alue, joilla palvelu on saatavilla'
  },
  areaInformationTooltip: {
    id: 'Containers.Services.AddService.Step1.AreaInformation.Tooltip',
    defaultMessage: 'Alue, joilla palvelu on saatavilla'
  },
  selectedRegionsAreasTitle: {
    id: 'Containers.Services.AddService.Step1.AreaInformation.Selected.Title',
    defaultMessage: 'Valinnat({count})'
  },
  userInfo: {
    id: 'Containers.Services.AddService.Step1.AreaInformation.UserInfo',
    defaultMessage: 'Olet tekemässä muutoksia aluetietoon. Muutokset tallentuvat kaikkiin kieliversioihin.'
  },
  areaInformationInfoText: {
    id: 'Containers.Services.AddService.Step1.AreaInformation.Type.Title',
    defaultMessage: 'Aluetieto täytetty organisaation mukaan. Voit muuttaa valintaa.'
  }
})

const AreaInformationExpanded = ({
  isTreeActive,
  hideType,
  intl: { formatMessage },
  isReadOnly,
  isCompareMode,
  formName,
  ...rest
}) => {
  const showLabel = formName !== formTypesEnum.ORGANIZATIONFORM
  const basicCompareModeClass = isCompareMode ? 'col-md-24' : 'col-md-12'
  const areaWrapClass = cx(
    styles.areaWrap,
    {
      [styles.withoutLabel]: !showLabel
    }
  )
  return (
    <div className={areaWrapClass}>
      {!hideType && <div>
        {!isReadOnly && showLabel &&
          <div className='form-row'>
            <div className='row'>
              <div className={isCompareMode ? 'col-lg-12' : 'col-lg-24'}>
                <Label
                  infoLabel
                  labelText={formatMessage(areaInformationMessages.areaInformationInfoText)}
                />
              </div>
            </div>
          </div>
        }
        <div className='row'>
          <div className={basicCompareModeClass}>
            <AreaInformationType radio {...rest} />
          </div>
        </div>
      </div>}
      {(isTreeActive || hideType) &&
      <div className='form-row'>
        <AreaTrees {...rest} />
      </div>
      }
    </div>
  )
}

AreaInformationExpanded.propTypes = {
  isTreeActive: PropTypes.bool.isRequired,
  hideType: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  isCompareMode: PropTypes.bool,
  intl: intlShape,
  formName: PropTypes.string
}

export default compose(
  injectIntl,
  injectFormName,
  connect(
    (state, ownProps) => ({
      isTreeActive: getIsTreeActive(state, ownProps)
    })
  ),
  asSection('areaInformation'),
  asGroup({
    title: areaInformationMessages.areaInformationTitle,
    tooltip: <FormattedMessage {...areaInformationMessages.areaInformationTooltip} />,
    required: true
  }),
  withFormStates
)(AreaInformationExpanded)
