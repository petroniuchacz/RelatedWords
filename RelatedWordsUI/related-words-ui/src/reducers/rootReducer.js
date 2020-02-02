import { combineReducers } from 'redux';
import user from './user';
import notifications from "./notifications";
import projects from "./projects"

const rootReducer = combineReducers({
  user,
  notifications,
  projects
});

export default rootReducer;