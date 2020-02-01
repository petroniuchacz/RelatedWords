import React, {Component} from 'react';
import { connect } from 'react-redux';
import { Switch, Route, Redirect } from 'react-router-dom';
import { fetchLoginStatus } from './actions/user';
import AppPage from './components/AppPage';
import LoginPage from './components/LoginPage';
import PrivateRoute from './components/PrivateRoute';
import Notifications from './components/Notifications';

class App extends Component {

  componentDidMount() {
    this.props.dispatch(fetchLoginStatus());
  }

  render() {
    return(
      <div className="app">
        <Notifications/>
        <Switch>
          <PrivateRoute path="/app" component={AppPage}/>
          <Route path="/login" component={LoginPage}/>
        </Switch>
        {this.props.loginStatus ? <Redirect to='/app'/> : <Redirect to='/login'/>}
      </div>
    );
  }
}

function mapStateToProps(state) {               
  return {
    loginStatus: state.user.loginStatus,
    user: state.user.user                         
  }
}

export default connect(mapStateToProps)(App);
