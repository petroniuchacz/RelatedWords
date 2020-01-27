import { combineReducers } from 'redux';
import user from './user';
import notifications from "./notifications";

const rootReducer = combineReducers({
  user,
  notifications
});

export default rootReducer;