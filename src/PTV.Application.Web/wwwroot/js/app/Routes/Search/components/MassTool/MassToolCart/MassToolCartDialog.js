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
import React from 'react'
import PropTypes from 'prop-types'
import ModalDialog from 'appComponents/ModalDialog'
import { ModalContent } from 'sema-ui-components'
import styles from '../styles.scss'
import MassToolDataTable from '../MassToolDataTable'
import MassToolCartTitle from './MassToolCartTitle'

const MassToolCartDialog = ({
  title,
  description
}) => {
  return (
    <ModalDialog
      name='massToolCartDialog'
      style={{ content: { minWidth: '48.75rem', width: '48.75rem' } }}
    >
      <h1 className={styles.modalTitle}>
        {title || <MassToolCartTitle />}
      </h1>
      {description && <p className={styles.modalDescription}>{description}</p>}
      <ModalContent>
        <MassToolDataTable />
      </ModalContent>
    </ModalDialog>
  )
}

MassToolCartDialog.propTypes = {
  title: PropTypes.node,
  description: PropTypes.node
}

export default MassToolCartDialog
