import { combineReducers } from 'redux';
import user from './user';
import notifications from "./notifications";
import projects from "./projects";
import projectComponents from './projectComponents';

const rootReducer = combineReducers({
  user,
  notifications,
  projects,
  projectComponents
});

export default rootReducer;