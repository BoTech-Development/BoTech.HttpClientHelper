namespace BoTech.HttpClientHelper.Tests;

public class TestDto
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public DateTime Birthday { get; set; }

    public override bool Equals(object? obj)
    {
        if(obj is TestDto dto)
            return dto.Name == Name && dto.Age == Age && dto.Birthday.Equals(Birthday);
        return false;
    }

    protected bool Equals(TestDto other)
    {
        return Name == other.Name && Age == other.Age && Birthday.Equals(other.Birthday);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Age, Birthday);
    }
}