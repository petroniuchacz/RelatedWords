import Project from '../models/Project';
import * as api from '../api/projects';
//import {newNotification} from  './notifications';
//import {userSignOut} from './user';
import apiErrorHandling from './helpers/apiErrorHandling'

function createNewProject(project) {
  return {
    type: 'NEW_PROJECT',
    payload: {
      project: project
    }
  }
}

function createProjectActive(project) {
  return {
    type: 'MAKE_ACTIVE',
    payload: {
      project: project
    }
  }
}

function populateProjects(projects) {
  return {
    type: 'POPULATE_PROJECTS',
    payload: {
      projects: projects
    }
  }
}

function removeProjects(projects) {
  return {
    type: 'REMOVE_PROJECTS',
    payload: {
      projects: projects
    }
  }
}

export function newProject(token) {
  return dispatch => {
    api.createNewProject(token).then(resp => {
      if (resp.error != null) {
        apiErrorHandling(resp, dispatch, 'Project creation failed.');
      } else {
        const project = new Project({...resp});
        dispatch(createNewProject(project));
        dispatch(makeProjectActive(project));
      }
    })
  }
}

export function deleteProject(token, projectId) {
  return dispatch => {
    api.deleteUserProject(token, projectId).then(resp => {
      if (resp.error != null) {
        apiErrorHandling(resp, dispatch, 'Project deletion failed.');
      } else {
        const project = resp;
        dispatch(removeProjects([project]));
      }  
    })
  }  
}

export function makeProjectActive(project) {
  return dispatch => {
    dispatch(createProjectActive(project));
  }
}

export function getUserProjects(token) {
  return dispatch => {
    api.getUserProjects(token).then(resp => {
      if (resp.error != null) {
        apiErrorHandling(resp, dispatch, 'Project fetching failed.');
      } else {
        const projects = resp;
        dispatch(populateProjects(projects));
      }  
    })
  }
}
