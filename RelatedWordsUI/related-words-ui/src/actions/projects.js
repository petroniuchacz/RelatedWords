import Project from '../models/Project';
import ProjectLoadingStatus from '../models/ProjectLoadingStatus';
import projectLoadingStatusEnum from '../models/helpers/projectLoadingStatusEnum';
import * as projApi from '../api/projects';
import * as pageApi from '../api/pages';
//import {newNotification} from  './notifications';
//import {userSignOut} from './user';
import apiErrorHandling from './helpers/apiErrorHandling'

function createNewProject(project) {
  return {
    type: 'NEW_PROJECT',
    payload: {
      project: project,
      projectLoadingStatus: new ProjectLoadingStatus({
        projectId: project.projectId,
        loadingStatus: projectLoadingStatusEnum['FINISHED']
      })
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
      projects: projects,
      projectLoadingStatuses: projects.map((p) => 
      new ProjectLoadingStatus({
        projectId: p.projectId,
        loadingStatus: projectLoadingStatusEnum['NOT_STARTED']
      }))
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

function addUpdateProjects(projects, 
  projectLoadingStatuses = projects.map((p) => 
  new ProjectLoadingStatus({
    projectId: p.projectId,
    loadingStatus: projectLoadingStatusEnum['NOT_STARTED']
  }))) {
  return {
    type: 'ADD_OR_UPDATE_PROJECTS',
    payload: {
      projects: projects,
      projectLoadingStatuses: projectLoadingStatuses
    }    
  }
}

function addUpdateProjectLoadingStatus(status) {
  return {
    type: 'ADD_OR_UPDATE_PROJECTS_LOADING_STATUSES',
    payload: {
      projectLoadingStatuses: [status]
    }
  }
}

function populateProjPages(projectId, pages){
  return {
    type: 'POPULATE_PROJECT_PAGES',
    payload: {
      projectId: projectId,
      pages: pages
    }
  } 
}

function populateProjWords(projectId, words){
  return {
    type: 'POPULATE_PROJECT_WORDS',
    payload: {
      projectId: projectId,
      words: words
    }
  } 
}

export function newProject(token) {
  return dispatch => {
    projApi.createNewProject(token).then(resp => {
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
    projApi.deleteUserProject(token, projectId).then(resp => {
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

export function getProject(token, projectId, 
  {startingStatus, loadingStatus, loadingStatusError}
  ) {
  return dispatch => {
    if (startingStatus != null)
      dispatch(addUpdateProjectLoadingStatus(startingStatus));
    projApi.getUserProject(token, projectId).then(resp => {
      if (resp.error != null) {
        apiErrorHandling(resp, dispatch, 'Loading project failed.');
        if (loadingStatusError != null)
          dispatch(addUpdateProjectLoadingStatus(loadingStatusError));
      } else {
        const project = resp;
        dispatch(addUpdateProjects([project], [loadingStatus]));
      }  
    })
  }   
}

export function getUserProjects(token) {
  return dispatch => {
    projApi.getUserProjects(token).then(resp => {
      if (resp.error != null) {
        apiErrorHandling(resp, dispatch, 'Project fetching failed.');
      } else {
        const projects = resp;
        dispatch(populateProjects(projects));
      }  
    })
  }
}

export function changeProjectLoadingStatus (status) {
  return dispatch => {
    dispatch(addUpdateProjectLoadingStatus(status))
  }
}

export function loadProjectComponents(token, project) {
  return dispatch => {
    dispatch(changeProjectLoadingStatus (new ProjectLoadingStatus({
      projectId: project.projectId,
      loadingStatus: projectLoadingStatusEnum['FETCHING_PROJECT_COMPONENTS'],
    })))

    let promises = [];

    const pagesPromise = pageApi.getUserPages(token, project.projectId)
    promises = promises.concat(pagesPromise);

    // If backend processing has finished, there are words available.
    if (project.processingStatus === 2) {
      const wordsPromise = projApi.getWords(token, project.projectId)
      promises = promises.concat(wordsPromise);
    }

    Promise.all(promises).then(([pageRes, wordRes]) => {
      try {
        if (pageRes != null) {
          if (pageRes.error != null) {
            apiErrorHandling(pageRes, dispatch, 'Pages fetching failed.');
            throw pageRes.error;
          } else {
            const pages = pageRes;
            dispatch(populateProjPages(project.projectId, pages));
          }         
        }

        if (wordRes != null) {
          if (wordRes.error != null) {
            apiErrorHandling(wordRes, dispatch, 'Word fetching failed.');
            throw wordRes.error;
          } else {
            const words = wordRes;
            dispatch(populateProjWords(project.projectId, words));  
          }    
        }

        dispatch(changeProjectLoadingStatus (new ProjectLoadingStatus({
          projectId: project.projectId,
          loadingStatus: projectLoadingStatusEnum['FINISHED'],
          processingRevisionNumber: project.processingRevisionNumber
        })))

      } catch(e) {
        dispatch(changeProjectLoadingStatus (new ProjectLoadingStatus({
          projectId: project.projectId,
          loadingStatus: projectLoadingStatusEnum['FAILED'],
        })))
      }     
    })
  }
}