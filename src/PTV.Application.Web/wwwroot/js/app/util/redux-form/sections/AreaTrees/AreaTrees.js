/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import { injectIntl, defineMessages, intlShape } from 'util/react-intl'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { AreaType,
  AreaTypeProvince,
  AreaTypeProvinceList,
  AreaTypeMunicipality,
  AreaTypeMunicipalityList,
  AreaTypeHospitalRegion,
  AreaTypeHospitalRegionList,
  AreaTypeBusinessRegion,
  AreaTypeBusinessRegionList
} from 'util/redux-form/fields'
import { compose } from 'redux'
import {
  getFormAreaTypeCodeLower,
  getSelectedAreasCount,
  getIsAreaInformationTypeAreaTypeSelected
} from './selectors'
import { Label } from 'sema-ui-components'
import commonMessages from 'util/redux-form/messages'
import styles from './styles.scss'

const tree = {
  municipality: AreaTypeMunicipality,
  province: AreaTypeProvince,
  businessregions: AreaTypeBusinessRegion,
  hospitalregions: AreaTypeHospitalRegion
}

const messages = defineMessages({
  selectedRegionsAreasTitle: {
    id: 'Containers.Channels.Common.InformationArea.Selected.Title',
    defaultMessage: 'Valinnat({count})'
  }
})

const AreaTrees = ({
  formAreaTypeCodeLower,
  selectedCount,
  singleSelect,
  isReadOnly,
  isDisabled,
  isCompareMode,
  isLimitedAccessSelected,
  intl: { formatMessage }
}) => {
  const Tree = !isReadOnly && formAreaTypeCodeLower && tree[formAreaTypeCodeLower] || null
  const basicCompareModeClass = isCompareMode ? 'col-lg-24' : 'col-lg-12'
  const disabled = isDisabled || (!!singleSelect && selectedCount > 0)
  return (
    <div>
      {!isReadOnly &&
        <div className='form-row'>
          <div className='row'>
            <div className={basicCompareModeClass}>
              <AreaType
                disabled={disabled}
                required={isLimitedAccessSelected}
              />
            </div>
          </div>
        </div>
      }
      <div className='row'>
        {Tree !== null &&
          <div className={basicCompareModeClass}>
            <Tree />
          </div>}
        <div className={basicCompareModeClass}>
          {selectedCount > 0 && !isReadOnly &&
            <Label
              labelText={formatMessage(
                messages.selectedRegionsAreasTitle,
                { count: selectedCount }
              )}
            />}
          <div className={styles.tagListWrap}>
            <AreaTypeMunicipalityList
              disabled={isDisabled}
              label={singleSelect
                ? null
                : formatMessage(commonMessages.municipalityLabel)}
            />
            <AreaTypeProvinceList
              disabled={isDisabled}
              label={singleSelect
                ? null
                : formatMessage(commonMessages.provinceLabel)}
            />
            <AreaTypeBusinessRegionList
              disabled={isDisabled}
              label={singleSelect
                ? null
                : formatMessage(commonMessages.businessRegionLabel)}
            />
            <AreaTypeHospitalRegionList
              disabled={isDisabled}
              label={singleSelect
                ? null
                : formatMessage(commonMessages.hospitalRegionLabel)}
            />
          </div>
        </div>
      </div>
    </div>
  )
}

AreaTrees.propTypes = {
  formAreaTypeCodeLower: PropTypes.string.isRequired,
  selectedCount: PropTypes.number,
  singleSelect: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  isDisabled: PropTypes.bool,
  isCompareMode: PropTypes.bool,
  isLimitedAccessSelected: PropTypes.bool,
  intl: intlShape
}

export default compose(
  injectFormName,
  injectIntl,
  withFormStates,
  connect(
    (state, ownProps) => ({
      formAreaTypeCodeLower: getFormAreaTypeCodeLower(state, ownProps),
      selectedCount: getSelectedAreasCount(state, ownProps),
      isLimitedAccessSelected: getIsAreaInformationTypeAreaTypeSelected(state, ownProps)
    })
  )
)(AreaTrees)
