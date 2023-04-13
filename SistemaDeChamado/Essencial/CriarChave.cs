using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SistemaDeChamado.Essencial
{
    public class CriarChave
    {
        public static string Chave()
        {
            var chars = "abcdefghijklmnozABCDEFGHIJKLMNOZ1234567890@#$%&*!";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 15)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
    }
}
