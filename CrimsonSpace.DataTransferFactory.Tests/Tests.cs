namespace CrimsonSpace.DataTransferFactory.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SampleClasses;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class Tests
    {
        #region Test Methods

        [TestMethod]
        [TestCategory("ConstuctDTO")]
        public void ConstructDTO_TransferAllMembers_SimpleTypes_PropertiesOnly()
        {
            var personEntity = InitializeSinglePersonEntity();

            var personDto = personEntity.ConstructDTO<PersonDTO>();

            Assert.IsTrue(personDto.Name.Equals(personEntity.Name)
                          && personDto.HomeTown.Equals(personEntity.HomeTown)
                          && personDto.DateOfBirth.Equals(personEntity.DateOfBirth));
        }

        [TestMethod]
        [TestCategory("ConstuctDTO")]
        public void ConstructDTOCollection_TransferAllMembers_SimpleTypes_PropertiesOnly()
        {
            var personEntities = InitializePersonEntityList();

            var personDtoList = personEntities.ConstructDTOCollection<PersonDTO>().ToList();

            bool person1Valid = personDtoList[0].Name.Equals(personEntities[0].Name)
                                && personDtoList[0].HomeTown.Equals(personEntities[0].HomeTown)
                                && personDtoList[0].DateOfBirth.Equals(personEntities[0].DateOfBirth);
            bool person2Valid = personDtoList[1].Name.Equals(personEntities[1].Name)
                                && personDtoList[1].HomeTown.Equals(personEntities[1].HomeTown)
                                && personDtoList[1].DateOfBirth.Equals(personEntities[1].DateOfBirth);
            bool person3Valid = personDtoList[2].Name.Equals(personEntities[2].Name)
                                && personDtoList[2].HomeTown.Equals(personEntities[2].HomeTown)
                                && personDtoList[2].DateOfBirth.Equals(personEntities[2].DateOfBirth);

            Assert.IsTrue(person1Valid && person2Valid && person3Valid);
        }

        [TestMethod]
        [TestCategory("ConstuctDTO")]
        public void ConstructDTO_TransferSubMembers_PropertiesOnly()
        {
            var bookEntity = InitializeSingleBookEntity();

            var bookDto = bookEntity.ConstructDTO<BookDTO>();

            bool titleValid = bookDto.Title.Equals(bookEntity.Title);
            bool authorValid = bookDto.Author.Name.Equals(bookEntity.Author.Name)
                               && bookDto.Author.HomeTown.Equals(bookEntity.Author.HomeTown)
                               && bookDto.Author.DateOfBirth.Equals(bookEntity.Author.DateOfBirth);
            bool genresValid = bookDto.Genres[0].Name.Equals(bookEntity.Genres[0].Name)
                               && bookDto.Genres[1].Name.Equals(bookEntity.Genres[1].Name)
                               && bookDto.Genres[2].Name.Equals(bookEntity.Genres[2].Name);

            Assert.IsTrue(titleValid && authorValid && genresValid);
        }

        [TestMethod]
        [TestCategory("ConstuctDTO")]
        public void ConstructDTOCollection_TransferSubMembers_PropertiesOnly()
        {
            var bookEntities = InitializeBookEntityList();

            var bookDtos = bookEntities.ConstructDTOCollection<BookDTO>().ToList();

            bool book1titleValid = bookDtos[0].Title.Equals(bookEntities[0].Title);
            bool book1authorValid = bookDtos[0].Author.Name.Equals(bookEntities[0].Author.Name)
                               && bookDtos[0].Author.HomeTown.Equals(bookEntities[0].Author.HomeTown)
                               && bookDtos[0].Author.DateOfBirth.Equals(bookEntities[0].Author.DateOfBirth);
            bool book1genresValid = bookDtos[0].Genres[0].Name.Equals(bookEntities[0].Genres[0].Name)
                               && bookDtos[0].Genres[1].Name.Equals(bookEntities[0].Genres[1].Name)
                               && bookDtos[0].Genres[2].Name.Equals(bookEntities[0].Genres[2].Name);

            bool book2titleValid = bookDtos[1].Title.Equals(bookEntities[1].Title);
            bool book2authorValid = bookDtos[1].Author.Name.Equals(bookEntities[1].Author.Name)
                               && bookDtos[1].Author.HomeTown.Equals(bookEntities[1].Author.HomeTown)
                               && bookDtos[1].Author.DateOfBirth.Equals(bookEntities[1].Author.DateOfBirth);
            bool book2genresValid = bookDtos[1].Genres[0].Name.Equals(bookEntities[1].Genres[0].Name)
                               && bookDtos[1].Genres[1].Name.Equals(bookEntities[1].Genres[1].Name)
                               && bookDtos[1].Genres[2].Name.Equals(bookEntities[1].Genres[2].Name);

            bool book3titleValid = bookDtos[2].Title.Equals(bookEntities[2].Title);
            bool book3authorValid = bookDtos[2].Author.Name.Equals(bookEntities[2].Author.Name)
                               && bookDtos[2].Author.HomeTown.Equals(bookEntities[2].Author.HomeTown)
                               && bookDtos[2].Author.DateOfBirth.Equals(bookEntities[2].Author.DateOfBirth);
            bool book3genresValid = bookDtos[2].Genres[0].Name.Equals(bookEntities[2].Genres[0].Name)
                               && bookDtos[2].Genres[1].Name.Equals(bookEntities[2].Genres[1].Name)
                               && bookDtos[2].Genres[2].Name.Equals(bookEntities[2].Genres[2].Name);

            Assert.IsTrue(book1titleValid && book1authorValid && book1genresValid &&
                          book2titleValid && book2authorValid && book2genresValid &&
                          book3titleValid && book3authorValid && book3genresValid);
        }

        [TestMethod]
        [TestCategory("DeconstuctDTO")]
        public void DeconstructDTO_TransferAllMembers_SimpleTypes_PropertiesOnly()
        {
            string name = "Jimny Cricket";
            string homeTown = "Orlando";
            DateTime dateOfBirth = new DateTime(1985, 03, 05);
            var personDto = new PersonDTO() { Name = name, HomeTown = homeTown, DateOfBirth = dateOfBirth };

            var personEntity = personDto.DeconstructDTO<PersonEntity>();

            Assert.IsTrue(personEntity.Name.Equals(name) && personEntity.HomeTown.Equals(homeTown) && personEntity.DateOfBirth.Equals(dateOfBirth));
        }

        [TestMethod]
        [TestCategory("DeconstuctDTO")]
        public void DeconstructDTOCollection_TransferAllMembers_SimpleTypes_PropertiesOnly()
        {
            string[] names = { "Jimny Cricket", "Bobby Boucher", "Steve McQueen" };
            string[] homeTowns = { "Orlando", "New York", "Puxville" };
            DateTime[] datesOfBirth = { new DateTime(1985, 03, 05), new DateTime(1985, 03, 05), new DateTime(1985, 03, 05) };

            List<PersonDTO> personDtoList = new List<PersonDTO>();
            personDtoList.Add(new PersonDTO() { Name = names[0], HomeTown = homeTowns[0], DateOfBirth = datesOfBirth[0] });
            personDtoList.Add(new PersonDTO() { Name = names[1], HomeTown = homeTowns[1], DateOfBirth = datesOfBirth[1] });
            personDtoList.Add(new PersonDTO() { Name = names[2], HomeTown = homeTowns[2], DateOfBirth = datesOfBirth[2] });

            var personEntities = personDtoList.DeconstructDTOCollection<PersonEntity>().ToList();

            bool person1Valid = personEntities[0].Name.Equals(names[0]) && personEntities[0].HomeTown.Equals(homeTowns[0]) && personEntities[0].DateOfBirth.Equals(datesOfBirth[0]);
            bool person2Valid = personEntities[1].Name.Equals(names[1]) && personEntities[1].HomeTown.Equals(homeTowns[1]) && personEntities[0].DateOfBirth.Equals(datesOfBirth[1]);
            bool person3Valid = personEntities[2].Name.Equals(names[2]) && personEntities[2].HomeTown.Equals(homeTowns[2]) && personEntities[2].DateOfBirth.Equals(datesOfBirth[2]);

            Assert.IsTrue(person1Valid && person2Valid && person3Valid);
        }

        [TestMethod]
        [TestCategory("DeconstuctDTO")]
        public void DeconstructDTO_TransferSubMembers_PropertiesOnly()
        {
            string title = "Killing Floor";
            string name = "Lee Child";
            string homeTown = "Los Angeles";
            DateTime dateOfBirth = new DateTime(1965, 03, 05);
            var author = new PersonDTO() { Name = name, HomeTown = homeTown, DateOfBirth = dateOfBirth };

            var genres = new List<GenreDTO>()
            {
                new GenreDTO() { Id = 1, Name = "Action" },
                new GenreDTO() { Id = 2, Name = "Thriller" },
                new GenreDTO() { Id = 3, Name = "Military" }
            };

            var bookDto = new BookDTO() { Title = title, Author = author, Genres = genres };

            var bookEntity = bookDto.DeconstructDTO<BookEntity>();

            bool titleValid = bookEntity.Title.Equals(title);
            bool authorValid = bookEntity.Author.Name.Equals(name) && bookEntity.Author.HomeTown.Equals(homeTown) && bookEntity.Author.DateOfBirth.Equals(dateOfBirth);
            bool genresValid = bookEntity.Genres[0].Name.Equals(genres[0].Name) && bookEntity.Genres[1].Name.Equals(genres[1].Name) && bookEntity.Genres[2].Name.Equals(genres[2].Name);

            Assert.IsTrue(titleValid && authorValid && genresValid);
        }

        [TestMethod]
        [TestCategory("DeconstuctDTO")]
        public void DeconstructDTOCollection_TransferSubMembers_PropertiesOnly()
        {
            var bookDtos = InitializeBookDTOList();

            var bookEntities = bookDtos.DeconstructDTOCollection<BookEntity>().ToList();

            bool book1titleValid = bookEntities[0].Title.Equals(bookDtos[0].Title);
            bool book1authorValid = bookEntities[0].Author.Name.Equals(bookDtos[0].Author.Name)
                               && bookEntities[0].Author.HomeTown.Equals(bookDtos[0].Author.HomeTown)
                               && bookEntities[0].Author.DateOfBirth.Equals(bookDtos[0].Author.DateOfBirth);
            bool book1genresValid = bookEntities[0].Genres[0].Name.Equals(bookDtos[0].Genres[0].Name)
                               && bookEntities[0].Genres[1].Name.Equals(bookDtos[0].Genres[1].Name)
                               && bookEntities[0].Genres[2].Name.Equals(bookDtos[0].Genres[2].Name);

            bool book2titleValid = bookEntities[1].Title.Equals(bookDtos[1].Title);
            bool book2authorValid = bookEntities[1].Author.Name.Equals(bookDtos[1].Author.Name)
                               && bookEntities[1].Author.HomeTown.Equals(bookDtos[1].Author.HomeTown)
                               && bookEntities[1].Author.DateOfBirth.Equals(bookDtos[1].Author.DateOfBirth);
            bool book2genresValid = bookEntities[1].Genres[0].Name.Equals(bookDtos[1].Genres[0].Name)
                               && bookEntities[1].Genres[1].Name.Equals(bookDtos[1].Genres[1].Name)
                               && bookEntities[1].Genres[2].Name.Equals(bookDtos[1].Genres[2].Name);

            bool book3titleValid = bookEntities[2].Title.Equals(bookDtos[2].Title);
            bool book3authorValid = bookEntities[2].Author.Name.Equals(bookDtos[2].Author.Name)
                               && bookEntities[2].Author.HomeTown.Equals(bookDtos[2].Author.HomeTown)
                               && bookEntities[2].Author.DateOfBirth.Equals(bookDtos[2].Author.DateOfBirth);
            bool book3genresValid = bookEntities[2].Genres[0].Name.Equals(bookDtos[2].Genres[0].Name)
                               && bookEntities[2].Genres[1].Name.Equals(bookDtos[2].Genres[1].Name)
                               && bookEntities[2].Genres[2].Name.Equals(bookDtos[2].Genres[2].Name);

            Assert.IsTrue(book1titleValid && book1authorValid && book1genresValid &&
                          book2titleValid && book2authorValid && book2genresValid &&
                          book3titleValid && book3authorValid && book3genresValid);
        }

        #endregion Test Methods

        #region Test Object Builder Methods

        private PersonEntity InitializeSinglePersonEntity()
        {
            string name = "Lee Child";
            string homeTown = "Los Angeles";
            DateTime dateOfBirth = new DateTime(1965, 03, 05);
            return new PersonEntity() { Name = name, HomeTown = homeTown, DateOfBirth = dateOfBirth };
        }

        private PersonDTO InitializeSinglePersonDTO()
        {
            string name = "Lee Child";
            string homeTown = "Los Angeles";
            DateTime dateOfBirth = new DateTime(1965, 03, 05);
            return new PersonDTO() { Name = name, HomeTown = homeTown, DateOfBirth = dateOfBirth };
        }

        private List<PersonEntity> InitializePersonEntityList()
        {
            string[] names = { "Lee Child", "Michael Crichton", "Robert J. Sawyer" };
            string[] homeTowns = { "Los Angeles", "Dallas", "New Orleans" };
            DateTime[] datesOfBirth = { new DateTime(1950, 03, 05), new DateTime(1960, 03, 05), new DateTime(1970, 03, 05) };

            var personEntities = new List<PersonEntity>();
            personEntities.Add(new PersonEntity() { Name = names[0], HomeTown = homeTowns[0], DateOfBirth = datesOfBirth[0] });
            personEntities.Add(new PersonEntity() { Name = names[1], HomeTown = homeTowns[1], DateOfBirth = datesOfBirth[1] });
            personEntities.Add(new PersonEntity() { Name = names[2], HomeTown = homeTowns[2], DateOfBirth = datesOfBirth[2] });

            return personEntities;
        }

        private List<PersonDTO> InitializePersonDTOList()
        {
            string[] names = { "Lee Child", "Michael Crichton", "Robert J. Sawyer" };
            string[] homeTowns = { "Los Angeles", "Dallas", "New Orleans" };
            DateTime[] datesOfBirth = { new DateTime(1950, 03, 05), new DateTime(1960, 03, 05), new DateTime(1970, 03, 05) };

            var personDtos = new List<PersonDTO>();
            personDtos.Add(new PersonDTO() { Name = names[0], HomeTown = homeTowns[0], DateOfBirth = datesOfBirth[0] });
            personDtos.Add(new PersonDTO() { Name = names[1], HomeTown = homeTowns[1], DateOfBirth = datesOfBirth[1] });
            personDtos.Add(new PersonDTO() { Name = names[2], HomeTown = homeTowns[2], DateOfBirth = datesOfBirth[2] });

            return personDtos;
        }

        private List<GenreEntity> InitializeGenreEntityList()
        {
            var genres = new List<GenreEntity>()
            {
                new GenreEntity() { Id = 1, Name = "Action" },
                new GenreEntity() { Id = 2, Name = "Thriller" },
                new GenreEntity() { Id = 3, Name = "Military" }
            };

            return genres;
        }

        private List<GenreDTO> InitializeGenreDTOList()
        {
            var genres = new List<GenreDTO>()
            {
                new GenreDTO() { Id = 1, Name = "Action" },
                new GenreDTO() { Id = 2, Name = "Thriller" },
                new GenreDTO() { Id = 3, Name = "Military" }
            };

            return genres;
        }

        private List<List<GenreEntity>> InitializeGenreEntityListCollection()
        {
            var genresList = new List<List<GenreEntity>>()
            {
               new List<GenreEntity>()
                {
                    new GenreEntity() { Id = 1, Name = "Action" },
                    new GenreEntity() { Id = 2, Name = "Thriller" },
                    new GenreEntity() { Id = 3, Name = "Military" }
                },
                new List<GenreEntity>()
                {
                    new GenreEntity() { Id = 1, Name = "Action" },
                    new GenreEntity() { Id = 2, Name = "Thriller" },
                    new GenreEntity() { Id = 3, Name = "Sci-Fi" }
                },
                new List<GenreEntity>()
                {
                    new GenreEntity() { Id = 1, Name = "Thriller" },
                    new GenreEntity() { Id = 2, Name = "Sci-Fi" },
                    new GenreEntity() { Id = 3, Name = "Time Travel" }
                }
            };

            return genresList;
        }

        private List<List<GenreDTO>> InitializeGenreDTOListCollection()
        {
            var genresList = new List<List<GenreDTO>>()
            {
               new List<GenreDTO>()
                {
                    new GenreDTO() { Id = 1, Name = "Action" },
                    new GenreDTO() { Id = 2, Name = "Thriller" },
                    new GenreDTO() { Id = 3, Name = "Military" }
                },
                new List<GenreDTO>()
                {
                    new GenreDTO() { Id = 1, Name = "Action" },
                    new GenreDTO() { Id = 2, Name = "Thriller" },
                    new GenreDTO() { Id = 3, Name = "Sci-Fi" }
                },
                new List<GenreDTO>()
                {
                    new GenreDTO() { Id = 1, Name = "Thriller" },
                    new GenreDTO() { Id = 2, Name = "Sci-Fi" },
                    new GenreDTO() { Id = 3, Name = "Time Travel" }
                }
            };

            return genresList;
        }

        private BookEntity InitializeSingleBookEntity()
        {
            string title = "Killing Floor";
            var author = InitializeSinglePersonEntity();
            var genres = InitializeGenreEntityList();

            return new BookEntity() { Title = title, Author = author, Genres = genres };
        }

        private BookDTO InitializeSingleBookDTO()
        {
            string title = "Killing Floor";
            var author = InitializeSinglePersonDTO();
            var genres = InitializeGenreDTOList();

            return new BookDTO() { Title = title, Author = author, Genres = genres };
        }

        private List<BookEntity> InitializeBookEntityList()
        {
            string[] titles = { "Killing Floor", "Jurassic Park", "Flashforward" };
            var authors = InitializePersonEntityList();
            var genresList = InitializeGenreEntityListCollection();
            var bookEntities = new List<BookEntity>()
            {
                new BookEntity() { Title = titles[0], Author = authors[0], Genres = genresList[0] },
                new BookEntity() { Title = titles[1], Author = authors[1], Genres = genresList[1] },
                new BookEntity() { Title = titles[2], Author = authors[2], Genres = genresList[2] }
            };

            return bookEntities;
        }

        private List<BookDTO> InitializeBookDTOList()
        {
            string[] titles = { "Killing Floor", "Jurassic Park", "Flashforward" };
            var authors = InitializePersonDTOList();
            var genresList = InitializeGenreDTOListCollection();
            var bookDtos = new List<BookDTO>()
            {
                new BookDTO() { Title = titles[0], Author = authors[0], Genres = genresList[0] },
                new BookDTO() { Title = titles[1], Author = authors[1], Genres = genresList[1] },
                new BookDTO() { Title = titles[2], Author = authors[2], Genres = genresList[2] }
            };

            return bookDtos;
        }

        #endregion Test Object Builder Methods
    }
}