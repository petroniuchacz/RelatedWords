function user (state = {loginStatus: null}, action) {
  const {payload} = action;
  switch (action.type) {
    case 'USER_LOGGED_IN':
      return {user: payload.user, loginStatus: payload.loginStatus};
    case 'USER_NOT_LOGGED_IN':
      return {loginStatus: payload.loginStatus};
    default:
      return state;
  }
}

export default user;