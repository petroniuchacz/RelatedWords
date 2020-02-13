import isDefinedOrNull from './helpers/isDefinedOrNull';

class Page {
  constructor(props) {
    this.projectId = isDefinedOrNull(props.projectId);
    this.pageId = isDefinedOrNull(props.pageId);
    this.originalContent = isDefinedOrNull(props.originalContent);
    this.filteredContent = isDefinedOrNull(props.filteredContent);
    this.url = isDefinedOrNull(props.url);
    this.processingStatus = isDefinedOrNull(props.processingStatus);
  }

  processingStatusToString() {
    switch(this.processingStatus) {
      case 0:
        return 'Not Started';
      case 1:
        return 'Processing';
      case 2:
        return 'Filtered';
      case 3:
        return 'Finished';
      case 4:
        return 'Canceled';
      case 5:
        return 'Failed';
      case 6:
        return 'Unknown';
      default:
        return 'Unknown';
    }
  }
}

export default Page;