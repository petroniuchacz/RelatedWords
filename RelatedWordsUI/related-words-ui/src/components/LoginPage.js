import React, { Component } from 'react';
import { connect } from 'react-redux';
import {userLogin} from '../actions/user'
import {newNotification} from '../actions/notifications'

class LoginPage extends Component {
  constructor(props) {
    super(props);
    this.state = {
      email: "",
      password: "",
      keepSignedIn: false
    }
  }

  onEmailChange = (e) => {               
    this.setState({ email: e.target.value });
  }

  onPasswordChange = (e) => {               
    this.setState({ password: e.target.value });
  }

  onKeepSignedInChange = (e) => {               
    this.setState({ keepSignedIn: e.target.value });
  }

  onUserLogin = (e) => {
    e.preventDefault();
    const email = this.state.email;
    const password = this.state.password;
    if(!!email.trim() && !!password.trim()) {
      this.props.dispatch(
        userLogin(email, password, this.state.keepSignedIn)
        );
    } else {
      this.props.dispatch(
        newNotification("error", "Please fill in email and password fields.")
      );
    }
  }

  render() {
    return (
      <div className="login-page">
        <form className="login-form" onSubmit={this.onUserLogin}>
          <input 
            className="login-input"
            type="email"
            value={this.state.email}
            onChange={this.onEmailChange}
            placeholder="e-mail"
            />
          <input
            className="login-input"
            type="password"
            value={this.state.password}
            onChange={this.onPasswordChange}
            placeholder="password"
            />
            <p>
            <input
              className="login-checkbox"
              type="checkbox"
              value={this.state.keepSignedIn}
              onChange={this.onKeepSignedInChange}
            />
            Keep me signed in
            </p>
          <button
            className="button"
            type="submit">
              Sign in
          </button>
        </form>
      </div>
    )
  }
}

export default connect()(LoginPage);