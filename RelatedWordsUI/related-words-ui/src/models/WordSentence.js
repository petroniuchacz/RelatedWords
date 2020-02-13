import isDefinedOrNull from './helpers/isDefinedOrNull';

class WordSentence {
  constructor(props) {
    this.wordId = isDefinedOrNull(props.wordId);
    this.sentenceId = isDefinedOrNull(props.sentenceId);
    this.count = isDefinedOrNull(props.count);
  }
}

export default WordSentence;