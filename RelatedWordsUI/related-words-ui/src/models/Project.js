import isDefinedOrNull from './helpers/isDefinedOrNull';

class Project {
  constructor(props) {
    this.projectId = isDefinedOrNull(props.projectId);
    this.userId = isDefinedOrNull(props.userId);
    this.name = isDefinedOrNull(props.name);
    this.processingStatus = isDefinedOrNull(props.processingStatus);
    this.createdDate = isDefinedOrNull(props.createdDate);
  }
}

export default Project;