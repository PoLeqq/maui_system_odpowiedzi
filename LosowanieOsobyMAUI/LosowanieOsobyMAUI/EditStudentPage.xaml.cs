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

        // Ustaw wartoœci pól formularza na istniej¹ce dane studenta
        idEntry.Text = student.Id.ToString();
        nameEntry.Text = student.Name;
        surnameEntry.Text = student.Surname;
        clazzEntry.Text = student.Clazz;
        presentSwitch.IsToggled = student.Present;
    }

    private void OnSaveChangesClicked(object sender, EventArgs e)
    {
        // SprawdŸ, czy wprowadzone Id jest poprawne
        if(!(int.TryParse(idEntry.Text, out int newId) && newId > 0))
        {
            DisplayAlert("Error", "Niepoprawny numer w dzienniku. Podaj liczbê wiêksz¹ od 0!", "OK");
            return;
        }

        if(student.Id != newId && students.IsAnyDupe(newId, clazzEntry.Text))
        {
            DisplayAlert("B³¹d", $"Istnieje ju¿ uczeñ w klasie {clazzEntry.Text} i numerze {newId}!", "OK");
            return;
        }

        // Zapisz wprowadzone zmiany do istniej¹cego studenta
        student.Id = newId;
        student.Name = nameEntry.Text;
        student.Surname = surnameEntry.Text;
        student.Clazz = clazzEntry.Text;
        student.Present = presentSwitch.IsToggled;

        // Zapisz listê studentów do pliku po zaktualizowaniu danych
        students.SaveToFile();
        mainPage.UpdateView();

        // Powróæ do poprzedniego ekranu (MainPage)
        Navigation.PopAsync();
    }
}