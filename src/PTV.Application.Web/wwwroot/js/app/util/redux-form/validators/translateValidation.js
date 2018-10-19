import { noop, isArray } from 'lodash'

const translateValidation = (validate, formatMessage, label, getValue) => {
  if (!label) {
    throw new Error('No label provided for translateValidation')
  }
  if (!validate) {
    return noop
  } else if (!isArray(validate)) {
    validate = [validate]
  }
  return validate.map(validate => validate(formatMessage, label, getValue))
}

export default translateValidation
