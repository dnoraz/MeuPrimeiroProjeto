using System;
namespace MeuPrimeiroProjeto
{
    public class PrimeiraClasse
    {
        public void HelloWorld()
        {
            Console.WriteLine("Digite Seu nome");
            string nome = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("=====================================");
            Console.WriteLine("Hello " + nome);
            Console.WriteLine("Pressione qualquer tecla para continuar");
            Console.WriteLine("=====================================");
            Console.ReadKey();
        }
            
    }
}
