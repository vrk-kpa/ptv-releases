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
import { callApiDirect } from 'Middleware/Api'
import { RenderAsyncSelect, RenderSelectDisplay } from 'util/redux-form/renders'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import { Field } from 'redux-form/immutable'
import { translateValidation } from 'util/redux-form/validators'
import asComparable from 'util/redux-form/HOC/asComparable'
import { connect } from 'react-redux'
import { getOntologyTermOptionsJS } from './selectors'
import { normalize } from 'normalizr'
import { FintoSchemas } from 'schemas'

const messages = defineMessages({
  label: {
    id: 'FrontPage.OntologyTerms.Title',
    defaultMessage: 'Asiasana'
  },
  placeholder: {
    id: 'FrontPage.OntologyTerms.Placeholder',
    defaultMessage: 'Hae ontologiakäsitteellä'
  },
  tooltip: {
    id: 'FrontPage.OntologyTerms.Tooltip',
    defaultMessage: 'Voit hakea palveluita myös ontologiakäsitteen mukaan. Palvelutietovarannossa palveluiden asiasisältö kuvataan ontologiakäsitteillä, jotka ovat tietokoneluettavia, mahdollisimman yksiselitteisiä käsitteitä. Kirjoita kenttään palvelun asiasisältöön liittyvä sana ja valitse ennakoivan tekstinsyötön tarjoamista vaihtoehdoista ontologiakäsite.'
  }
})

const OntologyTerm = ({
  intl: { formatMessage },
  validate,
  dispatch,
  ...rest
}) => {
  const getOntologyTerms = async query => {
    if (!query || query.length < 3) return
    try {
      const json = await callApiDirect('service/GetFilteredList', {
        searchValue: query,
        treeType: 'OntologyTerm'
      })
      return {
        options: json.result && json.result.map((oWord) => ({
          label: oWord.name,
          value: oWord.id,
          entity: oWord
        })) || [],
        complete: true
      }
    } catch (err) {
      console.log(`Error occured when searching for ontology term: ${err}`)
    }
  }
  const onChangeObject = (object) => {
    object && dispatch({ type: 'ONTOLOGYTERMS', response: normalize(object.entity, FintoSchemas.ONTOLOGY_TERM) })
  }

  return (
    <Field
      label={formatMessage(messages.label)}
      tooltip={formatMessage(messages.tooltip)}
      placeholder={formatMessage(messages.placeholder)}
      component={RenderAsyncSelect}
      loadOptions={getOntologyTerms}
      onChangeObject={onChangeObject}
      searchable
      name='ontologyTerms'
      //validate={translateValidation(validate, formatMessage, messages.label)}
      {...rest}
    />
  )
}
OntologyTerm.propTypes = {
  intl: intlShape,
  validate: PropTypes.func
}

export default compose(
  injectIntl,
  connect(state => ({
    options: getOntologyTermOptionsJS(state)
  })),
  asComparable({ DisplayRender: RenderSelectDisplay })
)(OntologyTerm)
