import React from 'react'
import {
  Attachment
} from 'util/redux-form/sections'
import { compose } from 'redux'
import {
  asCollection,
  asContainer,
  asLocalizableSection
} from 'util/redux-form/HOC'

const AttachmentCollection = compose(
  asContainer({ title: 'Attachments', simple: true }),
  asLocalizableSection('attachments'),
  asCollection({ name: 'attachment' })
)(props => <Attachment {...props} />)

export default AttachmentCollection
