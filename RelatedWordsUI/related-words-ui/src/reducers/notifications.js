function notifications (state = [], action) {
  const {payload} = action;
  switch (action.type) {
    case 'CREATE_NOTIFICATION':
      return [...state].concat(payload.notification);
    case 'REMOVE_NOTIFICATION':
      return state.filter(notification => notification.id !== payload.id);
    default:
      return state;
  }
}

export default notifications;