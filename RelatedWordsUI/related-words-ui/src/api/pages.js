import * as axios from 'axios';
import Page from '../models/Page';

const pagesUrl = process.env.REACT_APP_API_PAGES_URL;

export function getUserPages(token, projectId) {
  return axios({
    method: 'get',
    url: `${pagesUrl}/${projectId}`,
    headers: {'Authorization': 'Bearer '.concat(token) },
    timeout: 10000
  })
    .then(function (response) {
      const data = response.data;
      const responsePages = data.map(p => new Page(p))
      return responsePages;
    })
    .catch(function (error) {
      return { error: error };
    });
}