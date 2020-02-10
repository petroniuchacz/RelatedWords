import React, { Fragment } from 'react';
import { connect } from 'react-redux';
import ProjectPage from './ProjectPage';
import ProjectMenu from './ProjectMenu';
import NavBar from './NavBar';
import {Switch, Route, Redirect, withRouter, useRouteMatch} from 'react-router-dom';

const AppPage = props => {

  const { path, url } = useRouteMatch();
  return (
    <div className="appPage">
      {props.loginStatus ? (
        <Fragment>
          <NavBar currentPath={path}/>
          <div id="central-area">
            <Switch>
              <Route path={`${path}/project/:projecId`} component={ProjectPage}/>
              <Route path={`${path}/projectmenu`} component={ProjectMenu}/>
            </Switch>
          </div>
        </Fragment>
      ): (<Redirect to='/login'/>)}
    </div>
  ) 

}

function mapStateToProps(state) {               
  return {
    user: state.user.user,
  }
}

export default withRouter(connect(mapStateToProps)(AppPage));