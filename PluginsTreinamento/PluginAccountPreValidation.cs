using System;
using Microsoft.Xrm.Sdk;

namespace PluginsTreinamento
{
    public class PluginAccountPreValidation : IPlugin
    {
        //método requerido para a execuçao do Plugin recebendo como parametro os dados do provedor de serviço
        public void Execute(IServiceProvider serviceProvider)
        {
            //Variavel contendo o contexto da execução
            IPluginExecutionContext context = 
                (IPluginExecutionContext) serviceProvider.GetService(typeof(IPluginExecutionContext));

            //Variavel contendo o Service Factory da Organização
            IOrganizationServiceFactory serviceFactory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            //Variavel contendo o Service Admin que estabelce os serviços de conexao com o Dataverse
            IOrganizationService serviceAdmin = serviceFactory.CreateOrganizationService(null);

            //Variavel do Trace que armazena informaçoes de Log
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            //Variavel do tipo Entity vazia
            Entity entidadeContexto = null;

            if (context.InputParameters.Contains("Target")) //Verifica se contem dados para o destino
            {
                entidadeContexto = (Entity)context.InputParameters["Target"]; //atribui o contexto da entidade para a variavel

                trace.Trace("Entidade do contexto: " + entidadeContexto.Attributes.Count); //armazena informçoes de LOG

                if(entidadeContexto == null) //verifica se a entidade do context esta vazia
                {
                    return; // caso verdadeira retorna sem nada a executar
                }

                if (!entidadeContexto.Contains("telephone1")) //verifica se o atributo telephone1 nao esta presente no contexto
                {
                    throw new InvalidPluginExecutionException("Campo telefone principal é obrigatório!"); //exibe Exception de Erro
                } 
                
            }
        }
    }
}
