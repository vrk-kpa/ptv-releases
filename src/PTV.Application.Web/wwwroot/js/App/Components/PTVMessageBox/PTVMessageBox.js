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
import React, {PropTypes, Component} from 'react';
import PTVLabel from '../PTVLabel';
import { PTVTextArea } from '../PTVTextArea';
import PTVOverlay from '../PTVOverlay';
import PTVIcon from '../PTVIcon';
import { ButtonDelete } from '../../Containers/Common/Buttons';
import '../Styles/PTVCommon.scss';
import styles from './styles.scss';

export class PTVMessageBox extends Component {

	constructor(props) {
		super(props);
		this.state = {stack:"", show:this.props.messages.length>0, isVisible:false};
		this.getAllLabels = this.getAllLabels.bind(this);
		this.closeClick = this.closeClick.bind(this);
	}

	getAllLabels(messages) {
		return (messages.map((message, index) => {
			const messageObject = (typeof message === 'string') ? { message } : message;
			return <div key={index}>
          <div>
						<PTVLabel key={index}>
								{messageObject.message}
								{	messageObject.info && (typeof messageObject.info === 'string')?
									<PTVIcon
										className = { "information-error-icon color-citizen" }
										name = { "icon-info" }
										onClick={() => {
											this.setState({stack: messageObject.info});
											this.setState({isVisible: true});
										}}
									/>
								: null }
						</PTVLabel>
					</div>				
        </div>
		}))
	}
	
	closeClick = () => {
		if (this.props.onClose){
			this.props.onClose(this.props.referenceId)
		}
		this.setState({show: false});
	}
	

	render() {
		return (
			<div className={"box msg-box " + (this.props.isValid ? "success-box" : "error-box") + (this.state.show?"":" closed") + " clearfix"}>
				
				<ButtonDelete className="close" onClick={this.closeClick} withIcon={true} iconName="icon-times-circle" /> 
				
				<div className="flex-container">
					<div className="message-box-icon">
						<PTVIcon 
							width={50}
							height={50}
							className = { this.props.isValid ? "color-leaf" : "color-chili-crimson" }
							name = { this.props.isValid ? 'icon-check' : 'icon-exclamation' }
						/>
					</div>
					
					<div>
						{this.getAllLabels(this.props.messages)}
					</div>
				</div>

				<PTVOverlay isVisible={ this.state.isVisible } title="Stack trace" onCloseClicked={ ()=>{ this.setState({isVisible: false}) } }>
					<PTVTextArea
						minRows={20}
						className="info-text"
						name="StackTrace"
						value={this.state.stack} />
				</PTVOverlay>				 				
			</div>
			)
		}
	}

PTVMessageBox.propTypes = {
	messages: PropTypes.any.isRequired,
	referenceId: PropTypes.any,
	onClose: PropTypes.func
}

export default PTVMessageBox;
