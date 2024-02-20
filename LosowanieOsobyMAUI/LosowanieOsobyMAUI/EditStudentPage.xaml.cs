using LosowanieOsobyMAUI.Objects;

namespace LosowanieOsobyMAUI;

public partial class EditStudentPage : ContentPage
{
    private Student student;
    private Students students;
    private MainPage mainPage;

    public EditStudentPage(Student student, Students students, MainPage mainPage)
    {
        InitializeComponent();
        this.student = student;
        this.students = students;
        this.mainPage = mainPage;

        // Ustaw warto�ci p�l formularza na istniej�ce dane studenta
        idEntry.Text = student.Id.ToString();
        nameEntry.Text = student.Name;
        surnameEntry.Text = student.Surname;
        clazzEntry.Text = student.Clazz;
        presentSwitch.IsToggled = student.Present;
    }

    private void OnSaveChangesClicked(object sender, EventArgs e)
    {
        // Sprawd�, czy wprowadzone Id jest poprawne
        if(!(int.TryParse(idEntry.Text, out int newId) && newId > 0))
        {
            DisplayAlert("Error", "Niepoprawny numer w dzienniku. Podaj liczb� wi�ksz� od 0!", "OK");
            return;
        }

        if(student.Id != newId && students.IsAnyDupe(newId, clazzEntry.Text))
        {
            DisplayAlert("B��d", $"Istnieje ju� ucze� w klasie {clazzEntry.Text} i numerze {newId}!", "OK");
            return;
        }

        // Zapisz wprowadzone zmiany do istniej�cego studenta
        student.Id = newId;
        student.Name = nameEntry.Text;
        student.Surname = surnameEntry.Text;
        student.Clazz = clazzEntry.Text;
        student.Present = presentSwitch.IsToggled;

        // Zapisz list� student�w do pliku po zaktualizowaniu danych
        students.SaveToFile();
        mainPage.UpdateView();

        // Powr�� do poprzedniego ekranu (MainPage)
        Navigation.PopAsync();
    }
}