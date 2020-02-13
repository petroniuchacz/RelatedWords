import isDefinedOrNull from './helpers/isDefinedOrNull';

class Project {
  constructor(props) {
    this.projectId = isDefinedOrNull(props.projectId);
    this.userId = isDefinedOrNull(props.userId);
    this.name = isDefinedOrNull(props.name);
    this.processingStatus = isDefinedOrNull(props.processingStatus);
    this.createdDate = isDefinedOrNull(props.createdDate);
    this.editRevisionNumber = isDefinedOrNull(props.editRevisionNumber);
    this.editPagesRevisionNumber = isDefinedOrNull(props.editPagesRevisionNumber);
    this.processingRevisionNumber = isDefinedOrNull(props.processingRevisionNumber);
    this.processedPagesRevisionNumber = isDefinedOrNull(props.processedPagesRevisionNumber);
  }

  processingStatusToString() {
    switch(this.processingStatus) {
      case 0:
        return 'Not Started';
      case 1:
        return 'Processing';
      case 2:
        return 'Finished';
      case 3:
        return 'Canceled';
      case 4:
        return 'Failed';
      case 5:
        return 'Unknown';
      default:
        return 'Unknown';
    }
  }
}

export default Project;