import isDefinedOrNull from './helpers/isDefinedOrNull';
import projectLoadingStatusEnum from './helpers/projectLoadingStatusEnum';

class ProjectLoadingStatus {
  constructor(props) {
    this.projectId = isDefinedOrNull(props.projectId);
    this.loadingStatus = isDefinedOrNull(
      projectLoadingStatusEnum[props.loadingStatus]
      );
    //processingRevisionNumber of the project after loading
    this.processingRevisionNumber = isDefinedOrNull(
      props.processingRevisionNumber)
  }
}

export default ProjectLoadingStatus;