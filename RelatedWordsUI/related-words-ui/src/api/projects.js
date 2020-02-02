import * as axios from 'axios';
import Project from '../models/Project';
import removeNullAttributes from '../models/helpers/removeNullAttributes'

const projectUrl = process.env.REACT_APP_API_PROJECTS_URL;

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