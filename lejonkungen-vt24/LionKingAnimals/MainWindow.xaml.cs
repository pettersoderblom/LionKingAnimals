using LionKingAnimals.DAL;
using LionKingAnimals.Models;
using System.Windows;
using System.Windows.Controls;

namespace LionKingAnimals
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly dbRepository dbRepository = new();

        public MainWindow()
        {
            InitializeComponent();
            _ = StartUp();
        }

        public async Task StartUp()
        {
            await UpdateInfo();
        }

        // Updates GUI
        public async Task UpdateInfo()
        {
            await UpdateAnimalList();
            await UpdateNumberOfAnimals();
            await UpdateAnimals();
            await UpdateSpecies();
            await UpdateClasses();
            await UpdateComboBoxes();
        }
        public async Task UpdateComboBoxes()
        {
            List<ClassInfo> classes = await dbRepository.ReadAllAnimalClasses();
            List<SpeciesInfo> species = await dbRepository.ReadSpecies();

            comboBoxClass.ItemsSource = null;
            comboBoxClass.ItemsSource = classes;
            comboBoxShowClass.ItemsSource = null;
            comboBoxShowClass.ItemsSource = classes;
            comboBoxSpecies.ItemsSource = null;
            comboBoxSpecies.ItemsSource = species;
        }
        public async Task UpdateNumberOfAnimals()
        {
            List<int> numberOfAnimals = await dbRepository.ReadNumberStatistics();

            labelNumberOfAnimals.Content = numberOfAnimals[0].ToString();
            labelNumberOfSpecies.Content = numberOfAnimals[1].ToString();
            labelNumberOfClasses.Content = numberOfAnimals[2].ToString();
        }
        public async Task UpdateAnimals()
        {
            List<AnimalInfo> animal = await dbRepository.ReadAllAnimals();

            listBoxAnimals.ItemsSource = null;
            listBoxAnimals.ItemsSource = animal;
        }
        public async Task UpdateSpecies()
        {
            List<SpeciesInfo> speciesInfo = await dbRepository.ReadSpecies();

            listBoxSpecies.ItemsSource = null;
            listBoxSpecies.ItemsSource = speciesInfo;
        }
        public async Task UpdateClasses()
        {
            List<ClassInfo> classInfo = await dbRepository.ReadClasses();

            listBoxClasses.ItemsSource = null;
            listBoxClasses.ItemsSource = classInfo;
        }
        public async Task UpdateAnimalList()
        {
            if (radioShowAllAnimals.IsChecked == true)
            {
                await ShowAllAnimals();
            }
            if (radioShowAllAnimalsWithNames.IsChecked == true)
            {
                await ShowAllAnimalsWithNames();
            }
            else if (radioShowAllAnimalsInClass.IsChecked == true)
            {
                if (comboBoxShowClass.SelectedIndex != -1)
                {
                    ClassInfo classInfo = (ClassInfo)comboBoxShowClass.SelectedItem;
                    await ShowAllAnimalsInClass(classInfo.Name);
                }
            }
        }

        private async Task ShowAllAnimals()
        {
            List<AnimalInfo> animals = await dbRepository.ReadAllAnimals();
            listbox.ItemsSource = null;
            listbox.ItemsSource = animals;
        }
        private async Task ShowAllAnimalsWithNames()
        {
            List<AnimalInfo> animals = await dbRepository.ReadAllAnimalsWithName();
            listbox.ItemsSource = null;
            listbox.ItemsSource = animals;
        }
        private async Task ShowAllAnimalsInClass(string name)
        {
            List<AnimalInfo> animals = await dbRepository.ReadAllAnimalsInClass(name);
            listbox.ItemsSource = null;
            listbox.ItemsSource = animals;
        }

        private async void RadioButtonClick(object sender, RoutedEventArgs e)
        {
            await UpdateAnimalList();
        }
        private void DoubleClickListBox(object sender, RoutedEventArgs e)
        {
            ListBox listbox = (ListBox)sender;
            AnimalInfo animal = (AnimalInfo)listbox.SelectedItem;
            txtSearchName.Text = animal.Name;
            txtSearchSpecies.Text = animal.Species;
            txtSearchLatin.Text = animal.LatinSpecies;
            txtSearchClass.Text = animal.Class;
        }
        private async void ComboboxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (radioShowAllAnimalsInClass.IsChecked == true)
            {
                if (comboBoxShowClass.SelectedIndex == -1) return;
                ClassInfo classInfo = (ClassInfo)comboBoxShowClass.SelectedItem;
                await ShowAllAnimalsInClass(classInfo.Name);
            }
        }

        // Real-time search
        private async void AnimalTextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtAnimalName.Text.Length == 0)
            {
                await UpdateAnimals();
            }
            else
            {
                List<AnimalInfo> animalInfo = await dbRepository.ReadAnimalsFromString(txtAnimalName.Text);
                listBoxAnimals.ItemsSource = null;
                listBoxAnimals.ItemsSource = animalInfo;
            }
        }
        private async void SpeciesTextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSpeciesName.Text.Length == 0)
            {
                await UpdateSpecies();
            }
            else
            {
                List<SpeciesInfo> speciesInfo = await dbRepository.ReadSpeciesFromString(txtSpeciesName.Text);
                listBoxSpecies.ItemsSource = null;
                listBoxSpecies.ItemsSource = speciesInfo;
            }
        }
        private async void ClassTextChanged(object sender, TextChangedEventArgs e) 
        {
            if (txtClassName.Text.Length == 0)
            {
                await UpdateClasses();
            }
            else
            {
                List<ClassInfo> classInfo = await dbRepository.ReadClassesFromString(txtClassName.Text);
                listBoxClasses.ItemsSource = null;
                listBoxClasses.ItemsSource = classInfo;
            }
        }

        // Calling database
        private async void AddAnimal(object sender, RoutedEventArgs e)
        {
            string? name = string.IsNullOrEmpty(txtAnimalName.Text) ? null : txtAnimalName.Text;
            SpeciesInfo speciesInfo = (SpeciesInfo)comboBoxSpecies.SelectedItem;

            if (comboBoxSpecies.SelectedIndex == -1)
            {
                MessageBox.Show("You must connect animal to a species.", "Choose species", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool success = await dbRepository.CreateAnimal(name, speciesInfo.Id);

            if (success)
            {
                await UpdateInfo();
                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show($"Successfully added a {speciesInfo.Name}.", "Confirmed creation", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Successfully added {name} as a {speciesInfo.Name}.", "Confirmed creation", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                txtAnimalName.Clear();
            }
        }
        private async void AddSpecies(object sender, RoutedEventArgs e)
        {
            string name = txtSpeciesName.Text;
            string? latinName = txtInfoSpeciesLatinName.Text.Length == 0 ? null : txtInfoSpeciesLatinName.Text;

            if (comboBoxClass.SelectedIndex == -1)
            {
                MessageBox.Show("You must connect species to a class.", "Choose class", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ClassInfo classInfo = (ClassInfo)comboBoxClass.SelectedItem;

            bool success = await dbRepository.CreateSpecies(name, classInfo.Id, latinName);

            if(success)
            {
                await UpdateInfo();
                MessageBox.Show($"The species {name} have been added to the {classInfo.Name} class.", "Confirmed creation", MessageBoxButton.OK, MessageBoxImage.Information);
                txtSpeciesName.Clear();
            }
        }
        private async void AddClass(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtClassName.Text)) return;

            bool success = await dbRepository.CreateClass(txtClassName.Text);

            if (success)
            {
                await UpdateInfo();
                MessageBox.Show($"The class {txtClassName.Text} have been added.", "Confirmed creation", MessageBoxButton.OK, MessageBoxImage.Information);
                txtClassName.Clear();
            }
        }
        private async void SearchAnimal(object sender, RoutedEventArgs e)
        {
            string name = txtSearch.Text;
            if (string.IsNullOrEmpty(name))
            {
                await UpdateAnimalList();
                return;
            }

            List<AnimalInfo> animalInfos = await dbRepository.ReadAnimal(name);
            if (animalInfos.Count == 0)
            {
                MessageBox.Show($"Could not find any animal with the name {name}.", "Name doesn't exist" ,MessageBoxButton.OK, MessageBoxImage.Error);
            }

            listbox.ItemsSource = null;
            listbox.ItemsSource = animalInfos;
            txtSearch.Clear();
        }
        private async void UpdateAnimal(object sender, RoutedEventArgs e)
        {
            string name = txtSearchName.Text;
            string species = txtSearchSpecies.Text;

            AnimalInfo oldAnimal = (AnimalInfo)listbox.SelectedItem;
            if (oldAnimal is null) return;

            bool success = await dbRepository.UpdateAnimal(oldAnimal.Id, name, species);

            if (success)
            {
                MessageBox.Show($"The animal info where updated for {name}", "Confirmed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private async void DeleteAnimal(object sender, RoutedEventArgs e)
        {
            AnimalInfo animalInfo = (AnimalInfo)listBoxAnimals.SelectedItem;

            if (animalInfo is null) return;

            bool success = await dbRepository.DeleteAnimal(animalInfo);

            if (success)
            {
                MessageBox.Show($"Deletion was successfull!", "Confirmed delition", MessageBoxButton.OK, MessageBoxImage.Information);
                await UpdateInfo();
            }
        }
        private async void DeleteSpecies(object sender, RoutedEventArgs e)
        {
            SpeciesInfo speciesInfo = (SpeciesInfo)listBoxSpecies.SelectedItem;

            if (speciesInfo is null) return; // lämnar metoden

            bool success = await dbRepository.DeleteSpecies(speciesInfo);

            if (success)
            {
                MessageBox.Show($"Deletion was successfull!", "Confirmed delition", MessageBoxButton.OK, MessageBoxImage.Information);
                await UpdateInfo();
            }
        }
        private async void DeleteClass(object sender, RoutedEventArgs e)
        {

            ClassInfo classInfo = (ClassInfo)listBoxClasses.SelectedItem;
            if (classInfo is null) return;

            bool success = await dbRepository.DeleteClass(classInfo);

            if (success)
            {
                MessageBox.Show($"Deletion was successfull!", "Confirmed delition", MessageBoxButton.OK, MessageBoxImage.Information);
                await UpdateInfo();
            }
        }
    }
}