import * as axios from 'axios';
import Project from '../models/Project';
import Word from '../models/Word';
import WordPage from '../models/WordPage';
import WordSentence from '../models/WordSentence';
import removeNullAttributes from '../models/helpers/removeNullAttributes'

const projectUrl = process.env.REACT_APP_API_PROJECTS_URL;
const getUserProjectsUrl = process.env.REACT_APP_API_GET_USER_PROJECTS_URL;
const getWordsUrl = process.env.REACT_APP_API_GET_WORDS_URL;
const getWordPagesUrl = process.env.REACT_APP_API_GET_WORD_PAGES_URL;
const getWordSentencesUrl = process.env.REACT_APP_API_GET_WORD_SENTENCES_URL;

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

export function getUserProject(token, projectId) {
  return axios({
    method: 'get',
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

export function getWords(token, projectId) {
  return axios({
    method: 'get',
    url: `${getWordsUrl}/${projectId}`,
    headers: {'Authorization': 'Bearer '.concat(token) },
    timeout: 10000
  })
    .then(function (response) {
      const data = response.data;
      const responseWords = data.map(w => new Word(w))
      return responseWords;
    })
    .catch(function (error) {
      return { error: error };
    });
}

export function getWordPages(token, wordId) {
  return axios({
    method: 'get',
    url: `${getWordPagesUrl}/${wordId}`,
    headers: {'Authorization': 'Bearer '.concat(token) },
    timeout: 30000
  })
    .then(function (response) {
      const data = response.data;
      const responseWordPages = data.map(e => new WordPage(e))
      return responseWordPages;
    })
    .catch(function (error) {
      return { error: error };
    });
}

export function getWordSentences(token, wordId) {
  return axios({
    method: 'get',
    url: `${getWordSentencesUrl}/${wordId}`,
    headers: {'Authorization': 'Bearer '.concat(token) },
    timeout: 10000
  })
    .then(function (response) {
      const data = response.data;
      const responseWordSentences = data.map(e => new WordSentence(e))
      return responseWordSentences;
    })
    .catch(function (error) {
      return { error: error };
    });
}