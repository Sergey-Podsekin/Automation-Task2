using System;

namespace Task1.SourceCode.exception
{
    public class FileNameAlreadyExistsException : Exception
    {
        public FileNameAlreadyExistsException() 
            : base("File with this name already exists.") {}
    }
}
