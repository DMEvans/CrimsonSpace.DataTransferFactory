namespace CrimsonSpace.DataTransferFactory.Tests.SampleClasses
{
    using System.Collections.Generic;

    [TransferrableTypes(typeof(BookEntity))]
    public class BookDTO : ITransferDTO
    {
        [TransferrableSubMember]
        public PersonDTO Author { get; set; }

        [TransferrableSubMember(true)]
        public List<GenreDTO> Genres { get; set; }

        [TransferrableMember]
        public string Title { get; set; }
    }
}