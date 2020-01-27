import React from 'react';
import { connect } from 'react-redux';
import NavBar from './NavBar';

const AppPage = props => {
  return (
    <div className="appPage">
      <NavBar/>
    </div>
  )
}

function mapStateToProps(state) {               
  return {
    user: state.user.user,
  }
}

export default connect(mapStateToProps)(AppPage);