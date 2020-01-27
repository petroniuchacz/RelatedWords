function notifications (state = [], action) {
  const {payload} = action;
  switch (action.type) {
    case 'CREATE_NOTIFICATION':
      return [...state].concat(payload.notification);
    default:
      return state;
  }
}

export default notifications;