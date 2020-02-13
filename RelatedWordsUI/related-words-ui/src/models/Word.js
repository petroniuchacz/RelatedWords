import isDefinedOrNull from './helpers/isDefinedOrNull';

class Word {
  constructor(props) {
    this.wordId = isDefinedOrNull(props.wordId);
    this.projectId = isDefinedOrNull(props.projectId);
    this.wordContent = isDefinedOrNull(props.wordContent);
  }
}

export default Word;