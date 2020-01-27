import {uuid} from 'uuidv4'

function createNotification(type, message, id) {
  return {
    type: 'CREATE_NOTIFICATION',
    payload: {notification: {
      type: type,
      message: message,
      id: id
    }}}
}

export function newNotification(type, message) {
  const id = uuid();
  return dispatch => {
    dispatch(createNotification(type, message, id));
  }
}