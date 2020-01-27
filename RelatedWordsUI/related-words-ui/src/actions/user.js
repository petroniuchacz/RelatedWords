import * as api from '../api/user'
import User from '../models/User'
import {newNotification} from  './notifications'

function createUserLoggedIn(user) {
  return {
    type: 'USER_LOGGED_IN',
    payload: {
      user: user,
      loginStatus: true
    }
  }
}

function createUserNotLoggedIn() {
  return {
    type: 'USER_NOT_LOGGED_IN',
    payload: {
      loginStatus: false
    }
  }
}

export function fetchLoginStatus() {
  return dispatch => {
    let user = null;
    try {
      if(sessionStorage['user'] != null) {
        user = JSON.parse(sessionStorage['user']);
      } else if(localStorage['user'] != null) {
        user = JSON.parse(localStorage['user']);
      }
    } 
    catch {}

    if (user == null) {
      dispatch(createUserNotLoggedIn());
    } else if (user.token == null) {
      dispatch(createUserNotLoggedIn());
    } else {
      api.getUser(user).then(resp => {
        if (resp.error != null) {
          console.log(resp.error);
          dispatch(createUserNotLoggedIn());
        } else {
          const user = new User({...resp})
          dispatch(createUserLoggedIn(user));
        }
      })
    }
  }
}

export function userLogin(email, password, keepSignedIn) {
  return dispatch => {
    api.userLogin(email, password).then(resp => {
      if (resp.error != null) {
        console.log(resp.error);
        if (resp.error.response.status === 401) {
          dispatch(newNotification('error', 'Wrong credentials. Please try again.')); 
        } else {
          dispatch(newNotification('error', 'Sign in failed. Please try again.')); 
        }
      } else {
        const user = new User({...resp})
        sessionStorage.setItem('user', JSON.stringify(user));
        if (keepSignedIn)
          localStorage.setItem('user', JSON.stringify(user));
        dispatch(createUserLoggedIn(user));
      }
    })
  }
}