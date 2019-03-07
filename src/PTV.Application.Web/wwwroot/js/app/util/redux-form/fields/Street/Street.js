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
import { localizeList } from 'appComponents/Localize'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import { normalize } from 'normalizr'
import PropTypes from 'prop-types'
import React from 'react'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { Field, change } from 'redux-form/immutable'
import { CodeListSchemas } from 'schemas'
import {
  getStreetOptionsJS,
  getStreetDefaultOptionsJS,
  getFormPostalCodeId,
  getFormStreetNumber,
  getLanguageId
} from './selectors'
import { getActiveFormField } from 'selectors/base'
import { getContentLanguageCode } from 'selectors/selections'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import asComparable from 'util/redux-form/HOC/asComparable'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import CommonMessages from 'util/redux-form/messages'
import { RenderAsyncSelect, RenderAsyncSelectDisplay } from 'util/redux-form/renders'
import withValidation from 'util/redux-form/HOC/withValidation'
import { isRequiredCustom } from 'util/redux-form/validators'
import withAccessibilityPrompt from 'util/redux-form/HOC/withAccessibilityPrompt'
import styles from './styles.scss'
import { getPathForOtherField } from 'util/redux-form/util'
import cx from 'classnames'
import { getSelectedLanguage } from 'Intl/Selectors'
import { isAddressNumberInRange } from '../AddressNumber/selectors'

const messages = defineMessages({
  placeholder : {
    id: 'Containers.Channels.Address.Street.Placeholder',
    defaultMessage: 'esim. Mannerheimintie'
  },
  visitingTooltip : {
    id: 'Containers.Channels.VistingAddress.Street.Tooltip',
    defaultMessage: 'Kirjoita tarkka katuosoite ja tarkka osoitenumero erillisille kentille.'
  },
  postalTooltip : {
    id: 'Containers.Channels.PostalAddress.Street.Tooltip',
    defaultMessage: 'Kirjoita tarkka katuosoite ja tarkka osoitenumero erillisille kentille.'
  },
  deliveryTooltip : {
    id: 'Containers.Channels.DeliveryAddress.Street.Tooltip',
    defaultMessage: 'Kirjoita tarkka katuosoite ja tarkka osoitenumero erillisille kentille.'
  },
  visitingTooltip1 : {
    id: 'Containers.Channels.VistingAddress.Street.First.Tooltip',
    defaultMessage: 'Tooltip for the first visiting address'
  },
  postalTooltip1 : {
    id: 'Containers.Channels.PostalAddress.Street.First.Tooltip',
    defaultMessage: 'Kirjoita tarkka katuosoite ja tarkka osoitenumero erillisille kentille.',
    description: 'Containers.Channels.PostalAddress.Street.Tooltip'
  },
  deliveryTooltip1 : {
    id: 'Containers.Channels.DeliveryAddress.Street.First.Tooltip',
    defaultMessage: 'Kirjoita tarkka katuosoite ja tarkka osoitenumero erillisille kentille.',
    description: 'Containers.Channels.DeliveryAddress.Street.Tooltip'
  },
  menuStreetName: {
    id: 'Containers.Channels.Address.Street.Name',
    defaultMessage: 'Kadunnimi'
  },
  menuPostalCode: {
    id: 'Containers.Cahnnels.Address.PostalCode',
    defaultMessage: 'Postinumero'
  },
  menuPostOffice: {
    id: 'Containers.Channels.Address.PostOffice',
    defaultMessage: 'Postitoimipaikka'
  },
  visitingStreetNotFoundError: {
    id: 'Containers.Channels.Address.VisitingStreet.NameNotFound',
    defaultMessage: 'Osoitetta ei löydy järjestelmästä. Tarkista osoite tai lisää uusi osoite käyttämällä. Muu sijantitieto -elementtiä.',
    description: 'Containers.Channels.Address.Street.NameNotFound'
  },
  deliveryStreetNotFoundError: {
    id: 'Containers.Channels.Address.DeliveryStreet.NameNotFound',
    defaultMessage: 'Osoitetta ei löydy järjestelmästä. Tarkista osoite.'

  },
  postalStreetNotFoundError: {
    id: 'Containers.Channels.PostalAddress.PostalStreet.NameNotFound',
    defaultMessage: 'Osoitetta ei löydy järjestelmästä. Tarkista osoite.'
  }
})

const StreetNameRender = compose(
  connect(
    (state, { input: { value } }) => ({
      defaultOptions: value && getStreetDefaultOptionsJS(state, { streetId: value })
    })
  )
)(RenderAsyncSelect)

// class CustomOption extends Component {
//   constructor (props) {
//     super(props)
//     this.myRef = React.createRef()
//   }

//   componentDidUpdate () {
//     if (this.myRef.current && this.props.isFocused) {
//       this.myRef.current.scrollIntoView({ block: 'nearest' })
//     }
//   }

//   render () {
//     const { option, selectValue, focusOption, isFocused } = this.props
//     const optionClass = cx(styles.option, {
//       [styles.focused]: isFocused
//     })
//     return (
//       <div
//         ref={this.myRef}
//         className={optionClass}
//         onMouseOver={() => focusOption(option)}
//         onClick={() => selectValue(option)}>
//         <div className='row'>
//           <span className='col-sm-9'>{option.label}</span>
//           <span className='col-sm-6'>{option.postalCode.code}</span>
//           <span className='col-sm-9'>{option.postalCode.postOffice}</span>
//         </div>
//       </div>
//     )
//   }
// }

const CustomOption = ({ option, selectValue, focusOption, isFocused }) => {
  const optionClass = cx(styles.option, {
    [styles.focused]: isFocused
  })
  return (
    <div
      className={optionClass}
      onMouseOver={() => focusOption(option)}
      onClick={() => selectValue(option)}>
      <div className='row'>
        <div className='col-sm-9'><span className={styles.street}>{option.label}</span></div>
        <div className='col-sm-6'><span className={styles.postalCode}>{option.postalCode.code}</span></div>
        <div className='col-sm-9'><span>{option.postalCode.postOffice}</span></div>
      </div>
    </div>
  )
}

const Street = ({
  intl: { formatMessage },
  activeFieldPath,
  addressUseCase,
  defaultValue,
  formName,
  dispatch,
  onChange,
  language,
  loadedPostalCodeId,
  isFirst,
  contentLanguageId,
  streetNumber,
  ...rest
}) => {
  const filterOption = () => true
  const inputProps = { maxLength: 100 }

  const formatResponse = data => {
    const result = []
    const indexMap = new Map()

    for (const street of data) {
      for (const streetNumber of street.streetNumbers) {
        const key = `${street.id}${streetNumber.postalCode.code}`
        if (!indexMap.has(key) && (!loadedPostalCodeId || loadedPostalCodeId === streetNumber.postalCode.id)) {
          indexMap.set(key, true)
          result.push({
            label: street.translation.texts[language] || street.defaultTranslation,
            value: street.id,
            entity: street,
            postalCode: streetNumber.postalCode,
            key
          })
        }
      }
    }

    const sortedResult = result.sort((a, b) => {
      if (a.label < b.label) {
        return -1
      }
      if (a.label > b.label) {
        return 1
      }
      if (a.postalCode.postOffice < b.postalCode.postOffice) {
        return -1
      }
      return 1
    })

    return sortedResult
  }

  const getStreetNumberRangeId = (ranges, streetNumber, postalCodeId) => {
    const filteredRanges = ranges.filter((range, key) => {
      return range.postalCode.id === postalCodeId &&
        isAddressNumberInRange(streetNumber, range.startNumber, range.endNumber, range.isEven)
    })

    if (!filteredRanges || filteredRanges.length === 0) {
      return null
    }

    if (filteredRanges.length > 1) {
      console.error('Multiple street ranges detected!')
    }

    return filteredRanges[0].id
  }

  const callDispatchChange = (fieldName, value) => {
    dispatch(change(formName, getPathForOtherField(activeFieldPath, fieldName), value))
  }

  const handleOnChangeObject = object => {
    const entity = object && object.entity
    const postalCodeId = object && object.postalCode && object.postalCode.id

    entity && dispatch({
      type: 'STREETS',
      response: normalize(entity, CodeListSchemas.STREET)
    })
    callDispatchChange('postalCode', postalCodeId)
    callDispatchChange('postOffice', postalCodeId)
    callDispatchChange('municipality', entity && entity.municipalityId)
    callDispatchChange('postalCodeOutsideChange', null)

    if (entity && streetNumber) {
      callDispatchChange('streetNumberRange', getStreetNumberRangeId(entity.streetNumbers, streetNumber, postalCodeId))
    }

    onChange && onChange(entity)
  }

  const renderOptionsTable = menuProps => {
    const { options, focusedOption } = menuProps
    return (
      <div className={styles.customOptionMenu}>
        <h5 className={styles.optionHeading}>
          <div className='row'>
            <div className='col-sm-9'><span>{formatMessage(messages.menuStreetName)}</span></div>
            <div className='col-sm-6'><span>{formatMessage(messages.menuPostalCode)}</span></div>
            <div className='col-sm-9'><span>{formatMessage(messages.menuPostOffice)}</span></div>
          </div>
        </h5>
        {
          options.map((option, index) => {
            const isFocused = option.key === focusedOption.key
            return <menuProps.optionComponent key={option.key} option={option} isFocused={isFocused} {...menuProps} />
          })
        }
      </div>
    )
  }

  const getSearchData = input => ({
    searchedStreetName: input,
    postalCodeId: loadedPostalCodeId || null,
    languageId: contentLanguageId
  })

  return (
    <Field
      label={formatMessage(CommonMessages.street)}
      tooltip={isFirst
        ? formatMessage(messages[`${addressUseCase}Tooltip1`])
        : formatMessage(messages[`${addressUseCase}Tooltip`])
      }
      component={StreetNameRender}
      menuRenderer={renderOptionsTable}
      optionComponent={CustomOption}
      valueKey='key'
      onChangeObject={handleOnChangeObject}
      formatResponse={formatResponse}
      filterOption={filterOption}
      url='common/GetStreets'
      name='street'
      minLength={3}
      inputProps={inputProps}
      getQueryData={getSearchData}
      className={styles.addressSelect}
      debounceDelay={300}
      {...rest}
    />
  )
}

Street.propTypes = {
  intl: intlShape,
  addressUseCase: PropTypes.string,
  defaultValue: PropTypes.string,
  formName: PropTypes.string.isRequired,
  dispatch: PropTypes.func.isRequired,
  activeFieldPath: PropTypes.string,
  onChange: PropTypes.func,
  language: PropTypes.string,
  loadedPostalCodeId: PropTypes.string,
  isFirst: PropTypes.bool,
  contentLanguageId: PropTypes.string,
  streetNumber: PropTypes.string
}

export default compose(
  withValidation({
    label: CommonMessages.street,
    validate: isRequiredCustom((props) => messages[`${props.addressUseCase}StreetNotFoundError`])
  }),
  injectIntl,
  injectFormName,
  asComparable({ DisplayRender: RenderAsyncSelectDisplay }),
  asDisableable,
  injectSelectPlaceholder(),
  withAccessibilityPrompt,
  connect(
    (state, ownProps) => {
      const languageCode = getContentLanguageCode(state, ownProps) || getSelectedLanguage(state)

      return {
        activeFieldPath: getActiveFormField(ownProps.formName)(state),
        options: getStreetOptionsJS(state),
        triggerOnChange: true,
        language: languageCode,
        loadedPostalCodeId: getFormPostalCodeId(state, ownProps),
        contentLanguageId: getLanguageId(languageCode)(state, ownProps),
        streetNumber: getFormStreetNumber(state, ownProps)
      }
    }
  ),
  localizeList({
    input: 'options',
    idAttribute: 'value',
    nameAttribute: 'label',
    isSorted: true
  })
)(Street)
