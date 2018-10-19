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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import NoDataLabel from 'appComponents/NoDataLabel/NoDataLabel'
import { Label } from 'sema-ui-components'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withPath from 'util/redux-form/HOC/withPath'
import asComparable from 'util/redux-form/HOC/asComparable'
// import { getContentLanguageCode } from 'selectors/selections'
import { List, Iterable } from 'immutable'
import { injectIntl, intlShape } from 'util/react-intl'
import AttachmentTitle from 'appComponents/AttachmentCollection/AttachmentTitle'
import PostalAddressTitle from 'appComponents/PostalAddressCollection/PostalAddressTitle'
import VisitingAddressTitle from 'appComponents/VisitingAddressCollection/VisitingAddressTitle'
import ConnectionTypeTitle from 'util/redux-form/fields/ConnectionType/ConnectionTypeTitle'
import AreaInformationTitle from 'util/redux-form/sections/AreaInformation/AreaInformationTitle'
import EmailTitle from 'util/redux-form/sections/EmailCollection/EmailTitle'
import FaxNumberTitle from 'Routes/Channels/routes/ServiceLocation/components/FaxNumberCollection/FaxNumberTitle'
import PhoneNumberTitle from 'util/redux-form/sections/PhoneNumberCollection/PhoneNumberTitle'
import ServiceCollectionTitle from 'Routes/Service/components/ServiceCollections/ServiceCollectionTitle'
import ServiceVoucherTitle from 'util/redux-form/sections/ServiceVoucher/ServiceVoucherTitle'
import ElectronicInvoicingAddressTitle from 'util/redux-form/sections/ElectronicInvoicingAddressCollection/ElectronicInvoicingAddressTitle'
import commonMessages from 'util/redux-form/messages'
import styles from './styles.scss'
import { getLanguageCode } from './selectors'

const TitleComponent = compose(
  // asComparable({ DisplayRender: TitleComponent })
)(({ dataPath, ...rest }) => {
  let Component = null
  switch (dataPath) {
    case 'attachments':
    case 'webPages':
      Component = AttachmentTitle
      break
    case 'areaInformation':
      Component = AreaInformationTitle
      break
    case 'electronicInvoicingAddresses':
      Component = ElectronicInvoicingAddressTitle
      break
    case 'emails':
      Component = EmailTitle
      break
    case 'faxNumbers':
      Component = FaxNumberTitle
      break
    case 'phoneNumbers':
      Component = PhoneNumberTitle
      break
    case 'connectionType':
      Component = ConnectionTypeTitle
      break
    case 'postalAddresses':
      Component = PostalAddressTitle
      break
    case 'serviceCollections':
      Component = ServiceCollectionTitle
      break
    case 'visitingAddresses':
      Component = VisitingAddressTitle
      break
    default:
      Component = null
      break
  }
  return Component && <Component {...rest} /> || null
})

class PreviewData extends Component {
  handleDataPath = (data, dataPath, lang, withLabel) => {
    const labelText = commonMessages[dataPath] && this.props.intl.formatMessage(commonMessages[dataPath]) || dataPath
    let dataSet = List()
    switch (dataPath) {
      case 'attachments':
      case 'emails':
      case 'faxNumbers':
      case 'phoneNumbers':
      case 'webPages':
        dataSet = Iterable.isIterable(data) && data.has(lang) && data.get(lang) || dataSet
        return (
          <div key={`${dataPath}_${lang}`} className={styles.previewValue}>
            {withLabel && <Label labelText={labelText} />}
            {dataSet.size === 0 &&
              <NoDataLabel /> ||
              dataSet.map((item, index, items) => (
                <TitleComponent
                  dataPath={dataPath}
                  items={items}
                  index={index}
                  key={index} />
              ))
            }
          </div>
        )
      case 'electronicInvoicingAddresses':
      case 'postalAddresses':
      case 'visitingAddresses':
        dataSet = Iterable.isIterable(data) && data || dataSet
        return (
          <div key={dataPath} className={styles.previewValue}>
            {withLabel && <Label labelText={labelText} />}
            {dataSet.size === 0 &&
              <NoDataLabel /> ||
              dataSet.map((item, index, items) => (
                <TitleComponent
                  dataPath={dataPath}
                  items={items}
                  index={index}
                  key={index}
                  compare={this.props.compare}
                  formatted />
              ))
            }
          </div>
        )
      case 'areaInformation':
      case 'connectionType':
      case 'serviceCollections':
        return <TitleComponent
          dataPath={dataPath}
          data={data}
          className={styles.previewValue} />
      case 'serviceVouchers':
        const voucherSetWrap = Iterable.isIterable(data) && data.get('serviceVouchers') || Map()
        const inUse = Iterable.isIterable(data) && data.get('serviceVoucherInUse') || false
        const voucherSet = Iterable.isIterable(voucherSetWrap) && voucherSetWrap.has(lang) && voucherSetWrap.get(lang) || List()
        return (
          <div key={`${dataPath}_${lang}`} className={styles.previewValue}>
            {withLabel && <Label labelText={labelText} />}
            {
              !inUse && <NoDataLabel /> ||
              <ServiceVoucherTitle
                dataPath={dataPath}
                data={voucherSet}
                inUse={inUse} />
            }
          </div>
        )
      default:
        return <div className={styles.previewValue}>Not defined yet</div>
    }
  }

  getIsEmpty = (data, dataPath, lang) => {
    const dataSet = data.get(dataPath) || List()
    const dataSetLoc = Iterable.isIterable(dataSet) && dataSet.has(lang) && dataSet.get(lang) || dataSet
    return dataSetLoc.size === 0
  }

  render () {
    const {
      data,
      dataPaths,
      lang
    } = this.props

    // handle nested datapaths
    if (Array.isArray(dataPaths)) {
      const isEmpty = dataPaths.every(dataPath => {
        return this.getIsEmpty(data, dataPath, lang)
      })
      return isEmpty && <div className={styles.previewValue}><NoDataLabel /></div> ||
        dataPaths.map(dataPath => {
          return this.handleDataPath(data.get(dataPath), dataPath, lang, true)
        })
    }
    // handle individual datapaths
    return this.handleDataPath(data, dataPaths, lang)
  }
}

PreviewData.propTypes = {
  data: PropTypes.any,
  lang: PropTypes.string,
  dataPaths: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.array
  ]),
  intl: intlShape
}

export default compose(
  injectIntl,
  injectFormName,
  withPath,
  asComparable({ DisplayRender: TitleComponent }),
  connect((state, { formName, isLocalized, compare }) => {
    return {
      // // lang: getContentLanguageCode(state)
      lang: getLanguageCode(state, { compare })
    }
  })
)(PreviewData)
