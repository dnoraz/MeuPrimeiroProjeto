

namespace MeuPrimeiroProjeto
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var crmService = new Conexao().ObterConexao();
            DataModel dataModel = new DataModel();
            dataModel.FetchXML(crmService);
        }

    }
}
