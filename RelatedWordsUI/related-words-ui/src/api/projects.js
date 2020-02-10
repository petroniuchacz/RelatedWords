import * as axios from 'axios';
import Project from '../models/Project';
import removeNullAttributes from '../models/helpers/removeNullAttributes'

const projectUrl = process.env.REACT_APP_API_PROJECTS_URL;
const getUserProjectsUrl = process.env.REACT_APP_API_GET_USER_PROJECTS_URL;

export function createNewProject(
    token, 
    project = removeNullAttributes(new Project({name: "New project"}))
  )
{
  return axios({
    method: 'post',
    url: projectUrl,
    headers: {'Authorization': 'Bearer '.concat(token) },
    data: {...project},
    timeout: 10000
  })
    .then(function (response) {
      const data = response.data;
      const responseProject = new Project({...data})
      return responseProject;
    })
    .catch(function (error) {
      return { error: error };
    });
}

export function getUserProjects(token) {
  return axios({
    method: 'get',
    url: getUserProjectsUrl,
    headers: {'Authorization': 'Bearer '.concat(token) },
    timeout: 10000
  })
    .then(function (response) {
      const data = response.data;
      const responseProjects = data.map(p => new Project(p))
      return responseProjects;
    })
    .catch(function (error) {
      return { error: error };
    });
}

export function deleteUserProject(token, projectId) {
  return axios({
    method: 'delete',
    url: `${projectUrl}/${projectId}`,
    headers: {'Authorization': 'Bearer '.concat(token) },
    timeout: 10000
  })
    .then(function (response) {
      const data = response.data;
      const responseProject = new Project(data);
      return responseProject;
    })
    .catch(function (error) {
      return { error: error };
    });
}