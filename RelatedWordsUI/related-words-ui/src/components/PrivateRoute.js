// This is used to determine if a user is authenticated and
// if they are allowed to visit the page they navigated to.

// If they are: they proceed to the page
// If not: they are redirected to the login page.
import React, {Component} from 'react';
import { Redirect, Route } from 'react-router-dom';
import { connect } from 'react-redux';

class PrivateRoute extends Component {

  constructor(props){
    super(props);
    const { component, ...rest } = props;
    this.state = {
      rest: {...rest},
      PassedComponent: component
    };
  }

  render() {
    const loginStatus = this.props.loginStatus;
    const PassedComponent = this.state.PassedComponent;
    return (
      <Route
        {...this.state.rest}
        render={props =>
          loginStatus ? (
            <PassedComponent {...props} />
          ) : (
            <Redirect to={{ pathname: '/login', state: { from: props.location } }} />
          )
        }
      />
    )
  }

}

function mapStateToProps(state) {               
  return {
    loginStatus: state.user.loginStatus,                     
  }
}

export default connect(mapStateToProps)(PrivateRoute);