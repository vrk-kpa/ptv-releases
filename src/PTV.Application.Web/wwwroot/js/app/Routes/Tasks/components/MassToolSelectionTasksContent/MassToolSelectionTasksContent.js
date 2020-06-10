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
import React, { Fragment } from 'react'
import { compose } from 'redux'
import MassToolConfirm from 'Routes/Search/components/MassTool/MassToolConfirm'
import TooltipLabel from 'appComponents/TooltipLabel'
import Paragraph from 'appComponents/Paragraph'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import styles from './styles.scss'

const messages = defineMessages({
  massToolTasksTitle: {
    id: 'MassToolSelection.Tasks.Title',
    defaultMessage: 'Massajulkaisu'
  },
  massToolTasksTooltip: {
    id: 'MassToolSelection.Tasks.Tooltip',
    defaultMessage: 'Mass Tool selection at tasks page tooltip'
  },
  massToolTasksDescription1: {
    id: 'MassToolSelection.Tasks.Description1',
    defaultMessage: 'Valitse tehtävälistasta sisällöt, jotka haluat käsitellä massana. Valitut sisällöt kerääntyvät Sisältökärryyn, jota painamalla voit tarvittaessa muokata niitä.' // eslint-disable-line
  },
  massToolTasksDescription2: {
    id: 'MassToolSelection.Tasks.Description2',
    defaultMessage: 'Siirry sinisellä painikkeella erilliseen näkymään, jossa hyväksyt sisällöt yksitellen, jonka jälkeen voit julkaista kaikki sisällöt kerralla.' // eslint-disable-line
  }
})

const MassToolSelectionTasksContent = ({ intl: { formatMessage } }) => {
  return (
    <Fragment>
      <TooltipLabel
        labelProps={{ labelText: formatMessage(messages.massToolTasksTitle) }}
        tooltipProps={{ tooltip: formatMessage(messages.massToolTasksTooltip) }}
        componentClass={styles.label}
      />
      <div className='row'>
        <div className='col-lg-12'>
          <Paragraph>
            {formatMessage(messages.massToolTasksDescription1)}
            <br />
            {formatMessage(messages.massToolTasksDescription2)}
          </Paragraph>
        </div>
        <div className='col-lg-12'>
          <MassToolConfirm />
        </div>
      </div>
    </Fragment>
  )
}

MassToolSelectionTasksContent.propTypes = {
  intl: intlShape
}

export default compose(
  injectIntl
)(MassToolSelectionTasksContent)
