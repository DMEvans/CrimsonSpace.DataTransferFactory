namespace CrimsonSpace.DataTransferFactory.Tests.SampleClasses
{
    using System;

    [TransferAllMembers]
    public class PersonDTO : ITransferDTO
    {
        public string Name { get; set; }
        public string HomeTown { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}