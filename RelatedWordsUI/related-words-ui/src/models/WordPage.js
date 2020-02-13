import isDefinedOrNull from './helpers/isDefinedOrNull';

class WordPage {
  constructor(props) {
    this.wordId = isDefinedOrNull(props.wordId);
    this.pageId = isDefinedOrNull(props.pageId);
    this.count = isDefinedOrNull(props.count);
  }
}

export default WordPage;