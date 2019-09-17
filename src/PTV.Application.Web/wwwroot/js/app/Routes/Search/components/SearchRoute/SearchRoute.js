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
import Search from '../Search'
import { compose } from 'redux'
import { connect } from 'react-redux'
import {
  getDomainSearchRows,
  getSearchDomain,
  getIsFrontPageSearching,
  getDomainSearchPageNumber,
  getDomainSearchPageSize,
  getDomainShowMoreAvailable,
  getDomainSearchTotal
} from '../../selectors'
import { loadMore } from 'Routes/Search/actions'
import { injectIntl } from 'util/react-intl'
import { isDirty } from 'redux-form/immutable'
import withWindowDimensionsContext from 'util/redux-form/HOC/withWindowDimensions/withWindowDimensionsContext'
import { BREAKPOINT_MD, BREAKPOINT_LG, breakPointsEnum } from 'enums'

export default compose(
  withWindowDimensionsContext,
  connect(
    (state, { dimensions }) => {
      const filter = dimensions.windowWidth < BREAKPOINT_LG && dimensions.windowWidth >= BREAKPOINT_MD
        ? breakPointsEnum.MD
        : null
      return {
        rows: getDomainSearchRows(state, { filter }),
        searchDomain: getSearchDomain(state),
        isSearching: getIsFrontPageSearching(state),
        page: getDomainSearchPageNumber(state),
        pageSize: getDomainSearchPageSize(state),
        isShowMoreAvailable: getDomainShowMoreAvailable(state),
        total: getDomainSearchTotal(state),
        dirty: isDirty('frontPageSearch')(state)
      }
    }, {
      loadMore
    }
  ),
  injectIntl
)(Search)
