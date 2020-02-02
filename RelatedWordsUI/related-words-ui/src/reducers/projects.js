import Project from "../models/Project";

function projects(state = {projects: [], active: null}, action) {
  const {payload} = action;
  switch(action.type){
    case 'NEW_PROJECT':
      return {...state, projects: [...state.projects].concat(payload.project)}
    case 'MAKE_ACTIVE':
      return {active: payload.project, projects: [...state.projects]}
    default:
      return state;
  }
}

export default projects;