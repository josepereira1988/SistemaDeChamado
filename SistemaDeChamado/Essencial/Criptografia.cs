using System.Security.Cryptography;
using System.Text;

namespace SistemaDeChamado.Essencial
{
    public class Criptografia
    {
        public static string Criptografar(string informacao, string chave)
        {
            var codificado = EncodeTo64(informacao);

            var codificarComChave = string.Concat(chave, "=====", codificado);

            var codificado2X = EncodeTo64(codificarComChave);

            return EncodeTo64(codificado2X);

        }

        public static string DesCriptografar(string informacao, string chave)
        {

            try
            {

                var decodificar = DecodeFrom64(informacao);

                var decodificar2XInformacaoCompleta = DecodeFrom64(decodificar);

                string[] separador = { "=====" };
                int count = 2;
                var separadorInformacao = decodificar2XInformacaoCompleta.Split(separador, count, StringSplitOptions.None);

                if (separadorInformacao[0].ToString() == chave)
                {
                    return DecodeFrom64(separadorInformacao[1].ToString());
                }

            }
            catch (Exception)
            {

            }

            return string.Empty;

        }

        static private string DecodeFrom64(string encodeData)
        {
            byte[] encodeDataAsByte = Convert.FromBase64String(encodeData);

            string returnValue = Encoding.UTF8.GetString(encodeDataAsByte);

            return returnValue;
        }

        static private string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsByte = Encoding.UTF8.GetBytes(toEncode);

            string returnValue = Convert.ToBase64String(toEncodeAsByte);

            return returnValue;
        }
    }
}
