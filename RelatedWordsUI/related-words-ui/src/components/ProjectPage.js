import React, {Component, Fragment} from 'react';
import { connect } from "react-redux";
import {withRouter} from 'react-router-dom';
import { uuid } from 'uuidv4';
import _ from 'lodash';
import {getProject, loadProjectComponents} from '../actions/projects';
import ProjectLoadingStatus from '../models/ProjectLoadingStatus';
import projectLoadingStatusEnum from '../models/helpers/projectLoadingStatusEnum';

class ProjectPage extends Component {

  componentDidMount() {
    this.handleProjectLoading();
  }

  componentDidUpdate() {
    this.handleProjectLoading();
  }

  handleProjectLoading(){
    let loadingStatus;
    try {
      loadingStatus = this.props.projectLoadingStatus.loadingStatus;
    } catch (e) {
      this.fetchProject()
      return
    }

    switch(loadingStatus) {
      case projectLoadingStatusEnum['NOT_STARTED']:
        this.fetchProject();
        break;
      case projectLoadingStatusEnum['FETCHING_PROJECT']:
        break;
      case projectLoadingStatusEnum['FETCHED_PROJECT']:
        this.fetchProjectComponents();
        break;
      case projectLoadingStatusEnum['FETCHING_PROJECT_COMPONENTS']:
        break;
      case projectLoadingStatusEnum['FINISHED']:
        break;
      case projectLoadingStatusEnum['FAILED']:
        break;
      default:
        this.fetchProject();
        break;
    }
  }

  fetchProject() {
    this.props.dispatch(getProject(
      this.props.user.token, 
      parseInt(this.props.match.params.projectId),
      {
        startingStatus: new ProjectLoadingStatus({
          projectId: parseInt(this.props.match.params.projectId),
          loadingStatus: projectLoadingStatusEnum['FETCHING_PROJECT']
        }),
        loadingStatus: new ProjectLoadingStatus({
          projectId: parseInt(this.props.match.params.projectId),
          loadingStatus: projectLoadingStatusEnum['FETCHED_PROJECT']
        }),
        loadingStatusError: new ProjectLoadingStatus({
          projectId: parseInt(this.props.match.params.projectId),
          loadingStatus: projectLoadingStatusEnum['FAILED']          
        })
      }));
  }

  fetchProjectComponents() {
    this.props.dispatch(loadProjectComponents(
      this.props.user.token, this.props.project));
  }

    
  render() {
    return (
        <div className="project-page">
          This is a projecpage
        </div>
      ) 
  }
}

function mapStateToProps(state, ownProps) { 
  const projectId = parseInt(ownProps.match.params.projectId);
  const project = state.projects.projects.find(
    p => p.projectId === projectId
  );
  const projectLoadingStatus = state.projects.projectLoadingStatuses.find(
    p => p.projectId === projectId
  );
  return {
    project: project,
    projectLoadingStatus: projectLoadingStatus,
    projectWords: state.projectComponents.projectWords[project.projectId],
    projectPages: state.projectComponents.projectPages[project.projectId],
    user: state.user.user                         
  };
}

export default withRouter(connect(mapStateToProps)(ProjectPage));