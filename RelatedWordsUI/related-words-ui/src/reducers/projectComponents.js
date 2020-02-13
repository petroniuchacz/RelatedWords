function projectComponents(state = {
  projectWords: {}, 
  projectPages: {},
}, action) {
  const {payload} = action;

  switch(action.type){
    case 'POPULATE_PROJECT_PAGES':
      return updateOneArr(state, 'projectPages', 
        payload.projectId, payload.pages);
    case 'POPULATE_PROJECT_WORDS':
      return updateOneArr(state, 'projectWords', 
        payload.projectId, payload.words);
    default:
      return state;
  }
}

function updateOneArr (state, attr, projectId, newarr) {
  const newState = {...state};
  newState[attr][projectId] = [...newarr];
  return newState
}

export default projectComponents;