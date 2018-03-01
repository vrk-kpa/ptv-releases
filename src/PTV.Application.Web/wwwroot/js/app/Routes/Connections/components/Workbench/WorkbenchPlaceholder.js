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
import { compose } from 'redux'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import styles from './styles.scss'

const messages = defineMessages({
  guideTitle: {
    id: 'Routes.Connections.Components.Connections.GuideTitle.Text',
    defaultMessage: 'Työpöydällä voit käsitellä palveluiden ja asiointikanavien välisiä liitoksia.'
  },
  guideStep1: {
    id: 'Routes.Connections.Components.Connections.GuideStep1.Text',
    defaultMessage: 'Valitse palveluita tai asiointikanavia työpöydälle pohjaksi.'
  },
  guideStep2: {
    id: 'Routes.Connections.Components.Connections.GuideStep2.Text',
    defaultMessage: 'Työpöydällä voit käsitellä valitsemiesi sisältötyyppien nykyisiä liitoksia.'
  },
  guideStep3_1: {
    id: 'Routes.Connections.Components.Connections.GuideStep3_1.Text',
    defaultMessage: 'Uusien liitosten tekeminen.'
  },
  guideStep3_2: {
    id: 'Routes.Connections.Components.Connections.GuideStep3_2.Text',
    defaultMessage: 'jos työpöydällä on palveluita, avaa asiointikanavien välilehti, hae sieltä asiointikanavia ja liitä ne työpöydällä oleviin palveluihin.'
  },
  guideStep3_3: {
    id: 'Routes.Connections.Components.Connections.GuideStep3_3.Text',
    defaultMessage: 'Mikäli työpöydällä on asiointikanavia, tee palveluiden välilehdellä haku ja liitä palveluita työpöydällä oleviin asiointikanaviin.'
  },
  guideOptionA: {
    id: 'Routes.Connections.Components.Connections.GuideOptionA.Text',
    defaultMessage: '(Vaihtoehto A)'
  },
  guideOptionB: {
    id: 'Routes.Connections.Components.Connections.GuideOptionB.Text',
    defaultMessage: '(Vaihtoehto B)'
  }
})

const ConnectionsGuideText = ({
  intl: { formatMessage }
}) => {
  return (
    <div className={styles.info}>
      <span>{formatMessage(messages.guideTitle)}</span>
      <ol>
        <li>{formatMessage(messages.guideStep1)}</li>
        <li>{formatMessage(messages.guideStep2)}</li>
        <li>
          {formatMessage(messages.guideStep3_1)}
          <strong>{formatMessage(messages.guideOptionA)}</strong>
          {formatMessage(messages.guideStep3_2)}
          <strong>{formatMessage(messages.guideOptionB)}</strong>
          {formatMessage(messages.guideStep3_3)}
        </li>
      </ol>
    </div>
  )
}
ConnectionsGuideText.propTypes = {
  intl: intlShape
}

export default compose(
  injectIntl
)(ConnectionsGuideText)
