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
import { compose } from 'redux'
import { TypeSchemas } from 'schemas'
import { normalize } from 'normalizr'
import { OrderedSet, Map, List } from 'immutable'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { getKeywordsJS, hasGDKeywords, getGDKeywordLabelValues, getAllKeywordsJS, getLocalizedKeywords } from './selectors'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'
import asLocalizable from 'util/redux-form/HOC/asLocalizable'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { Field, change, formValueSelector } from 'redux-form/immutable'
import { RenderMultiSelectAsyncCreatable, RenderMultiSelectDisplay } from 'util/redux-form/renders'
import CommonMessages from 'util/redux-form/messages'
import { TreeListDisplay } from 'util/redux-form/fields/TreeListDisplay'

const messages = defineMessages({
  title: {
    id: 'Containers.Services.AddService.Step2.KeyWords.Title',
    defaultMessage: 'Vapaat asiasanat'
  },
  placeholder: {
    id: 'Containers.Services.AddService.Step2.KeyWords.Placeholder',
    defaultMessage: 'Lisää vapaita asiasanoja harkiten.'
  },
  tooltip: {
    id: 'Containers.Services.AddService.Step2.KeyWords.Tooltip',
    defaultMessage: 'Kirjoita vapaisiin asiasanoihin ainoastaan sellaisia käsitteitä, joita ei löydy ontologiakentästä, esimerkiksi puhekielisiä sanoja (työkkäri) tai käytöstä poistuneita nimiä (työvoimatoimisto), joilla käyttäjät saattavat etsiä palvelua. Kirjoita asiasanat kenttään pilkulla ja välilyönnillä erotettuina.'
  },
  gdListLabel: {
    id: 'Containers.Services.AddService.Step2.KeyWords.GeneralDescriptionList.Header',
    defaultMessage: 'Pohjakuvauksesta tulevat avainsanat:'
  },
  gdListTooltip: {
    id: 'Containers.Services.AddService.Step2.KeyWords.GeneralDescriptionList.Tooltip',
    defaultMessage: 'Et voi poistaa valintoja, mutta voit lisätä uusia.'
  }  
})

const formatResponse = data => {
  return data && data.keywords.map((keyword) => ({
    label: keyword.name,
    value: keyword.id,
    entity: keyword
  })) || {}
}

const Keywords = ({
  intl: { formatMessage },
  validate,
  language,
  dispatch,
  change,
  formName,
  newKeywords,
  hasGDKeywords,
  ...rest
}) => {
  const handleOnChange = (_, newValue, previousValue) => {
    let keywords
    dispatch(({ getState }) => {
      const state = getState()
      keywords = state.getIn(['common', 'entities', 'keywords'])
    })
    const newKeywordValues = newValue.get(language).filter(v => {
      const keyword = keywords.get(v)
      const entity = keyword.get('entity')
      return keyword && typeof entity === 'boolean' && !entity
    })
    change(formName, 'newKeywords', newKeywords.set(language, OrderedSet(newKeywordValues)))
  }

  const onChangeObject = object => {
    if (object) {
      const oldKeywords = object.filter(k => k.entity).map(k => {
        if (typeof k.entity === 'boolean') {
          return { id: k.value, name: k.label, entity: k.entity }
        } else {
          k.entity.entity = true
          return k.entity
        }
      })
      oldKeywords && oldKeywords.length > 0 &&
        dispatch({ type: 'KEYWORDS', response: normalize(oldKeywords, TypeSchemas.KEYWORD_ARRAY) })
      const newKeywords = object.filter(k => !k.entity).map(k => ({ id: k.value, name: k.label, entity: false }))
      newKeywords && newKeywords.length > 0 &&
          dispatch({ type: 'KEYWORDS', response: normalize(newKeywords, TypeSchemas.KEYWORD_ARRAY) })
    }
  }

  const getQueryData = query => {
    return { name: query, localizationCode: language }
  }

  const getLocalizedKeywordIds = (inputValue) => {   
    if (!inputValue || !Array.isArray(rest.localizedKeywords)) {
      return []
    }

    return rest.localizedKeywords.map(x => x.id)
  }

  const renderReadOnly = () => {
    return(
      <div className='form-row'>
        <Field
          name='keywords'
          label={formatMessage(messages.title)}
          tooltip={formatMessage(messages.tooltip)}
          component={RenderMultiSelectAsyncCreatable}
          getQueryData={getQueryData}
          {...rest}
          formatResponse={formatResponse}          
          url={'service/SearchKeywords'}
          options={rest.allOptions}
          getCustomValue={getLocalizedKeywordIds}
          />
      </div>
    )
  }
  
  const renderEditMode = () => {
    return(
      <div className='form-row'>
        <Field
          name='keywords'
          label={formatMessage(messages.title)}
          tooltip={formatMessage(messages.tooltip)}
          component={RenderMultiSelectAsyncCreatable}
          getQueryData={getQueryData}
          onChange={handleOnChange}
          {...rest}
          onChangeObject={onChangeObject}
          formatResponse={formatResponse}
          url={'service/SearchKeywords'}
          />

        {hasGDKeywords && <TreeListDisplay
          name='gdKeywords'
          label={formatMessage(messages.gdListLabel || messages.label)}
          tooltip={formatMessage(messages.gdListTooltip) || null}
          selector={getGDKeywordLabelValues}
          invalidItemTooltip={formatMessage(CommonMessages.invalidItemTooltip)}
          isReadOnly={false}
        />}
      </div>
    )
  }

  if (rest.isReadOnly) {
    return (renderReadOnly())
  }

  return (renderEditMode())

}
Keywords.propTypes = {
  formName: PropTypes.string.isRequired,
  validate: PropTypes.func,
  change: PropTypes.func.isRequired,
  options: ImmutablePropTypes.map.isRequired,
  allOptions : ImmutablePropTypes.map.isRequired,
  intl: intlShape.isRequired,
  inlineLabel: PropTypes.bool,
  hasGDKeywords: PropTypes.bool.isRequired
}

export default compose(
  injectIntl,
  injectFormName,
  asDisableable,
  connect(
    (state, ownProps) => ({
      options: getKeywordsJS(state),
      allOptions: getAllKeywordsJS(state, ownProps),
      newKeywords: formValueSelector(ownProps.formName)(state, 'newKeywords') || Map(),
      hasGDKeywords: hasGDKeywords(state, ownProps),
      localizedKeywords: getLocalizedKeywords(state, ownProps)
    }
    ), { change }
  ),
  asComparable({ DisplayRender: RenderMultiSelectDisplay }),
  asLocalizable,
  injectSelectPlaceholder({ placeholder: messages.placeholder })
)(Keywords)
