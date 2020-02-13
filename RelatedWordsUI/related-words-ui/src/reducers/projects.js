import Project from "../models/Project";
import projectLoadingStatus from "../models/ProjectLoadingStatus";

function projects(state = {
  projects: [], 
  projectLoadingStatuses: [],
  active: null
}, action) {
  const {payload} = action;
  switch(action.type){

    case 'NEW_PROJECT':
      return {...state, 
        projects: [...state.projects].concat(payload.project),
        projectLoadingStatuses: [...state.projectLoadingStatuses].concat(payload.projectLoadingStatus)
      }
    case 'MAKE_ACTIVE':
      return {...state, active: payload.project, projects: [...state.projects],
        projectLoadingStatuses: [...state.projectLoadingStatuses]}

    case 'POPULATE_PROJECTS':
      // Use old loading status it matches the processingRevisionNumber of the newly
      // populated project. Else use loading status passed with the payload.
      let loadingStatuses = filterLowerRevisionLoadingStatus(
        payload.projects, 
        payload.projectLoadingStatuses, 
        state.projectLoadingStatuses);

      return {...state, 
        projects: [...payload.projects], 
        projectLoadingStatuses: [...loadingStatuses]};

    case 'REMOVE_PROJECTS':
      let projectIds1 = payload.projects.map(p => p.projectId);
      return {...state, 
        projects: state.projects.filter(p => !projectIds1.includes(p.projectId)),
        projectLoadingStatuses: state.projectLoadingStatuses.filter(p => 
          !projectIds1.includes(p.projectId)
        )};

    case 'ADD_OR_UPDATE_PROJECTS':
      // Use old loading status it matches the processingRevisionNumber of the newly
      // populated project. Else use loading status passed with the payload.
      const loadingStatuses2 = filterLowerRevisionLoadingStatus(
        payload.projects, 
        payload.projectLoadingStatuses, 
        state.projectLoadingStatuses);
      const loadingStatuses2ProjectIds = loadingStatuses2.map(pLS => pLS.projectId)

      const projectIds2 = payload.projects.map(p => p.projectId);
      return {...state, 
          projects: state.projects.filter(p => 
            !projectIds2.includes(p.projectId))
            .concat([...payload.projects]),
          projectLoadingStatuses: state.projectLoadingStatuses.filter(pLS => 
            !loadingStatuses2ProjectIds.includes(pLS.projectId))
            .concat([...loadingStatuses2]),
        };
    
    case 'ADD_OR_UPDATE_PROJECTS_LOADING_STATUSES':
      const loadingStatuses3ProjectIds = payload.projectLoadingStatuses.map(pLS => pLS.projectId);
      return {
        ...state,
        projects: [...state.projects],
        projectLoadingStatuses: state.projectLoadingStatuses.filter(pLS => 
          !loadingStatuses3ProjectIds.includes(pLS.projectId))
          .concat(payload.projectLoadingStatuses)
      }

    default:
      return state;
  }
}

// Use old loading status it matches the processingRevisionNumber of the newly
// populated project. Else use loading status passed with the payload.
function filterLowerRevisionLoadingStatus(newProjs, new_pLS, passed_old_pLS) {
  return newProjs.map((p, i) => {
    const old_pLS = passed_old_pLS.find(pLS => 
      pLS.projectId === p.projectId
      );
    try {
       return old_pLS.processingRevisionNumber === p.processingRevisionNumber ?
        old_pLS : new_pLS[i];
    } catch {
      return new_pLS[i];
    }
  })
}

export default projects;