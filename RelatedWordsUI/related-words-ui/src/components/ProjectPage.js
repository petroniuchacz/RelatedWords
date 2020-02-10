import React, {Component, Fragment} from 'react';
import { connect } from "react-redux";
import {withRouter} from 'react-router-dom';
import { uuid } from 'uuidv4';
import _ from 'lodash';

class ProjectPage {

  componentDidMount() {
 
  }

    
  render() {
    return (
        <div className="project-page">
          This is a projecpage
        </div>
      ) 
  }
}

function mapStateToProps(state) {               
  return {
    projects: state.projects.projects,
    user: state.user.user                         
  }
}

export default withRouter(connect(mapStateToProps)(ProjectPage));