import React from 'react'
import { AddressSwitch } from 'util/redux-form/sections'
import { compose } from 'redux'
import asContainer from 'util/redux-form/HOC/asContainer'
import asLocalizableSection from 'util/redux-form/HOC/asLocalizableSection'
import asCollection from 'util/redux-form/HOC/asCollection'
import asSection from 'util/redux-form/HOC/asSection'

const AddressCollection = compose(
  asContainer({ title: 'Address' }),
  asLocalizableSection('addresses'),
  asCollection({ name: 'address', pluralName: 'addresses' }),
  asSection()
)(props => <AddressSwitch {...props} />)

export default AddressCollection
