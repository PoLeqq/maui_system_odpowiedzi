using System.Diagnostics;
using System.Text.Json;

namespace LosowanieOsobyMAUI.Objects;

public class Students
{
    public List<Student> StudentList { get; set; }

    // Konstruktor
    public Students()
    {
        StudentList = new List<Student>();
    }

    // Dodaj studenta do listy
    public void AddStudent(Student student)
    {
        StudentList.Add(student);
        SortStudents();
    }

    public void RemoveStudent(Student student)
    {
        StudentList.Remove(student);
    }

    public void SortStudents()
    {
        StudentList = StudentList.OrderBy(student => student.Clazz).ThenBy(student => student.Id).ToList();
    }

    // Odczytaj listę studentów z pliku
    public async Task LoadFromFile()
    {
        try
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "students.txt");

            if (File.Exists(filePath))
            {
                string jsonContent = await File.ReadAllTextAsync(filePath);
                StudentList = JsonSerializer.Deserialize<List<Student>>(jsonContent);
            }
            Debug.WriteLine("Załadowano z pliku!");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Błąd podczas odczytu z pliku: {ex.Message}");
        }
    }

    // Zapisz listę studentów do pliku
    public async Task SaveToFile()
    {
        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "students.txt");
        try
        {
            JsonSerializerOptions serializeOptions = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            serializeOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            string jsonContent = JsonSerializer.Serialize(StudentList, serializeOptions);

            await File.WriteAllTextAsync(filePath, jsonContent);
            Debug.WriteLine($"Zapisano do pliku: {filePath}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Błąd podczas zapisu do pliku: {ex.Message}");
            Debug.WriteLine($"Próbowano utworzyć plik: {filePath}");
        }
    }

    public bool IsAnyDupe(int id, string clazz)
    {
        return StudentList.Any(student => student.Id == id && student.Clazz == clazz);
    }
}