Console.WriteLine("Skriv inn tekst du vil hashe:");
var input = Console.ReadLine() ?? throw new NullReferenceException("Tekst kan ikke være null");
var pw = BCrypt.Net.BCrypt.EnhancedHashPassword(input);
Console.WriteLine($"Base64 encoded hash: {Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(pw))}");
