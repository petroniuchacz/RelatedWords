import * as api from '../api/user'
import User from '../models/User'
import {newNotification} from  './notifications'

function createUserSignedIn(user) {
  return {
    type: 'USER_LOGGED_IN',
    payload: {
      user: user,
      loginStatus: true
    }
  }
}

function createUserNotSignedIn() {
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
      dispatch(createUserNotSignedIn());
    } else if (user.token == null) {
      dispatch(createUserNotSignedIn());
    } else {
      api.getUser(user).then(resp => {
        if (resp.error != null) {
          console.log(resp.error);
          dispatch(createUserNotSignedIn());
        } else {
          const user = new User({...resp})
          dispatch(createUserSignedIn(user));
        }
      })
    }
  }
}

export function userSignIn(email, password, keepSignedIn) {
  return dispatch => {
    api.userLogin(email, password).then(resp => {
      if (resp.error != null) {
        console.log(resp.error);
        try {
          if (resp.error.response.status === 401) {
            dispatch(newNotification('error', 'Wrong credentials. Please try again.')); 
          } else {
            dispatch(newNotification('error', 'Sign in failed. Please try again.')); 
          }
        } 
        catch {
          dispatch(newNotification('error', 'Sign in failed. Please try again.')); 
        }
      } else {
        const user = new User({...resp})
        sessionStorage.setItem('user', JSON.stringify(user));
        if (keepSignedIn)
          localStorage.setItem('user', JSON.stringify(user));
        dispatch(createUserSignedIn(user));
      }
    })
  }
}

export function userSignOut() {
  return dispatch => {
    sessionStorage.removeItem('user');
    localStorage.removeItem('user');
    dispatch(createUserNotSignedIn());
  }
}