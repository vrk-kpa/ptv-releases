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
import { FieldArray } from 'redux-form/immutable'
import RenderServiceCollections from './RenderServiceCollections'
import { compose } from 'redux'
import { defineMessages } from 'util/react-intl'
import asContainer from 'util/redux-form/HOC/asContainer'

const messages = defineMessages({
  tooltip:{
    id: 'Routes.Service.Components.ServiceCollections.Tooltip',
    defaultMessage : 'Tooltip'
  },
  title: {
    id : 'Routes.Service.Components.ServiceCollections.Title',
    defaultMessage: 'Liitokset palvelukokonaisuuksiin'
  }
})

const ServiceCollectionsArray = props => (
  <FieldArray
    name='serviceCollections'
    component={RenderServiceCollections}
    {...props}
  />
)

export const ServiceConnections = compose(
  asContainer({
    title: messages.title,
    tooltip: messages.tooltip,
    dataPaths: 'serviceCollections'
  })
)(props => <ServiceCollectionsArray {...props} />)

export default ServiceConnections
