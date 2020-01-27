import * as axios from 'axios';
import User from '../models/User'

const userUrl = process.env.REACT_APP_API_USERS_URL;
const authenticationUrl = process.env.REACT_APP_API_AUTHENTICATION_URL;

export function getUser(user) {
  return axios({
    method: 'get',
    url: userUrl + '/' + user.userId,
    headers: {'Authorization': 'Bearer '.concat(user.token) },
    timeout: 10000
  })
    .then(function (response) {
      const data = response.data;
      const responseUser = new User({
        userId: data.userId,
        email: data.email,
        role: data.role,
        token: user.token
      })
      return responseUser;
    })
    .catch(function (error) {
      return { error: error };
    });
}

export function userLogin(email, password) {
  return axios({
    method: 'post',
    url: authenticationUrl,
    data: {Email: email, Password: password},
    timeout: 10000
  })
    .then(function (response) {
      const data = response.data;
      const responseUser = new User({
        userId: data.userId,
        email: data.email,
        role: data.role,
        token: data.token
      })
      return responseUser;
    })
    .catch(function (error) {
      return { error: error };
    });
}