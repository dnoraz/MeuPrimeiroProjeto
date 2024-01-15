using System;
using System.Text;
using Microsoft.Xrm.Sdk;
using System.Net;

namespace PluginsTreinamento
{
    public class ActionCEP : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            //Variavel contendo o contexto da execução
            IPluginExecutionContext context =
                (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            //Variavel contendo o Service Factory da Organização
            IOrganizationServiceFactory serviceFactory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            //Variavel contendo o Service Admin que estabelce os serviços de conexao com o Dataverse
            IOrganizationService serviceAdmin = serviceFactory.CreateOrganizationService(null);

            //Variavel do Trace que armazena informaçoes de Log
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            var cep = context.InputParameters["CepInput"];
            trace.Trace("Cep informado: " + cep);

            var viaCEPUrl = $"https://viacep.com.br/ws/{cep}/json";
            string result = string.Empty;
            using (WebClient client = new WebClient()) 
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Encoding = Encoding.UTF8;
                result = client.DownloadString(viaCEPUrl);
            }
            context.OutputParameters["ResultadoCEP"] = result;

            trace.Trace("Resultado: " + result);
        }
    }
}
