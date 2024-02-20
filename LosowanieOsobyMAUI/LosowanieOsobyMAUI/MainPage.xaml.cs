using LosowanieOsobyMAUI.Objects;
using System.Diagnostics;

namespace LosowanieOsobyMAUI
{
    public partial class MainPage : ContentPage
    {
        Students students;
        int count = 0;
        private string selectedClass = "";
        private int? luckyNumber = null;
        private bool excludeAbsent = true;

        public MainPage()
        {
            InitializeComponent();
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            students = new Students();
            await students.LoadFromFile();
            UpdateView();
        }

        public void UpdateView()
        {
            UpdateListView();
            UpdatePickerView();
        }

        private void UpdatePickerView()
        {
            List<string> uniqueClasses = students.StudentList.Select(student => student.Clazz).Distinct().ToList();
            uniqueClasses.Insert(0, " ");

            classPicker.ItemsSource = uniqueClasses;
            //// Dodaj klasy do picker'a
            //foreach (var clazz in uniqueClasses)
            //{
            //    classPicker.Items.Add(clazz);
            //    Debug.WriteLine(clazz);
            //}
        }

        private void OnExcludeAbsentToggled(object sender, ToggledEventArgs e)
        {
            excludeAbsent = e.Value;
        }


        private void UpdateListView()
        {
            List<Student> filteredStudents;

            if (selectedClass == "")
                filteredStudents = students.StudentList;
            else
                filteredStudents = students.StudentList.Where(student => student.Clazz == selectedClass).ToList();

            // Sortuj i ustaw widok listy
            students.SortStudents();
            listView.ItemsSource = null;
            listView.ItemsSource = filteredStudents;
        }

        private void OnSubmitClicked(object sender, EventArgs e)
        {
            if(!(int.TryParse(idEntry.Text, out int id) && id > 0))
            {
                DisplayAlert("Błąd", "Niepoprawny numer w dzienniku. Podaj liczbę większą od 0!", "OK");
                return;
            }

            string name = nameEntry.Text;
            string surname = surnameEntry.Text;
            string clazz = clazzEntry.Text;
            bool isPresent = presentSwitch.IsToggled;

            if(string.IsNullOrEmpty(name))
            {
                DisplayAlert("Błąd", "Uzupełnij pole imię!", "OK");
                return;
            }
            if(string.IsNullOrEmpty(surname))
            {
                DisplayAlert("Błąd", "Uzupełnij pole nazwisko!", "OK");
                return;
            }
            if(string.IsNullOrEmpty(clazz))
            {
                DisplayAlert("Błąd", "Uzupełnij pole klasa!", "OK");
                return;
            }

            if(students.IsAnyDupe(id,clazz))
            {
                DisplayAlert("Błąd", $"Istnieje już uczeń w klasie {clazz} i numerze {id}!", "OK");
                return;
            }

            students.AddStudent(new Student(id, name, surname, clazz, isPresent, 0));
            students.SaveToFile();

            idEntry.Text = string.Empty;
            nameEntry.Text = string.Empty;
            surnameEntry.Text = string.Empty;
            clazzEntry.Text = string.Empty;
            presentSwitch.IsToggled = true;

            UpdateView();
        }

        private void OnClassSelected(object sender, EventArgs e)
        {
            // Pobierz zaznaczoną klasę z elementu interfejsu użytkownika (classPicker)
            if (classPicker.SelectedItem != null)
            {
                // Pobierz zaznaczoną klasę z elementu interfejsu użytkownika (classPicker)
                if (classPicker.SelectedItem.ToString() == " ")
                    selectedClass = "";
                else
                    selectedClass = classPicker.SelectedItem.ToString();
                UpdateListView();
            }
        }

        private void OnRemoveStudentClicked(object sender, EventArgs e)
        {
            // Pobierz przycisk, który został kliknięty
            Button button = (Button)sender;

            // Pobierz kontekst danych dla tego przycisku (czyli dane studenta)
            Student selectedStudent = button.BindingContext as Student;

            if (selectedStudent != null)
            {
                // Wywołaj funkcję usuwającą studenta
                students.RemoveStudent(selectedStudent);
                students.SaveToFile();

                // Zaktualizuj widok listy po usunięciu studenta
                UpdateView();
            }
        }

        private void OnEditStudentClicked(object sender, EventArgs e)
        {
            // Pobierz przycisk, który został kliknięty
            Button button = (Button)sender;

            // Pobierz kontekst danych dla tego przycisku (czyli dane studenta)
            Student selectedStudent = button.BindingContext as Student;

            if (selectedStudent != null)
            {
                // Przejdź do nowego ekranu edycji, przekazując dane studenta
                Navigation.PushAsync(new EditStudentPage(selectedStudent, students, this));
            }
        }

        private void GetRandomStudent(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedClass))
            {
                DisplayAlert("Błąd", $"Wybierz klasę, z której chcesz losować ucznia!", "OK");
                return;
            }

            var studentsInSelectedClass = students.StudentList.Where(student => student.Clazz == selectedClass).ToList();

            if (studentsInSelectedClass.Any())
            {
                // Wybierz losowego ucznia tylko z wybranej klasy, pomijając szczęśliwy numer
                Random random = new Random();
                int attemptsThreshold = 3;

                // Pomijaj uczniów, którzy przekroczyli próg liczby prób
                var availableStudents = studentsInSelectedClass
                    .Where(student => student.BanCounter <= 0)
                    .ToList();

                if (availableStudents.Any())
                {
                    int randomIndex = random.Next(availableStudents.Count);
                    Student randomStudent = availableStudents[randomIndex];

                    // Wyświetl informacje o losowym uczniu
                    DisplayAlert("Losowy uczeń", $"{randomStudent.Name} {randomStudent.Surname} (numer: {randomStudent.Id})", "OK");

                    // Wyklucz wybranego ucznia z losowań przez 3 kolejne próby
                    foreach (var student in studentsInSelectedClass)
                    {
                        if (student == randomStudent)
                        {
                            student.BanCounter = attemptsThreshold;
                        }
                        else if (student.BanCounter > 0)
                        {
                            student.BanCounter--;
                        }
                    }
                }
                else
                {
                    foreach (var student in studentsInSelectedClass)
                    {
                        if (student.BanCounter > 0)
                        {
                            student.BanCounter--;
                        }
                    }

                    // Jeśli brak dostępnych uczniów w wybranej klasie, wyświetl odpowiedni komunikat
                    DisplayAlert("Brak dostępnych uczniów", "Brak dostępnych uczniów w wybranej klasie do losowania.", "OK");
                }
                students.SaveToFile();
            }
            else
            {
                // Jeśli brak uczniów w wybranej klasie, wyświetl odpowiedni komunikat
                DisplayAlert("Brak uczniów", $"Brak uczniów w klasie {selectedClass} do losowania.", "OK");
            }
        }

        private void OnLuckyNumberChanged(object sender, TextChangedEventArgs e)
        {
            // Sprawdź, czy wpisany tekst w polu luckyNumberEntry jest liczbą i ustaw luckyNumber
            if (int.TryParse(luckyNumberEntry.Text, out int number))
            {
                luckyNumber = number;
            }
            else
            {
                luckyNumber = null;
            }
        }
    }
}