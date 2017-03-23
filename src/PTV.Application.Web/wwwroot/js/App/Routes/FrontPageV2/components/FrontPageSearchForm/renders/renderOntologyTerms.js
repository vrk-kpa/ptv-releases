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
import { Select } from 'sema-ui-components'
import { callApiDirect } from 'Middleware/Api'
import { injectIntl, intlShape, defineMessages } from 'react-intl'
import { connect } from 'react-redux'
import InfoBox from 'appComponents/InfoBox'

const messages = defineMessages({
  title: {
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

const renderOntologyTerms = ({
  input,
  intl: { formatMessage },
  inlineLabel,
  ...rest
}) => {
  const getOntologyTerms = async query => {
    if (!query || query.length < 3) return
    try {
      const json = await callApiDirect('service/GetFilteredList', {
        resultType: 'List',
        searchValue: query,
        treeType: 'OntologyTerm'
      })
      return {
        options: json.model && json.model.map(({ name, id }) => ({
          label: name,
          value: id
        })) || [],
        complete: true
      }
    } catch (err) {
      console.log(`Error occured when searching for ontology term: ${err}`)
    }
  }
  return (
    <div>
      {!inlineLabel &&
        <div className='form-field tooltip-label'>
          <strong>
            {formatMessage(messages.title)}
            <InfoBox tip={formatMessage(messages.tooltip)} />
          </strong>
        </div>
      }
      <Select.Async
        searchable
        ignoreAccents={false}
        label={inlineLabel && formatMessage(messages.title)}
        placeholder={formatMessage(messages.placeholder)}
        loadOptions={getOntologyTerms}
        {...input}
        onChange={(option) => {
          input.onChange(option ? option.value : null)
        }}
        size='full'
        clearable
        {...rest}
        onBlur={() => {}} // onBlurResetsInput doesn't work react-select #751
        onBlurResetsInput={false}
      />
    </div>
  )
}
renderOntologyTerms.propTypes = {
  input: PropTypes.object.isRequired,
  intl: intlShape.isRequired,
  inlineLabel: PropTypes.bool
}

renderOntologyTerms.defaultProps = {
  inlineLabel: false
}

export default connect()(injectIntl(renderOntologyTerms))
