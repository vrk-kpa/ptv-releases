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
import { connect } from 'react-redux'
import { compose } from 'redux'
import { injectIntl } from 'react-intl'

// components
import { Areas, AreaType, AreaInformationType } from '../../../Common/Areas'
import { PTVPreloader, PTVAddItem } from '../../../../Components'
import * as PTVValidatorTypes from '../../../../Components/PTVValidators'

// actions
import * as serviceActions from '../../Service/Actions'
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps'

// selectors
import * as ServiceSelectors from '../../Service/Selectors'
import * as CommonSelectors from '../../../Common/Selectors'

// enums
import { areaTypes } from '../../../Common/Enums'

const ServiceAreaInformation = ({
    messages,
    readOnly,
    language,
    translationMode,
    areaInformatioTypeId,
    areaTypeId,
    areaTypeVisible,
    actions,
    areaInformationIsFetching,
    selectedAreaCount,
    showUserInfo,
    intl }) => {
  const onInputChange = (input, isSet = false) => value => {
    actions.onServiceInputChange(input, value, language, isSet)
  }
  const onListChange = (input) => (value, isAdd) => {
    actions.onListChange(input, value, language, isAdd)
  }
  const onListRemove = (input) => (value) => {
    actions.onListChange(input, value, language, false)
  }
  const validators = [PTVValidatorTypes.IS_REQUIRED]
  const configuration = {
    [areaTypes.MUNICIPALITY] : {
      listSelector:ServiceSelectors.getSelectedAreaMunicipality,
      getItemSelector:CommonSelectors.getMunicipality,
      onChange:onListChange('areaMunicipality'),
      onNodeRemove:onListRemove('areaMunicipality')
    },
    [areaTypes.PROVINCE] : {
      listSelector:ServiceSelectors.getSelectedAreaProvince,
      getItemSelector:CommonSelectors.getProvince,
      onChange:onListChange('areaProvince'),
      onNodeRemove:onListRemove('areaProvince')
    },
    [areaTypes.BUSINESS_REGION] : {
      listSelector:ServiceSelectors.getSelectedAreaBusinessRegions,
      getItemSelector:CommonSelectors.getBusinessRegion,
      onChange:onListChange('areaBusinessRegions'),
      onNodeRemove:onListRemove('areaBusinessRegions')
    },
    [areaTypes.HOSPITAL_REGION] : {
      listSelector:ServiceSelectors.getSelectedAreaHospitalRegions,
      getItemSelector:CommonSelectors.getHospitalRegion,
      onChange:onListChange('areaHospitalRegions'),
      onNodeRemove:onListRemove('areaHospitalRegions')
    }
  }
  return (
    <div>
      <div>
        {!areaInformationIsFetching
        ? <div>
          <div className='row'>
            <div className='col-xs-12'>
              <AreaInformationType
                language={language}
                messages={messages}
                readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}
                changeCallback={onInputChange('areaInformationTypeId')}
                areaInformationType={areaInformatioTypeId}
                radio
              />
            </div>
          </div>
          {areaTypeVisible &&
          <div>
            {(!readOnly && translationMode === 'none')
            ? <div className='row'>
              <div className='col-xs-6'>
                <AreaType
                  language={language}
                  messages={messages}
                  readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}
                  changeCallback={onInputChange('areaTypeId')}
                  areaType={areaTypeId}
                  validators={selectedAreaCount !== 0 ? [] : validators}
                  showUserInfo={showUserInfo}
                  />
              </div>
            </div> : null}
            <div className='row'>
              <Areas
                language={language}
                messages={messages}
                readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}
                configuration={configuration}
                areaType={areaTypeId}
                validators={selectedAreaCount || validators}
                selectedCount={selectedAreaCount} />
            </div>
          </div>
        }
        </div>
        : <PTVPreloader />}
      </div>
    </div>
  )
}

ServiceAreaInformation.propTypes = {
  messages: PropTypes.any.isRequired,
  readOnly: PropTypes.bool.isRequired,
  language: PropTypes.string.isRequired,
  translationMode: PropTypes.string.isRequired,
  actions: PropTypes.object.isRequired,
  intl: PropTypes.object.isRequired,
  areaInformatioTypeId: PropTypes.string,
  areaTypeId: PropTypes.string,
  areaTypeVisible: PropTypes.bool.isRequired,
  areaInformationIsFetching: PropTypes.bool.isRequired,
  selectedAreaCount: PropTypes.number.isRequired,
  showUserInfo: PropTypes.bool.isRequired
}

function mapStateToProps (state, ownProps) {
  const defaultAreaInformatioTypeId = ServiceSelectors.getAreaInformationTypeId(state, ownProps)
  const defaultAreaTypeId = ServiceSelectors.getAreaTypeId(state, ownProps)
  const languages = CommonSelectors.getEntityAvailableLanguagesCodes(state, { entitiesType:'services', id: ownProps.entityId })
  const translationExists = languages.some(lang => lang.code === ownProps.language.toUpperCase())
  const languageCount = translationExists ? languages.size : languages.size + 1
  return {
    areaInformatioTypeId: defaultAreaInformatioTypeId,
    areaTypeVisible: defaultAreaInformatioTypeId === CommonSelectors.getAreaInformationTypeId(state, 'areatype'),
    areaTypeId: defaultAreaTypeId,
    areaInformationIsFetching: CommonSelectors.getAreaInformationIsFetching(state, { keyToState : 'service' }),
    selectedAreaCount: ServiceSelectors.getSelectedAreaCount(state, ownProps),
    showUserInfo: languageCount > 1
  }
}

const actions = [
  serviceActions
]

const composeAddItem = InnerComponent => {
  const AddItem = props => {
    const render = () => (<InnerComponent {...props} />)

    return (
      <PTVAddItem {...props}
        readOnly={props.readOnly && props.translationMode === 'none'}
        collapsible={false}
        renderItemContent={render}
        messages={{
          'label': props.intl.formatMessage(props.messages.areaInformationTitle),
          'tooltip': props.intl.formatMessage(props.messages.areaInformationTooltip) }}
            />
    )
  }
  return AddItem
}

export default compose(
  connect(mapStateToProps, mapDispatchToProps(actions)),
  injectIntl,
  composeAddItem
)(ServiceAreaInformation)

