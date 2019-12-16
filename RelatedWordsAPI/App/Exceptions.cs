using System;
using System.Collections.Generic;
using System.Text;

namespace RelatedWordsAPI.App
{
    class RelatedWordsProcessorException : Exception
    {
        public RelatedWordsProcessorException() : base()
        {

        }

        public RelatedWordsProcessorException(string mess) : base(mess)
        {

        }
    }

    class CouldNotGetFromDatabase : RelatedWordsProcessorException
    {
        public CouldNotGetFromDatabase() : base()
        {

        }

        public CouldNotGetFromDatabase(string mess) : base(mess)
        {

        }
    }

    class InputProjectNotValid : RelatedWordsProcessorException
    {
        public InputProjectNotValid() : base()
        {

        }

        public InputProjectNotValid(string mess) : base(mess)
        {

        }
    }

    class CannotEvalException : RelatedWordsProcessorException
    {
        public CannotEvalException() : base()
        {

        }

        public CannotEvalException(string mess) : base(mess)
        {

        }
    }
}
