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
import React from 'react';
import { search, Search, SearchColumns } from 'reactabular';
import EasyTable from 'reactabular-easy';
import VisibilityToggles from 'reactabular-visibility-toggles';
import HTML5Backend from 'react-dnd-html5-backend';
import { DragDropContext } from 'react-dnd';
import cloneDeep from 'lodash/cloneDeep';
import findIndex from 'lodash/findIndex'; 
import { injectIntl, intlShape } from 'react-intl';

class PTVTable2 extends React.Component {
  constructor(props) {
    super(props);
        
    this.table = null; 
    this.onToggleColumn = this.onToggleColumn.bind(this);
    this.onSelectRow = this.onSelectRow.bind(this);
    this.onSort = this.onSort.bind(this);
    this.onToggleShowingChildren = this.onToggleShowingChildren.bind(this);
    
  }
     
  render() {
    const sortingColumns = {}
    const query = {}
    const { tableRows, tableColumns } = this.props;
    //const visibleColumns = this.state.columns.filter(column => column.visible);
 
    return (
      <div>
        <EasyTable
          ref={table => {
            this.table = table
          }}
          rows={tableRows}
          rowKey="id"
          sortingColumns={sortingColumns}
          tableWidth={1000}
          tableHeight={400}
          columns={tableColumns}
          query={query}
          classNames={{
            table: {
              wrapper: 'pure-table pure-table-striped'
            }
          }}

          toggleChildrenProps={{ className: 'toggle-children' }} 
          idField="id"
          parentField="parent"           
          onDragColumn={this.onDragColumn}
          onMoveColumns={this.onMoveColumns}
          onSelectRow={this.onSelectRow}
          onSort={this.onSort}
          onRow={this.onRow}
          onToggleShowingChildren={this.onToggleShowingChildren}
        />
      </div>
    );
  }
    
  onToggleShowingChildren(rowIndex) {
    console.log('onToggleShowingChildren', rowIndex);
  } 
  onToggleColumn(columnIndex) {
    console.log('onToggleColumn', columnIndex);
  }  
  onDragColumn(width, { columnIndex }) {
    console.log('onDragColumn', width);
  }
  onMoveColumns(columns) {
    console.log('onMoveColumns', columns);
  }
  onSelectRow({ selectedRowId, selectedRow }) {
    console.log('onSelectRow', selectedRowId, selectedRow);
  }
  onSort(sortingColumns) {
    console.log('onSort', sortingColumns);    
  }
  
  onRow(row, { rowIndex }) {
    return {
      className: rowIndex % 2 ? 'odd-row' : 'even-row'
    };
  }
      
}
 
export default injectIntl(PTVTable2);
