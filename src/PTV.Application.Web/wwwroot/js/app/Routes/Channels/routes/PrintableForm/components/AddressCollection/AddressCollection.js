import React from 'react'
import { AddressSwitch } from 'util/redux-form/sections'
import { compose } from 'redux'
import { asContainer, asLocalizableSection, asCollection } from 'util/redux-form/HOC'

const AddressCollection = compose(
  asContainer({ title: 'Address', simple: true }),
  asLocalizableSection('addresses'),
  asCollection({ name: 'address', pluralName: 'addresses' })
)(props => <AddressSwitch {...props} />)

export default AddressCollection
