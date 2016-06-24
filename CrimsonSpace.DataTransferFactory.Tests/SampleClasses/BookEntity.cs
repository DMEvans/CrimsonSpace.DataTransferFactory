namespace CrimsonSpace.DataTransferFactory.Tests.SampleClasses
{
    using System.Collections.Generic;

    public class BookEntity
    {
        public PersonEntity Author { get; set; }
        public List<GenreEntity> Genres { get; set; }
        public string Title { get; set; }
    }
}