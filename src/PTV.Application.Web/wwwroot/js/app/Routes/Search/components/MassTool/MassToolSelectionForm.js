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
import React, { Component, Fragment } from 'react'
import {
  reduxForm
} from 'redux-form/immutable'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { MassToolType } from 'util/redux-form/fields'
import MassToolConfirm from './MassToolConfirm'
import {
  massToolTypes,
  formTypesEnum
} from 'enums'
import styles from './styles.scss'
import withMassToolActive from './withMassToolActive'
import Spacer from 'appComponents/Spacer'

class MassToolSelectionForm extends Component {
  render () {
    return (
      <Fragment>
        <Spacer marginSize='m0' />
        <div className={styles.massToolForm}>
          <div className='row'>
            <div className='col-lg-6'>
              <div className={styles.massToolType}>
                <MassToolType small />
              </div>
            </div>
            <div className='col-lg-18'>
              <MassToolConfirm />
            </div>
          </div>
        </div>
      </Fragment>
    )
  }
}

const onSubmit = () => console.log('selection form')

export default compose(
  withMassToolActive,
  connect(state => {
    const initialValues = {
      type: massToolTypes.PUBLISH
    }
    return {
      initialValues
    }
  }),
  reduxForm({
    form: formTypesEnum.MASSTOOLSELECTIONFORM,
    destroyOnUnmount : false,
    onSubmit
  })
)(MassToolSelectionForm)
