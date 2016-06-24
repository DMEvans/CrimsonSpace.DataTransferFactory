namespace CrimsonSpace.DataTransferFactory.Tests.SampleClasses
{
    [TransferrableTypes(typeof(GenreEntity))]
    public class GenreDTO : ITransferDTO
    {
        [TransferrableMember]
        public int Id { get; set; }

        [TransferrableMember]
        public string Name { get; set; }
    }
}