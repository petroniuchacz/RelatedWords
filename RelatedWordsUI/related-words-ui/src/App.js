import React, {Component} from 'react';
import { connect } from 'react-redux';
import LoginPage from './components/LoginPage';
import { fetchLoginStatus } from './actions/user';
import AppPage from './components/AppPage';

class App extends Component {

  componentDidMount() {
    this.props.dispatch(fetchLoginStatus());
  }

  render() {
    const loginStatus = this.props.loginStatus;
    return(
      <div className="app">
        {loginStatus && (<AppPage/>)}
        {!loginStatus && loginStatus != null && (<LoginPage/>)}
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
