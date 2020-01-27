import isDefinedOrNull from './helpers/isDefinedOrNull'

class User {
  constructor(props) {
    this.userId = isDefinedOrNull(props.userId);
    this.email = isDefinedOrNull(props.email);
    this.password = isDefinedOrNull(props.password);
    this.token = isDefinedOrNull(props.token);
    this.role = isDefinedOrNull(props.role);
  }
}

export default User;