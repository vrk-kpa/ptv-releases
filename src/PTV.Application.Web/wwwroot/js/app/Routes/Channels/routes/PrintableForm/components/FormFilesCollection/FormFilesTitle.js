import React from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectIntl } from 'util/react-intl'
import NoDataLabel from 'appComponents/NoDataLabel'
import { getFormFilesTitle } from './selectors'

const FormFilesTitle = ({
  className,
  formFilesTitle,
  ...rest
}) => {
  return (
    formFilesTitle && <div className={className}>
      <span>{formFilesTitle}</span>
    </div> || <NoDataLabel />
  )
}

FormFilesTitle.propTypes = {
  className: PropTypes.string,
  formFilesTitle: PropTypes.string
}

export default compose(
  injectIntl,
  connect((state, ownProps) => ({
    formFilesTitle: getFormFilesTitle(state, ownProps)
  }))
)(FormFilesTitle)
