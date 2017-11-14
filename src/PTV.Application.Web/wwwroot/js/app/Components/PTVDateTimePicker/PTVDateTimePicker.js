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
import React,  {Component, PropTypes} from 'react';
import { connect } from 'react-redux'; 
import DateTime from "react-datetime";
import PTVLabel from '../PTVLabel';
import '../styles/react-datetime.scss';
import styles from './styles.scss';
import moment from 'moment';
import { composePTVComponent, ValidatePTVComponent, getRequiredLabel } from '../PTVComponent';
import * as PTVValidatorTypes from '../PTVValidators';

const timeFormat = "HH:mm";
const dateFormat = "DD.MM.YYYY";
const dateTimeFormat = dateFormat + " " + timeFormat;

const DateTimeModes = {
    timeOnly: "timeOnly",
    time: "time", 
    date: "date"
}

const getFormat = (mode) => {
        switch (mode){
            case DateTimeModes.timeOnly:
            case DateTimeModes.time: 
                return timeFormat;
            case DateTimeModes.date:
                return dateFormat;
            default:
                return dateTimeFormat;
        }
    } 
    
const getMode = (mode) => {
        switch (mode){
            case DateTimeModes.timeOnly:
            case DateTimeModes.time: 
                return DateTimeModes.time;
            case DateTimeModes.date:
                return DateTimeModes.date;
            default:
                return DateTimeModes.dateTime;
            }
        }    
    
export function getDateTimeToDisplay(dateTimeTicks, dateTimeMode)
{
    if (dateTimeTicks != null && dateTimeTicks > 0 )
    {    
       return moment(dateTimeTicks).format(getFormat(dateTimeMode));
    }    
    
    return null;
}

class PTVDateTimePicker extends Component {
    
    constructor(props) {
		var base = super(props);
        
        this.state = { 
            //value: moment().valueOf(),
            inputFormat: getFormat(this.props.mode) 
        };
        
	};  

    handleFocus = () => {
        if (this.props.relatedValue && !isNaN(Number(this.props.relatedValue)) && !this.props.value) {
            this.props.onChangeValid(this.props.relatedValue);
        }
    }

    handleChange = (newDate) => {
        //console.log("newDate", newDate);
        if (moment.isMoment(newDate) && this.props.onChangeValid){
            if (this.props.mode === DateTimeModes.timeOnly){
                newDate = newDate.valueOf() - moment().startOf('day').valueOf();
                this.props.onChangeValid(newDate);
            }
            else{
                this.props.onChangeValid(newDate.valueOf());
            }
        }
        else{
            this.props.onChangeValid(newDate);
        }
        
      //  return this.setState({date: newDate});
    }
    
    getDateValue = (date) => {
        if (date != null && Number.isInteger(date) && this.props.mode === DateTimeModes.timeOnly){
            return moment().startOf('day').valueOf() + date;
        }
        return date;
    } 
        
    render() {
        const date = this.getDateValue(this.props.value);
        const resultDate = getDateTimeToDisplay(date, this.props.mode);
        const { readOnly } = this.props;
        const defaultText = resultDate ? {} : { defaultText: "" };
        
        return (
            <div className="ptv-datetime">
                
                {!readOnly ?
                <DateTime
                    value={date ? date : 0}
                    viewMode={this.props.viewMode}
                    dateFormat={this.props.mode === "date" ? dateFormat : false}
                    timeFormat={this.props.mode === "time" || this.props.mode === "timeOnly" ? timeFormat : false}
                    onChange={this.handleChange}
                    onFocus={this.handleFocus}
                    closeOnSelect={this.props.closeOnSelect}
                    timeConstraints={this.props.timeConstraints}
                    inputProps={this.props.inputProps}
                    isValidDate={this.props.isValidDate}
                 />
                :                    
                <PTVLabel>                
                   { resultDate }
                </PTVLabel>               
                }  
                <ValidatePTVComponent {...this.props} valueToValidate={ date } /> 
            </div>
            
        )    
    }
}  

PTVDateTimePicker.propTypes = {
  value:  
    PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.number
  ]),
  onChangeValid: PropTypes.func, 
  onChangeInvalid: PropTypes.func, 
  //showDialog: PropTypes.bool,
  readOnly: PropTypes.bool,
  timeConstraints: PropTypes.object,
  inputProps: PropTypes.object
};

PTVDateTimePicker.defaultProps = {
  // value: ''
//   validators: 
} 

const mapStateToProps = (state, ownProps) => {
  const localValidators = [PTVValidatorTypes.IS_DATETIME]
  const passedValidators = ownProps.validators ? ownProps.validators : []
  return {
    validators: [...localValidators, ...passedValidators]
  }
}

export default connect(mapStateToProps)(composePTVComponent(PTVDateTimePicker))
