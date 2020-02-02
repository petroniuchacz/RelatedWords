import Project from '../models/Project';
import * as api from '../api/projects';
import {newNotification} from  './notifications';
import {userSignOut} from './user';

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

export function newProject(token) {
  return dispatch => {
    api.createNewProject(token).then(resp => {
      if (resp.error != null) {
        console.log(resp.error);
        try {
          if (resp.error.response.status === 401) {
            dispatch(newNotification('error', 'The session expired. Please sign in again.')); 
            dispatch(userSignOut());
          } else {
            dispatch(newNotification('error', 'Project creation failed.')); 
          }
        } 
        catch {
          dispatch(newNotification('error', 'Project creation failed.')); 
        }
      } else {
        const project = new Project({...resp})
        dispatch(createNewProject(project));
        dispatch(makeProjectActive(project));
      }
    })
  }
}

export function makeProjectActive(project) {
  return dispatch => {
    dispatch(createProjectActive(project));
  }
}