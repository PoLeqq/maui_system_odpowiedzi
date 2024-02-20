using System.Text.Json;
using System.Diagnostics;

namespace LosowanieOsobyMAUI.Objects
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Clazz { get; set; }
        public bool Present { get; set; }
        public int BanCounter { get; set; }

        public Student(int id, string name, string surname, string clazz, bool present, int banCounter)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Clazz = clazz;
            Present=present;
            BanCounter=banCounter;
        }

        public string JsonSerialize()
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(this);
                return jsonString;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd podczas serializacji: {ex.Message}");
                return null;
            }
        }

        public static Student JsonDeserialize(string jsonString)
        {
            try
            {
                Student student = JsonSerializer.Deserialize<Student>(jsonString);
                return student;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd podczas deserializacji: {ex.Message}");
                return null;
            }
        }
    }
}