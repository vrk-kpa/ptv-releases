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
import { FieldArray } from 'redux-form/immutable'
import { compose } from 'redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { injectIntl, defineMessages } from 'util/react-intl'
import AccordionCollectionItem from './AccordionCollectionItem'
import CollectionItem from './CollectionItem'
import DefaultRemoveButton from './DefaultRemoveButton'
import DefaultAddButton from './DefaultAddButton'
import { isUndefined } from 'lodash'
import NoDataLabel from 'appComponents/NoDataLabel'

export const messages = defineMessages({
  buttonOk: {
    id: 'Containers.Services.NameOverwriteDialog.Accept',
    defaultMessage: 'Kyllä'
  },
  buttonCancel: {
    id: 'Buttons.Cancel.Title',
    defaultMessage: 'Peruuta'
  },
  removalPlaceholder: {
    id: 'Common.HOC.AsCollection.RemovalPlaceholder',
    defaultMessage: 'Tämän elementin poistaminen vaikuttaa muihin kieliversioihin, sillä toisessa kieliversiossa on sisältöä.'
  }
})

const asCollection = ({
  name,
  pluralName = name + 's',
  simple,
  stacked,
  dragAndDrop,
  shouldHideControls,
  addBtnTitle,
  addNewBtnTitle,
  RemoveButton = DefaultRemoveButton,
  AddButton = DefaultAddButton,
  Title = NoDataLabel
}) => WrappedComponent => {
  const InnerComponent = props => {
    const component = dragAndDrop && AccordionCollectionItem || CollectionItem
    return (
      <FieldArray
        {...props}
        name={props.name || pluralName}
        component={component}
        shouldHideControls={shouldHideControls}
        simple={simple}
        stacked={stacked}
        dragAndDrop={dragAndDrop}
        nested={props.nested}
        WrappedComponent={WrappedComponent}
        addBtnTitle={addBtnTitle}
        addNewBtnTitle={addNewBtnTitle}
        collectionName={props.name || pluralName}
        RemoveButton={RemoveButton}
        AddButton={AddButton}
        Title={Title}
      />
    )
  }

  InnerComponent.propTypes = {
    name: PropTypes.string,
    nested: PropTypes.bool
  }

  return compose(
    injectIntl,
    injectFormName
  )(InnerComponent)
}

export default asCollection
