import Project from "../models/Project";

function projects(state = {projects: [], active: null}, action) {
  const {payload} = action;
  switch(action.type){
    case 'NEW_PROJECT':
      return {...state, projects: [...state.projects].concat(payload.project)}
    case 'MAKE_ACTIVE':
      return {active: payload.project, projects: [...state.projects]}
    case 'POPULATE_PROJECTS':
      return {...state, projects: [...payload.projects]}
    case 'REMOVE_PROJECTS':
      let projectIds = payload.projects.map(p => p.projectId);
      return {...state, projects: state.projects.filter(p => 
        !projectIds.includes(p.projectId)
        )};
    default:
      return state;
  }
}

export default projects;