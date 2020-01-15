# RelatedWords
A software solution that will allow to compare how often different words appear in the same document and sentence.
It is extracting non-html content, applying NLP to lemmatize the text, as well as applying other filters.
Currently only web sites are supported as text sources.

# RelatedWordsAPI
REST API that allows to manage the backend processing of RelatedWords.
Currently it triggers background tasks that do the processing.
The processing functionality shall be moved to a separate service.
The end result of the processing are objects representing the occurances of uniqe words in given pages and sentences.

# TODO
1. User Interface that calculates final scores and presents the results.
2. Move the text processing functionality to a separate service.
4. Custom whitelist and blacklist filters for words.
5. Custom mechanisms to fetch text from different web sites.
6. Support for multiple text sources (for example txt format on local file system).