import {newNotification} from '../notifications';
import {userSignOut} from '../user';

export default function apiErrorHandling(resp, dispatch, notificationMessage = "Oops... Something went wrong...") {
  console.log(resp.error);
  try {
    if (resp.error.response.status === 401) {
      dispatch(newNotification('error', 'The session expired. Please sign in again.')); 
      dispatch(userSignOut());
    } else {
      dispatch(newNotification('error', notificationMessage)); 
    }
  } 
  catch {
    dispatch(newNotification('error', notificationMessage)); 
  }
}