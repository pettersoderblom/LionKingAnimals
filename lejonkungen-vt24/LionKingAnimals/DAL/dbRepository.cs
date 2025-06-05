using LionKingAnimals.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Windows;

namespace LionKingAnimals.DAL
{

    internal class dbRepository
    {
        private readonly string _connectionString;

        public dbRepository()
        {
            var config = new ConfigurationBuilder()
            .AddUserSecrets<dbRepository>()
            .Build();
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        /* C = Create new entries of the database
         * R = Read data about entries in the database
         * U = Update existing data in the database
         * D = Delete existing data in the database
        */

        /* Create
         * 1: Add animal by name + species
         * 2: Add species with or without latin name connected to class
         * 3: Add class
        */

        public async Task<bool> CreateAnimal(string? name, int speciesId) // 1
        {
            bool success = false;
            try
            {
                using NpgsqlConnection connection = new(_connectionString);
                connection.Open();
                await CreateAnimalQuery(connection, name, speciesId);
                success = true;
            }
            catch (PostgresException)
            {
                MessageBox.Show($"Something went wrong with the database.");
            }
            return success;
        }
        public async Task<bool> CreateSpecies(string name, int classId, string? nameLatin = null) // 2
        {
            bool success = false;
            try
            {
                using NpgsqlConnection connection = new(_connectionString);
                connection.Open();
                await CreateSpeciesQuery(connection, name, classId, nameLatin);
                success = true;
            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == "23505") // unique_violation
                {
                    MessageBox.Show($"The species {name} already exists in this database.");
                }
                else
                {
                    MessageBox.Show($"Something went wrong with the database.");
                }
            }
            return success;
        }
        public async Task<bool> CreateClass(string name) // 3
        {
            bool success = false;
            try
            {
                using NpgsqlConnection connection = new(_connectionString);
                connection.Open();
                await CreateClassQuery(connection, name);
                success = true;
            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == "23505") // unique_violation
                {
                    MessageBox.Show($"The class {name} already exists in this database.");
                }
                else
                {
                    MessageBox.Show($"Something went wrong with the database.");
                }
            }
            return success;
        }

        // Queries
        public static async Task CreateAnimalQuery(NpgsqlConnection connection, string? name, int speciesId)
        {
            using NpgsqlCommand command = new();
            command.CommandText = @"INSERT INTO animal(name, animal_species_id, date_created, date_last_changed)
                                     VALUES (@name, @animal_species_id ,NOW() ,NOW());";
            command.Parameters.AddWithValue("@name", string.IsNullOrEmpty(name) ? DBNull.Value : name);
            command.Parameters.AddWithValue("@animal_species_id", speciesId);
            command.Connection = connection;
            await command.ExecuteNonQueryAsync();
        }
        public static async Task CreateSpeciesQuery(NpgsqlConnection connection, string name, int classId, string? latinName)
        {
            using NpgsqlCommand command = new();
            command.CommandText = @"INSERT INTO animal_species(name, name_latin, animal_class_id, date_created, date_last_changed)
                                            VALUES (@name, @name_latin, @animal_class_id, NOW(), NOW())";
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@name_latin", latinName is null ? DBNull.Value : latinName);
            command.Parameters.AddWithValue("@animal_class_id", classId);
            command.Connection = connection;
            await command.ExecuteNonQueryAsync();
        }
        public static async Task CreateClassQuery(NpgsqlConnection connection, string name)
        {
            using NpgsqlCommand command = new();
            command.CommandText = @"INSERT INTO animal_class(name, date_created, date_last_changed)
                                            VALUES (@name, NOW(), NOW())";
            command.Parameters.AddWithValue("@name", name);
            command.Connection = connection;
            await command.ExecuteNonQueryAsync();
        }

        /* READ
         * 1. Get list of all animals that matches search
         * 2. Get list of all animals
         * 3. Get list of all animals with names
         * 4. Get list of all classes sorted by name
         * 5. Get list of all animals in specific class
         * 6. Get list of all species sorted by name
         * 7. Get list of all classes sorted by amount of animals in them
         * 8. Get number of classes, species and animals
        */

        public async Task<List<AnimalInfo>> ReadAnimal(string name) // 1
        {
            List<AnimalInfo> animals = [];
            try
            {
                using NpgsqlConnection connection = new(_connectionString);
                connection.Open();
                animals = await ReadAnimalQuery(connection, name);
            }
            catch (PostgresException)
            {
                MessageBox.Show($"Something went wrong with the database.");
            }
            return animals;
        }
        public async Task<List<AnimalInfo>> ReadAllAnimals() // 2
        {
            List<AnimalInfo> animals = [];
            try
            {
                using NpgsqlConnection connection = new(_connectionString);
                connection.Open();
                animals = await ReadAllAnimalsQuery(connection);
            }
            catch (PostgresException)
            {
                MessageBox.Show($"Something went wrong with the database.");
            }
            return animals;
        }
        public async Task<List<AnimalInfo>> ReadAllAnimalsWithName() // 3
        {
            List<AnimalInfo> animals = [];
            try
            {
                using NpgsqlConnection connection = new(_connectionString);
                connection.Open();
                animals = await ReadAllAnimalsWithName(connection);
            }
            catch (PostgresException)
            {
                MessageBox.Show($"Something went wrong with the database.");
            }
            return animals;
        }
        public async Task<List<ClassInfo>> ReadAllAnimalClasses() // 4
        {
            List<ClassInfo> classes = [];
            try
            {
                using NpgsqlConnection connection = new(_connectionString);
                connection.Open(); 
                classes = await ReadAllAnimalClassesQuery(connection);
            }
            catch (PostgresException)
            {
                MessageBox.Show($"Something went wrong with the database.");
            }
            return classes;
        }
        public async Task<List<AnimalInfo>> ReadAllAnimalsInClass(string name) // 5
        {
            List<AnimalInfo> animals = [];
            try
            {
                using NpgsqlConnection connection = new(_connectionString);
                connection.Open();
                animals = await ReadAllAnimalsInClassQuery(connection, name);
            }
            catch (PostgresException)
            {
                MessageBox.Show($"Something went wrong with the database.");
            }
            return animals;
        }
        public async Task<List<SpeciesInfo>> ReadSpecies() // 6
        {
            List<SpeciesInfo> species = [];
            try
            {
                using NpgsqlConnection connection = new(_connectionString);
                connection.Open();
                species = await ReadSpeciesQuery(connection);
            }
            catch (PostgresException)
            {
                MessageBox.Show($"Something went wrong with the database.");
            }
            return species;
        }
        public async Task<List<ClassInfo>> ReadClasses() // 7
        {
            List<ClassInfo> classes = [];
            try
            {
                using NpgsqlConnection connection = new(_connectionString);
                connection.Open();
                classes = await ReadClassesQuery(connection);
            }
            catch (PostgresException)
            {
                MessageBox.Show($"Something went wrong with the database.");
            }
            return classes;
        }
        public async Task<List<int>> ReadNumberStatistics() // 8
        {
            List<int> stats = [];
            try
            {
                using NpgsqlConnection connection = new(_connectionString);
                connection.Open();
                stats = await ReadNumberStatisticsQuery(connection);
            }
            catch (PostgresException)
            {
                MessageBox.Show($"Something went wrong with the database.");
            }
            return stats;
        }
        // Queries
        public static async Task<List<AnimalInfo>> ReadAnimalQuery(NpgsqlConnection connection, string name) // 1q
        {
            using NpgsqlCommand command = new();
            command.CommandText = @"SELECT a.id AS id, a.name AS animal_name, s.name AS species_name, s.name_latin AS latin_species, c.name AS class_name
                                    FROM animal a
                                    JOIN animal_species s ON a.animal_species_id = s.id
                                    JOIN animal_class c ON s.animal_class_id = c.id
                                    WHERE a.name ~* @name";
            command.Parameters.AddWithValue("@name", name);
            command.Connection = connection;
            List<AnimalInfo> animalInfos = [];
            using (var reader = command.ExecuteReader())
            {
                while (await reader.ReadAsync())
                {
                    AnimalInfo animalInfo = new()
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["animal_name"].ToString(),
                        Species = reader["species_name"].ToString(),
                        LatinSpecies = reader["latin_species"].ToString(),
                        Class = reader["class_name"].ToString()
                    };
                    animalInfos.Add(animalInfo);
                }
            }
            return animalInfos;
        }
        public static async Task<List<AnimalInfo>> ReadAllAnimalsQuery(NpgsqlConnection connection) // 2q
        {
            using NpgsqlCommand command = new();
            command.CommandText = @"SELECT a.id AS id, a.name AS animal_name, s.name AS species_name, s.name_latin AS name_latin, c.name AS class_name
                                    FROM animal a
                                    JOIN animal_species s ON a.animal_species_id = s.id
                                    JOIN animal_class c ON s.animal_class_id = c.id
                                    ORDER BY species_name ASC;";
            command.Connection = connection;

            List<AnimalInfo> animals = [];
            using (var reader = command.ExecuteReader())
            {
                while(await reader.ReadAsync())
                {
                    AnimalInfo animal = new()
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["animal_name"].ToString(),
                        Species = reader["species_name"].ToString(),
                        LatinSpecies = reader["name_latin"].ToString(),
                        Class = reader["class_name"].ToString()
                    };
                    if (string.IsNullOrEmpty(animal.Name)) animal.Name = null;
                    if (string.IsNullOrEmpty(animal.LatinSpecies)) animal.LatinSpecies = null;
                    animals.Add(animal);
                }
            }
            return animals;
        }
        public static async Task<List<AnimalInfo>> ReadAllAnimalsWithName(NpgsqlConnection connection) //3q
        {
            using NpgsqlCommand command = new();
            command.CommandText = @"SELECT a.id AS id, a.name AS animal_name, s.name AS species_name, s.name_latin AS name_latin, c.name AS class_name
                                    FROM animal a
                                    JOIN animal_species s ON a.animal_species_id = s.id
                                    JOIN animal_class c ON s.animal_class_id = c.id
                                    WHERE a.name IS NOT NULL
                                    ORDER BY animal_name ASC;";
            command.Connection = connection;

            List<AnimalInfo> animals = [];
            using (var reader = command.ExecuteReader())
            {
                while (await reader.ReadAsync())
                {
                    AnimalInfo animal = new()
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["animal_name"].ToString(),
                        Species = reader["species_name"].ToString(),
                        LatinSpecies = reader["name_latin"].ToString(),
                        Class = reader["class_name"].ToString()
                    };
                    if (string.IsNullOrEmpty(animal.Name)) animal.Name = null;
                    if (string.IsNullOrEmpty(animal.LatinSpecies)) animal.LatinSpecies = null;
                    animals.Add(animal);
                }
            }
            return animals;
        }
        public static async Task<List<ClassInfo>> ReadAllAnimalClassesQuery(NpgsqlConnection connection) // 4q
        {
            using NpgsqlCommand command = new();
            command.CommandText = @"SELECT c.id, c.name, COUNT(*) 
                                    FROM animal_class c
                                    LEFT JOIN animal_species s ON c.id = s.animal_class_id
                                    LEFT JOIN animal a ON s.id = a.animal_species_id
                                    GROUP BY c.name, c.id
                                    ORDER BY c.name";
            command.Connection = connection;
            List<ClassInfo> animalClasses = [];
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    ClassInfo animal = new()
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        Animals = Convert.ToInt32(reader["count"])
                    };
                    animalClasses.Add(animal);
                }
            }
            return animalClasses;
        }
        public static async Task<List<AnimalInfo>> ReadAllAnimalsInClassQuery(NpgsqlConnection connection, string name) // 5q
        {
            using NpgsqlCommand command = new();
            command.CommandText = @"SELECT a.id AS id, a.name AS animal, s.name AS species, s.name_latin AS latin
                                    FROM animal_class c
                                    JOIN animal_species s ON c.id = s.animal_class_id
                                    JOIN animal a ON s.id = a.animal_species_id
                                    WHERE c.name = @name
                                    ORDER BY s.name ASC";
            command.Parameters.AddWithValue("@name", name);
            command.Connection = connection;
            List<AnimalInfo> animals = [];
            using (var reader = command.ExecuteReader())
            {
                while (await reader.ReadAsync())
                {
                    AnimalInfo animal = new()
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["animal"].ToString(),
                        Species = reader["species"].ToString(),
                        LatinSpecies = reader["latin"].ToString(),
                        Class = name 
                    };
                    if (string.IsNullOrEmpty(animal.Name)) animal.Name = null;
                    if (string.IsNullOrEmpty(animal.LatinSpecies)) animal.LatinSpecies = null;
                    animals.Add(animal);
                }
            }
            return animals;
        }
        public static async Task<List<SpeciesInfo>> ReadSpeciesQuery(NpgsqlConnection connection) // 6q
        {
            using NpgsqlCommand command = new();
            command.CommandText = @"SELECT s.id, s.name, COUNT(a.id)
                                    FROM animal_species s
                                    LEFT JOIN animal a ON s.id = a.animal_species_id
                                    GROUP BY s.name, s.id
                                    ORDER BY s.name;";
            command.Connection = connection;

            List<SpeciesInfo> allSpecies = [];
            using (var reader = command.ExecuteReader())
            {
                while (await reader.ReadAsync())
                {
                    SpeciesInfo species = new()
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        Animals = Convert.ToInt32(reader["count"])
                    };
                    allSpecies.Add(species);
                }
            }
            return allSpecies;
        }
        public static async Task<List<ClassInfo>> ReadClassesQuery(NpgsqlConnection connection) // 7q
        {
            using NpgsqlCommand command = new();
            command.CommandText = @"SELECT c.id, c.name, COUNT(a.id)
                                    FROM animal_class c
                                    LEFT JOIN animal_species s ON c.id = s.animal_class_id
                                    LEFT JOIN animal a ON s.id = a.animal_species_id
                                    GROUP BY c.name, c.id
                                    ORDER BY COUNT DESC, c.name;";
            command.Connection = connection;

            List<ClassInfo> classes = [];
            using (var reader = command.ExecuteReader())
            {
                while (await reader.ReadAsync())
                {
                    ClassInfo classInfo = new()
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        Animals = Convert.ToInt32(reader["count"])

                    };
                    classes.Add(classInfo);
                }
            }
            return classes;
        }
        public static async Task<List<int>> ReadNumberStatisticsQuery(NpgsqlConnection connection) // 8q
        {
            using NpgsqlCommand command = new();
            command.CommandText = $@"SELECT COUNT(a.id) AS animals, COUNT(DISTINCT s.id) AS species, COUNT(DISTINCT c.id) AS classes
                                        FROM animal_class c
                                        JOIN animal_species s ON c.id = s.animal_class_id
                                        JOIN animal a ON s.id = a.animal_species_id;";
            command.Connection = connection;

            List<int> numberOfAnimals = [];
            using (var reader = command.ExecuteReader())
            {
                while (await reader.ReadAsync())
                {
                    numberOfAnimals.Add(Convert.ToInt32(reader["animals"]));
                    numberOfAnimals.Add(Convert.ToInt32(reader["species"]));
                    numberOfAnimals.Add(Convert.ToInt32(reader["classes"]));
                }
            }
            connection.Close();
            return numberOfAnimals;
        }

        // Real time search
        public async Task<List<AnimalInfo>> ReadAnimalsFromString(string data)
        {
            List<AnimalInfo> animals = [];
            try
            {
                using NpgsqlConnection connection = new(_connectionString);
                connection.Open();
                animals = await ReadAnimalsFromStringQuery(connection, data);
            }
            catch (PostgresException)
            {
                MessageBox.Show($"Something went wrong with the database.");
            }
            return animals;
        }
        public async Task<List<SpeciesInfo>> ReadSpeciesFromString(string data)
        {
            List<SpeciesInfo> species = [];
            try
            {
                using NpgsqlConnection connection = new(_connectionString);
                connection.Open();
                species = await ReadSpeciesFromStringQuery(connection, data);
            }
            catch (PostgresException)
            {
                MessageBox.Show($"Something went wrong with the database.");
            }
            return species;
        }
        public async Task<List<ClassInfo>> ReadClassesFromString(string data)
        {
            List<ClassInfo> classes = [];
            try
            {
                using NpgsqlConnection connection = new(_connectionString);
                connection.Open();
                classes = await ReadClassesFromStringQuery(connection, data);
            }
            catch (PostgresException)
            {
                MessageBox.Show($"Something went wrong with the database.");
            }
            return classes;
        }
        public static async Task<List<AnimalInfo>> ReadAnimalsFromStringQuery(NpgsqlConnection connection, string data)
        {
            using NpgsqlCommand command = new();
            command.CommandText = @"SELECT a.id AS id, a.name AS name, s.name AS species, s.name_latin AS latinSpecies, c.name AS class
                                    FROM animal_class c
                                    LEFT JOIN animal_species s ON c.id = s.animal_class_id
                                    LEFT JOIN animal a ON s.id = a.animal_species_id
                                    WHERE a.name ~* @data
                                    ORDER BY a.name ASC";
            command.Parameters.AddWithValue("@data", data);
            command.Connection = connection;

            List<AnimalInfo> animalInfo = [];
            using (var reader = command.ExecuteReader())
            {
                while (await reader.ReadAsync())
                {
                    AnimalInfo animal = new()
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        Species = reader["species"].ToString(),
                        LatinSpecies = reader["latinSpecies"].ToString(),
                        Class = reader["class"].ToString()
                    };
                    animalInfo.Add(animal);
                }
            }
            return animalInfo;
        }
        public static async Task<List<SpeciesInfo>> ReadSpeciesFromStringQuery(NpgsqlConnection connection, string data)
        {
            using NpgsqlCommand command = new();
            command.CommandText = @"SELECT s.id, s.name, COUNT(a.id) 
                                    FROM animal_species s
                                    LEFT JOIN animal a ON s.id = a.animal_species_id
                                    GROUP BY s.name, s.id
                                    HAVING s.name ~* @data
                                    ORDER BY COUNT DESC, s.name ASC";
            command.Parameters.AddWithValue("@data", data);
            command.Connection = connection;

            List<SpeciesInfo> speciesInfo = [];
            using (var reader = command.ExecuteReader())
            {
                while (await reader.ReadAsync())
                {
                    SpeciesInfo species = new()
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        Animals = Convert.ToInt32(reader["count"])
                    };
                    speciesInfo.Add(species);
                }
            }
            return speciesInfo;
        }
        public static async Task<List<ClassInfo>> ReadClassesFromStringQuery(NpgsqlConnection connection, string data)
        {
            using NpgsqlCommand command = new();
            command.CommandText = @"SELECT c.id, c.name, COUNT(a.id) 
                                    FROM animal_class c
                                    LEFT JOIN animal_species s ON c.id = s.animal_class_id
                                    LEFT JOIN animal a ON s.id = a.animal_species_id
                                    GROUP BY c.name, c.id
                                    HAVING c.name ~* @data
                                    ORDER BY COUNT DESC, c.name ASC";
            command.Parameters.AddWithValue("@data", data);
            command.Connection = connection;

            List<ClassInfo> classInfo = [];
            using (var reader = command.ExecuteReader())
            {
                while (await reader.ReadAsync())
                {
                    ClassInfo temp = new()
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        Animals = Convert.ToInt32(reader["count"])
                    };
                    classInfo.Add(temp);
                }
            }
            return classInfo;
        }

        /* Update
         * 1. Update name and species in any animal
        */

        public async Task<bool> UpdateAnimal(int id, string name, string species) // 1
        {
            bool success = false;
            try
            {
                using NpgsqlConnection connection = new(_connectionString);
                connection.Open();
                await UpdateAnimalQuery(connection, id, name, species);
                success = true;
            }
            catch (PostgresException ex)
            {
                if (ex.SqlState == "23502") // not_null_violation
                {
                    MessageBox.Show($"Must connect animal to existing species.");
                }
                else
                {
                    MessageBox.Show($"Something went wrong with the database.");
                }
            }
            return success;
        }

        // Queries
        public static async Task UpdateAnimalQuery(NpgsqlConnection connection, int id, string name, string species) // 1q
        {
            using NpgsqlCommand command = new();
            command.CommandText = @"UPDATE animal
                                    SET name = @name, animal_species_id =
                                    (
	                                    SELECT id
	                                    FROM animal_species
	                                    WHERE name = @species
                                    )
                                    WHERE id = @id;";
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@species", species);
            command.Parameters.AddWithValue("@id", id);
            command.Connection = connection;
            await command.ExecuteNonQueryAsync();
        }


        /* Delete
         * 1. Delete animal by name
         * 2. Delete species and all animals under it
         * 3. Delete class and all species and animals under it
        */

        public async Task<bool> DeleteAnimal(AnimalInfo animalInfo) // 1
        {
            bool success = false;
            try
            {
                using NpgsqlConnection connection = new(_connectionString);
                connection.Open();
                if (ConfirmDeletionAnimal(animalInfo))
                {
                    await DeleteAnimalQuery(connection, animalInfo.Id);
                    success = true;
                }
            }
            catch (PostgresException)
            {
                MessageBox.Show($"Something went wrong with the database.");
            }
            return success;
        }
        public async Task<bool> DeleteSpecies(SpeciesInfo speciesInfo) // 2
        {
            bool successs = false;
            try
            {
                using NpgsqlConnection connection = new(_connectionString);
                connection.Open();
                if (ConfirmDeletionSpecies(speciesInfo.Animals, speciesInfo.Name))
                {
                    await DeleteSpeciesQuery(connection, speciesInfo.Id);
                    successs = true;
                }
            }
            catch (PostgresException)
            {
                MessageBox.Show($"Something went wrong with the database.");
            }
            return successs;
        }
        public async Task<bool> DeleteClass(ClassInfo classInfo) // 3
        {
            bool success = false;
            try
            {
                using NpgsqlConnection connection = new(_connectionString);
                connection.Open();
                if (ConfirmDeletionClass(classInfo.Animals, classInfo.Name))
                {
                    await DeleteClassQuery(connection, classInfo.Id);
                    success = true;
                }
            }
            catch (PostgresException)
            {
                MessageBox.Show($"Something went wrong with the database.");
            }
            return success;
        }

        // Queries
        public static async Task DeleteAnimalQuery(NpgsqlConnection connection, int id)
        {
            using NpgsqlCommand command = new();
            command.CommandText = @"DELETE FROM animal WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            command.Connection = connection;
            await command.ExecuteNonQueryAsync();
        }
        public static async Task DeleteSpeciesQuery(NpgsqlConnection connection, int id)
        {
            using NpgsqlCommand command = new();
            command.CommandText = @"DELETE FROM animal_species
                                    WHERE id = @id;";
            command.Parameters.AddWithValue("@id", id);
            command.Connection = connection;
            await command.ExecuteNonQueryAsync();
        }
        public static async Task DeleteClassQuery(NpgsqlConnection connection, int id)
        {
            using NpgsqlCommand command = new();
            command.CommandText = @"DELETE FROM animal_class 
                                        WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);
            command.Connection = connection;
            await command.ExecuteNonQueryAsync();
        }

        // Confirmation when deleting animal, species or class with data inside of them
        private static bool ConfirmDeletionAnimal(AnimalInfo animalInfo)
        {
            MessageBoxResult messageBoxResult;
            if (string.IsNullOrEmpty(animalInfo.Name))
            {
                 messageBoxResult = MessageBox.Show
                    ($"Delete one of the {animalInfo.Species}?",
                    "Confirm deletion",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);
            }
            else
            {
                messageBoxResult = MessageBox.Show
                    ($"Delete {animalInfo.Name} from the {animalInfo.Species}?",
                    "Confirm deletion",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);
            }
            return messageBoxResult == MessageBoxResult.Yes;
        }
        private static bool ConfirmDeletionSpecies(int amountOfAnimals, string name)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show
                ($"Delete {amountOfAnimals} animals and the species {name}?",
                "Confirm deletion",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return messageBoxResult == MessageBoxResult.Yes;
        }
        private static bool ConfirmDeletionClass(int amountOfAnimals, string name)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show
                ($"Delete {amountOfAnimals} animals and the class {name}?",
                "Confirm deletion",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return messageBoxResult == MessageBoxResult.Yes;
        }
    }
}
